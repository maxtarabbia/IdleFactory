using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public class Splitter : MonoBehaviour
{
    public WorldGeneration world;
    Vector2 pos;
    public float Speed = 1f;

    public Vector2[] itemsID;

    Vector2[] OutputPos;

    TickEvents tickEvents;

    float timeSinceFixed;

    GameObject[] sprites;
    SpriteRenderer[] SRs;
    float justAdded;

    int inIter;
    public GameObject splitterTop;

    bool[] canOutput;

    Sprite[] spriteAssets;
    // Start is called before the first frame update
    private void Awake()
    {
        itemsID = new Vector2[3];
        OutputPos = new Vector2[3];
    }
    void Start()
    {
        FindObjectOfType<Skins>().Setskin(Skin.SkinType.Belt, gameObject);
        canOutput = new bool[3];
        world = FindObjectOfType<WorldGeneration>();
        spriteAssets = world.items.GroupBy(o => o.sprite).Select(g=>g.First().sprite).ToArray();
        pos = transform.position;

        //x is ID
        //y is time spent on belt

        sprites = new GameObject[3];
        SRs = new SpriteRenderer[3];
        for (int i = 0; i <sprites.Length; i++)
        {
            canOutput[i] = true;
            sprites[i] = new GameObject("Sprite " + i);
            sprites[i].transform.position = pos + new Vector2(0, 0);
            sprites[i].transform.parent = gameObject.transform;
            sprites[i].transform.eulerAngles = (Vector3.zero);

            SRs[i] = sprites[i].AddComponent<SpriteRenderer>();
            SRs[i].sortingLayerName = "Buildings";
            SRs[i].sortingOrder = 1;

            itemsID[i] = new Vector2(-1, 0);
        }

        Speed = world.speedstates.BeltInfo.speed;

        FixOutputs();

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;

        FindObjectOfType<StateSaveLoad>().Save();
    }
    void SetSpritePos(float Offset)
    {
        float curOffset;
        for (int i = 0; i < sprites.Length; i++)
        {
            curOffset = Offset;
            if ((itemsID[i].y + Offset) >= 1 / Speed && !canOutput[i])
            {
                curOffset = 1 / Speed - itemsID[i].y;
            }
            Vector2 Vec2 = GetPosition(curOffset, i);
            sprites[i].transform.localPosition = new Vector3(Vec2.x, Vec2.y, (itemsID[i].y + curOffset));
        }
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
        //UpdatedBlockedSprite();
    }
    void UpdatedBlockedSprite()
    {
        if (itemsID[0].x != -1)
        {
            sprites[0].transform.localScale = Vector3.zero;
        }
    }
    Vector2 GetPosition(float Offset, int index)
    {
        Vector2[] LocalPos = new Vector2[3] 
        {
            new Vector2(0, 1),
            new Vector2(-1, 0),
            new Vector2(0, -1)
        };
        Vector2 output = new Vector2();
        Vector2 Dir = (LocalPos[index]) / 2;

        float factor = ((itemsID[index].y + Offset) * Speed);
        output.x = 0.5f - factor;
        output.y = 0;

        if ((itemsID[index].y + Offset) * Speed > 0.5)
            {
                output.x = 0f;
                output = Vector2.LerpUnclamped(output, Dir,(factor - 0.5f) * 2f);
            }
        
        return output;
    }
    void OnTick()
    {
        if (justAdded == Time.fixedTime)
        {
            timeSinceFixed = 0;
            justAdded += 0.01f;
            return;
        }
        Profiler.BeginSample("Splitter Tick Logic");
        UpdateSpritePositions(true);
        timeSinceFixed = 0;
        Profiler.EndSample();
    }
    void UpdateSpritePositions(bool moveForward)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (itemsID[i].x != -1)
            {
                if (moveForward)
                    itemsID[i].y += Time.fixedDeltaTime;
                SRs[i].sprite = spriteAssets[(int)itemsID[i].x];
                sprites[i].transform.localPosition = new Vector2(0.5f - (itemsID[i].y * Speed), 0);
            }
            else
            {
                SRs[i].sprite = null;
            }
            if (itemsID[i].y >= 1/Speed)
            {
                if (OutputItem((int)itemsID[i].x,i))
                {
                    timeSinceFixed = 0;
                    SRs[i].sprite = null;
                    itemsID[i] = new Vector2(-1, 0);
                    canOutput[i] = true;
                }
                else
                {
                    itemsID[i].y = 1/Speed;
                    canOutput[i] = false;
                }
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
    public bool inputItem(int inItem, Vector2Int inPos, float Offset)
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
            return inputItem(inItem, Offset);
        }
        return false;
    }
    public bool inputItem(int initemID, float time)
    {
        int curiter = inIter;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (curiter + i >= 3)
                curiter -= 3;

            if (itemsID[curiter + i].x == -1)
            {
                justAdded = Time.fixedTime;
                this.itemsID[curiter + i].y = time;
                this.itemsID[curiter + i].x = initemID;
                inIter++;
                if(inIter >=3)
                    inIter= 0;
                UpdateSpritePositions(false);
                return true;
            }
        }

        return false;
    }
    bool OutputItem(int itemID, int i)
    {
        Vector2 outputCoord = OutputPos[i];
        GameObject OutputObj;
        world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);
        if (OutputObj != null)
        {
            return ItemReceiver.CanObjectAcceptItem(OutputObj, itemID, Vector2Int.RoundToInt(pos), this.itemsID[i].y - 1 / Speed);
        }
        return false;
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
        world.inv.AddItem((int)builds.AllBuildings[3].cost[0].x, Mathf.Clamp(builds.AllBuildings[3].count - 2, 1, int.MaxValue));

        world.inv.AddItem((int)itemsID[0].x, 1);

        world.OccupiedCells.Remove(pos);

        builds.AllBuildings[3].count--;
 
        Destroy(gameObject);
    }
    void FixRotations()
    {
        FindObjectOfType<Buildings>().AllBuildings[3].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        foreach (GameObject sprite in sprites)
        {
            sprite.transform.eulerAngles = Vector3.zero;
        }
        FixOutputs();
    }
    private void OnDestroy()
    {
        tickEvents.MyEvent -= OnTick;
    }
}
