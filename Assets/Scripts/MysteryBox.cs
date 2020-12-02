using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : ActualThing
{
  public GameObject[] options;
  public float nothingChance;

  // Start is called before the first frame update
  void Start()
  {
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    tile = gameController.getTile(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
    float[] rands = gameController.getRands(tile.GetComponent<Tile>().pos);
    if (rands[0]>nothingChance){
      GameObject newThing = Instantiate(options[Mathf.FloorToInt(rands[1]*options.Length)]);
      newThing.transform.position = transform.position;
      newThing.transform.Rotate(new Vector3(0, Mathf.Round(4f*rands[1])*90f, 0), Space.World);
      newThing.transform.parent = transform.parent;
    }
    Destroy(gameObject);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
