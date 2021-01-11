using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
  public GameController gameController;
  public CPU cpu;
  public Pathfinder pathfinder;
  public List<string> knownDangers = new List<string>();
  public List<float> knownDangerWeights = new List<float>();
  Vector2Int[] surroundings = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
  navTile[,] memoryTiles;
  List<navTile> powerList = new List<navTile>();
  List<navTile> dangerList = new List<navTile>();
  List<navTile> friendList = new List<navTile>();
  float mapAge = 0;
  Car firstCarVars;

  // Start is called before the first frame update
  void Start(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
    pathfinder = gameObject.GetComponent<Pathfinder>();
  }

  // Update is called once per frame
  void Update(){

  }

  public void changeMind(){
    Debug.Log("Reconsidering");
    firstCarVars = cpu.cars[0].GetComponent<Car>();
    getWeightedMap();
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
    List<(float, Stack<navTile>, navTile, navTile)> ideaPaths = new List <(float, Stack<navTile>, navTile, navTile)>();
    List<float> ideaWeights = new List <float>();
    List<GameObject> ideaObjectives = new List <GameObject>();
    //Get energy
    if (cpu.powerAvailable/cpu.maxPower<.5 && powerList.Count>0){
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
        Tile tileVars = powerList[chosen].tile.GetComponent<Tile>();
        ideaObjectives.Insert(0, gameObject);
        for (int i=0; i<tileVars.actualThings.Count; i++){
          if (tileVars.actualThings[i].GetComponent<Powered>()!=null) {
            ideaObjectives[0] = tileVars.actualThings[i];
            break;
          }
        }
        //Insert idea path
        ideaPaths.Insert(0, pathfinder.getPath(nextDoor));
        //Insert idea weight
        ideaWeights.Insert(0, (1f/ideaPaths[0].Item1)+.5f-(cpu.powerAvailable/cpu.maxPower));
        //...and undo insertions if anything is wrong:
        if (ideaPaths[0].Item1==0 || ideaObjectives[0]==gameObject){
          ideaWeights.RemoveAt(0);
          ideaObjectives.RemoveAt(0);
          ideaPaths.RemoveAt(0);
        }
      }
    }
    //Stay near others
    //Give energy
    if (cpu.powerAvailable/cpu.maxPower>.5){

    }
    //Player ideas

    //Choose one idea
    if (ideaPaths.Count==0) return;
    int topIdea=0;
    for (int i=0; i<ideaPaths.Count; i++){
      if (ideaWeights[i]>ideaWeights[topIdea]) topIdea=i;
    }
    if (cpu.objective!=ideaObjectives[topIdea]) Debug.Log("Changed objective");
    cpu.objective = ideaObjectives[topIdea];
    pathfinder.setPath(ideaPaths[topIdea].Item1,ideaPaths[topIdea].Item2,ideaPaths[topIdea].Item3,ideaPaths[topIdea].Item4);
  }

  public navTile[,] getWeightedMap(){
    //if (Time.time-mapAge<.2) return memoryTiles;
    mapAge=Time.time;
    Vector2Int pos = cpu.cars[0].GetComponent<Car>().tile.GetComponent<Tile>().pos;
    GameObject[,] mTiles = gameController.getSquare(new Vector3Int(pos.x, pos.y, cpu.sight));
    memoryTiles = new navTile[mTiles.GetLength(0), mTiles.GetLength(1)];
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        navTile nt = new navTile();
        nt.tile = mTiles[x,y];
        nt.tileVars = mTiles[x,y].GetComponent<Tile>();
        nt.walkable = true;
        nt.current = false;
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
      for (int x = 0; x<mTiles.GetLength(0); x++){
        for (int y = 0; y<mTiles.GetLength(1); y++){
          List<GameObject> ats = mTiles[x,y].GetComponent<Tile>().actualThings;
          foreach (GameObject aThing in ats){
            Car friend = aThing.GetComponent<Car>();
            if (friend!=null){
              if (friend.cpu==cpu) continue;
              memoryTiles[x,y].friend = true;
            }
            Danger danger = aThing.GetComponent<Danger>();
            if (danger!=null){
              int i = knownDangers.IndexOf(danger.dangerName);
              if (i>-1) {
                foreach (Vector2Int offset in surroundings){
                  Vector2Int tileLoc = new Vector2Int(x+offset.x, y+offset.y);
                  if (tileLoc.x>=0 && tileLoc.y>=0 && tileLoc.x<mTiles.GetLength(0) && tileLoc.y<mTiles.GetLength(1)){
                    memoryTiles[tileLoc.x,tileLoc.y].weight = Mathf.Max(knownDangerWeights[i], memoryTiles[tileLoc.x,tileLoc.y].weight);
                    memoryTiles[tileLoc.x,tileLoc.y].danger = true;
                  }
                }
              }
            }
            Powered powerSource = aThing.GetComponent<Powered>();
            if (powerSource!=null){
              if (firstCarVars.overlapsVertically(aThing)==true){
                memoryTiles[x,y].power = Mathf.Max(powerSource.power, memoryTiles[x,y].power);
              }
            }
          }
        }
      }
    }
    return memoryTiles;
  }

  public void witnessDanger(Vector3 epicenter, float weight, string danger){
    if (Mathf.Abs(gameObject.transform.position.x-epicenter.x)>cpu.sight || Mathf.Abs(gameObject.transform.position.z-epicenter.z)>cpu.sight) return;
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
        float fit = square[x,y].GetComponent<Tile>().canFit(cpu.cars[0],true);
        if (dist<min && Mathf.Abs(cpu.cars[0].transform.position.y-fit)<.1){
          min=dist;
          adjacentTile=square[x,y];
        }
      }
    }
    return adjacentTile;
  }
}
