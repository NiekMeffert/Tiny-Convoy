using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Upgrade
{

  public float fSpeed;
  public float bSpeed;
  public float turnSpeed;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void moveForward(){}

  public virtual void moveBackward(){}
}
