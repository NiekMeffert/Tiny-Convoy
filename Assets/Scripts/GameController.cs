using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour{

  public int mode = 0; //0 paused, 1 normal, 2 upgrade, 3 long-distance scanner, 4 conversation
  public int nextMode = 1;
  public int level = 0;
  public int randomSeedX;
  public int randomSeedY;
  public float totemCounter = 0;
  public GameObject tilePrefab;
  public GameObject CPUPrefab;
  public GameObject carPrefab;
  public GameObject upgradeTilePrefab;
  public GameObject[] plantPrefabs;
  public GameObject[] bigTilePrefabs;
  public GameObject[] specialBigTilePrefabs;
  public GameObject bigBotPrefab;
  public GameObject mainCamera;
  public GameObject lastBigTile;
  public List<GameObject> CPUs = new List<GameObject>();
  public GameObject totem;
  public Vector2Int totemPos;
  public float totemSight = 2;
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
  public List<GameObject> bigBots;
  SparseMatrix<GameObject> forcedBigTiles = new SparseMatrix<GameObject>();
  public GameObject inventory;
  public GameObject scanner;
  public bool uiBlocker = false;
  RectTransform scannerNoise;
  public GameObject[] particles;
  public GameObject selectedUpgrade;
  float upgradeTimer;
  public GameObject upgradeSpacerPrefab;
  public List<GameObject> upgradeSpacers = new List<GameObject>();
  public Upgrade partStat;
  public GameObject upgradeStats;
  public GameObject upgradeStatsUIHolder;
  bool gameHasStarted = false;
    public GameObject GameOverUI;

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
    upgradeStats = GameObject.Find("UpgradesStats");
  }

  // Update is called once per frame
  void Update(){
    reticule.SetActive(false);
    reticulePlant.SetActive(false);
    reticuleUpgrade.SetActive(false);
    reticuleCharge.SetActive(false);
    upgradeStats.SetActive(false);
    partStat = null;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    //paused
    if (mode==0) {
      Time.timeScale = 0;
    }
    //normal game mode
    if (mode==1) {
      Time.timeScale = 1;
      totemCounter-=Time.deltaTime;
      if (totemCounter<0 && CPUs.Count>0){
        gameHasStarted=true;
        totem = CPUs[Mathf.FloorToInt(Random.value*CPUs.Count)];
        totemCounter=120;
        bigBotCheck();
      }
      if (CPUs.Count==0 && gameHasStarted==true){
        GameOverUI.SetActive(true);
      }
      if (totem!=null){
        GameObject firstCar = totem.GetComponent<CPU>().cars[0];
        Vector2Int currPos = firstCar.GetComponent<Car>().tile.GetComponent<Tile>().pos;
        if (totemPos != currPos || totemSight!=totem.GetComponent<CPU>().sight) moveFog(currPos);
        if (Physics.Raycast(ray, out hit) && uiBlocker==false){
          mouseOver = hit.collider.gameObject;
          float maxDist = Mathf.Max(Mathf.Abs(mouseOver.transform.position.x-totem.transform.position.x), Mathf.Abs(mouseOver.transform.position.z-totem.transform.position.z));
          int seeDist = totem.GetComponent<CPU>().sight;
          if (maxDist<=seeDist){
            Tile mouseTile = mouseOver.GetComponent<Tile>();
            Plant mousePlant = mouseOver.GetComponent<Plant>();
            Battery mouseBattery = mouseOver.GetComponent<Battery>();
            Upgrade mouseUpgrade = mouseOver.GetComponent<Upgrade>();
            if (mouseTile!=null){
              if (Mathf.Abs(firstCar.transform.position.y-mouseTile.canFit(firstCar,true))<.1){
                reticule.SetActive(true);
                reticule.transform.position = mouseOver.transform.position;
              }
            } else if (mousePlant!=null){
              if (firstCar.GetComponent<Car>().overlapsVertically(mouseOver)){
                reticulePlant.SetActive(true);
                reticulePlant.transform.position = mouseOver.transform.position;
              } else {
                mouseOver=null;
              }

            } else if (mouseBattery!=null && mouseUpgrade.cpu!=null && mouseUpgrade.cpu!=totem){
              if (firstCar.GetComponent<Car>().overlapsVertically(mouseOver)){
                reticuleCharge.SetActive(true);
                reticuleCharge.transform.position = mouseOver.transform.position;
              } else {
                mouseOver=null;
              }
            } else if (mouseUpgrade!=null){
              upgradeStats.SetActive(true);
              upgradeStats.transform.position = mouseOver.transform.position;
              if (firstCar.GetComponent<Car>().overlapsVertically(mouseOver)){
                reticuleUpgrade.SetActive(true);
                reticuleUpgrade.transform.position = mouseOver.transform.position;
              } else {
                mouseOver=null;
              }
            } else {
              mouseOver=null;
            }
          }
        } else {
          mouseOver=null;
        }
        if (Input.GetMouseButtonUp(0) && uiBlocker==false && mouseOver!=null){
          pickDefaultAction();
        }
      }
    }
    if (mode==2){
      Time.timeScale = 0;
      if (Physics.Raycast(ray, out hit) && uiBlocker==false){
        mouseOver = hit.collider.gameObject;
        Upgrade mouseUpgrade = mouseOver.GetComponent<Upgrade>();
        if (mouseUpgrade!=null) partStat = mouseUpgrade;
        if (mouseOver.GetComponent<ActualThing>()!=null){
          Vector2Int totemTilePos = totem.GetComponent<CPU>().cars[0].GetComponent<Car>().tile.GetComponent<Tile>().pos;
          Vector2Int tilePos = mouseOver.GetComponent<ActualThing>().tile.GetComponent<Tile>().pos;
          if (Mathf.Abs(totemTilePos.x-tilePos.x)>1 || Mathf.Abs(totemTilePos.y-tilePos.y)>1) mouseOver=null;
        } else {
          mouseOver=null;
        }
      } else {
        mouseOver=null;
      }
      if (mouseOver!=null && totem!=null){
        if (selectedUpgrade==null){
          reticuleUpgrade.SetActive(true);
          reticuleUpgrade.transform.position = mouseOver.transform.position;
          if (mouseOver.GetComponent<Upgrade>()!=null){
            upgradeStats.SetActive(true);
            upgradeStats.transform.position = mouseOver.transform.position;
          }
          if (Input.GetMouseButtonDown(0) && mouseOver.GetComponent<ActualThing>()!=null && mouseOver.GetComponent<CPU>()==null && mouseOver.GetComponent<UpgradeSpacer>()==null){
            upgradeTimer = Time.time;
            selectedUpgrade = mouseOver;
          }
        } else {
          if (Input.GetMouseButtonUp(0)){
            Upgrade upVars = selectedUpgrade.GetComponent<Upgrade>();
            if (upVars!=null){
              if (Time.time-upgradeTimer<.2f && selectedUpgrade.GetComponent<CPU>()==null && upVars.cpu!=null){
                if (upVars.on==true){
                  upVars.turnOff();
                } else {
                  upVars.turnOn();
                }
              }
            }
          } else if (mouseOver!=selectedUpgrade){
            Vector3 oldVect = selectedUpgrade.transform.position;
            selectedUpgrade.transform.position = mouseOver.transform.position;
            mouseOver.GetComponent<ActualThing>().tile.GetComponent<Tile>().moveOntoTile(selectedUpgrade);
            mouseOver.GetComponent<ActualThing>().tile.GetComponent<Tile>().fixHeightsNeeded=true;
            HashSet<CPU> uniqueCPUs = new HashSet<CPU>();
            foreach (GameObject upSpacer in upgradeSpacers){
              foreach (GameObject aThing in upSpacer.GetComponent<UpgradeSpacer>().tile.GetComponent<Tile>().actualThings){
                if (aThing.GetComponent<Car>()!=null) uniqueCPUs.Add(aThing.GetComponent<Car>().cpu.GetComponent<CPU>());
              }
            }
            foreach (CPU c in uniqueCPUs) c.setUpUpgrades();
          }
        }
      }
      if (Input.GetMouseButtonUp(0)){
        selectedUpgrade=null;
      }
    }
    if (mode==3){
      Time.timeScale = 0;
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
    if (nextMode!=mode){
      if (mode==1){}
      if (mode==2){
        inventory.SetActive(false);
        removeUpgradeSpacers();
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
        addUpgradeSpacers();
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

  public void addUpgradeSpacers(){
    GameObject[,] tileSquare = getSquare(new Vector3Int(totemPos.x, totemPos.y, 1));
    for (int x = 0; x<tileSquare.GetLength(0); x++){
      for (int y = 0; y<tileSquare.GetLength(1); y++){
        Tile tileVars = tileSquare[x,y].GetComponent<Tile>();
        if (tileVars.actualThings.Count>0){
          int i=0;
          while (i<tileVars.actualThings.Count){
            if (tileVars.actualThings[i].GetComponent<UpgradeSpacer>()==null && (i==tileVars.actualThings.Count-1 || tileVars.actualThings[i].GetComponent<Car>()==null)){
              GameObject spacer = Instantiate(upgradeSpacerPrefab);
              spacer.transform.position = tileVars.actualThings[i].transform.position;
              spacer.transform.position += new Vector3(0, tileVars.actualThings[i].GetComponent<ActualThing>().bottomTop[1], 0);
              tileVars.moveOntoTile(spacer);
              upgradeSpacers.Add(spacer);
            }
            i++;
          }
        } else {
          GameObject spacer = Instantiate(upgradeSpacerPrefab);
          spacer.transform.position = tileSquare[x,y].transform.position;
          tileSquare[x,y].GetComponent<Tile>().moveOntoTile(spacer);
          upgradeSpacers.Add(spacer);
        }
      }
    }
  }

  public void removeUpgradeSpacers(){
    foreach (GameObject ups in upgradeSpacers){
      ups.GetComponent<ActualThing>().tile.GetComponent<Tile>().removeFromTile(ups);
    }
    foreach (GameObject ups in upgradeSpacers){
      Destroy(ups);
    }
    upgradeSpacers.Clear();
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

  public void bigBotCheck(){
    int botNumber = 1 + (Mathf.FloorToInt(level*.4f));
    if (bigBots.Count<botNumber){
      GameObject newBigBot = Instantiate(bigBotPrefab);
      BigBot botVars = newBigBot.GetComponent<BigBot>();
      float edge1 = Random.value * botVars.maxDistance;
      if (Random.value>.5f) edge1 *= -1f;
      float edge2 = botVars.maxDistance;
      if (Random.value>.5f) edge2 *= -1f;
      Vector3 botVec = new Vector3(edge1,0,edge2);
      if (Random.value>.5) botVec = new Vector3(edge2,0,edge1);
      botVec += new Vector3(mainCamera.transform.position.x,0,mainCamera.transform.position.z);
      newBigBot.transform.position = botVec;
      bigBots.Add(newBigBot);
    }
  }

  public void moveFog(Vector2Int newPos){
    fog1=totem.GetComponent<CPU>().sight;
    int boxChange = fog2-(fog1+totem.GetComponent<CPU>().memory);
    fog2 = fog1+totem.GetComponent<CPU>().memory;
    if (newPos.x>totemPos.x) totemPos+=Vector2Int.right;
    if (newPos.x<totemPos.x) totemPos+=Vector2Int.left;
    if (newPos.y>totemPos.y) totemPos+=Vector2Int.up;
    if (newPos.y<totemPos.y) totemPos+=Vector2Int.down;
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
    totemSight=fog1;
  }
}
