using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
  public Vector2Int pos;
  public bool full;
  public GameObject[] heightSlots = new GameObject[16];
  public int level;
  public GameObject bigTile;
  public int fogLevel;

  // Start is called before the first frame update
  void Start(){

  }

  // Update is called once per frame
  void Update(){

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
    foreach (GameObject m in heightSlots){
      if (m!=null) m.GetComponent<ActualThing>().setFog(nextFog);
    }
  }
}
