using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualThing : MonoBehaviour
{
  public GameObject tile;
  public GameController gameController;
  public float mass;
  public Vector2 momentum;
  public float height;
  public float[] bottomTop = new float[2]{0,0};
  public bool standable;
  public int fogLevel;
  public float health;
  public float maxHealth;
  public float armorTink;
  public float armorMultiplier;
  public Animator animator;
  public bool flying=false;

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

  public virtual void setUpVars(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
    bottomTop[0]=transform.position.y; bottomTop[1]=transform.position.y+height;
  }

  public virtual void setUpPosition(){
    GameObject tempTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    Tile tileVars = tempTile.GetComponent<Tile>();
    //float fit = tileVars.canFit(gameObject, false);
    //transform.position = new Vector3(transform.position.x, fit, transform.position.z);
    tileVars.moveOntoTile(gameObject);
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
  }

  public virtual void takeDamage(float damage, string dangerName){
  }

  public virtual void die(){
    tile.GetComponent<Tile>().removeFromTile(gameObject);
    Destroy(gameObject);
  }

  public virtual void fall(){}

  public virtual string analyze(){
    return "Nothing much.";
  }

  public virtual void bumpInto(GameObject otherThing){
  }
}
