using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class Camera_Movement : MonoBehaviour
{
    // Start is called before the first frame update
    KeyCode Up = KeyCode.W;
    KeyCode Left = KeyCode.A;
    KeyCode Right = KeyCode.D;
    KeyCode Down = KeyCode.S;
    
    public float movementSpeed = 0.01f;
    public float zoomSpeed = 1f;

    float timeMoving;

    Vector3 frameOffset;

    public Vector2 minMaxSpeed;

    public Vector2 MinMaxSize;
    WorldGeneration world;
    Camera cam;

    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        cam = GetComponent<Camera>();
        updateCamSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("isLoaded") == 0)
            return;
        if (GameObject.Find("PauseMenu(Clone)"))
            return;
        if (GameObject.Find("World Inv(Clone)"))
            return;
        Profiler.BeginSample("MoveCam");
        frameOffset = Vector3.zero;
        if (Input.GetKey(Up))
        {
            MoveCam(0);
        }
        if (Input.GetKey(Left))
        {
            MoveCam(1);
        }
        if (Input.GetKey(Right))
        {
            MoveCam(2);
        }
        if (Input.GetKey(Down))
        {
            MoveCam(3);
        }
        Profiler.EndSample();


        Profiler.BeginSample("Updating cammovement");
        gameObject.transform.position += frameOffset * Time.deltaTime;

        if (frameOffset.magnitude > 0)
        {
            timeMoving += Time.deltaTime;
            updateCamSize();
        }
        else
        {
            timeMoving = 0;
        }

        if (Input.mouseScrollDelta.sqrMagnitude > 0)
        {
            updateCamSize();
            ZoomCam(Input.mouseScrollDelta.y * -0.01f * zoomSpeed);
        }
        Profiler.EndSample();
    }
    public void updateCamSize()
    {
        Profiler.BeginSample("Updating vars and camsize");
        Vector2 Camsize = new Vector2();

        Camsize.y = cam.orthographicSize;
        Camsize.x = Camsize.y*cam.aspect;
        world.CamSize= Camsize;
        world.CamCoord = gameObject.transform.position;
        world.UpdateNewBlocks();
        Profiler.EndSample();
    }
    void ZoomCam(float dist)
    {
        Profiler.BeginSample("Zooming Cam");

        if (cam.orthographicSize * 1 + dist < MinMaxSize.y && cam.orthographicSize * 1 + dist > MinMaxSize.x)
        {
            cam.orthographicSize *= 1 + dist;
            Transform background = gameObject.transform.GetChild(0);
            background.localScale *= 1 + dist;
        }
        else
        {
            if(cam.orthographicSize * 1 + dist >= MinMaxSize.y)
            {
                cam.orthographicSize = MinMaxSize.y;
            }
            else
            {
                cam.orthographicSize = MinMaxSize.x;
            }
        }
        Profiler.EndSample();
    }
    void MoveCam (int direction) 
    {
        float speed = movementSpeed * cam.orthographicSize;
        speed *= Mathf.Lerp(minMaxSpeed.x, minMaxSpeed.y, Mathf.Clamp01(timeMoving*0.5f));
        switch (direction)
        {
            case 0:
                frameOffset += new Vector3(0, speed);
                break;

            case 1:
                frameOffset += new Vector3(-speed, 0);
                break;

            case 2:
                frameOffset += new Vector3(speed, 0);
                break;

            case 3:
                frameOffset += new Vector3(0, -speed);
                break;
        }
    }
}
