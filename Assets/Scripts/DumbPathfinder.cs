using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbPathfinder : Pathfinder
{

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
    if (destination!=null || gameController.mode==1 || firstCarVars.tile!=null){
      moveToTile();
    }
  }

  public override void moveNextTo(GameObject tile){
    GameObject[,] adjacents = gameController.getSquare(new Vector3Int(tile.GetComponent<Tile>().pos.x, tile.GetComponent<Tile>().pos.y, 1));
    destination=adjacents[Mathf.RoundToInt(Random.Range(0,2)), Mathf.RoundToInt(Random.Range(0,2))];
    if (destination==tile){
      destination=adjacents[0,0];
    }
  }
}
