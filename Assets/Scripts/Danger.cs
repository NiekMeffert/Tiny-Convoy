using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danger : MonoBehaviour
{
  public float counter;
  public int mode = 0; //0 idle, 1 warn, 2 attack, 4 defused, 5 dead
  public GameController gameController;
  public string dangerName;
  public Animator animator;
  public List<CPU> damageQueue;
  public List<CPU> witnessQueue;

  // Start is called before the first frame update
  void Start(){
    setUpDanger();
  }

  public virtual void setUpDanger(){
    counter = Random.value;
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update(){
    endanger();
  }

  public virtual void endanger(){
    counter-=Time.deltaTime;
    if (counter<0){
      counter += 1f;
    }
  }

  public virtual void scareAll(){
    foreach (GameObject cpu in gameController.CPUs){
      cpu.GetComponent<AI>().witnessDanger(gameObject.transform.position, Random.value, dangerName);
    }
  }
}
