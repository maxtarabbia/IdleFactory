using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Splitter : MonoBehaviour
{
    public WorldGeneration world;
    Vector2 pos;
    public float timeTotravel = 2f;
    public Vector2 itemID;

    Vector2[] OutputPos;
    int OutIter = 0;

    TickEvents tickEvents;

    float timeSinceFixed;

    GameObject sprite;

    bool canOutput;

    Sprite[] spriteAssets;
    // Start is called before the first frame update
    void Start()
    {
        OutputPos = new Vector2[3];
        world = FindObjectOfType<WorldGeneration>();
        spriteAssets = world.items.GroupBy(o => o.sprite).Select(g=>g.First().sprite).ToArray();
        pos = transform.position;
        //x is ID
        //y is time spent on belt

        sprite = new GameObject("Sprite ");
        sprite.transform.position = pos + new Vector2(0, 0);
        sprite.transform.parent = gameObject.transform;
        sprite.transform.eulerAngles = (Vector3.zero);
        sprite.AddComponent<SpriteRenderer>();
        sprite.GetComponent<SpriteRenderer>().sortingLayerName = "Particles";

        itemID = new Vector2(-1, 0);

        timeTotravel = world.speedstates.BeltInfo.speed;

        FixOutputs();

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
 
    }
    void SetSpritePos(float Offset)
    {
        float xVal = 0;
        float yVal = 0;

        if ((itemID.y + Offset) >= timeTotravel && !canOutput)
        {
            Offset = timeTotravel - itemID.y;
        }

                xVal = 0.5f - ((itemID.y + Offset) / timeTotravel);
                yVal = 0;

        sprite.transform.localPosition = new Vector3(xVal, yVal, (itemID.y + Offset));
    }
    void FixOutputs()
    {
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {

            case 0:
                OutputPos[0] = pos + new Vector2(0, 1);
                OutputPos[1] = pos + new Vector2(-1, 0);
                OutputPos[2] = pos + new Vector2(0, -1);
                break;
            case 90:
                OutputPos[0] = pos + new Vector2(-1, 0);
                OutputPos[1] = pos + new Vector2(0, -1);
                OutputPos[2] = pos + new Vector2(1, 0);
                break;
            case 180:
                OutputPos[0] = pos + new Vector2(0, -1);
                OutputPos[1] = pos + new Vector2(1, 0);
                OutputPos[2] = pos + new Vector2(0, 1);
                break;
            case 270:
                OutputPos[0] = pos + new Vector2(1, 0);
                OutputPos[1] = pos + new Vector2(0, 1);
                OutputPos[2] = pos + new Vector2(-1, 0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceFixed += Time.deltaTime;
        SetSpritePos(timeSinceFixed);
        UpdateSprites();
    }
    void UpdateSprites()
    {
        if (itemID.x != -1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = spriteAssets[(int)itemID.x];
            sprite.transform.localPosition = new Vector2(0.5f - (itemID.y / timeTotravel), 0);
        }
    }
    private void FixedUpdate()
    {
        //OnTick();
    }
    void OnTick()
    {
        UpdateSpritePositions(true);
        timeSinceFixed = 0;
    }
    void UpdateSpritePositions(bool moveForward)
    {
        if (itemID.x != -1)
        {
            if (moveForward)
                itemID.y += Time.fixedDeltaTime;
            sprite.GetComponent<SpriteRenderer>().sprite = spriteAssets[(int)itemID.x];
            sprite.transform.localPosition = new Vector2(0.5f - (itemID.y / timeTotravel), 0);
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().sprite = null;
        }
        if (itemID.y >= timeTotravel)
        {
            if (OutputItem((int)itemID.x))
            {
                timeSinceFixed = 0;
                sprite.GetComponent<SpriteRenderer>().sprite = null;
                itemID = new Vector2(-1, 0);
                canOutput = true;
            }
            else
            {
                itemID.y = timeTotravel;
                canOutput = false;
            }
        }
    }
    public bool inputItem(int initemID, float time)
    {
        if (itemID.x == -1)
        {
            itemID.y = time;
            itemID.x = initemID;
            UpdateSpritePositions(false);


            return true;

        }
        return false;
    }
    bool OutputItem(int itemID)
    {
        if(OutIter >= 3)
        {
            OutIter -= 3;
        }

        Vector2 outputCoord = OutputPos[OutIter];

        OutIter++;

        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltscript = cellObj.GetComponent<Belt>();
            Refinery refineryScript = cellObj.GetComponent<Refinery>();
            Splitter splitter = cellObj.GetComponent<Splitter>();
            Core corescript = cellObj.GetComponent<Core>();
            Assembler assembler = cellObj.GetComponent<Assembler>();
            if (assembler != null)
            {
                if (assembler.InputItem(itemID, 1, pos))
                {
                    return true;
                }
            }
            if (beltscript != null)
            {
                float spot = 0;
                if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 90 || Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 270)
                {
                    spot = 0.5f;
                }
                else if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 180)
                {
                    spot = 1;
                }
                if (beltscript.inputItem(itemID, spot))
                {
                    return true;
                }
            }
            else if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1, pos))
                {
                    return true;
                }
            }
            else if (splitter != null)
            {
                if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 0)
                {
                    if (splitter.inputItem(itemID, 0))
                    {
                        return true;
                    }
                }
            }
            else if (corescript != null)
            {
                corescript.InputItem(itemID);
                return true;
            }
        }
        return false;
    }
    private void OnMouseOver()
    {/*
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
            FixRotations();
     
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            FixRotations();
     
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[3].cost[0].x, (int)builds.AllBuildings[3].cost[0].y);

            world.OccupiedCells.Remove(pos);

            builds.AllBuildings[3].count--;
     
            Destroy(gameObject);
        }
        */
    }
    public void RotateCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        FixRotations();
 
    }
    public void RotateCCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        FixRotations();
 
    }
    public void Delete()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        world.inv.AddItem((int)builds.AllBuildings[3].cost[0].x, (int)builds.AllBuildings[3].cost[0].y);

        world.OccupiedCells.Remove(pos);

        builds.AllBuildings[3].count--;
 
        Destroy(gameObject);
    }
    void FixRotations()
    {
        FindObjectOfType<Buildings>().AllBuildings[3].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        sprite.transform.eulerAngles = (Vector3.zero);
        FixOutputs();
    }
    private void OnDestroy()
    {
        tickEvents.MyEvent -= OnTick;
    }
}
