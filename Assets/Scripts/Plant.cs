using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Powered
{

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {
  }

  public override float discharge(float requested){
    float juice = Mathf.Max(power, requested);
    power -= juice;
    Destroy(gameObject);
    return juice;
  }

  public override string analyze(){
    return "Available charge: "+power;
  }
}
