using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
    if (destination==null) return;
    if (gameController.mode!=1 || firstCarVars.tile==null) return;
    if (path.Count == 0 && destination!=null){
      itinerary i = getItinerary(destination);
      setItinerary(i);
      if (path.Count == 0) destination=null;
    }
    if (path.Count > 0 && path.Peek().tile!=destination){
      destination=path.Peek().tile;
    }
    if (destination!=null) moveToTile();
  }

  public override itinerary getItinerary(GameObject tile){
    itinerary newItin = new itinerary();
    newItin.cost = 0;
    List<navTile> selectableTiles2 = new List<navTile>();
    newItin.path = new Stack<navTile>();
    navTile currentTile;
    Tile firstCarTile = firstCarVars.tile.GetComponent<Tile>();
    Vector2Int offset = new Vector2Int(tile.GetComponent<Tile>().pos.x-firstCarTile.pos.x, tile.GetComponent<Tile>().pos.y-firstCarTile.pos.y);
    if (Mathf.Abs(offset.x)>cpu.sight || Mathf.Abs(offset.y)>cpu.sight) {
      return newItin;
    }
    ai.refreshWeightedMap();
    for (int x = 0; x<ai.memoryTiles.GetLength(0); x++){
      for (int y = 0; y<ai.memoryTiles.GetLength(1); y++){
        computeAdjacencyLists(x,y);
      }
    }
    currentTile = ai.memoryTiles[cpu.sight,cpu.sight];
    //currentTile.current = true;
    selectableTiles2 = FindSelectableTiles(currentTile, selectableTiles2);
    newItin.targetNavTile = ai.memoryTiles[cpu.sight+offset.x,cpu.sight+offset.y];
    newItin.targetNavTile.target = true;
    navTile next = newItin.targetNavTile;
    while (next !=null)
    {
      newItin.cost += next.weight;
      newItin.path.Push(next);
      next = next.parent;
    }
    //Return an empty path with cost 0 if no route found:
    if (newItin.path.Count==1 && (Mathf.Abs(offset.x)>1 || Mathf.Abs(offset.y)>1)){
      newItin.path.Pop();
      newItin.cost=0;
    }
    return newItin;
  }

  public override void setItinerary(itinerary newItin){
    path=newItin.path;
    destination = newItin.targetNavTile.tile;
  }

  public void computeAdjacencyLists(int ntx, int nty){
    Vector2Int[] neighbors = new Vector2Int[] {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1), new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)};
    foreach (Vector2Int n in neighbors){
      if (ntx+n.x>-1 && ntx+n.x<ai.memoryTiles.GetLength(0) && nty+n.y>-1 && nty+n.y<ai.memoryTiles.GetLength(1)){
        GameObject otherTile = ai.memoryTiles[ntx+n.x,nty+n.y].tile;
        if (Mathf.Abs(cpu.cars[0].transform.position.y-otherTile.GetComponent<Tile>().canFit(cpu.cars[0], true))<.1){
           ai.memoryTiles[ntx,nty].adjacencyList.Add(ai.memoryTiles[ntx+n.x,nty+n.y]);
        }
      }
    }
  }

  public List<navTile> FindSelectableTiles(navTile currentTile, List<navTile> selectableTiles2){
    Queue<navTile> process = new Queue<navTile>();
    process.Enqueue(currentTile);
    currentTile.visited = true;
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
