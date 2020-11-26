using UnityEngine;

public class CamZoom : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 0);
    public Transform target;

    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomSpeed = 4f;

    private float currentZoom = 10f;

    void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        transform.position = target.position - offset * currentZoom;
    }
}
