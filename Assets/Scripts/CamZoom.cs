using UnityEngine;
using UnityEngine.UI;

public class CamZoom : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 0);
    public Quaternion baseRotation;
    public GameController gameController;
    public GameObject mainCam;
    public GameObject camRotator;
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    private float currentZoom = 1f;
    private float currentRotation = 0f;
    public GameObject zoomSlider;
    GameObject fogOfWindow;

    void Start(){
      gameController=GameObject.Find("GameController").GetComponent<GameController>();
      fogOfWindow=GameObject.Find("FogOfWindow");
      offset = transform.position-camRotator.transform.position;
      baseRotation = camRotator.transform.rotation;
    }

    void Update(){
      currentZoom += Input.GetAxis("Vertical") * Time.deltaTime * 3;
      currentZoom -= Input.mouseScrollDelta.y * zoomSpeed;
      currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
      if (Mathf.Abs(Input.mouseScrollDelta.x)>.5) currentRotation -= Input.mouseScrollDelta.x;
      currentRotation += Input.GetAxis("Horizontal") * Time.deltaTime * 50;
      float fogScale = 200f * gameController.fog1;
      fogOfWindow.transform.localScale = new Vector3(fogScale,fogScale,fogScale);
      fogOfWindow.transform.position = new Vector3(gameController.totemPos.x,0,gameController.totemPos.y);
    }

    void LateUpdate(){
      if (gameController.totem!=null && gameController.mode!=2) transform.position = gameController.totem.transform.position;
      camRotator.transform.position = transform.position - offset;
      camRotator.transform.rotation = baseRotation;
      mainCam.transform.position = transform.position - (offset * currentZoom);
      camRotator.transform.RotateAround(transform.position,Vector3.up,currentRotation);
      zoomSlider.GetComponent<Slider>().value = (currentZoom-minZoom)/(maxZoom-minZoom);
      //BogBot shake:
      float botDist = 1000f;
      foreach (GameObject bot in gameController.bigBots){
        if (bot.GetComponent<BigBot>().currentDistance<botDist) botDist = bot.GetComponent<BigBot>().currentDistance;
      }
      if (botDist<40f && gameController.mode==1){
        float bounceAmplitude = (40f-botDist)/40f;
        Vector3 eulerRot = camRotator.transform.rotation.eulerAngles;
        eulerRot.x = 45f+(bounceAmplitude*Mathf.Pow(Mathf.PI-(Time.time*.5f % Mathf.PI),2f)*Mathf.Sin(Time.time*20f));
        Quaternion eulerRotQuat = Quaternion.identity;
        eulerRotQuat.eulerAngles = eulerRot;
        camRotator.transform.rotation = eulerRotQuat;
      }
    }

    public void zoomTo(){
      float z = zoomSlider.GetComponent<Slider>().value;
      z = (z*(maxZoom-minZoom))+minZoom;
      currentZoom = z;
      gameController.uiBlocker = true;
    }
}
