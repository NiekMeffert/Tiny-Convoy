﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTile : MonoBehaviour
{
  public Vector2Int pos;
  public int rarity;
  public GameObject[,] tiles = new GameObject[10,10];
  List<GameObject> bigObstructions = new List<GameObject>();
  List<GameObject> cpuBoxes = new List<GameObject>();
  List<GameObject> mysteryBoxes = new List<GameObject>();
  GameController gameController;
  public int minLevel;
  public int maxLevel;
  public int fogLevel = 2;
  public float lastTouched;

  // Start is called before the first frame update
  void Start(){
    lastTouched = Time.unscaledTime;
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    foreach (Transform child in transform){
      if (child.gameObject.GetComponent<ThingOnBigTile>()!=null){
        bigObstructions.Add(child.gameObject);
      }
      if (child.gameObject.GetComponent<CPUBox>()!=null){
        cpuBoxes.Add(child.gameObject);
      } else if (child.gameObject.GetComponent<MysteryBox>()!=null){
        mysteryBoxes.Add(child.gameObject);
      }
    }
    foreach (GameObject a in bigObstructions){
      setUpBigObstruction(a);
    }
    foreach (GameObject a in cpuBoxes){
      a.GetComponent<CPUBox>().setUpPosition();
    }
    foreach (GameObject a in mysteryBoxes){
      a.GetComponent<MysteryBox>().setUpPosition();
    }
  }

  // Update is called once per frame
  void Update(){}

  void setUpBigObstruction(GameObject obs){
    float[] rands = gameController.getRands(new Vector2Int(Mathf.RoundToInt(obs.transform.position.x), Mathf.RoundToInt(obs.transform.position.z)));
    //pseudorandomly rotate
    obs.transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[0])*90f, 0), Space.World);
    //pseudorandomly reposition
    Vector2 wiggleRoom = obs.GetComponent<ThingOnBigTile>().wiggleRoom;
    Vector3 wiggle = new Vector3(Mathf.Round((rands[1]*wiggleRoom.x*2)-wiggleRoom.x), 0, Mathf.Round((rands[2]*wiggleRoom.y*2)-wiggleRoom.y));
    obs.transform.position += wiggle;
    //move everything inside onto tiles
    foreach (Transform child in obs.transform){
      ActualThing actT = child.gameObject.GetComponent<ActualThing>();
      if (actT!=null) {
        actT.setUpVars();
        actT.setUpPosition();
        if (child.gameObject.GetComponent<InvisibleObstruction>()!=null) child.gameObject.GetComponent<InvisibleObstruction>().bigThing = obs;
      }
    }
    MeshRenderer[] mRenderers = transform.GetComponentsInChildren<MeshRenderer>(true);
    SkinnedMeshRenderer[] smRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
    foreach (MeshRenderer m in mRenderers){
      if (m.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) m.enabled=false;
    }
    foreach (SkinnedMeshRenderer sm in smRenderers){
      if (sm.gameObject.transform.parent.GetComponent<InvisibleObstruction>()==null) sm.enabled=false;
    }
  }
}
