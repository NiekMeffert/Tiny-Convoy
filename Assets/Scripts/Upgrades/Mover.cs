using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Upgrade
{

  public float hSpeed;
  public float turnSpeed;
  public bool waitForRotation;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void moveForward(){}

  public virtual void moveBackward(){}
}
