﻿using System.Collections;
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

  public override void setUpActualThing(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    moveOntoTile(myTile, Mathf.RoundToInt(transform.position.y*2f));
    turnOff();
  }

  public virtual void moveForward(){}

  public virtual void moveBackward(){}
}
