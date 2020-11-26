using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : ActualThing
{
  public GameObject owner;
  public float health;
  public float maxHealth;
  public float drain;

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
    //move from tile to tile - push
    if (tile.GetComponent<Tile>()!=null && newTile.GetComponent<Tile>()!=null){
      newTile.GetComponent<Tile>().full = true;
      if (tile!=null) tile.GetComponent<Tile>().full = false;
      tile = newTile;
    }
    //move from tile to upgradeTile - use
    if (tile.GetComponent<Tile>()!=null && newTile.GetComponent<UpgradeTile>()!=null){
      newTile.GetComponent<UpgradeTile>().full = true;
      if (tile!=null) tile.GetComponent<Tile>().full = false;
      tile = newTile;
    }
    //move from upgradeTile to upgradeTile - rewire
    if (tile.GetComponent<UpgradeTile>()!=null && newTile.GetComponent<UpgradeTile>()!=null){
      newTile.GetComponent<UpgradeTile>().full = true;
      if (tile!=null) tile.GetComponent<UpgradeTile>().full = false;
      tile = newTile;
    }
    //move from upgradeTile to tile - drop
    if (tile.GetComponent<UpgradeTile>()!=null && newTile.GetComponent<Tile>()!=null){
      newTile.GetComponent<Tile>().full = true;
      if (tile!=null) tile.GetComponent<UpgradeTile>().full = false;
      tile = newTile;
    }
  }
}
