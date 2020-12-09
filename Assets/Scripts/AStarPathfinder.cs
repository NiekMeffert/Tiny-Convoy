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

  public override (float, Stack<navTile>, List<navTile>, navTile, navTile) getPath(GameObject tile){
    float cost = 0;
    List<navTile> selectableTiles2 = new List<navTile>();
    //Stack<navTile> path2 = new Stack<navTile>();
    navTile currentTile2;
    navTile[,] memoryTiles;
    Tile firstCarTile = firstCarVars.tile.GetComponent<Tile>();
    Vector2Int offset = new Vector2Int(tile.GetComponent<Tile>().pos.x-firstCarTile.pos.x, tile.GetComponent<Tile>().pos.y-firstCarTile.pos.y);
    if (Mathf.Abs(offset.x)>freeMemory+cpu.memory || Mathf.Abs(offset.y)>freeMemory+cpu.memory) {
      return (cost, path, selectableTiles2, new navTile(), new navTile());
    }
    GameObject[,] mTiles = gameController.getSquare(new Vector3Int(firstCarTile.pos.x, firstCarTile.pos.y, freeMemory+cpu.memory));
    memoryTiles = new navTile[mTiles.GetLength(0), mTiles.GetLength(1)];
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        memoryTiles[x,y] = makeNavTile(mTiles[x,y]);
      }
    }
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        computeAdjacencyLists(x,y,memoryTiles);
      }
    }
    currentTile2 = memoryTiles[freeMemory+cpu.memory,freeMemory+cpu.memory];
    currentTile2.current = true;
    selectableTiles2 = FindSelectableTiles(currentTile2, selectableTiles2);
    navTile targetNavTile = memoryTiles[freeMemory+cpu.memory+offset.x,freeMemory+cpu.memory+offset.y];
    targetNavTile.target = true;
    navTile next = targetNavTile;
    while (next !=null)
    {
      cost += next.distance;
      path.Push(next);
      next = next.parent;
    }
    if (path.Count==1 && (Mathf.Abs(offset.x)>1 || Mathf.Abs(offset.y)>1)){
      path.Pop();
      cost=0;
    }
    return (cost, path, selectableTiles2, currentTile2, targetNavTile);
  }

  public void setPath(float cost, Stack<navTile> path2, List<navTile> selectableTiles2, navTile currentTile2){
    path=path2;
    selectableTiles=selectableTiles2;
    currentTile=currentTile2;
  }

  public override void moveNextTo(GameObject tile){
  }

  public navTile makeNavTile(GameObject tile){
    navTile nt = new navTile();
    nt.tile = tile;
    nt.tileVars = tile.GetComponent<Tile>();
    nt.walkable = true;
    nt.current = false;
    nt.target = false;
    nt.selectable = false;
    nt.adjacencyList = new List<navTile>();
    nt.visited = false;
    nt.parent = null;
    nt.distance = 0;
    return nt;
  }

  public void computeAdjacencyLists(int ntx, int nty, navTile[,] memoryTiles){
    Vector2Int[] neighbors = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
    foreach (Vector2Int n in neighbors){
      if (ntx+n.x>-1 && ntx+n.x<memoryTiles.GetLength(0) && nty+n.y>-1 && nty+n.y<memoryTiles.GetLength(1)){
        GameObject otherTile = memoryTiles[ntx+n.x,nty+n.y].tile;
        if (gameController.canFit(cpu.cars[0], otherTile)==0){
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
      //if (t.distance < move){
        foreach (navTile tile in t.adjacencyList){
          if (!tile.visited){
            tile.parent = t;
            tile.visited = true;
            tile.distance = 1 + t.distance;
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
