using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUBox : MysteryBox
{
  int altNewThing;
  Color randColor;
  GameObject spawned;
  bool spawnUnspawn = false;

  // Start is called before the first frame update
  void Start(){
  }

  // Update is called once per frame
  void Update(){
    if (spawnUnspawn==true){
      if (spawned==null){
        spawned = makeThing();
      } else {
        spawned.GetComponent<ActualThing>().die(0);
      }
      spawnUnspawn = false;
    }
    if (spawned!=null){
      if (spawned.GetComponent<ActualThing>().tile.GetComponent<Tile>().pos!=tile.GetComponent<Tile>().pos) die(0);
    }
  }

  public override void setUpPosition(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
    height = gameObject.GetComponent<BoxCollider>().size.y*transform.lossyScale.y;
    bottomTop[0]=transform.position.y; bottomTop[1]=transform.position.y+height;
    Tile tileVars = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z))).GetComponent<Tile>();
    float[] rands = gameController.getRands(tileVars.pos);
    float newX = transform.position.x + Mathf.Round((rands[1]*wiggleRoom.x*2)-wiggleRoom.x);
    float newY = transform.position.z + Mathf.Round((rands[2]*wiggleRoom.y*2)-wiggleRoom.y);
    Vector3 btPos = tileVars.bigTile.transform.position;
    newX = Mathf.Clamp(newX, btPos.x, btPos.x+9f);
    newY = Mathf.Clamp(newY, btPos.z, btPos.z+9f);
    transform.position = new Vector3(newX,0,newY);
    transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[4])*90f, 0), Space.World);
    tileVars = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z))).GetComponent<Tile>();
    tileVars.moveOntoTile(gameObject);
    //get stuff for generated object
    altNewThing = Mathf.FloorToInt(rands[3]*options.Length);
    randColor = new Color(rands[5],rands[6],rands[7],1);
  }

  public override void setFog(int nextFog){
    if (nextFog==fogLevel) return;
    if (nextFog<2 && fogLevel==2){
      //create
      spawnUnspawn = true;
      fogLevel=nextFog;
    } else if (nextFog==2 && fogLevel<2){
      //destroy
      if (spawned!=null) spawnUnspawn = true;
      fogLevel=nextFog;
    }
  }

  GameObject makeThing(){
    GameObject newThing;
    if (gameController.botsAllowed>gameController.CPUs.Count){
      newThing = Instantiate(gameController.CPUPrefab);
    } else {
      newThing = Instantiate(options[altNewThing]);
    }
    newThing.transform.position = new Vector3(transform.position.x, .01f, transform.position.z);
    newThing.transform.rotation = transform.rotation;
    if (newThing.GetComponent<Upgrade>()!=null){
      foreach (Renderer rend in newThing.GetComponentsInChildren<Renderer>()){
        for (int i=0; i<rend.materials.Length; i++){
          if (rend.materials[i].name.StartsWith("RobotBas")) {
            rend.materials[i].color = randColor;
          }
        }
      }
    }
    newThing.GetComponent<ActualThing>().setUpVars();
    newThing.GetComponent<ActualThing>().setUpPosition();
    return newThing;
  }

}
