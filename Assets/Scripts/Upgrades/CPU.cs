using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Upgrade {
  public GameObject[] cars = new GameObject[8];
  public int processing;
  public int memory;
  public int sight;
  public float battery;
  public int baseProcessing;
  public int baseMemory;
  public int baseInputs;
  public int baseOutputs;
  public int baseSight;
  public float baseBattery;
  public float hSpeed;
  public float turnSpeed;
  public float vSpeed;
  public float climb;
  public int flyHeight;
  public float sightDistance;
  public int scanner;
  public float meshDistance;
  public float waterResistance;
  Pathfinder pathfinder;
  public GameObject objective;
  public float objectiveWeight;
  public AI ai;
  public float powerNeeded;
  public float powerAvailable;
  public float maxPower;
  public float maxChargeIn;
  public float maxChargeOut;
  public bool waitForRotation;
  public float thoughtCounter = 1f;
  public bool waitingToStart = true;

  // Start is called before the first frame update
  void Start(){
    setUpVars();
    setUpPosition();
    pathfinder = gameObject.GetComponent<Pathfinder>();
    ai = gameObject.GetComponent<AI>();
  }

  // Update is called once per frame
  void Update(){
    if (waitingToStart==true) setUpCPU();
    if (gameController.mode!=1) return;
    updateStats();
    if (gameObject != gameController.totem){
      thoughtCounter -= Time.deltaTime*(.1f*processing);
      if (thoughtCounter<0){
        thoughtCounter=1f;
        ai.changeMind();
      }
    }
  }

  public void setUpCPU(){
    Random.InitState(Mathf.RoundToInt(transform.position.x)+gameController.randomSeedX+Mathf.RoundToInt(transform.position.z)+gameController.randomSeedY);
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
    baseSight = 1+Mathf.RoundToInt(rands[5]*randFactor);
    gameController.CPUs.Add(gameObject);
    cars[0] = Instantiate(gameController.carPrefab);
    cars[0].transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    Car carVars = cars[0].GetComponent<Car>();
    carVars.cpu = gameObject;
    carVars.registerElements(tile);
    carVars.setUpPosition();
    pathfinder.firstCarVars = carVars;
    waitingToStart=false;
  }

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
    hSpeed = 0;
    turnSpeed = 0;
    vSpeed = 0;
    flyHeight = 0;
    scanner = 1;
    meshDistance = 5f;
    waterResistance = 0;
    powerNeeded = 0;
    powerAvailable = 0;
    maxPower = 0;
    waitForRotation = false;
    //get charge & charge requirements
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.Count-1; h>=0; h--){
        if (carVars.upgrades[h]!=null){
          Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
          if (upVars.health<0) upVars.turnOff();
          if (upVars.on==true) powerNeeded+=upVars.drain;
          Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
          if (batteryVars != null){
            if (batteryVars.health<0) batteryVars.charge=0;
            powerAvailable+=batteryVars.charge;
            maxPower+=batteryVars.maxCharge;
            batteryVars.updateBatteryLevel();
          }
        }
      }
    }
    //shut stuff off until there's enough power
    if (powerAvailable<powerNeeded) {
      for (int i = cars.Length-1; i>=0; i--){
        Car carVars = cars[i].GetComponent<Car>();
        for (int h = carVars.upgrades.Count-1; h>=0; h--){
          if (carVars.upgrades[h]!=null && powerAvailable<powerNeeded){
            Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
            if (upVars.on==true && carVars.upgrades[h].GetComponent<CPU>()==null) {
              upVars.turnOff();
              powerNeeded-=upVars.drain;
            }
          }
        }
      }
    }
    //discharge batteries sequentially (backwards)
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.Count-1; h>=0; h--){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && powerNeeded>0 && batteryVars!=null){
          batteryVars.charge -= powerNeeded*Time.deltaTime;
          powerNeeded = 0;
          if (batteryVars.charge<0){
            powerNeeded = -1f*batteryVars.charge;
            batteryVars.charge=0;
          }
          batteryVars.updateBatteryLevel();
        }
      }
    }
    //update stats from upgrades
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      carVars.mass=0;
      for (int h = carVars.upgrades.Count-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          carVars.mass += upVars.mass;
          if (upVars.on==true){
            Sensor sensorVars = carVars.upgrades[h].GetComponent<Sensor>();
            if (sensorVars!=null){sight+=sensorVars.sightDistance;}
            LongRangeScanner scannerVars = carVars.upgrades[h].GetComponent<LongRangeScanner>();
            if (scannerVars!=null){scanner+=scannerVars.resolution;}
            Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
            if (moverVars!=null){
              hSpeed+=moverVars.hSpeed;
              turnSpeed+=moverVars.turnSpeed;
              if (moverVars.waitForRotation) waitForRotation=true;
            }
            Flyer flyerVars = carVars.upgrades[h].GetComponent<Flyer>();
            if (flyerVars!=null){
              vSpeed+=flyerVars.vSpeed;
              vSpeed+=flyerVars.vSpeed;
            }
            Router routerVars = carVars.upgrades[h].GetComponent<Router>();
            if (routerVars!=null){
              meshDistance+=routerVars.meshBonus;
            }
          }
        }
      }
    }
    //update stats from meshing
    foreach (GameObject cpu in gameController.CPUs) {
      if (cpu!=gameObject){
        CPU cpuVars = cpu.GetComponent<CPU>();
        float dist = Vector3.Distance(gameObject.transform.position, cpu.transform.position);
        if (dist<meshDistance){
          dist = 1f-(dist/meshDistance);
          if (cpuVars.baseProcessing>processing) processing+=Mathf.RoundToInt(dist*(cpuVars.baseProcessing-processing));
          if (cpuVars.baseMemory>memory) memory+=Mathf.RoundToInt(dist*(cpuVars.baseMemory-memory));
          if (cpuVars.baseInputs>inputs) inputs+=Mathf.RoundToInt(dist*(cpuVars.baseInputs-inputs));
          if (cpuVars.baseOutputs>outputs) outputs+=Mathf.RoundToInt(dist*(cpuVars.baseOutputs-outputs));
          if (cpuVars.baseSight>sight) sight+=Mathf.RoundToInt(dist*(cpuVars.baseSight-sight));
        }
      }
    }
  }

  public void startMovers(){
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.Count-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
          if (moverVars!=null){
            moverVars.turnOn();
          }
        }
      }
    }
  }

  public void stopMovers(){
    for (int i = cars.Length-1; i>=0; i--){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = carVars.upgrades.Count-1; h>=0; h--){
        Upgrade upVars = carVars.upgrades[h].GetComponent<Upgrade>();
        if (upVars!=null){
          Mover moverVars = carVars.upgrades[h].GetComponent<Mover>();
          if (moverVars!=null){
            moverVars.turnOff();
          }
        }
      }
    }
    if (objective!=null && objective.GetComponent<Plant>()!=null){
      harvest(objective);
    }
    if (objective!=null && objective.GetComponent<Upgrade>()!=null){
      if (objective.GetComponent<Battery>()!=null && objective.GetComponent<Upgrade>().cpu!=null){
        chargeBot(objective);
      } else {
        upgrade(objective);
      }
    }
  }

  public bool closeEnough(GameObject target){
    Vector2Int[] surroundings = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
    Vector2Int targetPos = target.GetComponent<ActualThing>().tile.GetComponent<Tile>().pos;
    for (int i=0; i<cars.Length; i++){
      Vector2Int myPos = cars[i].GetComponent<Car>().upgradeTile.GetComponent<Tile>().pos;
      if (Mathf.Abs(myPos.x-targetPos.x)<2 && Mathf.Abs(myPos.y-targetPos.y)<2) return true;
    }
    return false;
  }

  public void harvest(GameObject plant){
    if (closeEnough(plant)){
      chargeFrom(plant);
      objective=null;
    } else {
      pathfinder.moveNextTo(plant.GetComponent<Plant>().tile);
    }
  }

  public void chargeBot(GameObject friendBattery){
    GameObject friendCar = friendBattery.GetComponent<Battery>().cpu.GetComponent<CPU>().cars[0];
    if (closeEnough(friendBattery)){
      chargeTo(friendCar);
      objective=null;
    } else {
      pathfinder.moveNextTo(friendCar.GetComponent<Car>().tile);
    }
  }

  public void upgrade(GameObject newToy){
    Upgrade newToyVars = newToy.GetComponent<Upgrade>();
    GameObject newToyTile = newToyVars.tile;
    if (newToyVars.cpu!=null) newToyTile = newToyVars.cpu.GetComponent<CPU>().cars[0].GetComponent<Car>().tile;
    if (closeEnough(newToy)){
      //go into upgrade screen...
      gameController.setMode(2); //upgrade screen
      objective=null;
    } else {
      pathfinder.moveNextTo(newToyTile);
    }
  }

  public void chargeFrom(GameObject source){
    float intake = Mathf.Max(maxPower-powerAvailable, maxChargeIn);
    float chargeIn = source.GetComponent<Powered>().discharge(intake);
    if (intake>.1) {
      GameObject chargeEffect = Instantiate(gameController.particles[0]);
      Vector3 horizontal = new Vector3(source.transform.position.x, transform.position.y, source.transform.position.z);
      chargeEffect.transform.position = horizontal;
      chargeEffect.transform.rotation = Quaternion.LookRotation(transform.position-horizontal,Vector3.up);
    }
    for (int i = 0; i<cars.Length; i++){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = 0; h<carVars.upgrades.Count; h++){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && chargeIn>0 && batteryVars!=null){
          batteryVars.charge += chargeIn;
          chargeIn = 0;
          if (batteryVars.charge>batteryVars.maxCharge){
            chargeIn = batteryVars.charge-batteryVars.maxCharge;
            batteryVars.charge=batteryVars.maxCharge;
          }
          batteryVars.updateBatteryLevel();
        }
      }
    }
  }

  public void chargeTo(GameObject botCar){
    CPU botVars = botCar.GetComponent<Car>().cpu.GetComponent<CPU>();
    float spend = Mathf.Min(powerAvailable, maxChargeOut);
    spend = Mathf.Min(spend, botVars.maxPower-botVars.powerAvailable);
    spend = Mathf.Min(spend, botVars.maxChargeIn);
    float exchanged = spend;
    if (spend>.1) {
      GameObject chargeEffect = Instantiate(gameController.particles[0]);
      chargeEffect.transform.position = transform.position;
      Vector3 horizontal = new Vector3(botCar.transform.position.x, transform.position.y, botCar.transform.position.z);
      chargeEffect.transform.rotation = Quaternion.LookRotation(horizontal-transform.position,Vector3.up);
    }
    for (int i = 0; i<cars.Length; i++){
      Car carVars = cars[i].GetComponent<Car>();
      for (int h = 0; h<carVars.upgrades.Count; h++){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && spend>0 && batteryVars!=null){
          batteryVars.charge -= spend;
          spend = 0;
          if (batteryVars.charge<=0){
            spend = -batteryVars.charge;
            batteryVars.charge=0;
          }
          batteryVars.updateBatteryLevel();
        }
      }
    }
    for (int i = 0; i<botVars.cars.Length; i++){
      Car carVars = botVars.cars[i].GetComponent<Car>();
      for (int h = 0; h<carVars.upgrades.Count; h++){
        Battery batteryVars = carVars.upgrades[h].GetComponent<Battery>();
        if (carVars.upgrades[h]!=null && exchanged>0 && batteryVars!=null){
          batteryVars.charge += exchanged;
          exchanged = 0;
          if (batteryVars.charge>batteryVars.maxCharge){
            exchanged = batteryVars.charge-batteryVars.maxCharge;
            batteryVars.charge=batteryVars.maxCharge;
          }
          batteryVars.updateBatteryLevel();
        }
      }
    }
  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }

  public void setUpUpgrades(){
    foreach (GameObject c in cars){
      Car carVars = c.GetComponent<Car>();
      carVars.registerElements(tile);
    }
    updateStats();
  }
}
