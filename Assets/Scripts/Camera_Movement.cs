using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
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

    public int worldsize;

    public float timeMoving;
    public float distanceMoved;

    Vector3 frameOffset;

    public Vector2 minMaxSpeed;

    public Vector2 MinMaxSize;
    WorldGeneration world;
    Camera cam;

    float pinchDist;

    bool isTouch;

    void Start()
    {
        GameObject.Find("Background").GetComponent<SpriteRenderer>().material.SetFloat("_Seed", PlayerPrefs.GetInt("Seed"));
        world = FindObjectOfType<WorldGeneration>();
        worldsize = world.Worldsize + 5;
        cam = GetComponent<Camera>();
        updateCamSize();
        isTouch = world.isTouch;
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
        if (isTouch)
        {
            frameOffset = Vector3.zero;
            if(Input.touches.Length == 1)
            {
                frameOffset = Input.touches[0].deltaPosition * -0.11f * cam.orthographicSize;
            }
            if(Input.touchCount == 0)
            {
                timeMoving= 0f;
                distanceMoved = 0f;
            }
            else
            {
                timeMoving += Time.deltaTime;
            }
        }
        else
        {
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
        }
        Profiler.EndSample();


        Profiler.BeginSample("Updating cammovement");
        gameObject.transform.position += frameOffset * Time.deltaTime;

        Vector2 camPos = gameObject.transform.position;

        if(gameObject.transform.position.x > worldsize)
            camPos.x = worldsize;
        if (gameObject.transform.position.x < -worldsize)
            camPos.x = -worldsize;

        if (gameObject.transform.position.y > worldsize)
            camPos.y = worldsize;
        if (gameObject.transform.position.y < -worldsize)
            camPos.y = -worldsize;

        gameObject.transform.position = camPos;

        if (frameOffset.magnitude > 0)
        {
            timeMoving += Time.deltaTime;
            distanceMoved += frameOffset.magnitude;
            updateCamSize();
        }
        else
        {
            if(!isTouch) 
            timeMoving = 0;
        }
        if (!isTouch)
        {
            if (Input.mouseScrollDelta.sqrMagnitude > 0)
            {
                updateCamSize();
                ZoomCam(Input.mouseScrollDelta.y * -0.01f * zoomSpeed);
            }
        }
        else
        {
            if(Input.touches.Length == 2)
            {
                if (pinchDist != -1)
                {
                    float newPinchDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                    float zoomamount = (1-newPinchDist / pinchDist) * 0.5f;
                    ZoomCam(zoomamount);
                    updateCamSize();
                    pinchDist = newPinchDist;
                }
                else
                {
                    pinchDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                }
            }
            else
            {
                pinchDist = -1;
            }
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
