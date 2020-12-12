using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{

  public int mode = 1; //0 paused, 1 normal, 2 upgrade, 3 long-distance scanning
  public int level = 0;
  public int randomSeedX;
  public int randomSeedY;
  public float totemCounter = 0;
  public GameObject tilePrefab;
  public GameObject CPUPrefab;
  public GameObject carPrefab;
  public GameObject[] upgradePrefabs;
  public GameObject[] plantPrefabs;
  public GameObject[] bigTilePrefabs;
  public GameObject[] specialBigTilePrefabs;
  public GameObject mainCamera;
  public GameObject lastBigTile;
  public GameObject[] CPUs;
  public GameObject totem;
  float visibilityPainterX = .5f;
  float visibilityPainterY = .5f;
  public Material[] powerLevels;
  public GameObject reticule;
  public GameObject reticulePlant;
  public GameObject reticuleUpgrade;
  GameObject mouseOver;
  public GameObject bigBot;
  SparseMatrix<GameObject> forcedBigTiles = new SparseMatrix<GameObject>();
  GameObject inventory;

  // Start is called before the first frame update
  void Start(){
    mainCamera = GameObject.Find("Main Camera");
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
    reticule = GameObject.Find("Reticule");
    reticulePlant = GameObject.Find("Reticule-Plant");
    reticuleUpgrade = GameObject.Find("Reticule-Upgrade");
    inventory = GameObject.Find("Inventory");
    inventory.SetActive(false);
    forcedBigTiles[0,0] = specialBigTilePrefabs[0];
  }

  // Update is called once per frame
  void Update(){
    reticule.SetActive(false);
    reticulePlant.SetActive(false);
    reticuleUpgrade.SetActive(false);
    //normal game mode
    if (mode==1) {
      totemCounter-=Time.deltaTime;
      if (totemCounter<0&&(CPUs.Length>0)){
        totem = CPUs[Mathf.FloorToInt(Random.value*CPUs.Length)];
        totemCounter=120;
        bigBotCheck();
      }
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)){
        mouseOver = hit.collider.gameObject;
        float maxDist = Mathf.Max(Mathf.Abs(mouseOver.transform.position.x-totem.transform.position.x), Mathf.Abs(mouseOver.transform.position.z-totem.transform.position.z));
        int seeDist = totem.GetComponent<CPU>().memory + totem.GetComponent<Pathfinder>().freeMemory;
        if (maxDist<=seeDist){
          if (mouseOver.GetComponent<Tile>()!=null){
            if (canFit(totem,mouseOver,true)==0){
              reticule.SetActive(true);
              reticule.transform.position = mouseOver.transform.position;
            }
          }
          if (mouseOver.GetComponent<Plant>()!=null){
            reticulePlant.SetActive(true);
            reticulePlant.transform.position = mouseOver.transform.position;
          }
          if (mouseOver.GetComponent<Upgrade>()!=null){
            reticuleUpgrade.SetActive(true);
            reticuleUpgrade.transform.position = mouseOver.transform.position;
          }
        }
      } else {
        mouseOver=null;
      }
      if (Input.GetMouseButtonUp(0)){
        pickDefaultAction();
      }
    }
    if (mode==2){
    }
    updateVisibleTiles();
  }

  public void setMode(int newMode){
    if (newMode==1){
      mode=1;
      inventory.SetActive(false);
    }
    if (newMode==2){
      mode=2;
      inventory.SetActive(true);
      //upgradeWith(totem.objective);
    }
  }

  void pickDefaultAction(){
    if (mouseOver.GetComponent<Tile>() != null){
      totem.GetComponent<Pathfinder>().destination = mouseOver;
      totem.GetComponent<CPU>().objective = null;
    }
    if (mouseOver.GetComponent<Plant>() != null){
      totem.GetComponent<CPU>().harvest(mouseOver);
      totem.GetComponent<CPU>().objective = mouseOver;
    }
    if (mouseOver.GetComponent<Upgrade>() != null){
      totem.GetComponent<CPU>().upgrade(mouseOver);
      totem.GetComponent<CPU>().objective = mouseOver;
    }
  }

  public void updateVisibleTiles(){
    for (int i=1; i>=0; i--){
      visibilityPainterX+=.07f;
      if (visibilityPainterX>1.2f){
        visibilityPainterX -= 1.4f;
        visibilityPainterY+=.07f;
        if (visibilityPainterY>1.2f) visibilityPainterY-=1.4f;
      }
      Ray r = mainCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(visibilityPainterX, visibilityPainterY, 0));
      Vector3 zeroPoint = r.origin + (((r.origin.y) / -r.direction.y) * r.direction);
      getTile(new Vector2Int(Mathf.RoundToInt(zeroPoint.x), Mathf.RoundToInt(zeroPoint.z)));
    }
  }

  public GameObject getTile(Vector2Int target){
    GameObject bigTile = null;
    Vector2Int targetFloor = new Vector2Int(Mathf.FloorToInt(target.x/10f)*10, Mathf.FloorToInt(target.y/10f)*10);
    if (lastBigTile!=null && lastBigTile.GetComponent<BigTile>().pos.x==targetFloor.x && lastBigTile.GetComponent<BigTile>().pos.y==targetFloor.y){
      bigTile = lastBigTile;
    } else {
      GameObject[] allBigTiles = GameObject.FindGameObjectsWithTag("BigTile");
      foreach (GameObject candidate in allBigTiles){
        if (candidate.GetComponent<BigTile>().pos.x==targetFloor.x && candidate.GetComponent<BigTile>().pos.y==targetFloor.y){
          bigTile = candidate;
        }
      }
      if (bigTile==null) {
        bigTile = createBigTile(targetFloor);
      }
    }
    lastBigTile = bigTile;
    BigTile bigTileVars=bigTile.GetComponent<BigTile>();
    GameObject tile = bigTileVars.tiles[target.x-bigTileVars.pos.x, target.y-bigTileVars.pos.y];
    return tile;
  }

  public GameObject[,] getSquare(Vector3Int target){
    int arraySize = 1+(target.z*2);
    GameObject[,] tiles = new GameObject[arraySize, arraySize];
    Vector2Int rangeMin=new Vector2Int(target.x-target.z, target.y-target.z);
    Vector2Int rangeMax=new Vector2Int(target.x+target.z, target.y+target.z);
    for (int x=0; x<arraySize; x++){
      for (int y=0; y<arraySize; y++){
        tiles[x,y] = getTile(new Vector2Int(rangeMin.x+x, rangeMin.y+y));
      }
    }
    return tiles;
  }

  public float[] getRands(Vector2Int vec2){
    Random.InitState(vec2.x+randomSeedX);
    int a = Mathf.RoundToInt(Random.value*1000000);
    Random.InitState(vec2.y+randomSeedY);
    int b = Mathf.RoundToInt(Random.value*1000000);
    int r = a+b;
    float[] rands = new float[16];
    for (int i=0; i<rands.Length; i++){
      Random.InitState(r);
      rands[i]=Random.value;
      r++;
    }
    return rands;
  }

  public GameObject createBigTile(Vector2Int targetFloor){
    GameObject btPrefab = null;
    GameObject forcedBigTile = forcedBigTiles[targetFloor.x, targetFloor.y];
    if (forcedBigTile==null){
      float[] rands = getRands(targetFloor);
      btPrefab = bigTilePrefabs[Mathf.RoundToInt(rands[0]*(bigTilePrefabs.Length-1))];
    } else {
      btPrefab = forcedBigTile;
    }
    GameObject newBigTile = Instantiate(btPrefab);
    BigTile newBigTileVars = newBigTile.GetComponent<BigTile>();
    newBigTileVars.pos = targetFloor;
    newBigTile.transform.position = new Vector3(targetFloor.x,0,targetFloor.y);
    //init tiles
    foreach (Transform candidate in newBigTile.transform){
      if (candidate.parent==newBigTile.transform && candidate.gameObject.tag=="Tile"){
        Vector3 tilePos = candidate.position;
        Vector2Int tp = new Vector2Int(Mathf.FloorToInt(tilePos.x), Mathf.FloorToInt(tilePos.z));
        newBigTile.GetComponent<BigTile>().tiles[tp.x-newBigTileVars.pos.x, tp.y-newBigTileVars.pos.y] = candidate.gameObject;
        candidate.GetComponent<Tile>().pos = tp;
        candidate.GetComponent<Tile>().bigTile = newBigTile;
      }
    }
    return newBigTile;
  }

  public int canFit(GameObject load, GameObject tile, bool mustBeStandable){
    int loadHeight = load.GetComponent<ActualThing>().height;
    Tile tileVars = tile.GetComponent<Tile>();
    int fit = -1;
    for (int i=0; i<tileVars.heightSlots.Length; i++){
      if (fit > -1) {break;}
      bool safeHeight=true;
      for (int h=0; h<loadHeight; h++){
        if (i+h>=tileVars.heightSlots.Length || tileVars.heightSlots[i+h]!=null) {
          safeHeight=false;
        }
      }
      if (safeHeight==true) fit = i;
      if (fit>0 && mustBeStandable==true){
        GameObject topper = tileVars.heightSlots[fit-1];
        if (topper!=null){
          if (topper.GetComponent<ActualThing>().standable==false) fit = -1;
        }
      }
    }
    return fit;
  }

  public void bigBotCheck(){
    GameObject[] allBots = GameObject.FindGameObjectsWithTag("BigBot");
    int botNumber = 1 + (Mathf.FloorToInt(level*.4f));
    if (allBots.Length<botNumber){
      GameObject newBigBot = Instantiate(bigBot);
      BigBot botVars = newBigBot.GetComponent<BigBot>();
      float edge1 = Random.value * botVars.maxDistance;
      if (Random.value>.5f) edge1 *= -1f;
      float edge2 = botVars.maxDistance;
      if (Random.value>.5f) edge2 *= -1f;
      Vector3 botVec = new Vector3(edge1,0,edge2);
      if (Random.value>.5) botVec = new Vector3(edge2,0,edge1);
      botVec += mainCamera.transform.position;
      newBigBot.transform.position = botVec;
    }
  }
}
