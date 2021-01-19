using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : MonoBehaviour
{
  public float maxDistance = 60f;
  public float currentDistance;
  Vector3 nextHeading;
  Vector4 safeBox;
  Vector3 flipX = new Vector3(-1,1,1);
  Vector3 flipZ = new Vector3(1,1,-1);
  GameController gameController;

  // Start is called before the first frame update
  void Start(){
    nextHeading = Vector3.forward;
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    newHeading();
  }

  // Update is called once per frame
  void Update(){
    if (gameController.mode!=1) return;
    //safeBox.Set(gameController.mainCamera.transform.position.x+maxDistance, gameController.mainCamera.transform.position.z+maxDistance, gameController.mainCamera.transform.position.x-maxDistance, gameController.mainCamera.transform.position.z-maxDistance);
    transform.position += Vector3.ClampMagnitude(transform.forward, 1.5f*Time.deltaTime);
    Quaternion headingQuat = Quaternion.LookRotation(nextHeading, Vector3.up);
    float ang = Quaternion.Angle(headingQuat,transform.rotation);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, headingQuat, Mathf.Min(ang,2f*Time.deltaTime));
    currentDistance = Vector3.Distance(gameController.mainCamera.transform.position, transform.position);
    if (currentDistance>maxDistance) newHeading();
  }

  void newHeading(){
    nextHeading = gameController.mainCamera.transform.position - transform.position;
    nextHeading.y = 0;
    nextHeading.Normalize();
  }
}
