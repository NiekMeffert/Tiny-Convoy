using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
  public GameController gameController;
  public GameObject cpu;
  public CPU cpuVars;
  public Pathfinder pathfinder;
  public List<string> knownDangers = new List<string>();
  public List<float> knownDangerWeights = new List<float>();
  Vector2Int[] surroundings = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
  public navTile[,] memoryTiles;
  List<navTile> powerList = new List<navTile>();
  List<navTile> dangerList = new List<navTile>();
  List<navTile> friendList = new List<navTile>();
  float mapAge = 0;
  Car firstCarVars;

  // Start is called before the first frame update
  void Start(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject;
    cpuVars = gameObject.GetComponent<CPU>();
    pathfinder = gameObject.GetComponent<Pathfinder>();
  }

  // Update is called once per frame
  void Update(){

  }

  public void changeMind(){
    //Debug.Log("Reconsidering");
    firstCarVars = cpuVars.cars[0].GetComponent<Car>();
    refreshWeightedMap();
    powerList.Clear();
    friendList.Clear();
    dangerList.Clear();
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        if (memoryTiles[x,y].power > 0) powerList.Add(memoryTiles[x,y]);
        if (memoryTiles[x,y].friend==true) friendList.Add(memoryTiles[x,y]);
        if (memoryTiles[x,y].danger==true) dangerList.Add(memoryTiles[x,y]);
      }
    }
    List<itinerary> possibleItins = new List <itinerary>();
    List<float> possibleWeights = new List <float>();
    List<GameObject> possibleObjectives = new List <GameObject>();
    //Get energy
    if (cpuVars.powerAvailable/cpuVars.maxPower<.5 && powerList.Count>0){
      //Find closest power source & up to 2 random ones (just in case)
      for (int h = 0; h<Mathf.Min(powerList.Count,3); h++){
        int chosen = 0;
        if (h==0){
          float sqDist = 100000;
          for (int i=1; i<powerList.Count; i++){
            float newSqDist = (gameObject.transform.position-powerList[i].tile.transform.position).sqrMagnitude;
            if (newSqDist<sqDist){
              chosen=i;
              sqDist=newSqDist;
            }
          }
        } else {
          chosen = Mathf.RoundToInt(Random.value*(powerList.Count-1));
        }
        navTile possibleIdea = powerList[chosen];
        GameObject nextDoor = getAdjacentFreeTile(possibleIdea.tile);
        if (nextDoor==null) continue;
        //Insert objective item
        possibleObjectives.Insert(0, possibleIdea.powerObj);
        //Insert idea path
        possibleItins.Insert(0, pathfinder.getItinerary(nextDoor));
        //Insert idea weight
        possibleWeights.Insert(0, (1f/possibleItins[0].cost)+.5f-(cpuVars.powerAvailable/cpuVars.maxPower));
        //...and undo insertions if anything is wrong:
        if (possibleItins[0].cost==0){
          possibleWeights.RemoveAt(0);
          possibleObjectives.RemoveAt(0);
          possibleItins.RemoveAt(0);
        }
      }
    }
    //Stay near others
    navTile nearestFriend = null;
    float weight = 100000f;
    if (friendList.Count>0){
      //Find nearest friend
      foreach (navTile f in friendList){
        float newDist = (firstCarVars.tile.transform.position-f.tile.transform.position).magnitude;
        if (newDist<weight){
          weight=newDist;
          nearestFriend=f;
        }
      }
      if (weight < 2f) nearestFriend=null; //If they're close, don't worry
      weight = (weight-2f)*1.5f;
    } else {
      //worry
      nearestFriend = memoryTiles[Mathf.RoundToInt(Random.value*memoryTiles.GetLength(0)), Mathf.RoundToInt(Random.value*memoryTiles.GetLength(1))];
      weight = 2f;
    }
    if (nearestFriend!=null){
      GameObject nextDoor = getAdjacentFreeTile(nearestFriend.tile);
      if (nextDoor!=null){
        //Insert objective item
        possibleObjectives.Insert(0, nearestFriend.friendObj);
        //Insert idea path
        possibleItins.Insert(0, pathfinder.getItinerary(nextDoor));
        //Insert idea weight
        possibleWeights.Insert(0, weight);
        //...and undo insertions if anything is wrong:
        if (possibleItins[0].cost==0){
          possibleWeights.RemoveAt(0);
          possibleObjectives.RemoveAt(0);
          possibleItins.RemoveAt(0);
        }
      }
    }
    //Give energy
    if (cpuVars.powerAvailable/cpuVars.maxPower>.5){

    }
    //Player ideas

    //Choose one idea
    if (possibleItins.Count==0) return;
    int topIdea=0;
    for (int i=0; i<possibleItins.Count; i++){
      if (possibleWeights[i]>possibleWeights[topIdea]) topIdea=i;
    }
    //if (cpuVars.objective!=possibleObjectives[topIdea]) Debug.Log("Changed objective");
    cpuVars.objective = possibleObjectives[topIdea];
    cpuVars.objectiveWeight = possibleWeights[topIdea];
    pathfinder.setItinerary(possibleItins[topIdea]);
  }

  public void refreshWeightedMap(){
    if (Time.time-mapAge<.1) return;
    mapAge=Time.time;
    Vector2Int pos = cpuVars.cars[0].GetComponent<Car>().tile.GetComponent<Tile>().pos;
    GameObject[,] mTiles = gameController.getSquare(new Vector3Int(pos.x, pos.y, cpuVars.sight));
    memoryTiles = new navTile[mTiles.GetLength(0), mTiles.GetLength(1)];
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        navTile nt = new navTile();
        nt.tile = mTiles[x,y];
        nt.tileVars = mTiles[x,y].GetComponent<Tile>();
        nt.walkable = true;
        //nt.current = false;
        nt.target = false;
        nt.selectable = false;
        nt.adjacencyList = new List<navTile>();
        nt.visited = false;
        nt.parent = null;
        nt.weight = 0;
        memoryTiles[x,y] = nt;
      }
    }
    //if AI controlled, overlay danger weights & goodies
    if (gameObject != gameController.totem){
      for (int x = 0; x<memoryTiles.GetLength(0); x++){
        for (int y = 0; y<memoryTiles.GetLength(1); y++){
          List<GameObject> ats = memoryTiles[x,y].tile.GetComponent<Tile>().actualThings;
          foreach (GameObject aThing in ats){
            Car friend = aThing.GetComponent<Car>();
            if (friend!=null){
              if (friend.cpu!=cpu){
                memoryTiles[x,y].friend = true;
                memoryTiles[x,y].friendObj = aThing;
              }
            }
            Danger danger = aThing.GetComponent<Danger>();
            if (danger!=null){
              int i = knownDangers.IndexOf(danger.dangerName);
              if (i>-1) {
                foreach (Vector2Int offset in surroundings){
                  Vector2Int tileLoc = new Vector2Int(x+offset.x, y+offset.y);
                  if (tileLoc.x>=0 && tileLoc.y>=0 && tileLoc.x<memoryTiles.GetLength(0) && tileLoc.y<memoryTiles.GetLength(1)){
                    memoryTiles[tileLoc.x,tileLoc.y].weight = Mathf.Max(knownDangerWeights[i], memoryTiles[tileLoc.x,tileLoc.y].weight);
                    memoryTiles[tileLoc.x,tileLoc.y].danger = true;
                    memoryTiles[tileLoc.x,tileLoc.y].dangerObj = aThing;
                  }
                }
              }
            }
            Powered powerSource = aThing.GetComponent<Powered>();
            if (powerSource!=null){
              if (firstCarVars.overlapsVertically(aThing)==true){
                memoryTiles[x,y].power = Mathf.Max(powerSource.power, memoryTiles[x,y].power);
                memoryTiles[x,y].powerObj = aThing;
              }
            }
          }
        }
      }
    }
  }

  public void witnessDanger(Vector3 epicenter, float weight, string danger){
    if (Mathf.Abs(gameObject.transform.position.x-epicenter.x)>cpuVars.sight || Mathf.Abs(gameObject.transform.position.z-epicenter.z)>cpuVars.sight) return;
    learnDanger(weight, danger);
  }

  public void learnDanger(float weight, string danger){
    int i = knownDangers.IndexOf(danger);
    float prevWeight=0;
    if (i>-1){
      knownDangers.RemoveAt(i);
      prevWeight = knownDangerWeights[i];
      knownDangerWeights.RemoveAt(i);
    }
    knownDangers.Add(danger);
    knownDangerWeights.Add(weight);
  }

  public GameObject getAdjacentFreeTile(GameObject tile){
    GameObject adjacentTile = null;
    Tile tileVars = tile.GetComponent<Tile>();
    GameObject[,] square = gameController.getSquare(new Vector3Int(tileVars.pos.x, tileVars.pos.y, 1));
    float min=1000f;
    for (int x = 0; x<square.GetLength(0); x++){
      for (int y = 0; y<square.GetLength(1); y++){
        float dist = Vector3.Distance(square[x,y].transform.position, gameObject.transform.position);
        float fit = square[x,y].GetComponent<Tile>().canFit(cpuVars.cars[0],true);
        if (dist<min && Mathf.Abs(cpuVars.cars[0].transform.position.y-fit)<.1){
          min=dist;
          adjacentTile=square[x,y];
        }
      }
    }
    return adjacentTile;
  }
}
