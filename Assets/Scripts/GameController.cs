﻿using System.Collections;
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
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
    //createBigTile(new Vector2Int(0,0), null);
  }

  // Update is called once per frame
  void Update(){
    //normal game mode
    if (mode==1) {
      totemCounter-=Time.deltaTime;
      if (totemCounter<0){
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
  }

  void pickDefaultAction(RaycastHit hit){
    if (hit.collider.tag == "Tile"){
      totem.GetComponent<Pathfinder>().moveToTile(hit.collider.gameObject);
    }
    if (hit.collider.tag == "Plant"){
      totem.GetComponent<CPU>().harvest(hit.collider.gameObject);
      Debug.Log("Plant");
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
}
