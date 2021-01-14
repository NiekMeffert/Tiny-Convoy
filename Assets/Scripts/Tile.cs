using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
  public Vector2Int pos;
  public List<GameObject> actualThings = new List<GameObject>();
  public int level;
  public GameObject bigTile;
  public int fogLevel;
  public bool fixHeightsNeeded = false;

  // Start is called before the first frame update
  void Start(){

  }

  // Update is called once per frame
  void Update(){}

  void LateUpdate(){
    if (fixHeightsNeeded==true) fixHeights();
  }

  public float canFit(GameObject load, bool mustBeStandable){
    ActualThing loadVars = load.GetComponent<ActualThing>();
    float fit = 0;
    if (actualThings.Count>0){
      for (int h=0; h<actualThings.Count; h++){
        ActualThing hVars = actualThings[h].GetComponent<ActualThing>();
        if (hVars.bottomTop[0]>=loadVars.bottomTop[1]){
          break;
        } else {
          fit = hVars.bottomTop[1];
          if (mustBeStandable==true && hVars.standable==false) fit = -1f;
        }
      }
    }
    return fit;
  }

  public virtual void moveOntoTile(GameObject load){
    if (load.GetComponent<ActualThing>().tile==gameObject) return;
    ActualThing loadVars = load.GetComponent<ActualThing>();
    if (actualThings.Count>0){
      foreach (GameObject h in actualThings){
        if (h.transform.position.y==load.transform.position.y) {
          float tinyRandom = Random.value*.1f;
          h.transform.position += new Vector3(0,tinyRandom,0);
        }
      }
    }
    actualThings.Add(load);
    fixHeightsNeeded=true;
    if (loadVars.tile!=null) loadVars.tile.GetComponent<Tile>().removeFromTile(load);
    loadVars.tile = gameObject;
    Car carVars = load.GetComponent<Car>();
    if (carVars!=null) carVars.upgradeTile.GetComponent<Tile>().pos = pos;
    loadVars.setFog(fogLevel);
  }

  public virtual void removeFromTile(GameObject load){
    actualThings.Remove(load);
    fixHeightsNeeded=true;
  }

  public virtual void fixHeights(){
    if (actualThings.Count>1){
      actualThings.Sort((a, b) => (a.transform.position.y.CompareTo(b.transform.position.y)));
      float prevTop=0;
      for (int h=0; h<actualThings.Count; h++){
        ActualThing hVars = actualThings[h].GetComponent<ActualThing>();
        if ((hVars.flying==false) || (hVars.bottomTop[0]<=prevTop && hVars.flying==true)){
          hVars.bottomTop[0] = prevTop; hVars.bottomTop[1] = prevTop+hVars.height;
          actualThings[h].transform.position = new Vector3(actualThings[h].transform.position.x, hVars.bottomTop[0], actualThings[h].transform.position.z);
        }
        prevTop = hVars.bottomTop[1];
      }
    } else if (actualThings.Count==1){
      ActualThing hVars = actualThings[0].GetComponent<ActualThing>();
      if (hVars.bottomTop[0]!=0 && hVars.flying==false){
        hVars.bottomTop[0] = 0; hVars.bottomTop[1] = hVars.height;
        actualThings[0].transform.position = new Vector3(actualThings[0].transform.position.x, 0, actualThings[0].transform.position.z);
      }
    }
    fixHeightsNeeded=false;
  }

  public virtual void setFog(int nextFog){
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
    } else if (nextFog==2) {
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
    foreach (GameObject m in actualThings){
      m.GetComponent<ActualThing>().setFog(nextFog);
    }
  }
}
