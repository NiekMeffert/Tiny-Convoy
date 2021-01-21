using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISensor : MonoBehaviour
{
    public GameObject SensorUI;
    // Start is called before the first frame update
    void Start()
    {
        SensorUI.SetActive(false);
    }

    // Update is called once per frame
    public void OnMouseOver()
    {
        SensorUI.SetActive(true);
    }

    public void OnMouseExit()
    {
        SensorUI.SetActive(false);
    }
}
