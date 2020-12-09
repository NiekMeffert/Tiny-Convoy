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
  public GameObject mainCamera;
  public GameObject lastBigTile;
  public GameObject[] CPUs;
  public GameObject totem;
  float visibilityPainterX = .5f;
  float visibilityPainterY = .5f;
  public Material[] powerLevels;

  // Start is called before the first frame update
  void Start(){
    mainCamera = GameObject.Find("Main Camera");
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
  }

  // Update is called once per frame
  void Update(){
    //normal game mode
    if (mode==1) {
      totemCounter-=Time.deltaTime;
      if (totemCounter<0&&(CPUs.Length>0)){
        totem = CPUs[Mathf.FloorToInt(Random.value*CPUs.Length)];
        totemCounter=120;
      }
      if (Input.GetMouseButtonUp(0)){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
          pickDefaultAction(hit);
        }
      }
    }
    updateVisibleTiles();
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

  void pickDefaultAction(RaycastHit hit){
    if (hit.collider.tag == "Tile"){
      totem.GetComponent<Pathfinder>().destination = hit.collider.gameObject;
      totem.GetComponent<CPU>().objective = null;
    }
    if (hit.collider.tag == "Plant"){
      totem.GetComponent<CPU>().harvest(hit.collider.gameObject);
      totem.GetComponent<CPU>().objective = hit.collider.gameObject;
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

  public float[] getRands(Vector2Int vec2){
    int rInit = vec2.x+randomSeedX+vec2.y+randomSeedY;
    float[] rands = new float[16];
    for (int i=0; i<rands.Length; i++){
      Random.InitState(rInit);
      rands[i]=Random.value;
      rInit++;
    }
    return rands;
  }

  public GameObject createBigTile(Vector2Int targetFloor, GameObject forcedBigTile){
    GameObject btPrefab = null;
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
}
