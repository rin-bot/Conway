using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 origin;
    private Camera cam;
    private float zoom;
    private float zoomMultiplier = 4f;
    private float minZoom = 2f;
    private float maxZoom = 50f;
    private float velocity = 1f;
    private float smoothTime = 0.1f;



    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        zoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        DragCamera();
        ZoomCamera();
    }


    void DragCamera()
    {

        if (Input.GetMouseButtonDown(2)) origin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = origin - cam.ScreenToWorldPoint(Input.mousePosition);

            transform.position += difference;
        }
    }
    private void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }

}
