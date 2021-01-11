using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleObstruction : ActualThing
{
    // Start is called before the first frame update
    void Start()
    {
      transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled=false;
      if (height==0){
        height = 2*Mathf.RoundToInt(transform.localScale.y);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void setFog(int nextFog){
      if (nextFog==fogLevel) return;
      GameObject par = transform.parent.gameObject;
      if (nextFog==0){
        fogLevel=nextFog;
        if (par.GetComponent<ThingOnBigTile>()!=null){
          MeshRenderer[] mRenderers = par.transform.GetComponentsInChildren<MeshRenderer>(true);
          SkinnedMeshRenderer[] smRenderers = par.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
          foreach (MeshRenderer m in mRenderers){
            if (m.gameObject.GetComponent<ActualThing>()==null) m.enabled=true;
          }
          foreach (SkinnedMeshRenderer sm in smRenderers){
            if (sm.gameObject.GetComponent<ActualThing>()==null) sm.enabled=true;
          }
        }
      } else if (nextFog==2) {
        fogLevel=nextFog;
        if (par.GetComponent<ThingOnBigTile>()!=null){
          MeshRenderer[] mRenderers = par.transform.GetComponentsInChildren<MeshRenderer>(true);
          SkinnedMeshRenderer[] smRenderers = par.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
          foreach (MeshRenderer m in mRenderers){
            if (m.gameObject.GetComponent<ActualThing>()==null) m.enabled=false;
          }
          foreach (SkinnedMeshRenderer sm in smRenderers){
            if (sm.gameObject.GetComponent<ActualThing>()==null) sm.enabled=false;
          }
        }
      } else {
        fogLevel=nextFog;
      }
    }
}
