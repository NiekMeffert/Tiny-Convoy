﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : Danger
{
  public float agitateTime;
  public float safeDistance;
  public float baseDamage;
  public float blastFromHeight;
  public GameObject blast;
  //0 idle, 1 warn, 2 attack, 4 defused, 5 dead

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
    if (counter<0){
      counter += 2f;
      if (mode==0){
        foreach (GameObject cpu in gameController.CPUs){
          if (Vector3.Distance(gameObject.transform.position, cpu.transform.position) < safeDistance) {
            //Go to warn mode
            mode=1;
            counter=agitateTime;
            animator.SetBool("IsAgitated", true);
          }
        }
      } else if (mode==1){
        //Warn mode
        foreach (GameObject cpu in gameController.CPUs){
          if (Vector3.Distance(gameObject.transform.position, cpu.transform.position) < safeDistance) {
            //Go to attack mode
            mode=2;
                    }
        }
        if (mode!=2){
          //go back to idle
          mode=0;
          animator.SetBool("IsAgitated", false);
          animator.SetBool("IsIdle", true);
        }
      }
      if (mode==2){
        //explode mode
        Vector3 damSource = gameObject.transform.position+new Vector3(0,blastFromHeight,0);
        Tile tileVars = gameObject.GetComponent<ActualThing>().tile.GetComponent<Tile>();
        GameObject[,] damageSquare = gameController.getSquare(new Vector3Int(tileVars.pos.x,tileVars.pos.y,1));
        for (int x=0; x<damageSquare.GetLength(0); x++){
          for (int y=0; y<damageSquare.GetLength(1); y++){
            GameObject[] slots = damageSquare[x,y].GetComponent<Tile>().heightSlots;
            int carOffset = 0;
            GameObject car=null;
            for (int i=0; i<slots.GetLength(0); i++){
              if (slots[i]!=null){
                Vector3 dealTo = new Vector3(damageSquare[x,y].transform.position.x, i*.5f, damageSquare[x,y].transform.position.z);
                if (slots[i].GetComponent<Car>()==null){
                  float damage = baseDamage*(Vector3.Distance(damSource,dealTo)*safeDistance);
                  slots[i].GetComponent<ActualThing>().takeDamage(damage, dangerName);
                } else {
                  float damage = baseDamage*(Vector3.Distance(damSource,dealTo)*safeDistance);
                  if (car==null || car!=slots[i]) {
                    car = slots[i];
                    carOffset=-1*i;
                  }
                  slots[i].GetComponent<Car>().upgradeTile.GetComponent<UpgradeTile>().heightSlots[i-carOffset].GetComponent<ActualThing>().takeDamage(damage, dangerName);
                }
              }
            }
          }
        }
        scareAll();
        GameObject particle = Instantiate(blast);
        blast.transform.position = damSource;
        animator.SetBool("IsDead", true);
        Destroy(gameObject,.5f);
      }
    }
  }
}
