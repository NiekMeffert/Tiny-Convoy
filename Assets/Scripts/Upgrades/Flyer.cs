using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Mover
{

  public float upSpeed;
  public float upAccel;
  public float downSpeed;
  public float downAccel;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
