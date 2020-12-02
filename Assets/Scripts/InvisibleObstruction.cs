using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleObstruction : ActualThing
{
    // Start is called before the first frame update
    void Start()
    {
      transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled=false;
      if (height==0){
        height = 2*Mathf.RoundToInt(transform.localScale.y); 
      }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
