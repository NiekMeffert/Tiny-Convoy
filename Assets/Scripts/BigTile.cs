using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTile : MonoBehaviour
{

  public Vector2Int pos;
  public int rarity;
  public GameObject[,] tiles = new GameObject[10,10];
  List<GameObject> bigObstructions = new List<GameObject>();
  List<GameObject> mysteryBoxes = new List<GameObject>();
  GameController gameController;
  public int minLevel;
  public int maxLevel;

    // Start is called before the first frame update
    void Start(){
      gameController=GameObject.Find("GameController").GetComponent<GameController>();
      foreach (Transform child in transform){
        if (child.gameObject.GetComponent<ThingOnBigTile>()!=null){
          bigObstructions.Add(child.gameObject);
        }
        if (child.gameObject.GetComponent<MysteryBox>()!=null){
          mysteryBoxes.Add(child.gameObject);
        }
      }
      foreach (GameObject a in bigObstructions){
        setUpBigObstruction(a);
      }
      foreach (GameObject a in mysteryBoxes){
        setUpMysteryBox(a);
      }
    }

    // Update is called once per frame
    void Update(){

    }

    void setUpBigObstruction(GameObject obs){
      float[] rands = gameController.getRands(new Vector2Int(Mathf.RoundToInt(obs.transform.position.x), Mathf.RoundToInt(obs.transform.position.z)));
      //pseudorandomly rotate
      obs.transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[0])*90f, 0), Space.World);
      //pseudorandomly reposition
      Vector2 wiggleRoom = obs.GetComponent<ThingOnBigTile>().wiggleRoom;
      Vector3 wiggle = new Vector3(Mathf.Round((rands[1]*wiggleRoom.x*2)-wiggleRoom.x), 0, Mathf.Round((rands[2]*wiggleRoom.y*2)-wiggleRoom.y));
      obs.transform.position += wiggle;
      //move everything inside onto tiles
      foreach (Transform child in obs.transform){
        ActualThing actT = child.gameObject.GetComponent<ActualThing>();
        if (actT!=null) actT.setUpActualThing();
      }
    }

    void setUpMysteryBox(GameObject box){
      MysteryBox boxVars = box.GetComponent<MysteryBox>();
      GameObject tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(box.transform.position.x), Mathf.RoundToInt(box.transform.position.z)));
      float[] rands = gameController.getRands(tempTile.GetComponent<Tile>().pos);
      if (rands[0]>boxVars.nothingChance){
        float newX = box.transform.position.x + Mathf.Round((rands[1]*boxVars.wiggleRoom.x*2)-boxVars.wiggleRoom.x);
        float newY = box.transform.position.z + Mathf.Round((rands[2]*boxVars.wiggleRoom.y*2)-boxVars.wiggleRoom.y);
        newX = Mathf.Clamp(newX, transform.position.x, transform.position.x+9f);
        newY = Mathf.Clamp(newY, transform.position.z, transform.position.z+9f);
        GameObject newThing = Instantiate(boxVars.options[Mathf.FloorToInt(rands[3]*boxVars.options.Length)]);
        tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY)));
        int fitSlot = gameController.canFit(newThing, tempTile, true);
        newThing.transform.position = new Vector3(newX, fitSlot*.5f, newY);
        newThing.transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[4])*90f, 0), Space.World);
        newThing.transform.parent = box.transform.parent;
        if (fitSlot == -1){
          Destroy(newThing);
        }
      }
      Destroy(box);
    }
}
