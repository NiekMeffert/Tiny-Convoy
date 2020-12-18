using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danger : MonoBehaviour
{
  public float counter;
  public int mode = 0; //0 idle, 1 warn, 2 attack, 4 defused, 5 dead
  public GameController gameController;

  // Start is called before the first frame update
  void Start(){
    setUpDanger();
  }

  public virtual void setUpDanger(){
    counter = Random.value;
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
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
}
