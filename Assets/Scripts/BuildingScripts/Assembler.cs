using System;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

public class Assembler : MonoBehaviour
{
    WorldGeneration world;
    public Vector2Int pos;

    public GameObject RedX;
    float blinkingTimer;

    public Inventory inputInv;
    public Inventory inputInv2;
    public Inventory outputInv;

    public bool isJammed = false;

    Vector3 startingPos;
    bool isAssembling;
    float animationStrength = 0f;
    float MaxStrength = 0.1f;

    AudioSource AS;

    [SerializeField]
    public Recipes recipies;

    ResourceStats stats;

    string recipepath = "Recipes";
    string saveExtention = "/Assembler.dat";

    Vector2Int outputCoord = new Vector2Int();
    Vector2Int outFromCoord = new Vector2Int();
    Vector2Int inputCoord = new Vector2Int();
    Vector2Int inputCoord2 = new Vector2Int();

    TickEvents tickEvents;

    public float RProgress;
    public float Speed = 1;

    public GameObject RecipeDisplay;
    float timeHovering;

    [Serializable]
    public struct Recipes
    {
        public AssemblerRecipe[] values;
        public int selectedRecipe;
    }
    [Serializable]
    public struct AssemblerRecipe
    {
        public int inputItemID;
        public int inCount;
        public int inputItemID2;
        public int inCount2;
        public int outputItemID;
        public int outCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<ResourceStats>();
        AS = GetComponent<AudioSource>();
        if (File.Exists(recipepath + saveExtention))
        {
            recipies = (Recipes)JsonUtility.FromJson(System.IO.File.ReadAllText(recipepath + saveExtention), typeof(Recipes));
        }
        startingPos = transform.localPosition;
        world = FindObjectOfType<WorldGeneration>();
        pos = Vector2Int.RoundToInt((Vector2)gameObject.transform.position + new Vector2(-0.5f, -0.5f));
        if (inputInv2 == null || inputInv2.items.Length == 0)
            inputInv2 = new Inventory(1);
        if (inputInv == null || inputInv.items.Length == 0)
            inputInv = new Inventory(1);
        if (outputInv == null || outputInv.items.Length == 0)
            outputInv = new Inventory(1);

        FindObjectOfType<Skins>().Setskin(Skin.SkinType.Assembler, gameObject);

        inputInv.maxStackSize = 10;
        inputInv2.maxStackSize= 10;
        outputInv.maxStackSize = 10;
        SetOutput();

        Speed = world.speedstates.RefineryInfo.speed;

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().Save();
        SetDR();
        SetRecipe();
    }

