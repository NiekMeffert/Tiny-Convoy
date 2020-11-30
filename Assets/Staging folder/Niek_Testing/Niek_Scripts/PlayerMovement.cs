using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : TacticsMove
{
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        Init();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);
        if (!moving)
        {
            FindSelectableTiles();
            CheckMouse();
        }
        else
        {
            Move();
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tiles t = hit.collider.GetComponent<Tiles>();

                    if (t.selectable)
                    {
                        MoveToTile(t);

                    }
                }
            }
        }
    }
}
