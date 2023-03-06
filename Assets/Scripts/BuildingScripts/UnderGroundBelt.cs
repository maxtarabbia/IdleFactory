using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGroundBelt : MonoBehaviour
{
    public WorldGeneration world;
    Vector2 pos;
    public float timeTotravel = 1f;
    public Vector2 itemID;

    public Vector2 OutputCoord = new Vector2();
    Vector2 outFrom; 

    GameObject sprite;

    public int distance;

    TickEvents tickEvents;

    SpriteRenderer SR;

    float timeSinceFixed;
    bool canOutput;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        pos = transform.position;
        //x is ID
        //y is time spent on belt

        if (sprite == null)
        {
            sprite = new GameObject("Sprite");
            sprite.transform.position = pos + new Vector2(0, 0);
            sprite.transform.parent = gameObject.transform;
            sprite.transform.eulerAngles = (Vector3.zero);
        }
        SR = sprite.AddComponent<SpriteRenderer>();
        SR.sortingLayerName = "Buildings";
        SR.sortingOrder = 1;
        if (itemID == Vector2.zero)
            itemID = new Vector2(-1, 0);
        UpdateBeltInput();
        UpdateAdjacentBelts();

        timeTotravel = world.speedstates.BeltInfo.speed;

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().Save();
    }

    public void UpdateAdjacentBelts()
    {
        GameObject[] cells = new GameObject[4];
        world.OccupiedCells.TryGetValue(pos + new Vector2(0, 1), out cells[0]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(0, -1), out cells[1]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(1, 0), out cells[2]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(-1, 0), out cells[3]);

        foreach (GameObject cell in cells)
        {
            if (cell != null)
            {
                Belt belt;
                if (cell.TryGetComponent<Belt>(out belt))
                {
                    cell.GetComponent<Belt>().UpdateBeltInput();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceFixed += Time.deltaTime;
        SetSpritePos(timeSinceFixed);
    }
    public void UpdateBeltInput()
    {
        Vector2 Offset = Vector2.zero;
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                Offset = new Vector2(-1 - distance, 0);
                outFrom = new Vector2(distance * -1, 0);
                break;
            case 90:
                Offset = new Vector2(0, -1 - distance);
                outFrom = new Vector2(0,distance * -1);
                break;
            case 180:
                Offset = new Vector2(1 + distance, 0);
                outFrom = new Vector2(distance, 0);
                break;
            case 270:
                Offset = new Vector2(0, 1 + distance);
                outFrom = new Vector2(0,distance);
                break;
        }
        OutputCoord = pos + new Vector2(Offset.x, Offset.y);
    }
    private void FixedUpdate()
    {
    }
    void OnTick()
    {
        UpdateSpritePositions(true);
        timeSinceFixed = 0;
    }
    void SetSpritePos(float Offset)
    {
        float size;
        float xVal = GetXVal(Offset, out size);

        float yVal = 0;

        sprite.transform.localScale = new Vector3(size,size,size);
        sprite.transform.localPosition = new Vector3(xVal, yVal, (itemID.y + Offset));
    }
    float GetXVal(float Offset, out float size)
    {
        float xVal;
        if ((itemID.y + Offset) >= timeTotravel && !canOutput)
        {
            Offset = timeTotravel - itemID.y;
        }
        size = Mathf.Abs(0.5f - ((itemID.y + Offset) / timeTotravel)) * 2;
        if ((itemID.y + Offset) / timeTotravel > 0.5f)
        {
            xVal = 0.5f - ((itemID.y + Offset) / timeTotravel) - distance;
        }
        else
        {
            xVal = 0.5f - ((itemID.y + Offset) / timeTotravel);
        }
        return xVal;
    }
    public void UpdateSpritePositions(bool moveForward)
    {
        //progress forward
        if (itemID.x != -1)
        {
            float size;
            if (moveForward)
                itemID.y += Time.fixedDeltaTime;
            sprite.GetComponent<SpriteRenderer>().sprite = world.items[(int)itemID.x].sprite;
            float xVal = GetXVal(0f, out size);
            float yVal = 0;

            
            sprite.transform.localScale = new Vector3(size,size,size);
            sprite.transform.localPosition = new Vector3(xVal, yVal, itemID.y);


        }

        //check to output
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
                canOutput = false;
                itemID.y = timeTotravel;
            }
        }
    }
    public bool inputItem(int initemID, float time)
    {
        if (itemID.x == -1)
        {
            itemID.y = time * timeTotravel;
            itemID.x = initemID;
            UpdateSpritePositions(false);


            return true;

        }
        return false;
    }
    public bool inputItem(int inItem, Vector2Int inPos)
    {
        Vector2Int relativepos = inPos - Vector2Int.RoundToInt(pos);
        float spot = 0;
        Vector2Int outputpos = new Vector2Int(-1, 0);
        switch (gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputpos = new Vector2Int(-1, 0);
                break;
            case 90:
                outputpos = new Vector2Int(0, -1);
                break;
            case 180:
                outputpos = new Vector2Int(1, 0);
                break;
            case 270:
                outputpos = new Vector2Int(0, 1);
                break;
        }

        if (relativepos == outputpos)
        {
            spot = 0.9f;
        }
        if (inputItem(inItem, spot))
        {
            return true;
        }


        return false;
    }
    bool OutputItem(int initemID)
    {
        GameObject OutputObj = null;
        world.OccupiedCells.TryGetValue(OutputCoord, out OutputObj);

        if (OutputObj != null)
            return ItemReceiver.CanObjectAcceptItem(OutputObj, initemID, Vector2Int.RoundToInt(pos + outFrom));
        /*
        if(!world.OccupiedCells.TryGetValue(outputCoord, out OutputObj))
            return false;
        if (OutputObj != null)
        {
            Belt beltscript = OutputObj.GetComponent<Belt>();
            if (beltscript != null)
            {
                float spot = itemID.y - timeTotravel;
                spot /= timeTotravel;
                if (Mathf.Abs(OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 90 || Mathf.Abs(OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 270)
                {
                    spot = itemID.y - timeTotravel;
                    spot /= timeTotravel;
                }
                else if (Mathf.Abs(OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 180)
                {
                    spot = 0.9f;
                }
                if (beltscript.inputItem(initemID, spot))
                {
                    return true;
                }
            }
            Splitter splitter = OutputObj.GetComponent<Splitter>();
            if (splitter != null)
            {
                if (Mathf.Abs(OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 0)
                {
                    if (splitter.inputItem(initemID, 0))
                    {
                        return true;
                    }
                }

            }
            Assembler assembler = OutputObj.GetComponent<Assembler>();
            if (assembler != null)
            {
                if (assembler.InputItem(initemID, 1, pos))
                {
                    return true;
                }
            }
            Refinery refineryScript = OutputObj.GetComponent<Refinery>();
            if (refineryScript != null)
            {
                if (refineryScript.InputItem(initemID, 1, pos))
                {
                    return true;
                }
            }
            Core core = OutputObj.GetComponent<Core>();
            if (core != null)
            {
                core.InputItem(initemID);
                return true;
            }
        }
        */
        return false;

    }
    public void RotateCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        FixRotations();
        UpdateBeltInput();
        UpdateAdjacentBelts();

    }
    public void RotateCCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        FixRotations();
        UpdateBeltInput();
        UpdateAdjacentBelts();

    }
    public void DeleteThis()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        world.inv.AddItem((int)builds.AllBuildings[1].cost[0].x, (int)builds.AllBuildings[1].cost[0].y);

        world.OccupiedCells.Remove(pos);

        builds.AllBuildings[1].count--;

        Destroy(gameObject);
    }
    private void OnMouseOver()
    {
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
            FixRotations();
            UpdateBeltInput();
            UpdateAdjacentBelts();
             
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            FixRotations();
            UpdateBeltInput();
            UpdateAdjacentBelts();
             
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[1].cost[0].x, (int)builds.AllBuildings[1].cost[0].y);

            world.OccupiedCells.Remove(pos);

            builds.AllBuildings[1].count--;
             
            Destroy(gameObject);

        }
        */
    }
    void FixRotations()
    {
        FindObjectOfType<Buildings>().AllBuildings[1].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        sprite.transform.eulerAngles = (Vector3.zero);
    }
    private void OnDestroy()
    {
        tickEvents.MyEvent -= OnTick;
    }
}
