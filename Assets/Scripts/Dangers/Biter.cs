using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biter : Danger
{
  public float turnTime;
  public float biteDamage;
  public float biteFromHeight;
  Quaternion currentRotation;
  GameObject closestBot;
  //0 idle, 1 turn, 2 attack, 4 defused, 5 dead

  // Start is called before the first frame update
  void Start(){
    setUpDanger();
  }

  // Update is called once per frame
  void Update(){
    endanger();
  }

  public override void endanger(){
    counter-=Time.deltaTime;
    if (closestBot==null) mode = 0;
    if (counter<0){
      if (mode==0){
        //idle mode
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", true);
        counter = 1.6f;
        closestBot = null;
        float closestsDist = 25f;
        foreach (GameObject cpu in gameController.CPUs){
          if ((transform.position-cpu.transform.position).sqrMagnitude<closestsDist) closestBot = cpu;
        }
        if (closestBot!=null){
          currentRotation = transform.rotation;
          mode = 1;
        }
      } else if (mode==1){
        //turning mode
        Vector3 aimToBot = closestBot.transform.position-transform.position;
        aimToBot = new Vector3(aimToBot.x, 0, aimToBot.z);
        transform.rotation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(aimToBot, Vector3.up), -counter/turnTime);
        if (counter<(-turnTime)){
          if ((transform.position-closestBot.transform.position).magnitude<1.5f) {
            mode = 2;
            counter = .2f;
          } else {
            mode = 0;
            counter = .5f;
          }
        }
      } else if (mode==2){
        //attack mode
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsIdle", false);
        counter = .7f;
        if ((transform.position-closestBot.transform.position).magnitude<1.5f) {
          counter = .5f;
          GameObject tile = closestBot.GetComponent<CPU>().cars[0].GetComponent<Car>().tile;
          Vector3 damSource = gameObject.transform.position+new Vector3(0,biteFromHeight,0);
          List<GameObject> listCopy = new List<GameObject>(tile.GetComponent<Tile>().actualThings);
          foreach (GameObject t in listCopy){
            ActualThing damagedVars = t.GetComponent<ActualThing>();
            damagedVars.takeDamage(biteDamage, dangerName);
          }
          scareAll();
        } else {
          mode = 0;
          counter = .5f;
        }
      }
    }
  }
}
