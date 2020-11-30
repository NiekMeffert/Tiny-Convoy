﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Upgrade {
  public bool totem;
  public int processing;
  public int memory;
  public int inputs;
  public int outputs;
  public float battery;
  public float sight;
  public int baseProcessing;
  public int baseMemory;
  public int baseInputs;
  public int baseOutputs;
  public float baseBattery;
  public float baseSight;
  public float fSpeed;
  public float bSpeed;
  public float turnSpeed;
  public float upSpeed;
  public float downSpeed;
  public int flyHeight;
  public float sightDistance;
  public int longDistanceResolution;
  public float meshDistance;
  public float waterResistance;
  public GameObject[] cars;

  // Start is called before the first frame update
  void Start(){
  }

  // Update is called once per frame
  void Update(){

  }

  public void upgrade(){}

  public void pathfindTo(GameObject tile){}

  public void talkTo(GameObject CPU){}

  public override string analyze(){
    return "A robot!";
  }

  void updateStats(){
    processing = baseProcessing;
    memory = baseMemory;
    inputs = baseInputs;
    outputs = baseOutputs;
    battery = baseBattery;
    sight = baseSight;
    fSpeed = 0;
    bSpeed = 0;
    turnSpeed = 0;
    flyHeight = 0;
    longDistanceResolution = 1;
    meshDistance = 5;
    waterResistance = 0;
    //get charge & charge requirements
    float powerNeeded = 0;
    float powerAvailable = 0;
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        if (carVars.upgrades[h]==null){return;}
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars.on==true) powerNeeded+=upVars.drain;
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (batteryVars != null){powerAvailable+=batteryVars.charge;}
      }
    }
    //shut stuff off until there's enough power
    if (powerAvailable<powerNeeded) {
      for (int i = cars.Length-1; i>=0; i--){
        Car carVars = cars[i].GetComponent<Car>();
        for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
          if (carVars.upgrades[h]==null || powerAvailable>powerNeeded){return;}
          Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
          if (upVars.on==true && carVars.upgrades[h].GetComponent<CPU>()==null) {
            upVars.on=false;
            powerNeeded-=upVars.drain;
          }
        }
      }
    }
    //discharge batteries sequentially (backwards)
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]==null || powerNeeded<=0 || batteryVars==null){return;}
        batteryVars.charge -= powerNeeded;
        powerNeeded = 0;
        if (batteryVars.charge<0){
          powerNeeded = -1f*batteryVars.charge;
          batteryVars.charge=0;
        }
      }
    }
    //update stats from upgrades
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      carVars.mass=0;
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars==null){return;}
        carVars.mass += upVars.mass;
        if (upVars.on==false){return;}
        Sensor sensorVars = carVars.upgrades[h].GetComponent<Sensor>();
        if (sensorVars!=null){sight+=sensorVars.sightDistance;}
        LongRangeScanner scannerVars = carVars.upgrades[h].GetComponent<LongRangeScanner>();
        if (scannerVars!=null){longDistanceResolution+=scannerVars.resolution;}
        Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
        if (moverVars!=null){
          fSpeed+=moverVars.fSpeed;
          bSpeed+=moverVars.bSpeed;
          turnSpeed+=moverVars.turnSpeed;
        }
        Flyer flyerVars = carVars.upgrades[h].GetComponent<Flyer>();
        if (flyerVars!=null){
          upSpeed+=flyerVars.upSpeed;
          downSpeed+=flyerVars.downSpeed;
        }
      }
    }
    //update stats from meshing
  }
}