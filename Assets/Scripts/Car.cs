using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : ActualThing
{
  public GameObject cpu;
  public GameObject[] upgrades;
  public GameObject upgradeTile;
  public int carNumber;
  bool lateSetup = false;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (lateSetup==false) lateStart();
    //transform.position += new Vector3(0,0,-.5f*Time.deltaTime);
    //moveOntoTile(gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z))), 0);
  }

  void lateStart(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    GameObject tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    GameObject[] slots = tempTile.GetComponent<Tile>().heightSlots;
    for (int i=0; i<slots.Length; i++){
      if (slots[i]!=null) {
        Upgrade upg = slots[i].GetComponent<Upgrade>();
        if (upg!=null){
          height++;
          mass+=upg.mass/upg.height;
          slots[i].transform.parent = gameObject.transform;
          upg.moveOntoTile(upgradeTile, i);
        }
      }
    }
    setUpActualThing();
    lateSetup=true;
  }

  /*public override void moveOntoTile(GameObject newTile, int){
    newTile.GetComponent<Tile>().full = true;
    if (tile!=null) tile.GetComponent<Tile>().full = false;
    tile = newTile;
  }*/
}
