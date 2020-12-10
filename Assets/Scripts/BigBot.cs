using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : MonoBehaviour
{
  public float maxDistance = 200f;
  Vector3 nextHeading;
  Quaternion nextQuat;
  Vector4 safeBox;
  Vector3 flipX = new Vector3(-1,1,1);
  Vector3 flipZ = new Vector3(1,1,-1);
  GameController gameController;

  // Start is called before the first frame update
  void Start(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    newHeading();
  }

  // Update is called once per frame
  void Update(){
    if (gameController.mode!=1) return;
    safeBox.Set(gameController.mainCamera.transform.position.x+maxDistance, gameController.mainCamera.transform.position.z+maxDistance, gameController.mainCamera.transform.position.x-maxDistance, gameController.mainCamera.transform.position.z-maxDistance);
    float ang = Quaternion.Angle(transform.rotation,nextQuat);
    transform.rotation = Quaternion.Slerp(transform.rotation, nextQuat, Mathf.Min(.1f/ang,1f));
    transform.position += Vector3.ClampMagnitude(transform.forward, .5f*Time.deltaTime);
    if (ang<.1f) newHeading();
    //Debug.Log(Quaternion.Angle(transform.rotation,nextQuat));
  }

  void newHeading(){
    nextHeading.Set(Random.Range(-100f,100f),0, Random.Range(-100f,100f));
    nextHeading.Normalize();
    if (transform.position.x>safeBox.x && nextHeading.x>0) nextHeading.Scale(flipX);
    if (transform.position.z>safeBox.y && nextHeading.z>0) nextHeading.Scale(flipZ);
    if (transform.position.x<safeBox.z && nextHeading.x<0) nextHeading.Scale(flipX);
    if (transform.position.z<safeBox.w && nextHeading.z<0) nextHeading.Scale(flipZ);
    nextQuat = Quaternion.LookRotation(nextHeading, Vector3.up);
    Debug.Log("Change");
  }
}
