using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powered : ActualThing
{
  public float power;
  public float maxPower;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual float discharge(float requested){
    float juice = Mathf.Max(power, requested);
    power -= juice;
    return juice;
  }

  public virtual float charge(float offered){
    float juice = Mathf.Max(offered, power+maxPower);
    power += juice;
    return juice;
  }
}
