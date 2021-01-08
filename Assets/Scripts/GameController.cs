﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour{

  public int mode = 1; //0 paused, 1 normal, 2 upgrade, 3 long-distance scanner, 4 conversation
  public int nextMode = 1;
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
  public Vector2Int totemPos;
  public int fog1;
  public int fog2;
  float visibilityPainterX = .5f;
  float visibilityPainterY = .5f;
  public Material[] powerLevels;
  public GameObject reticule;
  public GameObject reticulePlant;
  public GameObject reticuleUpgrade;
  public GameObject reticuleCharge;
  GameObject mouseOver;
  public GameObject bigBot;
  SparseMatrix<GameObject> forcedBigTiles = new SparseMatrix<GameObject>();
  public GameObject inventory;
  public GameObject scanner;
  public bool uiBlocker = false;
  RectTransform scannerNoise;
  public GameObject[] particles;
  public List<GameObject> cleanupQueue = new List<GameObject>();
  //TEMPORARY:
  public GameObject selectedUpgrade;

  // Start is called before the first frame update
  void Start(){
    mainCamera = GameObject.Find("Main Camera");
    randomSeedX = (int) (Random.value * 1000000000f);
    randomSeedY = (int) (Random.value * 1000000000f);
    forcedBigTiles[0,0] = specialBigTilePrefabs[0];
    reticule = GameObject.Find("Reticule");
    reticulePlant = GameObject.Find("Reticule-Plant");
    reticuleUpgrade = GameObject.Find("Reticule-Upgrade");
    reticuleCharge = GameObject.Find("Reticule-Charge");
    inventory = GameObject.Find("InventoryCanvas");
    inventory.SetActive(false);
    scanner = GameObject.Find("LRScannerCanvas");
    scannerNoise = GameObject.Find("NoiseParent").transform.GetChild(0).GetComponent<RectTransform>();
    scanner.SetActive(false);
  }

  // Update is called once per frame
  void Update(){
    reticule.SetActive(false);
    reticulePlant.SetActive(false);
    reticuleUpgrade.SetActive(false);
    reticuleCharge.SetActive(false);

    //normal game mode
    if (mode==1) {
      //TEMPORARY:
      selectedUpgrade = null;

      totemCounter-=Time.deltaTime;
      if (totemCounter<0&&(CPUs.Length>0)){
        totem = CPUs[Mathf.FloorToInt(Random.value*CPUs.Length)];
        totemCounter=120;
        bigBotCheck();
      }
      if (totem!=null){
        if (totem.GetComponent<CPU>().cars[0].GetComponent<Car>().tile!=null){
          Vector2Int currPos = totem.GetComponent<CPU>().cars[0].GetComponent<Car>().tile.GetComponent<Tile>().pos;
          if (totemPos != currPos){
            moveFog();
          }
        }
      }
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)&&uiBlocker==false){
        mouseOver = hit.collider.gameObject;
        float maxDist = Mathf.Max(Mathf.Abs(mouseOver.transform.position.x-totem.transform.position.x), Mathf.Abs(mouseOver.transform.position.z-totem.transform.position.z));
        int seeDist = totem.GetComponent<CPU>().sight;
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
            if (mouseOver.GetComponent<Battery>()!=null && mouseOver.GetComponent<Upgrade>().cpu!=null && mouseOver.GetComponent<Upgrade>().cpu!=totem){
              reticuleCharge.SetActive(true);
              reticuleCharge.transform.position = mouseOver.transform.position;
            } else {
              reticuleUpgrade.SetActive(true);
              reticuleUpgrade.transform.position = mouseOver.transform.position;
            }
          }
        }
      } else {
        mouseOver=null;
      }
      if (Input.GetMouseButtonUp(0)&&uiBlocker==false){
        pickDefaultAction();
      }
    }
    if (mode==2){
      //TEMPORARY:

      if (selectedUpgrade!=null){
        if (selectedUpgrade.GetComponent<CPU>()!=null) selectedUpgrade=null;
      }
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)&&uiBlocker==false){
        mouseOver = hit.collider.gameObject;
        float maxDist = Mathf.Max(Mathf.Abs(mouseOver.transform.position.x-totem.transform.position.x), Mathf.Abs(mouseOver.transform.position.z-totem.transform.position.z));
        if (maxDist<=1.2f){
          if (mouseOver.GetComponent<Tile>()!=null && selectedUpgrade!=null){
            reticuleUpgrade.SetActive(true);
            reticuleUpgrade.transform.position = mouseOver.transform.position;
          }
          if (mouseOver.GetComponent<Upgrade>()!=null){
            reticuleUpgrade.SetActive(true);
            reticuleUpgrade.transform.position = mouseOver.transform.position;
          }
        }
      } else {
        mouseOver=null;
      }
      if (Input.GetMouseButtonUp(0) && uiBlocker==false && mouseOver!=null){
        if (selectedUpgrade==null){
          if (mouseOver.GetComponent<Upgrade>()!=null && mouseOver.GetComponent<CPU>()==null) selectedUpgrade=mouseOver;
        } else if (selectedUpgrade==mouseOver){
          selectedUpgrade = null;
        } else {
          Car carVars = totem.GetComponent<CPU>().cars[0].GetComponent<Car>();
          GameObject[] slots = carVars.upgradeTile.GetComponent<Tile>().heightSlots;
          if (mouseOver.GetComponent<Tile>()!=null){
            selectedUpgrade.transform.parent = null;
            selectedUpgrade.transform.position = mouseOver.transform.position;
            selectedUpgrade.GetComponent<Upgrade>().cpu = null;
            selectedUpgrade.GetComponent<ActualThing>().moveOntoTile(mouseOver,0);
            selectedUpgrade = null;
            totem.GetComponent<CPU>().setUpUpgrades();
          }
          Upgrade mOverUp = mouseOver.GetComponent<Upgrade>();
          if (mOverUp!=null){
            if (selectedUpgrade.GetComponent<Upgrade>().cpu==null){
              selectedUpgrade.transform.parent = totem.GetComponent<CPU>().cars[0].transform;
              selectedUpgrade.transform.position = mouseOver.transform.position;
              selectedUpgrade.GetComponent<ActualThing>().moveOntoTile(mouseOver.GetComponent<ActualThing>().tile,Mathf.RoundToInt(mouseOver.transform.position.y*.5f));
              selectedUpgrade = null;
              totem.GetComponent<CPU>().setUpUpgrades();
            }
          }
        }
      }

    }
    if (mode==3){
      if (totem != null){
        float opacity = .01f+Random.value;
        opacity *= 10f-totem.GetComponent<CPU>().scanner;
        opacity = Mathf.Clamp(opacity,0,1f);
        scannerNoise.GetComponent<CanvasRenderer>().SetAlpha(opacity);
      }
      if (Input.GetMouseButtonUp(0)){
        setMode(1);
      }
    }
    updateVisibleTiles();
    uiBlocker = false;
  }

  void LateUpdate(){
    cleanUpTiles();
    if (nextMode!=mode){
      if (mode==1){}
      if (mode==2){
        inventory.SetActive(false);
      }
      if (mode==3){
        scanner.SetActive(false);
      }
      if (nextMode==1){
        mode=1;
      }
      if (nextMode==2){
        mode=2;
        inventory.SetActive(true);
        //upgradeWith(totem.objective);
      }
      if (nextMode==3){
        mode=3;
        scanner.SetActive(true);
      }
    }
  }

  public void setMode(int newMode){
    nextMode = newMode;
    uiBlocker = true;
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
      if (mouseOver.GetComponent<Battery>()!=null && mouseOver.GetComponent<Upgrade>().cpu!=null && mouseOver.GetComponent<Upgrade>().cpu!=totem){
        totem.GetComponent<CPU>().chargeBot(mouseOver);
        totem.GetComponent<CPU>().objective = mouseOver;
      } else {
        totem.GetComponent<CPU>().upgrade(mouseOver);
        totem.GetComponent<CPU>().objective = mouseOver;
      }
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
      int level = Mathf.CeilToInt(Vector2Int.Distance(targetFloor,Vector2Int.zero)*.02f);
      List<GameObject> btOptions = new List<GameObject>();
      for (int n = bigTilePrefabs.Length-1; n>=0; n--){
        BigTile btVars = bigTilePrefabs[n].GetComponent<BigTile>();
        if (btVars.minLevel<=level && btVars.maxLevel>=level){
          int prei = 11-btVars.rarity;
          if (prei<1) prei=1;
          for (int i = prei; i>=0; i--){
            btOptions.Add(bigTilePrefabs[n]);
          }
        }
      }
      float[] rands = getRands(targetFloor);
      btPrefab = btOptions[Mathf.FloorToInt(rands[0]*(btOptions.Count))];
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
        candidate.GetComponent<Tile>().setFog(2);
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

  public void moveFog(){
    if (totem==null) return;
    fog1=totem.GetComponent<CPU>().sight;
    int boxChange = fog2-(fog1+totem.GetComponent<CPU>().memory);
    fog2 = fog1+totem.GetComponent<CPU>().memory;
    Vector2Int currentTotemPos = totem.GetComponent<CPU>().cars[0].GetComponent<Car>().tile.GetComponent<Tile>().pos;
    if (currentTotemPos.x>totemPos.x) totemPos+=Vector2Int.right;
    if (currentTotemPos.x<totemPos.x) totemPos+=Vector2Int.left;
    if (currentTotemPos.y>totemPos.y) totemPos+=Vector2Int.up;
    if (currentTotemPos.y<totemPos.y) totemPos+=Vector2Int.down;
    //update tiles affected
    GameObject[,] fogSquare = getSquare(new Vector3Int(totemPos.x,totemPos.y,fog2+boxChange+1));
    for (int x = 0; x<fogSquare.GetLength(0); x++){
      for (int y = 0; y<fogSquare.GetLength(1); y++){
        Tile t = fogSquare[x,y].GetComponent<Tile>();
        int fog = 0;
        if (Mathf.Abs(t.pos.x-totemPos.x)>fog1 || Mathf.Abs(t.pos.y-totemPos.y)>fog1) fog = 1;
        if (Mathf.Abs(t.pos.x-totemPos.x)>fog2 || Mathf.Abs(t.pos.y-totemPos.y)>fog2) fog = 2;
        t.setFog(fog);
      }
    }
    //Debug.Log(getTile(Vector2Int.zero).GetComponent<Tile>().fogLevel);
  }

  public void cleanUpTiles(){
    foreach (GameObject tile in cleanupQueue){
      cleanUpThisTile(tile);
    }
    cleanupQueue.Clear();
  }

  public void cleanUpThisTile(GameObject tile){
    Tile tileVars = tile.GetComponent<Tile>();
    GameObject[] slots = tileVars.heightSlots;
    GameObject[] newSlots = (GameObject[]) slots.Clone();
    int writeToHeight = 0;
    GameObject currentThing = null;
    for (int h=0; h<slots.Length; h++){
      newSlots[h] = null;
      if (slots[h]!=null){
        if (slots[h].GetComponent<ActualThing>().flying==true) writeToHeight=h;
        newSlots[writeToHeight] = slots[h];
        if (slots[h]!=currentThing){
          Vector3 pos = slots[h].transform.position;
          slots[h].transform.position = new Vector3(pos.x, (float)writeToHeight*.5f, pos.z);
          currentThing = slots[h];
        }
        writeToHeight++;
      }
    }
    slots = newSlots;
  }
}
