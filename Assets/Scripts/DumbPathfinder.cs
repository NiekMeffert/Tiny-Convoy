using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbPathfinder : Pathfinder
{

  // Start is called before the first frame update
  void Start(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
  }

  // Update is called once per frame
  void Update(){
    //Have a destination & in normal game mode
    Car car = cpu.cars[0].GetComponent<Car>();
    if (destination==null || gameController.mode!=1 || car.tile==null) return;
    Vector3 dir3 = destination.transform.position-transform.position;
    Vector2 dir2 = new Vector2(dir3.x, dir3.z);
    dir2 = Vector2.ClampMagnitude(dir2, cpu.fSpeed*Time.deltaTime);
    cpu.cars[0].transform.position += new Vector3(dir2.x, 0, dir2.y);
    car.moveOntoTile(gameController.getTile(Vector2Int.RoundToInt(cpu.cars[0].transform.position)),0);
    if (dir3.magnitude<.1) stop();
  }

  public override void moveToTile(GameObject tile){
    destination=tile;
    cpu.startMovers();
  }

  public override void moveNextTo(GameObject tile){
    GameObject[,] adjacents = gameController.getSquare(new Vector3Int(tile.GetComponent<Tile>().pos.x, tile.GetComponent<Tile>().pos.y, 1));
    destination=adjacents[Mathf.RoundToInt(Random.Range(0,2)), Mathf.RoundToInt(Random.Range(0,2))];
    if (destination==tile){
      destination=adjacents[0,0];
    }
    cpu.startMovers();
  }
}
