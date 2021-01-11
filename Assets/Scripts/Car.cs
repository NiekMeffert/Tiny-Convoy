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

  public void registerElements(GameObject fromTile){
    height = 0;
    mass = 0;
    upgrades.Clear();
    Tile tileVars = fromTile.GetComponent<Tile>();
    bool dontLiftCar = false;
    List<GameObject> listCopy = new List<GameObject>(tileVars.actualThings);
    for (int i=0; i<listCopy.Count; i++){
      GameObject aThing = listCopy[i];
      if (aThing == gameObject) continue;
      ActualThing thingVars = aThing.GetComponent<ActualThing>();
      if (dontLiftCar==false && aThing.GetComponent<Upgrade>()==null){
        transform.position = new Vector3(transform.position.x, thingVars.bottomTop[1], transform.position.z);
      } else {
        dontLiftCar=true;
      }
      if (thingVars.flying == true) break;
      if (aThing.transform.position.y>=transform.position.y){
        height+=thingVars.height;
        mass+=thingVars.mass;
        aThing.transform.parent = gameObject.transform;
        Upgrade upgradeVars = aThing.GetComponent<Upgrade>();
        if (upgradeVars!=null){
          upgrades.Add(aThing);
          upgradeVars.cpu = cpu;
        }
        upgradeTile.GetComponent<Tile>().moveOntoTile(aThing);
      }
    }
    bottomTop[0]=transform.position.y; bottomTop[1]=transform.position.y+height;
  }

  public override void setFog(int nextFog){
    if (nextFog==fogLevel) return;
    fogLevel=nextFog;
    upgradeTile.GetComponent<Tile>().setFog(nextFog);
  }
}
