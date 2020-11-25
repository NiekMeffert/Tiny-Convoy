using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualThing : MonoBehaviour
{
  public GameObject tile;
  GameController gameController;

  // Start is called before the first frame update
  void Start()
  {
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    tile.GetComponent<Tile>().full = true;
  }

  // Update is called once per frame
  void Update()
  {

  }

  void moveOntoTile(GameObject newTile){
    if (tile==null) {return;}
    newTile.GetComponent<Tile>().full = true;
    tile.GetComponent<Tile>().full = false;
    tile = newTile;
  }

  string analyze(){
    return "Nothing much.";
  }
}
