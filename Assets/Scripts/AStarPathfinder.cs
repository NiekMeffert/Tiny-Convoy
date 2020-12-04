using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : Pathfinder
{
  List<navTile> selectableTiles = new List<navTile>();
  Stack<navTile> path = new Stack<navTile>();
  navTile currentTile;
  public bool moving = false;
  public int move = 5;
  public float jumpHeight = 2;
  public float moveSpeed = 2;
  public float jumpVelocity = 4.5f;
  Vector3 velocity = new Vector3();
  Vector3 heading = new Vector3();
  float halfHeight = 0;
  bool fallingDown = false;
  bool jumpingUp = false;
  bool movingEdge = false;
  Vector3 jumpTarget;
  navTile[,] memoryTiles;

  // Start is called before the first frame update
  void Start(){
    setUpPathfinder();
  }

  // Update is called once per frame
  void Update(){
  }

  public override (float, Stack<navTile>) getPath(GameObject tile){
    float cost = 0;
    Tile firstCarTile = firstCarVars.tile.GetComponent<Tile>();
    GameObject[,] mTiles = gameController.getSquare(new Vector3Int(firstCarTile.pos.x, firstCarTile.pos.y, cpu.memory));
    navTile[,] memoryTiles = new navTile[mTiles.GetLength(0), mTiles.GetLength(1)];
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        memoryTiles[x,y] = makeNavTile(mTiles[x,y]);
      }
    }
    for (int x = 0; x<memoryTiles.GetLength(0); x++){
      for (int y = 0; y<memoryTiles.GetLength(1); y++){
        computeAdjacencyLists(x,y);
      }
    }
    currentTile = memoryTiles[cpu.memory,cpu.memory];
    currentTile.current = true;
    FindSelectableTiles();
    return (cost, path);
  }

  public override void moveNextTo(GameObject tile){
    GameObject[,] adjacents = gameController.getSquare(new Vector3Int(tile.GetComponent<Tile>().pos.x, tile.GetComponent<Tile>().pos.y, 1));
    destination=adjacents[Mathf.RoundToInt(Random.Range(0,2)), Mathf.RoundToInt(Random.Range(0,2))];
    if (destination==tile){
      destination=adjacents[0,0];
    }
    cpu.startMovers();
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

  public void computeAdjacencyLists(int ntx, int nty){
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

  public void FindSelectableTiles(){
    Queue<navTile> process = new Queue<navTile>();
    process.Enqueue(currentTile);
    currentTile.visited = true;
   // currentTile.parent = ?? leave as null
    while (process.Count > 0){
      navTile t = process.Dequeue();
      selectableTiles.Add(t);
      t.selectable = true;
      if (t.distance < move){
        foreach (navTile tile in t.adjacencyList){
          if (!tile.visited){
            tile.parent = t;
            tile.visited = true;
            tile.distance = 1 + t.distance;
            process.Enqueue(tile);
          }
        }
      }
    }
  }

  public void MoveToTile(navTile tile)
  {
      path.Clear();
      tile.target = true;
      moving = true;

      navTile next = tile;
      while (next !=null)
      {
          path.Push(next);
          next = next.parent;
      }
  }

  public void Move()
  {
      if (path.Count > 0)
      {
          navTile t = path.Peek();
          Vector3 target = t.tile.transform.position;

          //calculate the units position on top of the target tile
          target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

          if (Vector3.Distance(transform.position, target) >= 0.1f)
          {
              /* bool jump = transform.position.y != target.y;

               if (jump)
               {
                   Jump(target);
               }
               else
               {
                   CalculateHeading(target);
                   SetHorizontalVelocity();
               }
              */

              CalculateHeading(target);
              SetHorizontalVelocity();
              //Locomotion
              transform.forward = heading;
              transform.position += velocity * Time.deltaTime;
          }
          else
          {
              //Tile center reached
              transform.position = target;
              path.Pop();

          }


      }
      else
      {
          RemoveSelectableTiles();
         moving = false;
      }
  }

  protected void RemoveSelectableTiles()
  {
      if (currentTile != null)
      {
          currentTile.current = false;
          currentTile = null;
      }
      foreach (navTile tile in selectableTiles)
      {
          tile.Reset();
      }

      selectableTiles.Clear();
  }

  void CalculateHeading(Vector3 target)
  {
      heading = target - transform.position;
      heading.Normalize();
  }

  void SetHorizontalVelocity()
  {
      velocity = heading * moveSpeed;
  }

  void Jump(Vector3 target)
  {
      if (fallingDown)
      {
          FallDownward(target);
      }
      else if (jumpingUp)
      {
          JumpUpward(target);
      }
      else if (movingEdge)
      {
          MoveToEdge();
      }
      else
      {
          PrepareJump(target);
      }

  }

  void PrepareJump(Vector3 target)
  {
      float targetY = target.y;
      target.y = transform.position.y;

      CalculateHeading(target);

      if(transform.position.y > targetY)
      {
          fallingDown = false;
          jumpingUp = false;
          movingEdge = false;

          jumpTarget = transform.position + (target - transform.position) / 2.0f;
      }
      else
      {
          fallingDown = false;
          jumpingUp = false;
          movingEdge = false;

          velocity = heading * moveSpeed / 3.0f;

          float difference = targetY - transform.position.y;

          velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
      }
  }

  void FallDownward(Vector3 target)
  {
      velocity += Physics.gravity * Time.deltaTime;

      if (transform.position.y <= target.y)
      {
          fallingDown = false;

          Vector3 p = transform.position;
          p.y = target.y;
          transform.position = p;

          velocity = new Vector3();
      }
  }

  void JumpUpward(Vector3 target)
  {
      velocity += Physics.gravity * Time.deltaTime;

      if (transform.position.y > target.y)
      {
          jumpingUp = false;
          fallingDown = false;
      }
  }

  void MoveToEdge()
  {
      if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
      {
          SetHorizontalVelocity();
      }
      else
      {
          movingEdge = false;
          fallingDown = false;

          velocity /= 3.0f;
          velocity.y = 1.5f;
      }
  }
}
