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
      if (gameController.totem!=null){
        float fogScale = 200f * gameController.totem.GetComponent<CPU>().sight;
        fogOfWindow.transform.localScale = new Vector3(fogScale,fogScale,fogScale);
      }
    }

    void LateUpdate(){
      if (gameController.totem!=null) transform.position = gameController.totem.transform.position;
      camRotator.transform.position = transform.position - offset;
      camRotator.transform.rotation = baseRotation;
      mainCam.transform.position = transform.position - (offset * currentZoom);
      camRotator.transform.RotateAround(transform.position,Vector3.up,currentRotation);
      zoomSlider.GetComponent<Slider>().value = (currentZoom-minZoom)/(maxZoom-minZoom);
    }

    public void zoomTo(){
      float z = zoomSlider.GetComponent<Slider>().value;
      z = (z*(maxZoom-minZoom))+minZoom;
      currentZoom = z;
      gameController.uiBlocker = true;
    }
}
