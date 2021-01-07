using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeScanner : Upgrade{
  public int resolution;
  int baseResolution;

  // Start is called before the first frame update
  void Start(){
    baseResolution = resolution;
    setUpActualThing();
  }

  // Update is called once per frame
  void Update(){

  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    resolution = Mathf.RoundToInt(health/maxHealth)*baseResolution;
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
