using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBT : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, BoxContainer, fpsCam;

    public float pickupRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        //setup
        if (!equipped)
        {
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {
        // Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!equipped && distanceToPlayer.magnitude <= pickupRange && (Physics.Raycast(ray, out hit, 100)) && !slotFull) PickUp();
        }

        // Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        //Make box a child of the  boxcontainer and move it to default position
        transform.SetParent(BoxContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        // make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        //Set parent to null
        transform.SetParent(null);

        // make Rigidbody not kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;

        //box carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce to box
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.forward * dropUpwardForce, ForceMode.Impulse);

        //add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

    }

}
