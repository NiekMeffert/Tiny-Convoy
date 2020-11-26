using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualThing : MonoBehaviour
{
  public GameObject tile;
  GameController gameController;
  public float mass;
  public Vector2 momentum;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void setUpActualThing(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    moveOntoTile(myTile);
  }

  public virtual void moveOntoTile(GameObject newTile){
    newTile.GetComponent<Tile>().full = true;
    if (tile!=null) tile.GetComponent<Tile>().full = false;
    tile = newTile;
  }

  public virtual string analyze(){
    return "Nothing much.";
  }

  public virtual void bumpInto(GameObject otherThing){
  }

  public virtual void voxellate(bool voxIt){
    if (voxIt==true){
      //choose geometry based on surrounding NESW objects of same type
    } else {
      //set to no surroundings voxel
    }
  }
}
