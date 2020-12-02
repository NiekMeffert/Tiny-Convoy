using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingOnBigTile : MonoBehaviour
{
  public Vector2 wiggleRoom;
  GameController gameController;

  // Start is called before the first frame update
  void Start(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    float[] rands = gameController.getRands(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    //pseudorandomly rotate
    transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[0])*90f, 0), Space.World);
    //pseudorandomly reposition
    Vector3 wiggle = new Vector3(Mathf.Round((rands[1]*wiggleRoom.x*2)-wiggleRoom.x), 0, Mathf.Round((rands[2]*wiggleRoom.y*2)-wiggleRoom.y));
    transform.position += wiggle;
    //move everything inside
    foreach (Transform child in transform){
      ActualThing actT = child.gameObject.GetComponent<ActualThing>();
      if (actT!=null) actT.setUpActualThing();
    }
  }

  // Update is called once per frame
  void Update(){

  }
}
