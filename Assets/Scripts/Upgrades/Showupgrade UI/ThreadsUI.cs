using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadsUI : MonoBehaviour
{
    public GameObject threadsUI;
    // Start is called before the first frame update
    void Start()
    {
        threadsUI.SetActive(false);
    }

    // Update is called once per frame
    public void OnMouseOver()
    {
        threadsUI.SetActive(true);
    }

    public void OnMouseExit()
    {
        threadsUI.SetActive(false);
    }
}