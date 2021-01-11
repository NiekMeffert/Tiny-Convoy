using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : ActualThing
{
  public GameObject[] options;
  public float nothingChance;
  public Vector2Int wiggleRoom;

  // Start is called before the first frame update
  void Start(){
  }

  // Update is called once per frame
  void Update(){}

  public override void setUpPosition(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
    GameObject tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    Tile tileVars = tempTile.GetComponent<Tile>();
    float[] rands = gameController.getRands(tempTile.GetComponent<Tile>().pos);
    if (rands[0]>nothingChance){
      float newX = transform.position.x + Mathf.Round((rands[1]*wiggleRoom.x*2)-wiggleRoom.x);
      float newY = transform.position.z + Mathf.Round((rands[2]*wiggleRoom.y*2)-wiggleRoom.y);
      newX = Mathf.Clamp(newX, transform.position.x, transform.position.x+9f);
      newY = Mathf.Clamp(newY, transform.position.z, transform.position.z+9f);
      GameObject newThing = Instantiate(options[Mathf.FloorToInt(rands[3]*options.Length)]);
      tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY)));
      float fit = tileVars.canFit(newThing, true);
      if (fit < 0){
        Destroy(newThing);
      } else {
        newThing.transform.position = new Vector3(newX,fit,newY);
        newThing.transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[4])*90f, 0), Space.World);
        newThing.transform.parent = transform.parent;
      }
    }
    Destroy(gameObject);
  }
}
