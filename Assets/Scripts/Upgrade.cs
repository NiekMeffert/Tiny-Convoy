using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : ActualThing
{
  public GameObject cpu;
  public float health;
  public float maxHealth;
  public bool on;
  public float drain;

  // Start is called before the first frame update
  void Start()
  {
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public override void setUpActualThing(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    if (cpu==null){
      int fit = gameController.canFit(gameObject, myTile, true);
      if (fit==-1){
        Destroy(gameObject);
      } else {
        if (fit>Mathf.RoundToInt(transform.position.y*2f)) transform.position += new Vector3(0,(float)fit*.5f,0);
        moveOntoTile(myTile, fit);
      }
    } else {
      moveOntoTile(myTile, Mathf.RoundToInt(transform.position.y*2f));
    }
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
}
