using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class hoveringSprites : MonoBehaviour
{
    public bool isSelected;

    public Vector2 size;
    public Sprite CornerSprite;
    GameObject[] sprites = new GameObject[4];

    public UnityEvent RotateCW;
    public UnityEvent RotateCCW;

    public UnityEvent Delete;

    public Material DeletingBarMaterial;
    SpriteRenderer BarSR;

    KeyCode Rotate;
    KeyCode Pick;

    StateSaveLoad SSL;

    public float BarOffset;

    float timeHeld;
    bool isHovered;

    public int2[] inputCoords;
    public int2 outputCoord;

    float BracketOffset;

    bool isTouch;
    Camera_Movement cammove;

    public int objectID;
    // Start is called before the first frame update
    void Start()
    {
        SSL = FindObjectOfType<StateSaveLoad>();
        Rotate = System.Enum.Parse<KeyCode>(PlayerPrefs.GetString("Rotate", "R"));
        Pick = System.Enum.Parse<KeyCode>(PlayerPrefs.GetString("Pick", "Q"));
        BracketOffset = 0.1f;
        InitializeMaterial();
        AlignDeletingBar();
        for(int i = 0; i < 4; i++)
        {
            InitializeSprite(i);
            sprites[i].transform.localEulerAngles = (new Vector3(0,0,i*90));
            sprites[i].isStatic= true;
        }
        cammove= FindObjectOfType<Camera_Movement>();
        isTouch = FindObjectOfType<WorldGeneration>().isTouch;
        SetTranforms();
    }
    private void Update()
    {

        if (BracketOffset >= 0)
        {
            BracketOffset -= 0.8f * Time.deltaTime;
            SetTranforms();
            //print("BracketOffset:" + BracketOffset);
        }
        if (!isSelected && !isHovered)
            return;
        if (isTouch)
        {
            TouchScreenCheck();
        }
        else
        {
            MouseCheck();
        }
    }
    void SetTranforms()
    {
        float ydist = (size.y - 1) / 2;
        float xdist = (size.x - 1) / 2;

        xdist += BracketOffset * size.x;
        ydist += BracketOffset * size.y;

        sprites[0].transform.localPosition = new Vector3(-xdist, ydist, 0);
        sprites[1].transform.localPosition = new Vector3(-xdist, -ydist, 0);
        sprites[2].transform.localPosition = new Vector3(xdist, -ydist, 0);
        sprites[3].transform.localPosition = new Vector3(xdist, ydist, 0);
    }
    void InitializeMaterial()
    {
        SpriteRenderer[] SRs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer SR in SRs)
        {
            if(SR.gameObject.name == "DeletingBar")
            {
                BarSR = SR;
                DeletingBarMaterial = SR.material;
                return;
            }
        }
    }
    void InitializeSprite(int ID)
    {
        sprites[ID] = new GameObject();
        sprites[ID].transform.position = transform.position;
        sprites[ID].transform.parent = transform;
        SpriteRenderer SR = sprites[ID].AddComponent<SpriteRenderer>();
        SR.sprite = CornerSprite;

        SR.sortingLayerName = "UI";
        sprites[ID].SetActive(false);

    }
    private void OnMouseEnter()
    {
        isHovered= true;
        UpdateColors();
        BracketOffset = 0.1f;
        if (Input.GetKey(KeyCode.LeftControl))
            isSelected = true;
        foreach (GameObject sprite in sprites)
        {
            sprite.SetActive(true);
        }

    }
    public void Unhover(bool globalCall)
    {
        if (globalCall && isHovered)
            return;
        if (isSelected)
            return;

        foreach (GameObject sprite in sprites)
        {
            sprite.SetActive(false);
        }
    }
    void UpdateColors()
    {
        if(isHovered)
        {
            foreach (GameObject sprite in sprites)
            {
                sprite.GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);
            }
            return;
        }
        if(isSelected)
        {
            foreach (GameObject sprite in sprites)
            {
                sprite.GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.green);
            }
        }
    }
    private void OnMouseExit()
    {
        isHovered = false;
        DeletingBarMaterial.SetFloat("_Value", 0);
        bool isLingering = true;
        if(!Input.GetKey(KeyCode.LeftControl))
        {
            isLingering = false;
            Unhover(false);
        }
        if (isLingering)
        {
            isSelected = true;
            FindObjectOfType<Controls>().areSelectedBuildings = true;
        }
        UpdateColors();
        timeHeld = 0;
    }
    private void OnMouseUp()
    {
        if(isTouch && cammove.distanceMoved < 0.5f && cammove.timeMoving < 0.3f)
        {
            RotateCW?.Invoke();
        }
    }
    void TouchScreenCheck()
    {
        if (Input.touchCount == 1 && cammove.distanceMoved < 50f)
        {
            timeHeld += Time.deltaTime;
            DeletingBarMaterial.SetFloat("_Value", timeHeld / 0.5f);
            if (timeHeld > 0.5f)
            {
                Delete?.Invoke();
                SSL.LateSave();
            }
        }
        else
        {
            if (timeHeld != 0)
            {
                DeletingBarMaterial.SetFloat("_Value", 0);
                timeHeld = 0;
            }
        }
    }
    void MouseCheck()
    {
        if (Input.GetKeyDown(Rotate))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                RotateCCW?.Invoke();
            }
            else
            {
                RotateCW?.Invoke();
            }
            SSL.LateSave();
            AlignDeletingBar();
        }
        if (Input.GetMouseButtonDown(2) || Input.GetKey(Pick))
        {
            WorldGeneration world = FindObjectOfType<WorldGeneration>();

            int rotation = Mathf.RoundToInt(transform.rotation.eulerAngles.z);
            FindObjectOfType<Buildings>().AllBuildings[objectID].rotation = rotation;

            world.setBuildableIndex(objectID);
        }
        if (Input.GetMouseButton(1))
        {
            timeHeld += Time.deltaTime;
            DeletingBarMaterial.SetFloat("_Value", timeHeld / 0.5f);   
            if (timeHeld > 0.5f)
            {
                Delete?.Invoke();
                FindObjectOfType<Controls>().areSelectedBuildings = false;
                SSL.LateSave();
            }
        }
        else
        {
            if (timeHeld != 0)
            {
                DeletingBarMaterial.SetFloat("_Value", 0);
                timeHeld = 0;
            }
        }
    }
    private void OnMouseOver()
    {
        /*
        if (isTouch)
        {
            TouchScreenCheck();
        }
        else
        {
            MouseCheck();
        }
        */
    }
    void AlignDeletingBar()
    {
        BarSR.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        BarSR.gameObject.transform.position = gameObject.transform.position + new Vector3(0, BarOffset);
    }
}
