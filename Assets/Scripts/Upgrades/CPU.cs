using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Upgrade {
  public GameObject[] cars;
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
  Pathfinder pathfinder;
  public GameObject objective;
  public float powerNeeded;
  public float powerAvailable;
  public float maxPower;
  public float maxChargeIn;
  public float maxChargeOut;

  // Start is called before the first frame update
  void Start(){
    setUpActualThing();
    pathfinder = gameObject.GetComponent<Pathfinder>();
    Random.InitState(tile.GetComponent<Tile>().pos.x+gameController.randomSeedX+tile.GetComponent<Tile>().pos.y+gameController.randomSeedY);
    float[] rands = new float[]{Random.value, Random.value, Random.value, Random.value, Random.value, Random.value};
    float randsTotal = 0;
    foreach (float rand in rands){
      randsTotal+=rand;
    }
    float randFactor = 10f/randsTotal;
    baseProcessing = 1+Mathf.RoundToInt(rands[0]*randFactor);
    baseMemory = 1+Mathf.RoundToInt(rands[1]*randFactor);
    baseInputs = 1+Mathf.RoundToInt(rands[2]*randFactor);
    baseOutputs = 1+Mathf.RoundToInt(rands[3]*randFactor);
    baseBattery = 1f+Mathf.Round(rands[4]*randFactor);
    baseSight = 1f+Mathf.Round(rands[5]*randFactor);
    gameController.CPUs = GameObject.FindGameObjectsWithTag("CPU");
  }

  // Update is called once per frame
  void Update(){
    if (gameController.mode!=1){return;}
    updateStats();
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
    powerNeeded = 0;
    powerAvailable = 0;
    maxPower = 0;
    //get charge & charge requirements
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        if (carVars.upgrades[h]!=null){
          Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
          if (upVars.health<0) upVars.on=false;
          if (upVars.on==true) powerNeeded+=upVars.drain;
          Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
          if (batteryVars != null){
            if (batteryVars.health<0) batteryVars.charge=0;
            powerAvailable+=batteryVars.charge;
            maxPower+=batteryVars.maxCharge;
          }
        }
      }
    }
    //shut stuff off until there's enough power
    if (powerAvailable<powerNeeded) {
      for (int i = cars.Length-1; i>=0; i--){
        Car carVars = cars[i].GetComponent<Car>();
        for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
          if (carVars.upgrades[h]!=null && powerAvailable<powerNeeded){
            Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
            if (upVars.on==true && carVars.upgrades[h].GetComponent<CPU>()==null) {
              upVars.on=false;
              powerNeeded-=upVars.drain;
            }
          }
        }
      }
    }
    //discharge batteries sequentially (backwards)
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && powerNeeded>0 && batteryVars!=null){
          batteryVars.charge -= powerNeeded*Time.deltaTime;
          powerNeeded = 0;
          if (batteryVars.charge<0){
            powerNeeded = -1f*batteryVars.charge;
            batteryVars.charge=0;
          }
        }
      }
    }
    //update stats from upgrades
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      carVars.mass=0;
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          carVars.mass += upVars.mass;
          if (upVars.on==true){
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
      }
    }
    //update stats from meshing
  }

  public void startMovers(){
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
          if (moverVars!=null){
            moverVars.on=true;
          }
        }
      }
    }
  }

  public void stopMovers(){
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.GetLength(0)-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
          if (moverVars!=null){
            moverVars.on=false;
          }
        }
      }
    }
    if (objective!=null && objective.GetComponent<Plant>()!=null){
      harvest(objective);
    }
  }

  public void harvest(GameObject plant){
    bool closeEnough=false;
    for (int i=0; i<cars.Length; i++){
      if (Vector2.Distance(new Vector2(plant.transform.position.x, plant.transform.position.z), new Vector2(cars[i].transform.position.x, cars[i].transform.position.z))<1.3f) closeEnough=true;
    }
    if (closeEnough){
      chargeFrom(plant);
    } else {
      pathfinder.moveNextTo(plant.GetComponent<Plant>().tile);
      objective=plant;
    }
  }

  public void chargeFrom(GameObject source){
    float intake = Mathf.Max(maxPower-powerAvailable, maxChargeIn*Time.deltaTime);
    float chargeIn = source.GetComponent<Powered>().discharge(intake);
    for (int i = 0; i<cars.Length; i++){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = 0; h<carVars.upgrades.GetLength(0); h++){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && chargeIn>0 && batteryVars!=null){
          batteryVars.charge += chargeIn;
          chargeIn = 0;
          if (batteryVars.charge>batteryVars.maxCharge){
            chargeIn = batteryVars.charge-batteryVars.maxCharge;
            batteryVars.charge=batteryVars.maxCharge;
          }
        }
      }
    }
  }
}
