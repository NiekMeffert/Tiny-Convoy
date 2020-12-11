using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    bool isFocus = false;
    Transform player;

    private void Update()
    {
        if (isFocus)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= radius)
            {
                Debug.Log("INTERACT");
            }
        }
    }


    public void onFocused (Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
    }

    public void OnDefocused ()
    {
        isFocus = false;
        player = null;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
