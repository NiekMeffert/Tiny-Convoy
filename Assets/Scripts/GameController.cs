using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{

  public int mode = 1; //0 paused, 1 normal, 2 upgrade, 3 long-distance scanning
  public int level = 0;
  public int randomSeedX;
  public int randomSeedY;
  public float totemCounter = 120;
  public GameObject tilePrefab;
  public GameObject CPUPrefab;
  public GameObject carPrefab;
  public GameObject[] upgradePrefabs = new GameObject[16];
  public GameObject[] plantPrefabs = new GameObject[16];
  public GameObject[] bigTilePrefabs = new GameObject[16];
  public GameObject mainCamera;
  public GameObject lastBigTile;
  public GameObject[] CPUs;
  public GameObject totem;

  // Start is called before the first frame update
  void Start(){
    mainCamera = GameObject.Find("Main Camera");
    for (int i = 0; i<upgradePrefabs.Length; i++){
      if (upgradePrefabs[i]==null){upgradePrefabs[i]=upgradePrefabs[0];}
    }
    for (int i = 0; i<plantPrefabs.Length; i++){
      if (plantPrefabs[i]==null){plantPrefabs[i]=plantPrefabs[0];}
    }
    for (int i = 0; i<bigTilePrefabs.Length; i++){
      if (bigTilePrefabs[i]==null){
        bigTilePrefabs[i]=bigTilePrefabs[0];
      }
    }
    /*randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);*/
    //createBigTile(new Vector2Int(0,0), null);
  }

  // Update is called once per frame
  void Update(){
    if (mode==0) {return;} //paused
    totemCounter-=Time.deltaTime;
    if (totemCounter<0){
      totem = CPUs[Mathf.FloorToInt(Random.value*CPUs.Length)];
      totemCounter=120;
    }
    //Transform cam = GameObject.Find("Player").GetComponent<Transform>();
    //getSquare(new Vector3Int(Mathf.RoundToInt(cam.position.x),Mathf.RoundToInt(cam.position.z),20));
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
        bigTile = createBigTile(targetFloor, null);
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

  public GameObject createBigTile(Vector2Int targetFloor, GameObject forcedBigTile){
    GameObject btPrefab = null;
    if (forcedBigTile==null){
      Random.InitState(targetFloor.x+randomSeedX+targetFloor.y+randomSeedY);
      btPrefab = bigTilePrefabs[Mathf.RoundToInt(Random.value*(bigTilePrefabs.Length-1))];
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

  public int canFit(GameObject load, GameObject tile){
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
    }
    return fit;
  }

  public GameObject createCPU(Vector2Int target){
    GameObject newCPU = Instantiate(CPUPrefab);
    CPU newCPUVars = newCPU.GetComponent<CPU>();
    float[] rands = new float[]{Random.value, Random.value, Random.value, Random.value, Random.value, Random.value};
    float randsTotal = 0;
    foreach (float rand in rands){
      randsTotal+=rand;
    }
    float randFactor = randsTotal/6f;
    newCPUVars.baseProcessing = 1+Mathf.RoundToInt(rands[0]*randFactor);
    newCPUVars.baseMemory = 1+Mathf.RoundToInt(rands[1]*randFactor);
    newCPUVars.baseInputs = 1+Mathf.RoundToInt(rands[2]*randFactor);
    newCPUVars.baseOutputs = 1+Mathf.RoundToInt(rands[3]*randFactor);
    newCPUVars.baseBattery = 1f+Mathf.Round(rands[4]*randFactor);
    newCPUVars.baseSight = 1f+Mathf.Round(rands[5]*randFactor);
    newCPUVars.cars[0]=Instantiate(carPrefab);
    Car carVars = newCPUVars.cars[0].GetComponent<Car>();
    newCPUVars.cars[0].transform.position = new Vector3(target.x, 0, target.y);
    //newCPU.transform.position = new Vector3(target.x, 0, target.y);
    carVars.cpu = newCPU;
    carVars.upgrades[0]=newCPU;
    return newCPU;
  }
}
