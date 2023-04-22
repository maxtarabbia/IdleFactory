using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Belt : MonoBehaviour
{
    public WorldGeneration world;
    Vector2 pos;
    public float timeTotravel = 1f;
    public Vector2 itemID;

    Vector2 outputCoord = new Vector2();

    GameObject sprite;

    TickEvents tickEvents;

    SpriteRenderer spriteSR;

    public Sprite[] BeltRotations;
    int BeltRotationState = 0;
    float justAdded;
    float timeSinceFixed;
    bool canOutput;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        pos = transform.position;
        //x is ID
        //y is time spent on belt
        FindObjectOfType<Skins>().Setskin(Skin.SkinType.Belt, gameObject);

        if (sprite == null)
        {
            sprite = new GameObject("Sprite");
            sprite.transform.position = pos + new Vector2(0, 0);
            sprite.transform.parent = gameObject.transform;
            sprite.transform.eulerAngles = (Vector3.zero);
        }
        spriteSR = sprite.AddComponent<SpriteRenderer>();
        spriteSR.sortingLayerName = "Buildings";
        spriteSR.sortingOrder = 1;
        if(itemID == Vector2.zero)
            itemID = new Vector2(-1, 0);
        UpdateBeltInput();
        UpdateAdjacentBelts();

        timeTotravel = world.speedstates.BeltInfo.speed;

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().LateSave();
    }

    public void UpdateAdjacentBelts()
    {
        Profiler.BeginSample("UpdateAdjacentBelts");
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
                if (cell.TryGetComponent(out belt))
                {
                    belt.UpdateBeltInput();
                }
            }
        }
        Profiler.EndSample();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceFixed += Time.deltaTime;
        SetSpritePos(timeSinceFixed);
    }
    public void UpdateBeltInput()
    {
        Profiler.BeginSample("UpdateBeltInput");
        Vector2 Offset = Vector2.zero;
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                Offset = new Vector2(-1, 0);
                break;
            case 90:
                Offset = new Vector2(0, -1);
                break;
            case 180:
                Offset = new Vector2(1, 0);
                break;
            case 270:
                Offset = new Vector2(0, 1);
                break;
        }
        outputCoord = pos + new Vector2(Offset.x,Offset.y);
        GameObject rearObj;
        GameObject leftObj;
        GameObject rightObj;

        Vector2 location;
        location = (Vector2)(Quaternion.Euler(0, 0, 180) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out rearObj);
        location = (Vector2)(Quaternion.Euler(0, 0, 90) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out leftObj);
        location = (Vector2)(Quaternion.Euler(0, 0, 270) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out rightObj);

        bool rearOcc = false;
        bool leftOcc = false;
        bool rightOcc = false;

        if (rearObj != null)
            rearOcc = rearObj.GetComponent<Belt>() != null || rearObj.GetComponent<Splitter>() != null || rearObj.GetComponent<UnderGroundBelt>() != null;
        if (leftObj != null)
            leftOcc = leftObj.GetComponent<Belt>() != null || leftObj.GetComponent<Splitter>() != null || leftObj.GetComponent<UnderGroundBelt>() != null;
        if (rightObj != null)
            rightOcc = rightObj.GetComponent<Belt>() != null || rightObj.GetComponent<Splitter>() != null || rightObj.GetComponent<UnderGroundBelt>() != null;

        if (rearOcc && rearObj.GetComponent<Belt>() != null)
            rearOcc = rearObj.GetComponent<Belt>().outputCoord == pos;
        if (leftOcc && leftObj.GetComponent<Belt>() != null)
            leftOcc = leftObj.GetComponent<Belt>().outputCoord == pos;
        if (rightOcc && rightObj.GetComponent<Belt>() != null)
            rightOcc = rightObj.GetComponent<Belt>().outputCoord == pos;

        if(rearOcc && rearObj.GetComponent<UnderGroundBelt>() != null)
            rearOcc = rearObj.GetComponent<UnderGroundBelt>().OutputCoord == pos;
        if (leftOcc && leftObj.GetComponent<UnderGroundBelt>() != null)
            leftOcc = leftObj.GetComponent<UnderGroundBelt>().OutputCoord == pos;
        if (rightOcc && rightObj.GetComponent<UnderGroundBelt>() != null)
            rightOcc = rightObj.GetComponent<UnderGroundBelt>().OutputCoord == pos;


        if (!rearOcc && (leftOcc ^ rightOcc))
        {
            if (rightOcc) 
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[1];
                BeltRotationState = 1;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[2];
                BeltRotationState = 2;
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[0];
            BeltRotationState = 0;
        }

        Profiler.EndSample();
    }
    void OnTick()
    {
        if(justAdded == Time.fixedTime)
        {
            timeSinceFixed = 0;
            justAdded += 0.01f;
            return;
        }
        Profiler.BeginSample("Belt Tick Logic");
        UpdateSpritePositions(true);
        timeSinceFixed = 0;
        Profiler.EndSample();
    }
    void SetSpritePos(float Offset)
    {
        float xVal = 0;
        float yVal = 0;

        if ((itemID.y + Offset) >= timeTotravel && !canOutput)
        {
            Offset = timeTotravel - itemID.y;
        }
        switch (BeltRotationState)
        {
            case 0:
                xVal = 0.5f - ((itemID.y + Offset) / timeTotravel);
                yVal = 0;

                break;
            case 1:
                xVal = Mathf.Clamp((0.5f - ((itemID.y + Offset) / timeTotravel) - 0.2f) * 0.7f, -0.7f, 0);
                yVal = Mathf.Clamp(0.5f - ((itemID.y + Offset) / timeTotravel), 0, 0.5f);

                break;
            case 2:
                xVal = Mathf.Clamp((0.5f - ((itemID.y + Offset) / timeTotravel) - 0.2f) * 0.7f, -0.7f, 0);
                yVal = Mathf.Clamp(0.5f - ((itemID.y + Offset) / timeTotravel), 0, 0.5f) * -1;

                break;
        }

        sprite.transform.localPosition = new Vector3(xVal, yVal, (itemID.y + Offset));
    }
    public void UpdateSpritePositions(bool moveForward)
    {
        Profiler.BeginSample("Move Forward");
        //progress forward
        if (itemID.x != -1)
        {
            if (moveForward)
                itemID.y += Time.fixedDeltaTime;
            spriteSR.sprite = world.items[(int)itemID.x].sprite;

            SetSpritePos(0);


        }
        Profiler.EndSample();
        Profiler.BeginSample("Check For Output");
        //check to output
        if (itemID.y >= timeTotravel)
        {
            Profiler.BeginSample("Check Output Spot");
            bool outBool = OutputItem((int) itemID.x);
            Profiler.EndSample();
            if (outBool)
            {
                timeSinceFixed = 0;
                spriteSR.sprite = null;
                itemID = new Vector2(-1, 0);
                canOutput= true;
            }
            else
            {
                canOutput = false;
                itemID.y = timeTotravel;
            }
        }
        Profiler.EndSample();
    }
    public bool inputItem(int initemID, float time)
    {
        if (itemID.x == -1)
            {
                justAdded = Time.fixedTime;
                //itemID.y = time * timeTotravel;
                itemID.y = time;
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
        Vector2Int outputpos = new Vector2Int(-1,0);
        switch(gameObject.transform.rotation.eulerAngles.z)
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
    public bool inputItem(int inItem, Vector2Int inPos, float Offset)
    {
        Vector2Int relativepos = inPos - Vector2Int.RoundToInt(pos);
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
            Offset = 0.9f;
        }
        if (inputItem(inItem, Offset))
        {
            return true;
        }
        return false;
    }
    bool OutputItem(int initemID)
    {
        GameObject OutputObj;

        if (world.OccupiedCells.TryGetValue(outputCoord, out OutputObj))
            return ItemReceiver.CanObjectAcceptItem(OutputObj, initemID, Vector2Int.RoundToInt(pos), itemID.y - timeTotravel);
        
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
        world.inv.AddItem(builds.AllBuildings[1].cost[0].x, Mathf.Clamp(builds.AllBuildings[2].count - 2, 1, int.MaxValue));
        world.inv.AddItem((int)itemID.x, 1);


        world.OccupiedCells.Remove(pos);

        builds.AllBuildings[1].count--;
         
        Destroy(gameObject);
    }
    public GameObject RecipeDisplay;
    float timeHovering;
    private void OnMouseOver()
    {
        timeHovering += Time.deltaTime;
        if (timeHovering < 1)
            return;

        if (GetComponentInChildren<DisplayRecipes>() != null)
            return;

        GameObject rec = Instantiate(RecipeDisplay, transform);
        rec.transform.rotation = Quaternion.identity;
        rec.transform.position = new Vector3(3, 2, -0.01f) + gameObject.transform.position;
        rec.GetComponent<DisplayRecipes>().type = DisplayRecipes.BuildingType.Belt;

    }
    private void OnMouseExit()
    {
        timeHovering = 0;
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }

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
