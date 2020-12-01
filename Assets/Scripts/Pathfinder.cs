using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
  public GameController gameController;
  public CPU cpu;
  public GameObject destination;

  // Start is called before the first frame update
  void Start(){
  }

  // Update is called once per frame
  void Update(){
  }

  public virtual void moveToTile(GameObject tile){}

  public virtual void stop(){
    destination=null;
    cpu.stopMovers();
  }
}
