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
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    moveOntoTile(myTile);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void moveOntoTile(GameObject newTile){
    newTile.GetComponent<Tile>().full = true;
    if (tile!=null) tile.GetComponent<Tile>().full = false;
    tile = newTile;
  }

  string analyze(){
    return "Nothing much.";
  }
}
