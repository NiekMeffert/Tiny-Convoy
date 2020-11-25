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
}
