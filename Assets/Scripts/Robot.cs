using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Powered
{
  public MonoBehaviour CPU;
  public GameObject[] hardware;
  public bool totem;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
    CPU = gameObject.GetComponent<CPU>();
  }

  // Update is called once per frame
  void Update()
  {
  }

  public override string analyze(){
    return "Just a very troubled little robot.";
  }
}
