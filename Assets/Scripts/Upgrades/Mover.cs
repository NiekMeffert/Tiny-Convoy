using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Upgrade
{

  public float hSpeed;
  public float turnSpeed;
  public bool waitForRotation;
  float baseHSpeed;
  float baseTurnSpeed;

  // Start is called before the first frame update
  void Start()
  {
    baseHSpeed = hSpeed;
    baseTurnSpeed = turnSpeed;
    setUpVars();
    setUpPosition();
    turnOff();
  }

  // Update is called once per frame
  void Update(){}

  public virtual void moveForward(){}

  public virtual void moveBackward(){}

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    hSpeed = Mathf.RoundToInt(health/maxHealth)*baseHSpeed;
    turnSpeed = Mathf.RoundToInt(health/maxHealth)*baseTurnSpeed;
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
