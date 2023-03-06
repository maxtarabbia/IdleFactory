using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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
    SpriteRenderer SR;

    int blockedIndex;

    GameObject OutputObj;

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
        SR = sprite.AddComponent<SpriteRenderer>();
        SR.sortingLayerName = "Buildings";
        SR.sortingOrder = 1;

        itemID = new Vector2(-1, 0);

        timeTotravel = world.speedstates.BeltInfo.speed;

        FixOutputs();

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().Save();
    }
    void SetSpritePos(float Offset)
    {

        if ((itemID.y + Offset) >= timeTotravel && !canOutput)
        {
            Offset = timeTotravel - itemID.y;
        }
        Vector2 Vec2 = GetPosition(Offset);
        sprite.transform.localPosition = new Vector3(Vec2.x, Vec2.y, (itemID.y + Offset));
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
            SR.sprite = spriteAssets[(int)itemID.x];
            sprite.transform.localPosition = GetPosition(0);
            if (blockedIndex > 3)
                sprite.transform.localScale = Vector3.one * Mathf.Pow(0.95f, blockedIndex);
        }
    }
    Vector2 GetPosition(float Offset)
    {
        Vector2 Dir = (OutputPos[OutIter] - pos)/2;
        Vector2 output = new Vector2();

        output.x = 0.5f - ((itemID.y + Offset) / timeTotravel);
        output.y = 0;

        if ((itemID.y + Offset) / timeTotravel > 0.5)
        {
            output.x = 0f;
            output = Vector2.Lerp(output,Dir, (((itemID.y + Offset) / timeTotravel) - 0.5f) * 2f);
        }
        return output;
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
            SR.sprite = spriteAssets[(int)itemID.x];
            sprite.transform.localPosition = new Vector2(0.5f - (itemID.y / timeTotravel), 0);
        }
        else
        {
            SR.sprite = null;
        }
        if (itemID.y >= timeTotravel)
        {
            if (OutputItem((int)itemID.x))
            {
                timeSinceFixed = 0;
                SR.sprite = null;
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
    public bool inputItem(int inItem, Vector2Int inPos)
    {
        Vector2Int relativepos = inPos - Vector2Int.RoundToInt(pos);
        Vector2Int inputpos = new Vector2Int(-1, 0);
        switch (gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                inputpos = new Vector2Int(1, 0);
                break;
            case 90:
                inputpos = new Vector2Int(0, 1);
                break;
            case 180:
                inputpos = new Vector2Int(-1, 0);
                break;
            case 270:
                inputpos = new Vector2Int(0, -1);
                break;
        }

        if (relativepos == inputpos)
        {
           return inputItem(inItem, 0);
        }
        return false;
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
        bool o = false;

        Vector2 outputCoord = OutputPos[OutIter];


        GameObject OutputObj;
        world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);
        if (OutputObj != null)
        {
            if(!ItemReceiver.CanObjectAcceptItem(OutputObj, itemID, Vector2Int.RoundToInt(pos)))
            {
                o = false;
                blockedIndex++;
            }
            else
            {
                blockedIndex = 0;
                o = true;
            }
        }
        for(int i = 0; i<3;i++)
        {
            OutIter++;
            if (OutIter >= 3)
            {
                OutIter -= 3;
            }

            outputCoord = OutputPos[OutIter];
            world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);
            if (OutputObj != null)
                break;
        }

        




        return o;
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
