using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navTile
{
  public GameObject tile;
  public Tile tileVars;
  public bool walkable = true;
  //public bool current = false;
  public bool target = false;
  public bool selectable = false;
  public List<navTile> adjacencyList = new List<navTile>();
  public bool visited = false;
  public navTile parent = null;
  public float weight = 0;
  public float power;
  public bool friend;
  public bool danger;
  public GameObject powerObj;
  public GameObject friendObj;
  public GameObject dangerObj;
}
