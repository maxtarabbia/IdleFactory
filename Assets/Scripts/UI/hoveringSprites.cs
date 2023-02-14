using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class hoveringSprites : MonoBehaviour
{
    public Vector2 size;
    public Sprite CornerSprite;
    GameObject[] sprites = new GameObject[4];

    public UnityEvent RotateCW;
    public UnityEvent RotateCCW;

    public UnityEvent Delete;

    public Material DeletingBarMaterial;
    SpriteRenderer BarSR;

    public float BarOffset;

    float timeHeld;

    public int2[] inputCoords;
    public int2 outputCoord;

    float THDelSave;
    float BracketOffset;
    // Start is called before the first frame update
    void Start()
    {
        BracketOffset = 0.1f;
        InitializeMaterial();
        AlignDeletingBar();
        for(int i = 0; i < 4; i++)
        {
            InitializeSprite(i);
            sprites[i].transform.localEulerAngles = (new Vector3(0,0,i*90));
            sprites[i].isStatic= true;
        }

        SetTranforms();
    }
    private void Update()
    {
        if (BracketOffset >= 0)
        {
            BracketOffset -= 0.004f;
            SetTranforms();
            //print("BracketOffset:" + BracketOffset);
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
        foreach(GameObject sprite in sprites)
        {
            sprite.SetActive(true);
            BracketOffset = 0.1f;
        }
    }
    private void OnMouseExit()
    {
        foreach (GameObject sprite in sprites)
        {
            sprite.SetActive(false);
        }
        DeletingBarMaterial.SetFloat("_Value", 0);
        timeHeld = 0;
    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                RotateCCW?.Invoke();
            }
            else
            {
                RotateCW?.Invoke();
            }
            FindObjectOfType<StateSaveLoad>().Save();
            AlignDeletingBar();
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            if (THDelSave == -1)
                return;
            THDelSave += Time.deltaTime;
            if (THDelSave > 3)
            {
                THDelSave = -1;
                FindObjectOfType<StateSaveLoad>().DeleteSave();
            }
            print("Deleting save in: " + (3 - THDelSave));
        }
        else
        {
            THDelSave= 0;
        }
        if(Input.GetMouseButton(1))
        {
            timeHeld += Time.deltaTime;
            DeletingBarMaterial.SetFloat("_Value",timeHeld/0.5f);
            if(timeHeld > 0.5f)
            {
                Delete?.Invoke();
                FindObjectOfType<StateSaveLoad>().LateSave();
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
    void AlignDeletingBar()
    {
        BarSR.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        BarSR.gameObject.transform.position = gameObject.transform.position + new Vector3(0, BarOffset);
    }
}
