using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    bool isFocus = false;
    Transform player;
    public Transform interactionTransfrom;
    bool hasInteracted = false;

    public virtual void Interact ()
    {
        //this method is meant to be overwritten
        Debug.Log("Interacting with" + transform.name);
    }

    private void Update()
    {
        if (isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(player.position, interactionTransfrom.position);
            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }


    public void onFocused (Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
    }

    public void OnDefocused ()
    {
        isFocus = false;
        player = null;
        hasInteracted = false;
    }

    private void OnDrawGizmos()
    {
        if (interactionTransfrom == null)
            interactionTransfrom = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransfrom.position, radius);
    }
}
