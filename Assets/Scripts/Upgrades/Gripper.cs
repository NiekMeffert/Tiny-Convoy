using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripper : Upgrade {

  // Start is called before the first frame update
  void Start(){
    setUpVars();
    setUpPosition();
  }

  // Update is called once per frame
  void Update(){}

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
