using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : ActualThing
{
  public GameObject cpu;
  public bool on;
  public float drain;

  // Start is called before the first frame update
  void Start()
  {
    setUpVars();
    setUpPosition();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void turnOn(){
    on=true;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith("Light")) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.powerLevels[4];
        }
      }
      rend.materials = mats;
    }
  }

  public virtual void turnOff(){
    on=false;
    foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()){
      Material[] mats = rend.materials;
      for (int i=0; i<mats.Length; i++){
        if (mats[i].name.StartsWith("Light")) {
          Object.Destroy(mats[i]);
          mats[i]=gameController.powerLevels[0];
        }
      }
      rend.materials = mats;
    }
  }

  public override void setFog(int nextFog){
    if (nextFog==fogLevel) return;
    MeshRenderer[] mRenderers = transform.GetComponentsInChildren<MeshRenderer>(true);
    SkinnedMeshRenderer[] smRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
    if (nextFog==0){
      fogLevel=nextFog;
      foreach (MeshRenderer m in mRenderers){
        m.enabled=true;
      }
      foreach (SkinnedMeshRenderer sm in smRenderers){
        sm.enabled=true;
      }
    } else if (fogLevel==0){
      fogLevel=nextFog;
      foreach (MeshRenderer m in mRenderers){
        m.enabled=false;
      }
      foreach (SkinnedMeshRenderer sm in smRenderers){
        sm.enabled=false;
      }
    } else {
      fogLevel=nextFog;
    }
  }

  public override void takeDamage(float damage, string dangerName){
    health = Mathf.Clamp(health-damage,0,maxHealth);
    if (health==0) turnOff();
    if (cpu!=null) cpu.GetComponent<AI>().learnDanger(damage, dangerName);
  }
}
