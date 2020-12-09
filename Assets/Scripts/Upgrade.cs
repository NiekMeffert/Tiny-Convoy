using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : ActualThing
{
  public GameObject cpu;
  public float health;
  public float maxHealth;
  public bool on;
  public float drain;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  //public override void moveOntoTile(GameObject newTile, int heightSlot){}

  public virtual void turnOn(){
    on=true;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith(gameController.noPower.name)) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.fullPower;
        }
      }
      rend.materials = mats;
    }
  }

  public virtual void turnOff(){
    on=false;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith(gameController.fullPower.name)) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.noPower;
        }
      }
      rend.materials = mats;
    }
  }
}
