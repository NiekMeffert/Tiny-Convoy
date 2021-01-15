using System.Collections;
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
            foreach (GameObject t in damageSquare[x,y].GetComponent<Tile>().actualThings){
              ActualThing tVars = t.GetComponent<ActualThing>();
              List<GameObject> finalThings = new List<GameObject>();
              Car tCar = t.GetComponent<Car>();
              if (tCar==null){
                finalThings.Add(t);
              } else {
                finalThings.AddRange(tCar.upgradeTile.GetComponent<UpgradeTile>().actualThings);
              }
              foreach (GameObject damaged in finalThings){
                if (damaged==gameObject) continue;
                ActualThing damagedVars = damaged.GetComponent<ActualThing>();
                Vector3 dealTo = new Vector3(damaged.transform.position.x, .5f*(damagedVars.bottomTop[0]+damagedVars.bottomTop[1]), damaged.transform.position.z);
                float damage = baseDamage*(Vector3.Distance(damSource,dealTo)*safeDistance);
                damagedVars.takeDamage(damage, dangerName);
              }
            }
          }
        }
        scareAll();
        GameObject particle = Instantiate(blast);
        blast.transform.position = damSource;
        animator.SetBool("IsDead", true);
        gameObject.GetComponent<ActualThing>().die(.5f);
      }
    }
  }
}
