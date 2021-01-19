using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleObstruction : ActualThing
{
  public InvisibleObstruction[] compatriots;
  public int lastQuorum=2;
  public GameObject bigThing;

  // Start is called before the first frame update
  void Start(){
    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled=false;
    setUpVars();
    setUpPosition();
    compatriots = transform.parent.gameObject.transform.GetComponentsInChildren<InvisibleObstruction>(true);
  }

  // Update is called once per frame
  void Update(){}

  public override void setFog(int nextFog){
    if (nextFog==fogLevel) return;
    fogLevel=nextFog;
    int fogQuorum = fogLevel;
    foreach (InvisibleObstruction i in compatriots){
      fogQuorum = Mathf.Min(fogQuorum, i.fogLevel);
    }
    if (fogQuorum==lastQuorum) return;
    lastQuorum = fogQuorum;
    GameObject par = transform.parent.gameObject;
    if (fogQuorum<2){
      if (par.GetComponent<ThingOnBigTile>()!=null){
        MeshRenderer[] mRenderers = par.transform.GetComponentsInChildren<MeshRenderer>(true);
        SkinnedMeshRenderer[] smRenderers = par.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (MeshRenderer m in mRenderers){
          if (m.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) m.enabled=true;
        }
        foreach (SkinnedMeshRenderer sm in smRenderers){
          if (sm.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) sm.enabled=true;
        }
      }
    } else {
      if (par.GetComponent<ThingOnBigTile>()!=null){
        MeshRenderer[] mRenderers = par.transform.GetComponentsInChildren<MeshRenderer>(true);
        SkinnedMeshRenderer[] smRenderers = par.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (MeshRenderer m in mRenderers){
          if (m.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) m.enabled=false;
        }
        foreach (SkinnedMeshRenderer sm in smRenderers){
          if (sm.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) sm.enabled=false;
        }
      }
    }
  }

  public override void die(float afterTime){
    if (bigThing!=null){
      foreach (InvisibleObstruction inv in compatriots) {
        if (inv!=gameObject.GetComponent<InvisibleObstruction>()) inv.bigThing=null;
      }
      foreach (InvisibleObstruction inv in compatriots) {
        if (inv!=gameObject.GetComponent<InvisibleObstruction>()) inv.die(0);
      }
      foreach (Transform child in bigThing.transform){
        if (child.GetComponent<ActualThing>()==null) Destroy(child);
      }
    }
    Tile tileVars = tile.GetComponent<Tile>();
    tileVars.removeFromTile(gameObject);
    if (dieAsPrefab!=null){
      GameObject rubble = Instantiate(dieAsPrefab);
      rubble.transform.position = transform.position;
      rubble.transform.Rotate(new Vector3(0, Mathf.Round(4f*Random.value)*90f, 0), Space.World);
      tileVars.moveOntoTile(rubble);
    }
    Destroy(gameObject, 0);
  }
}
