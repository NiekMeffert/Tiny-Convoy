using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{

  public bool paused = false;
  public int level = 0;
  public int randomSeedX;
  public int randomSeedY;
  public float totemCounter = 120;
  public GameObject tilePrefab;
  public GameObject CPUPrefab;
  public GameObject carPrefab;
  public GameObject[] upgradePrefabs = new GameObject[16];
  public GameObject[] plantPrefabs = new GameObject[16];

  // Start is called before the first frame update
  void Start(){
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
    //find tiles
    GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
    foreach (GameObject candidate in allTiles){
      candidate.GetComponent<Tile>().pos = new Vector2Int(Mathf.RoundToInt(candidate.GetComponent<Transform>().position.x), Mathf.RoundToInt(candidate.GetComponent<Transform>().position.z));
    }
    foreach (GameObject candidate in allTiles){
      candidate.GetComponent<Tile>().pos = new Vector2Int(Mathf.RoundToInt(candidate.GetComponent<Transform>().position.x), Mathf.RoundToInt(candidate.GetComponent<Transform>().position.z));
    }
    Debug.Log(getSquare(new Vector3Int(5,5,5)));
  }

  // Update is called once per frame
  void Update(){
    if (paused==true) {return;}
    totemCounter-=Time.deltaTime;
    if (totemCounter<0){
      totemCounter=120;
    }
  }

  public GameObject getTile(Vector2Int target){
    //this must be amazingly slow
    GameObject tile = null;
    GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
    foreach (GameObject candidate in allTiles){
      if (candidate.GetComponent<Tile>().pos.x==target.x && candidate.GetComponent<Tile>().pos.y==target.y){
        tile = candidate;
      }
    }
    if (tile==null) {tile = createTile(target);}
    return tile;
  }

  public GameObject[,] getSquare(Vector3Int target){
    //this must be amazingly slow
    int arraySize = 1+(target.z*2);
    GameObject[,] tiles = new GameObject[arraySize, arraySize];
    GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
    Vector2Int rangeMin=new Vector2Int(target.x-target.z, target.y-target.z);
    Vector2Int rangeMax=new Vector2Int(target.x+target.z, target.y+target.z);
    foreach (GameObject candidate in allTiles){
      Vector2Int t = candidate.GetComponent<Tile>().pos;
      if (t.x>rangeMin.x && t.y>rangeMin.y && t.x<rangeMax.x && t.x<rangeMax.y){
        //Debug.Log(t.x+" "+t.y);
        tiles[t.x-rangeMin.x,t.y-rangeMin.y] = candidate;
      }
    }
    for (int x = 0; x < arraySize; x++) {
      for (int y = 0; y < arraySize; y++){
        if (tiles[x,y]==null){
          tiles[x,y] = createTile(new Vector2Int(rangeMin.x+x,rangeMin.y+y));
        }
      }
    }

    return tiles;
  }

  public GameObject createTile(Vector2Int target){
    GameObject newTile = Instantiate(tilePrefab);
    newTile.transform.position = new Vector3(target.x,0,target.y);
    newTile.GetComponent<Tile>().pos = target;
    return newTile;
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
    carVars.upgrades[0,0]=newCPU;
    return newCPU;
  }
}
