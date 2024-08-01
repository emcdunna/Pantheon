using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public float Pan_speed = 0.5f;
    public float Zoom_speed = 6.0f;
    public float cam_min_size = 0.1f;
    public float cam_max_size = 3.0f;
    public bool lock_camera = false;

    // Use this for initialization
    void Start()
    {
        
    }

    // Use this to change camera settings
    void ChangeSettings(float pan_spd, float zoom_spd, float min_dist, float max_dist)
    {
        Pan_speed = pan_spd;
        Zoom_speed = zoom_spd;
        cam_min_size = min_dist;
        cam_max_size = max_dist;
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = GetComponent<Camera>();
        Vector3 cam_tf = transform.position;

        if (lock_camera != true)
        {
            bool move_north = Input.GetKey(KeyCode.W);
            bool move_south = Input.GetKey(KeyCode.S);
            bool move_west = Input.GetKey(KeyCode.A);
            bool move_east = Input.GetKey(KeyCode.D);

            bool zoom_in = false;
            bool zoom_out = false;
            float zoomNum = 0; // Input.GetAxis("Mouse ScrollWheel");
            float zoom_speed = Zoom_speed; // + Mathf.Abs(zoomNum) * 500;
            if (zoomNum > 0 || Input.GetKey(KeyCode.X))
            {
                zoom_in = true;
            }
            else if (zoomNum < 0 || Input.GetKey(KeyCode.Z))
            {
                zoom_out = true;
            }


            if (move_north == true)
            {
                cam_tf += (Vector3.up * Pan_speed * Time.smoothDeltaTime);
            }
            if (move_south == true)
            {
                cam_tf += (Vector3.down * Pan_speed * Time.smoothDeltaTime);
            }
            if (move_west == true)
            {
                cam_tf += (Vector3.left * Pan_speed * Time.smoothDeltaTime);
            }
            if (move_east == true)
            {
                cam_tf += (Vector3.right * Pan_speed * Time.smoothDeltaTime);
            }
            transform.position = cam_tf; // set the position of the camera

            float curr_size = cam.orthographicSize;

            // zoom in or out
            if ((zoom_in == true) && (curr_size > cam_min_size))
            {
                cam.orthographicSize -= (curr_size * Time.smoothDeltaTime * zoom_speed);
            }
            if ((zoom_out == true) && (curr_size < cam_max_size))
            {
                cam.orthographicSize += (curr_size * Time.smoothDeltaTime * zoom_speed);
            }
        }
    }

    // 



}
