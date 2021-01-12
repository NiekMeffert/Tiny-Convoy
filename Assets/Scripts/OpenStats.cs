using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenStats : MonoBehaviour
{
    public GameObject stats;
    // Start is called before the first frame update
  
    public void Openstats()
    {
        if(stats !=null)
        {
            bool isActive = stats.activeSelf;

            stats.SetActive(!isActive);
        }
    }
}
