using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Powered
{

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {
  }

  public override float discharge(float requested){
    float juice = Mathf.Min(power, requested);
    power -= juice;
    if (power<=0) die();
    return juice;
  }

  public override string analyze(){
    return "Available charge: "+power;
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
    if (health==0) die();
  }
}
