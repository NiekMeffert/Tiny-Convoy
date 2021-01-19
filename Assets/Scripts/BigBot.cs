using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : MonoBehaviour
{
  public float maxDistance = 60f;
  public float currentDistance;
  public GameObject rightFoot;
  public GameObject leftFoot;
  Vector3 nextHeading;
  Vector4 safeBox;
  Vector3 flipX = new Vector3(-1,1,1);
  Vector3 flipZ = new Vector3(1,1,-1);
  GameController gameController;
  bool foot;

  // Start is called before the first frame update
  void Start(){
    nextHeading = Vector3.forward;
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    newHeading();
  }

  // Update is called once per frame
  void Update(){
    if (gameController.mode!=1) return;
    //safeBox.Set(gameController.mainCamera.transform.position.x+maxDistance, gameController.mainCamera.transform.position.z+maxDistance, gameController.mainCamera.transform.position.x-maxDistance, gameController.mainCamera.transform.position.z-maxDistance);
    transform.position += Vector3.ClampMagnitude(transform.forward, 1.5f*Time.deltaTime);
    Quaternion headingQuat = Quaternion.LookRotation(nextHeading, Vector3.up);
    float ang = Quaternion.Angle(headingQuat,transform.rotation);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, headingQuat, Mathf.Min(ang,2f*Time.deltaTime));
    currentDistance = Vector3.Distance(gameController.mainCamera.transform.position, transform.position);
    if (currentDistance>maxDistance) newHeading();
    for (int i = 0; i<7; i++){
      Vector3 footPos;
      if (foot==true) {
        footPos = rightFoot.transform.position;
        foot=false;
      } else {
        footPos = leftFoot.transform.position;
        foot=true;
      }
      Vector2 wrathPoint = (Random.insideUnitCircle*4f) + new Vector2(transform.position.x, transform.position.z);
      //GameObject tile = getTileIfAlreadyCreated(wrathPoint);
      Vector2Int target = Vector2Int.RoundToInt(wrathPoint);
      GameObject tile = gameController.getTile(target);
      if (tile!=null){
        //Killing a Car empties out its components, so total party kill the tile twice:
        Tile tileVars = tile.GetComponent<Tile>();
        List<GameObject> listCopy = new List<GameObject>(tileVars.actualThings);
        foreach (GameObject aThing in listCopy){
          if (aThing.GetComponent<ActualThing>().maxHealth!=-1){
            aThing.GetComponent<ActualThing>().die(0);
          }
        }
        listCopy = new List<GameObject>(tileVars.actualThings);
        foreach (GameObject aThing in listCopy){
          if (aThing.GetComponent<ActualThing>().maxHealth!=-1){
            aThing.GetComponent<ActualThing>().die(0);
          }
        }
      }
    }
  }

  void newHeading(){
    nextHeading = gameController.mainCamera.transform.position - transform.position;
    nextHeading.y = 0;
    nextHeading.Normalize();
  }

  public GameObject getTileIfAlreadyCreated(Vector2 point){
    Vector2Int target = Vector2Int.RoundToInt(point);
    GameObject bigTile = null;
    Vector2Int targetFloor = new Vector2Int(Mathf.FloorToInt(target.x/10f)*10, Mathf.FloorToInt(target.y/10f)*10);
    GameObject[] allBigTiles = GameObject.FindGameObjectsWithTag("BigTile");
    foreach (GameObject candidate in allBigTiles){
      if (candidate.GetComponent<BigTile>().pos.x==targetFloor.x && candidate.GetComponent<BigTile>().pos.y==targetFloor.y){
        bigTile = candidate;
      }
    }
    if (bigTile==null) return null;
    BigTile bigTileVars=bigTile.GetComponent<BigTile>();
    GameObject tile = bigTileVars.tiles[target.x-bigTileVars.pos.x, target.y-bigTileVars.pos.y];
    return tile;
  }
}
