using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTile : Tile{
  public GameObject car;

  // Start is called before the first frame update
  void Start(){}

  // Update is called once per frame
  void Update(){}

  public override void removeFromTile(GameObject load){
    Car carVars = car.GetComponent<Car>();
    actualThings.Remove(load);
    fixHeightsNeeded=true;
    Upgrade upVars = load.GetComponent<Upgrade>();
    if (upVars!=null){
      upVars.cpu = null;
      carVars.upgrades.Remove(load);
      load.transform.parent = null;
    }
  }
}
