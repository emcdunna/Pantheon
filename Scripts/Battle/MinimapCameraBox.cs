using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraBox : MonoBehaviour
{

    public GameObject GameCamera;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = GameCamera.GetComponent<Camera>();
        float height = cam.pixelHeight;
        float width = cam.pixelWidth;
        float aspect_ratio = height / width;
        transform.localScale = new Vector3(cam.orthographicSize, cam.orthographicSize*aspect_ratio);
    }
}