    void Update()
    {
        UpdateRedX();
        if (isAssembling)
        {
            AS.volume += Time.deltaTime;
            if (AS.volume > 1)
                AS.volume = 1;
            animationStrength += Time.deltaTime * MaxStrength;
        }
        else
        {
            
            AS.volume -= Time.deltaTime;
            if (AS.volume < 0)
                AS.volume = 0;
            animationStrength -= Time.deltaTime * MaxStrength;
        }
        animationStrength = Mathf.Clamp(animationStrength, 0, MaxStrength);
        transform.localPosition = startingPos + new Vector3(math.sin(Time.time*50*Speed), math.cos(Time.time*50*Speed),0) * animationStrength * .2f;
    }
    void UpdateSound()
    {
        if (isAssembling)
        {
            if (!AS.isPlaying)
            {
                AS.time = (float)new System.Random().NextDouble() * AS.clip.length;
                AS.Play();
            }
        }
    }
    void SetOutput()
    {
        
        
        inputCoord2 = pos + new Vector2Int(-1, 0);
        inputCoord2 = pos + new Vector2Int(1, -1);
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2Int(-1, 1);
                outFromCoord = pos + new Vector2Int(0, 1);
                inputCoord = pos + new Vector2Int(2, 0);
                inputCoord2 = pos + new Vector2Int(2, 1);
                break;
            case 90:
                outputCoord = pos + new Vector2Int(0, -1);
                outFromCoord = pos + new Vector2Int(0, 0);
                inputCoord = pos + new Vector2Int(1, 2);
                inputCoord2 = pos + new Vector2Int(0, 2);
                break;
            case 180:
                outputCoord = pos + new Vector2Int(2, 0);
                outFromCoord = pos + new Vector2Int(1, 0);
                inputCoord = pos + new Vector2Int(-1, 1);
                inputCoord2 = pos + new Vector2Int(-1, 0);
                break;
            case 270:
                outputCoord = pos + new Vector2Int(1, 2);
                outFromCoord = pos + new Vector2Int(1, 1);
                inputCoord = pos + new Vector2Int(0, -1);
                inputCoord2 = pos + new Vector2Int(1, -1);
                break;
        }
    }
    void OnTick()
    {
        Profiler.BeginSample("Assembler Tick Logic");

        if (inputInv.items[0].ID != recipies.values[recipies.selectedRecipe].inputItemID || inputInv2.items[0].ID != recipies.values[recipies.selectedRecipe].inputItemID2)
        {
            //wrong items
            isAssembling = false;
            UpdateSound();
            Profiler.EndSample();
            return;
        }
        if(inputInv.items[0].count < recipies.values[recipies.selectedRecipe].inCount || inputInv2.items[0].count < recipies.values[recipies.selectedRecipe].inCount2)
        {
            //not enough items
            isAssembling = false;
            UpdateSound();
            Profiler.EndSample();
            return;
        }
        if(!isJammed)
            RProgress += Time.fixedDeltaTime;
        isAssembling = true;
        if (RProgress >= 1/Speed)
        {
            AttemptBuild();
        }
        if (outputInv.items[0].count > 0)
        {
            if (OutputItem())
            {
                outputInv.RemoveItem(new int2[] { new int2(outputInv.items[0].ID, 1) }, 1.0f);
            }
            else
            {

            }
        }
        UpdateSound();
        Profiler.EndSample();
    }
    void AttemptBuild()
    {
        SetRecipe();
        int outID = recipies.values[recipies.selectedRecipe].outputItemID;
        if (!inputInv2.CheckRemoveItem(new int2[] { new int2(recipies.values[recipies.selectedRecipe].inputItemID2, recipies.values[recipies.selectedRecipe].inCount2) }, 1f))
            return;
        if (outputInv.AddItem(outID, recipies.values[recipies.selectedRecipe].outCount))
        {
            stats.Additem(outID, recipies.values[recipies.selectedRecipe].outCount);
            inputInv2.RemoveItem(new int2[] { new int2(recipies.values[recipies.selectedRecipe].inputItemID2, recipies.values[recipies.selectedRecipe].inCount2) }, 1f);
            inputInv.RemoveItem(new int2[] { new int2(inputInv.items[0].ID, recipies.values[recipies.selectedRecipe].inCount) }, 1.0f);
            RProgress -= 1 / Speed;
        }
        else
        {
            isAssembling = false;
            RProgress =1/Speed;
        }
    }
    private void OnMouseOver()
    {
        SetDR();
        /*
        TutorialState TS = FindObjectOfType<TutorialState>();
        TS.textMeshProUGUI.text = DebugString();
        TS.isoverRide = true;
        */
        }
    string DebugString()
    {
        string o = inputInv.items[0].ID.ToString() + ":" + inputInv.items[0].count.ToString();
        o += "\n" + inputInv2.items[0].ID.ToString() + ":" + inputInv2.items[0].count.ToString();
        o += "\n" + outputInv.items[0].ID.ToString() + ":" + outputInv.items[0].count.ToString();
        o += "\n" + recipies.selectedRecipe;


        return o;
    }
    void SetDR()
    {

        RedX.transform.rotation = Quaternion.identity;
        RedX.transform.position = gameObject.transform.position + new Vector3(0.5f, 0.5f, -0.001f);

        timeHovering += Time.deltaTime;
        if (timeHovering < 1)
            return;

        if (GetComponentInChildren<DisplayRecipes>() != null)
            return;

        GameObject rec = Instantiate(RecipeDisplay, transform);
        rec.transform.rotation = Quaternion.identity;
        rec.transform.position = new Vector3(3, 3, -0.01f) + gameObject.transform.position;
        rec.GetComponent<DisplayRecipes>().type = DisplayRecipes.BuildingType.Assembler;


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
    void UpdateRedX()
    {
        float rate = 1;
        if (isJammed)
        {
            blinkingTimer += Time.deltaTime;
            if (blinkingTimer > rate)
                blinkingTimer = 0;
            if (blinkingTimer > rate / 2)
            {
                RedX.GetComponent<SpriteRenderer>().color = Color.clear;
            }
            else
            {
                RedX.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        else
        {
            RedX.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }
    public void RotateCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        SetOutput();
        FindObjectOfType<Buildings>().AllBuildings[5].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }
        SetDR();
    }
    public void RotateCCW()
    {
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        SetOutput();
        FindObjectOfType<Buildings>().AllBuildings[5].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        SetDR();
    }
    public void Delete()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        world.inv.AddItem((int)builds.AllBuildings[5].cost[0].x, Mathf.Clamp(builds.AllBuildings[5].count - 2, 1, int.MaxValue));
        world.inv.AddItem((int)builds.AllBuildings[5].cost[1].x, Mathf.Clamp(builds.AllBuildings[5].count - 2, 1, int.MaxValue));

        world.inv.AddItem(inputInv.items[0].ID, inputInv.items[0].count);
        world.inv.AddItem(inputInv2.items[0].ID, inputInv2.items[0].count);
        world.inv.AddItem(outputInv.items[0].ID, outputInv.items[0].count);

        world.OccupiedCells.Remove(pos);
        world.OccupiedCells.Remove(pos + new Vector2(0, 1));
        world.OccupiedCells.Remove(pos + new Vector2(1, 0));
        world.OccupiedCells.Remove(pos + new Vector2(1, 1));

        builds.AllBuildings[5].count--;

        Destroy(gameObject);
    }
    bool OutputItem()
    {
        int itemID = outputInv.items[0].ID;

        GameObject OutputObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);

        if (OutputObj != null)
            return ItemReceiver.CanObjectAcceptItem(OutputObj, itemID, Vector2Int.RoundToInt(outFromCoord));

        /*

        if (cellObj != null)
        {
            Belt beltScript = cellObj.GetComponent<Belt>();
            Refinery refineryScript = cellObj.GetComponent<Refinery>();
            Splitter splitterscript = cellObj.GetComponent<Splitter>();
            Core corescript = cellObj.GetComponent<Core>();
            Assembler assembler = cellObj.GetComponent<Assembler>();
            if (assembler != null)
            {
                if (assembler.InputItem(itemID, 1, outFromCoord))
                {
                    return true;
                }
            }
            if (beltScript != null)
            {
                if (beltScript.inputItem(itemID, 0.5f))
                {
                    return true;
                }
            }
            else if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1, outFromCoord))
                {
                    return true;
                }
            }
            else if (splitterscript != null)
            {
                if (splitterscript.inputItem(itemID, 0.5f))
                {
                    return true;
                }
            }
            else if (corescript != null)
            {
                corescript.InputItem(itemID);
                return true;
            }
        }
        */
        return false;
    }
    void SetRecipe()
    {
        isJammed = true;
        for (int i = 0; i < recipies.values.Length; i++)
        {
            if (inputInv.items[0].ID == recipies.values[i].inputItemID && inputInv2.items[0].ID == recipies.values[i].inputItemID2)
            {
                recipies.selectedRecipe = i;
                isJammed = false;
                break;
            }
        }
    }
    public bool InputItem(int ID, int count, Vector2 inPos)
    {
        if ((inPos - inputCoord).sqrMagnitude <= 0.05f)
        {
            if (inputInv.AddItem(ID, count))
            {
                isJammed= true;
                SetRecipe();
                return true;
            }
            else
            {
                return false;
            }
        }
        else if((inPos - inputCoord2).sqrMagnitude <= 0.05f)
        {
            if (inputInv2.AddItem(ID, count))
            {
                SetRecipe();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private void OnDestroy()
    {
        tickEvents.MyEvent -= OnTick;
    }
}
