using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{
  //public int move = 9;

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
    if (destination==null) return;
    if (gameController.mode!=1 || firstCarVars.tile==null) return;
    if (path.Count == 0 && destination!=null){
      getPath(destination);
      if (path.Count == 0) destination=null;
    }
    if (path.Count > 0 && path.Peek().tile!=destination){
      destination=path.Peek().tile;
      //GameObject mkr = Instantiate(marker);
      //mkr.transform.position = destination.transform.position;
    }
    if (destination!=null) moveToTile();
  }

  public override (float, Stack<navTile>, navTile, navTile) getPath(GameObject tile){
    float cost = 0;
    List<navTile> selectableTiles2 = new List<navTile>();
    //Stack<navTile> path2 = new Stack<navTile>();
    navTile currentTile2;
    navTile[,] memoryTiles;
    Tile firstCarTile = firstCarVars.tile.GetComponent<Tile>();
    Vector2Int offset = new Vector2Int(tile.GetComponent<Tile>().pos.x-firstCarTile.pos.x, tile.GetComponent<Tile>().pos.y-firstCarTile.pos.y);
    if (Mathf.Abs(offset.x)>cpu.sight || Mathf.Abs(offset.y)>cpu.sight) {
      return (cost, path, new navTile(), new navTile());
    }
    memoryTiles = ai.getWeightedMap();
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        computeAdjacencyLists(x,y,memoryTiles);
      }
    }
    currentTile2 = memoryTiles[cpu.sight,cpu.sight];
    currentTile2.current = true;
    selectableTiles2 = FindSelectableTiles(currentTile2, selectableTiles2);
    navTile targetNavTile = memoryTiles[cpu.sight+offset.x,cpu.sight+offset.y];
    targetNavTile.target = true;
    navTile next = targetNavTile;
    while (next !=null)
    {
      cost += next.weight;
      path.Push(next);
      next = next.parent;
    }
    //Return an empty path with cost 0 if no route found:
    if (path.Count==1 && (Mathf.Abs(offset.x)>1 || Mathf.Abs(offset.y)>1)){
      path.Pop();
      cost=0;
    }
    return (cost, path, currentTile2, targetNavTile);
  }

  public override void setPath(float cost, Stack<navTile> path2, navTile currentTile2, navTile targetNavTile){
    path=path2;
    currentTile=currentTile2;
    destination = targetNavTile.tile;
  }

  public void computeAdjacencyLists(int ntx, int nty, navTile[,] memoryTiles){
    Vector2Int[] neighbors = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
    foreach (Vector2Int n in neighbors){
      if (ntx+n.x>-1 && ntx+n.x<memoryTiles.GetLength(0) && nty+n.y>-1 && nty+n.y<memoryTiles.GetLength(1)){
        GameObject otherTile = memoryTiles[ntx+n.x,nty+n.y].tile;
        if (Mathf.Abs(cpu.cars[0].transform.position.y-otherTile.GetComponent<Tile>().canFit(cpu.cars[0], true))<.1){
           memoryTiles[ntx,nty].adjacencyList.Add(memoryTiles[ntx+n.x,nty+n.y]);
        }
      }
    }
  }

  public List<navTile> FindSelectableTiles(navTile currentTile2, List<navTile> selectableTiles2){
    Queue<navTile> process = new Queue<navTile>();
    process.Enqueue(currentTile2);
    currentTile2.visited = true;
   // currentTile.parent = ?? leave as null
    while (process.Count > 0){
      navTile t = process.Dequeue();
      selectableTiles2.Add(t);
      t.selectable = true;
      //if (t.weight < move){
        foreach (navTile tile in t.adjacencyList){
          if (!tile.visited){
            tile.parent = t;
            tile.visited = true;
            tile.weight = 1 + t.weight;
            process.Enqueue(tile);
          }
        }
      //}
    }
    return selectableTiles2;
  }

  public override void stop(){
    if (path.Count > 1){
      path.Pop();
    } else {
      path.Pop();
      destination=null;
      cpu.stopMovers();
      moving=false;
    }
  }
}
