using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{

  public bool paused = false;
  public int level = 0;
  public int randomSeedX;
  public int randomSeedY;
  public float totemCounter = 120;

  // Start is called before the first frame update
  void Start(){
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
    Debug.Log(getTile(new Vector2Int(0,0)));
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
    return tile;
  }

  public GameObject[,] getSquare(Vector3Int target){
    //this must be amazingly slow
    GameObject[,] tiles = new GameObject[target.z*2, target.z*2];
    GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
    Vector2Int rangeMin=new Vector2Int(target.x-target.z, target.y-target.z);
    Vector2Int rangeMax=new Vector2Int(target.x+target.z, target.y+target.z);
    foreach (GameObject candidate in allTiles){
      Vector2Int t = candidate.GetComponent<Tile>().pos;
      if (t.x>=rangeMin.x && t.y>=rangeMin.y && t.x<=rangeMax.x && t.x<=rangeMax.y){
        tiles[t.x-rangeMin.x,t.y-rangeMin.y] = candidate;
      }
    }
    return tiles;
  }
}
