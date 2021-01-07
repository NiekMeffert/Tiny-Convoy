﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualThing : MonoBehaviour
{
  public GameObject tile;
  public GameController gameController;
  public float mass;
  public Vector2 momentum;
  public int height;
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
    setUpActualThing();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void setUpActualThing(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    animator = gameObject.GetComponent<Animator>();
    GameObject myTile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    int fit = gameController.canFit(gameObject, myTile, true);
    if (fit==-1){
      Destroy(gameObject);
    } else {
      if (fit>Mathf.RoundToInt(transform.position.y*2f)) transform.position += new Vector3(0,(float)fit*.5f,0);
      moveOntoTile(myTile, fit);
    }
  }

  public virtual void moveOntoTile(GameObject newTile, int heightSlot){
    if (newTile==tile) return;
    Tile newTileVars = newTile.GetComponent<Tile>();
    int newHeightSlot = heightSlot;
    if (newTileVars.heightSlots[heightSlot]!=null){
      GameObject[] heightSlotsClone = (GameObject[]) newTileVars.heightSlots.Clone();
      bool recording=false;
      int heightBump = newTileVars.heightSlots[heightSlot].GetComponent<ActualThing>().height;
      for (int h=0; h<newTileVars.heightSlots.GetLength(0); h++){
        if (newTileVars.heightSlots[h] == newTileVars.heightSlots[heightSlot]){
          recording=true;
          newHeightSlot = h;
        }
        if (recording==true && h+heightBump<=heightSlotsClone.Length) heightSlotsClone[h+heightBump] = newTileVars.heightSlots[h];
      }
      newTileVars.heightSlots = heightSlotsClone;
    }
    for (int h=newHeightSlot; h<height+newHeightSlot; h++){
      newTileVars.heightSlots[h] = gameObject;
    }
    if (tile!=null){
      GameObject[] oldSlots = tile.GetComponent<Tile>().heightSlots;
      for (int h=0; h<oldSlots.Length; h++){
        if (oldSlots[h]==gameObject) oldSlots[h]=null;
      }
      gameController.cleanupQueue.Add(tile);
    }
    gameController.cleanupQueue.Add(newTile);
    tile = newTile;
    setFog(newTileVars.fogLevel);
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
    Destroy(gameObject);
  }

  public virtual void fall(){}

  public virtual string analyze(){
    return "Nothing much.";
  }

  public virtual void bumpInto(GameObject otherThing){
  }

  public virtual void voxellate(bool voxIt){
    if (voxIt==true){
      //choose geometry based on surrounding NESW objects of same type
    } else {
      //set to no surroundings voxel
    }
  }
}
