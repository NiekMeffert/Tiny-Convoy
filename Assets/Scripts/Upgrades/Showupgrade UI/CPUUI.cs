using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUUI : MonoBehaviour
{
    public GameObject cpuUI;
    // Start is called before the first frame update
    void Start()
    {
        cpuUI.SetActive(false);
    }

    // Update is called once per frame
    public void OnMouseOver()
    {
        cpuUI.SetActive(true);
    }

    public void OnMouseExit()
    {
        cpuUI.SetActive(false);
    }
}
