using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : ActualThing
{
  public GameObject cpu;
  public List<GameObject> upgrades;
  public GameObject upgradeTile;
  public int carNumber;

  void Awake(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
    UpgradeTile upTileVars = upgradeTile.GetComponent<UpgradeTile>();
    upTileVars.car = gameObject;
    upTileVars.pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
  }

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update(){}

  public void registerElements(){
    Tile tileVars = tile.GetComponent<Tile>();
    tileVars.fixHeights();
    Tile upTileVars = upgradeTile.GetComponent<Tile>();
    upTileVars.fixHeights();
    transform.position = new Vector3(transform.position.x, -.1f, transform.position.z);
    bottomTop[0]=transform.position.y; bottomTop[1]=transform.position.y+height;
    bool dontLiftCar = false;
    List<GameObject> listCopy = new List<GameObject>(tileVars.actualThings);
    for (int i=0; i<listCopy.Count; i++){
      GameObject aThing = listCopy[i];
      if (aThing.GetComponent<UpgradeSpacer>()!=null) continue;
      ActualThing thingVars = aThing.GetComponent<ActualThing>();
      if (dontLiftCar==false && aThing.GetComponent<Upgrade>()==null && aThing!=gameObject){
        transform.position = new Vector3(transform.position.x, thingVars.bottomTop[1], transform.position.z);
      } else {
        dontLiftCar=true;
      }
      if (thingVars.flying==true) break;
      if (aThing.transform.position.y>=transform.position.y && aThing!=gameObject) upTileVars.moveOntoTile(aThing);
    }
    upTileVars.fixHeights();
    tileVars.fixHeights();
    height = 0;
    mass = 0;
    upgrades.Clear();
    foreach (GameObject aThing in upTileVars.actualThings){
      ActualThing thingVars = aThing.GetComponent<ActualThing>();
      height+=thingVars.height;
      mass+=thingVars.mass;
      aThing.transform.parent = gameObject.transform;
      aThing.transform.rotation = gameObject.transform.rotation;
      Upgrade upgradeVars = aThing.GetComponent<Upgrade>();
      if (upgradeVars!=null){
        upgrades.Add(aThing);
        upgradeVars.cpu = cpu;
        upgradeVars.turnOn();
      }
    }
    bottomTop[0]=transform.position.y; bottomTop[1]=transform.position.y+height;
    upTileVars.fixHeights();
    tileVars.fixHeights();
  }


  public override void setFog(int nextFog){
    if (nextFog==fogLevel) return;
    fogLevel=nextFog;
    upgradeTile.GetComponent<Tile>().setFog(nextFog);
  }

  public override void die(float afterTime){
    Tile tileVars = tile.GetComponent<Tile>();
    UpgradeTile upTileVars = upgradeTile.GetComponent<UpgradeTile>();
    List<GameObject> listCopy = new List<GameObject>(upTileVars.actualThings);
    foreach (GameObject aThing in listCopy){
      tileVars.moveOntoTile(aThing);
    }
    tileVars.removeFromTile(gameObject);
    Destroy(gameObject);
  }
}
