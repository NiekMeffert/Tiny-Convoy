using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Upgrade
{

  public float hSpeed;
  public float turnSpeed;
  public bool waitForRotation;
  public AudioSource soundPlayer;
  float baseHSpeed;
  float baseTurnSpeed;

  // Start is called before the first frame update
  void Start()
  {
    baseHSpeed = hSpeed;
    baseTurnSpeed = turnSpeed;
    soundPlayer = gameObject.GetComponent<AudioSource>();
    setUpVars();
    setUpPosition();
    turnOff();
  }

  // Update is called once per frame
  void Update(){}

  public virtual void moveForward(){}

  public virtual void moveBackward(){}

  public override void turnOn(){
    on=true;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith("Light")) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.powerLevels[4];
        }
      }
      rend.materials = mats;
    }
    soundPlayer.Play();
  }

  public override void turnOff(){
    on=false;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith("Light")) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.powerLevels[0];
        }
      }
      rend.materials = mats;
    }
    soundPlayer.Pause();
  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    hSpeed = (health/maxHealth)*baseHSpeed;
    turnSpeed = (health/maxHealth)*baseTurnSpeed;
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
