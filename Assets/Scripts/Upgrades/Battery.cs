using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Upgrade{
  public float charge;
  public float maxCharge;
  int batteryLevel;

  // Start is called before the first frame update
  void Start(){
    setUpActualThing();
  }

  // Update is called once per frame
  void Update(){

  }

  public void updateBatteryLevel(){
    float chargeRatio = charge/maxCharge;
    int newLevel = 0;
    if (chargeRatio>=.02) newLevel=1;
    if (chargeRatio>.3) newLevel=2;
    if (chargeRatio>.6) newLevel=3;
    if (chargeRatio>.9) newLevel=4;
    if (maxCharge==0) newLevel = 0;
    if (newLevel!=batteryLevel){
      batteryLevel=newLevel;
      foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
        Material[] mats = rend.materials;
        for (int i=0; i<mats.Length; i++){
          if (mats[i].name.StartsWith("Light")) {
            Object.Destroy(mats[i]);
            mats[i]=gameController.powerLevels[batteryLevel];
          }
        }
        rend.materials = mats;
      }
    }
  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    if (health==0){
      charge=0;
      maxCharge=0;
    }
  }
}
