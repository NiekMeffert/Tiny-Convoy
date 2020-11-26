using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : ActualThing
{
  public GameObject owner;
  public float health;
  public float maxHealth;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
