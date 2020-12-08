using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

  public GameController gameController;
  public CPU cpu;
  public GameObject destination;
  public Car firstCarVars;
  public Stack<navTile> path = new Stack<navTile>();
  public List<navTile> selectableTiles = new List<navTile>();
  public navTile currentTile;
  public bool moving = false;
  //public GameObject marker;

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
    if (destination==null) return;
    if (gameController.mode!=1 || firstCarVars.tile==null) return;
    moveToTile();
  }

  public virtual void setUpPathfinder(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
    firstCarVars = cpu.cars[0].GetComponent<Car>();
  }

  public virtual void moveToTile(){
    //Have a destination & in normal game mode
    if (moving==false) cpu.startMovers();
    moving=true;
    float totalTurn = 0;
    Vector3 dir3 = destination.transform.position-cpu.cars[0].transform.position;
    Vector3 twoDDir = new Vector3(dir3.x, 0, dir3.z);
    Vector3 twoDCar = new Vector3(cpu.cars[0].transform.position.x, 0, cpu.cars[0].transform.position.z);
    if (twoDDir.sqrMagnitude>.01f){
      Quaternion lookRot = Quaternion.LookRotation(twoDDir, Vector3.up);
      Quaternion carRot = Quaternion.LookRotation(twoDCar, Vector3.up);
      totalTurn = Quaternion.Angle(carRot, lookRot);
      float frameTurn = Mathf.Min(360f*cpu.turnSpeed*Time.deltaTime, totalTurn);
      cpu.cars[0].transform.rotation = Quaternion.Slerp(cpu.cars[0].transform.rotation, lookRot, frameTurn/totalTurn);
    }
    if (cpu.waitForRotation==false || totalTurn<.01f) {
      Vector3 twoDDirClamped = Vector3.ClampMagnitude(twoDDir, cpu.hSpeed*Time.deltaTime);
      cpu.cars[0].transform.position += twoDDirClamped;
      GameObject maybeNewTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(cpu.cars[0].transform.position.x), Mathf.RoundToInt(cpu.cars[0].transform.position.z)));
      if (maybeNewTile!=firstCarVars.tile) {
        //GameObject mkr2 = Instantiate(marker);
        //mkr2.transform.position = cpu.cars[0].GetComponent<Car>().tile.transform.position;
        int fitSlot = gameController.canFit(cpu.cars[0], maybeNewTile);
        if (fitSlot==0){
          firstCarVars.moveOntoTile(maybeNewTile,0);

        } else {
          cpu.cars[0].transform.position -= twoDDirClamped;
          stop();
        }

      }
    }
    if (twoDDir.magnitude<.1f && totalTurn<.01f) {
      cpu.cars[0].transform.position = destination.transform.position;
      //cpu.cars[0].transform.rotation = lookRot;
      stop();
    }
  }

  public virtual void moveNextTo(GameObject tile){}

  public virtual void stop(){
    destination=null;
    cpu.stopMovers();
    moving=false;
  }

  public virtual (float, Stack<navTile>, List<navTile>, navTile, navTile) getPath(GameObject tile){
    float cost = 0;
    return (cost, path, selectableTiles, currentTile, currentTile);
  }
}
