using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseClass : MonoBehaviour
{
  public int blobby;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual int getVal(){
      return 7;
    }
}
