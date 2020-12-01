using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Upgrade{
  public float charge;
  public float maxCharge;

  // Start is called before the first frame update
  void Start(){
    setUpActualThing();
  }

  // Update is called once per frame
  void Update(){

  }
}
