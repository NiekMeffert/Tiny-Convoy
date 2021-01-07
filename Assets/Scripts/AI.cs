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
    List<(float, Stack<navTile>, navTile, navTile)> ideas = new List <(float, Stack<navTile>, navTile, navTile)>();
    List<float> ideaWeights = new List <float>();
    List<GameObject> objectives = new List <GameObject>();
    //Get energy
    if (cpu.powerAvailable/cpu.maxPower<.5 && powerList.Count>0){
      //Find closest power source & up to 2 random ones (just in case)
      for (int h = 0; h<Mathf.Min(powerList.Count,3); h++){
        int chosen = 0;
        if (h==0){
          float sqDist = 100000;
          for (int i = 1; i<powerList.Count; i++){
            float newSqDist = (gameObject.transform.position-powerList[i].tile.transform.position).sqrMagnitude;
            if (newSqDist<sqDist){
              chosen=i;
              sqDist=newSqDist;
            }
          }
        } else if (h<3){
          chosen = Mathf.RoundToInt(Random.value*(powerList.Count-1));
        }
        navTile possibleIdea = powerList[chosen];
        //powerList.RemoveAt(chosen);
        GameObject nextDoor = getAdjacentFreeTile(possibleIdea.tile);
        if (nextDoor==null) continue;
        ideas.Insert(0, pathfinder.getPath(nextDoor));
        if (ideas[0].Item1==0){
          ideas.RemoveAt(0);
        } else {
          ideaWeights.Insert(0, (1f/ideas[0].Item1)+.5f-(cpu.powerAvailable/cpu.maxPower));
          for (int i=0; i<powerList[chosen].tile.GetComponent<Tile>().heightSlots.Length; i++){
            GameObject o = powerList[chosen].tile.GetComponent<Tile>().heightSlots[i];
            if (o!=null){
              if (o.GetComponent<Powered>()!=null) {
                objectives.Insert(0, o);
                break;
              }
            }
          }
        }
      }
    }
    //Stay near others
    //Give energy
    if (cpu.powerAvailable/cpu.maxPower>.5){

    }
    //Player ideas

    //Choose one idea
    if (ideas.Count==0) Debug.Log("No ideas.");
    if (ideas.Count==0) return;
    int topIdea=0;
    for (int i=0; i<ideas.Count; i++){
      if (ideaWeights[i]>ideaWeights[topIdea]) topIdea=i;
    }
    cpu.objective = objectives[topIdea];
    pathfinder.setPath(ideas[topIdea].Item1,ideas[topIdea].Item2,ideas[topIdea].Item3,ideas[topIdea].Item4);
  }

  public navTile[,] getWeightedMap(){
    if (Time.time-mapAge<.2) return memoryTiles;
    mapAge=Time.time;
    Vector2Int pos = cpu.cars[0].GetComponent<Car>().upgradeTile.GetComponent<Tile>().pos;
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
          GameObject lastSlot = null;
          GameObject[] slots = mTiles[x,y].GetComponent<Tile>().heightSlots;
          foreach (GameObject slot in slots){
            if (slot==null || lastSlot==slot) continue;
            lastSlot=slot;
            Danger danger = slot.GetComponent<Danger>();
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
            Powered powerSource = slot.GetComponent<Powered>();
            if (powerSource!=null){
              memoryTiles[x,y].power = Mathf.Max(powerSource.power, memoryTiles[x,y].power);
            }
            Car friend = slot.GetComponent<Car>();
            if (friend!=null){
              memoryTiles[x,y].friend = true;
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
        int slot = gameController.canFit(gameObject,square[x,y], true);
        if (dist<min && slot==0){
          min=dist;
          adjacentTile=square[x,y];
        }
      }
    }
    return adjacentTile;
  }
}
