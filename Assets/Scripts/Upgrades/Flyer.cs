﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Mover
{
  public float vSpeed;
  float baseVSpeed;

  // Start is called before the first frame update
  void Start()
  {
    baseVSpeed = vSpeed;
    setUpVars();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    vSpeed = (health/maxHealth)*baseVSpeed;
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
