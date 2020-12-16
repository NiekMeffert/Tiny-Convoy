using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripper : Upgrade {

  // Start is called before the first frame update
  void Start(){
    setUpActualThing();
  }

  // Update is called once per frame
  void Update(){

  }

  public override void takeDamage(float damage){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    if (health==0) turnOff();
  }
}
