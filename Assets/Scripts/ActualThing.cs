using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualThing : MonoBehaviour
{
  public GameObject tile;
  public GameController gameController;
  public float mass;
  public Vector2 momentum;
  public int height;
  public bool standable;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void setUpActualThing(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    int fit = gameController.canFit(gameObject, myTile, true);
    if (fit==-1){
      Destroy(gameObject);
    } else {
      if (fit>Mathf.RoundToInt(transform.position.y*2f)) transform.position += new Vector3(0,(float)fit*.5f,0);
      moveOntoTile(myTile, fit);
    }
  }

  public virtual void moveOntoTile(GameObject newTile, int heightSlot){
    if (newTile==tile) return;
    Tile newTileVars = newTile.GetComponent<Tile>();
    for (int h=heightSlot; h<height+heightSlot; h++){
      newTileVars.heightSlots[h] = gameObject;
    }
    if (tile!=null){
      GameObject[] oldSlots = tile.GetComponent<Tile>().heightSlots;
      for (int h=0; h<oldSlots.Length; h++){
        if (oldSlots[h]==gameObject) oldSlots[h]=null;
      }
    }
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
