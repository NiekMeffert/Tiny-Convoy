using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpacer : InvisibleObstruction
{

  // Start is called before the first frame update
  void Start(){
    setUpVars();
    setUpPosition();
  }

  // Update is called once per frame
  void Update(){}

  public override void setFog(int nextFog){}
}
