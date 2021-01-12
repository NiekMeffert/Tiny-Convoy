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
  public bool moving = false;
  public AI ai;
  //public GameObject marker;

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
    if (destination==null || gameController.mode!=1 || firstCarVars==null) return;
    moveToTile();
  }

  public virtual void setUpPathfinder(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
    ai = gameObject.GetComponent<AI>();
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
      Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(cpu.cars[0].transform.position.x), Mathf.RoundToInt(cpu.cars[0].transform.position.z));
      GameObject maybeNewTile = gameController.getTile(roundedPos);
      if (maybeNewTile!=firstCarVars.tile) {
        Tile tileVars = maybeNewTile.GetComponent<Tile>();
        float fit = tileVars.canFit(cpu.cars[0], true);
        if (Mathf.Abs(cpu.cars[0].transform.position.y-fit)<.1){
          tileVars.moveOntoTile(cpu.cars[0]);
          firstCarVars.upgradeTile.GetComponent<UpgradeTile>().pos = roundedPos;
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

  public virtual void moveNextTo(GameObject tile){
    GameObject adjacentTile = ai.getAdjacentFreeTile(tile);
    if (adjacentTile!=null){
      destination=adjacentTile;
    }
  }

  public virtual void stop(){
    destination=null;
    cpu.stopMovers();
    moving=false;
  }

  public virtual itinerary getItinerary(GameObject tile){
    itinerary i = new itinerary();
    return i;
  }

  public virtual void setItinerary(itinerary newItin){}
}
