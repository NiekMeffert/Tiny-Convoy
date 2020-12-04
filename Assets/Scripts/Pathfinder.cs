using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
  public class navTile
  {
    public GameObject tile;
    public Tile tileVars;
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public List<navTile> adjacencyList = new List<navTile>();
    public bool visited = false;
    public navTile parent = null;
    public int distance = 0;
  }

  public GameController gameController;
  public CPU cpu;
  public GameObject destination;
  public Car firstCarVars;
  public Stack<navTile> path = new Stack<navTile>();
  public List<navTile> selectableTiles = new List<navTile>();
  public navTile currentTile;

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

  public virtual void setUpPathfinder(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
    firstCarVars = cpu.cars[0].GetComponent<Car>();
  }

  public virtual void moveToTile(){
    //Have a destination & in normal game mode
    Vector3 dir3 = destination.transform.position-transform.position;
    Vector3 twoDDir = new Vector3(dir3.x, 0, dir3.z);
    Vector3 twoDDirClamped = Vector3.ClampMagnitude(twoDDir, cpu.hSpeed*Time.deltaTime);
    Quaternion lookRot = Quaternion.LookRotation(twoDDir, Vector3.up);
    float rotAngle = Mathf.Min(cpu.turnSpeed*Time.deltaTime, Quaternion.Angle(cpu.cars[0].transform.rotation, lookRot));
    cpu.cars[0].transform.rotation = Quaternion.Slerp(cpu.cars[0].transform.rotation, lookRot, rotAngle);
    if (cpu.waitForRotation==false || rotAngle<.1f) {
      cpu.cars[0].transform.position += twoDDirClamped;
      cpu.cars[0].GetComponent<Car>().moveOntoTile(gameController.getTile(Vector2Int.RoundToInt(cpu.cars[0].transform.position)),0);
    }
    if (twoDDir.magnitude<.1f && rotAngle<.1f) {
      transform.position = destination.transform.position;
      transform.rotation = lookRot;
      stop();
    }
  }

  public virtual void moveNextTo(GameObject tile){}

  public virtual void stop(){
    destination=null;
    cpu.stopMovers();
  }

  public virtual (float, Stack<navTile>, List<navTile>, navTile, navTile) getPath(GameObject tile){
    float cost = 0;
    return (cost, path, selectableTiles, currentTile, currentTile);
  }
}
