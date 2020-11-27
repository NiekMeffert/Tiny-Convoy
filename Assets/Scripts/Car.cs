using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : ActualThing
{
  public GameObject cpu;
  public GameObject[,] upgrades;
  public int carNumber;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();

  }

  // Update is called once per frame
  void Update()
  {

  }

  public override void moveOntoTile(GameObject newTile){
    newTile.GetComponent<Tile>().full = true;
    if (tile!=null) tile.GetComponent<Tile>().full = false;
    tile = newTile;
  }
}
