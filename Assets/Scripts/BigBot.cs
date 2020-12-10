using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : MonoBehaviour
{
  public float maxDistance = 200f;
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
    safeBox.Set(gameController.mainCamera.transform.position.x+maxDistance, gameController.mainCamera.transform.position.z+maxDistance, gameController.mainCamera.transform.position.x-maxDistance, gameController.mainCamera.transform.position.z-maxDistance);
    transform.position += Vector3.ClampMagnitude(transform.forward, 1.5f*Time.deltaTime);
    Quaternion headingQuat = Quaternion.LookRotation(nextHeading, Vector3.up);
    float ang = Quaternion.Angle(headingQuat,transform.rotation);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, headingQuat, Mathf.Min(ang,2f*Time.deltaTime));
    if (ang<.1f) newHeading();
  }

  void newHeading(){
    nextHeading.Set(Random.Range(-100f,100f),0, Random.Range(-100f,100f));
    nextHeading.Normalize();
    if (transform.position.x>safeBox.x && nextHeading.x>0) nextHeading.Scale(flipX);
    if (transform.position.z>safeBox.y && nextHeading.z>0) nextHeading.Scale(flipZ);
    if (transform.position.x<safeBox.z && nextHeading.x<0) nextHeading.Scale(flipX);
    if (transform.position.z<safeBox.w && nextHeading.z<0) nextHeading.Scale(flipZ);
  }
}
