using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class Refinery : MonoBehaviour
{
    WorldGeneration world;
    public Vector2Int pos;

    public Inventory inputInv;
    public Inventory outputInv;

    AudioSource AS;

    [SerializeField]
    public Recipes recipies;

    string recipepath = "Recipes";
    string saveExtention = "/Refinery.dat";

    Vector2Int outputCoord = new Vector2Int();
    Vector2Int inputCoord = new Vector2Int();
    Vector2Int outFromCoord = new Vector2Int();

    bool isSmelting;
    float smeltAnimationSpeed;

    Vector3 basePos;
    Transform[] transforms;

    TickEvents tickEvents;

    public float RProgress;
    public float RTime = 1;

    bool isJammed = false;

    public GameObject RecipeDisplay;
    float timeHovering;

    [Serializable]
    public struct Recipes
    {
        public RefineryRecipe[] values;
        public int selectedRecipe;
    }

    [Serializable]
    public struct RefineryRecipe
    {
        public int inputItemID;
        public int inCount;
        public int outputItemID;
        public int outCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        transforms = GetComponentsInChildren<Transform>();
        basePos = transforms[1].localPosition + new Vector3(0, 0.6f, 0);

        if (File.Exists(recipepath + saveExtention))
        {
            recipies = (Recipes)JsonUtility.FromJson(System.IO.File.ReadAllText(recipepath + saveExtention), typeof(Recipes));
        }
        /*
        Directory.CreateDirectory(recipepath);
        System.IO.File.WriteAllText(recipepath + saveExtention, JsonUtility.ToJson(recipies,true));
        */

        world = FindObjectOfType<WorldGeneration>();
        pos = Vector2Int.RoundToInt((Vector2)gameObject.transform.position + new Vector2(-0.5f, -0.5f));
        if (inputInv == null || inputInv.items.Length == 0)
            inputInv = new Inventory(1);
        if (outputInv == null || outputInv.items.Length == 0)
            outputInv = new Inventory(1);

        inputInv.maxStackSize = 10;
        outputInv.maxStackSize = 10;
        SetOutput();

        RTime = world.speedstates.RefineryInfo.speed;

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().Save();

    }
    private void Update()
    {
        SmeltingAnimation();
        if (isSmelting)
        {
            AS.volume += Time.deltaTime;
            if(AS.volume > 1)
                AS.volume = 1;
            smeltAnimationSpeed += Time.deltaTime;
            if(smeltAnimationSpeed > 1)
                smeltAnimationSpeed = 1;
        }
        else
        {
            AS.volume -= Time.deltaTime;
            if (AS.volume < 0)
                AS.volume = 0;
            smeltAnimationSpeed -= Time.deltaTime;
            if (smeltAnimationSpeed < 0)
                smeltAnimationSpeed = 0;
        }
    }
    void FixedUpdate()
    {
        // OnTick();
    }
    void SetOutput()
    {
        /*
         inputCoord = pos + new Vector2Int(2, 1);
         inputCoord = pos + new Vector2Int(0, 2);
         inputCoord = pos + new Vector2Int(-1, 0);
         inputCoord = pos + new Vector2Int(1, -1);
         */
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2Int(-1, 0);
                outFromCoord = pos;
                inputCoord = pos + new Vector2Int(2, 1);
                break;
            case 90:
                outputCoord = pos + new Vector2Int(1, -1);
                outFromCoord = pos + new Vector2Int(1, 0);
                inputCoord = pos + new Vector2Int(0, 2);
                break;
            case 180:
                outputCoord = pos + new Vector2Int(2, 1);
                outFromCoord = pos + new Vector2Int(1, 1);
                inputCoord = pos + new Vector2Int(-1, 0);
                break;
            case 270:
                outputCoord = pos + new Vector2Int(0, 2);
                outFromCoord = pos + new Vector2Int(0, 1);
                inputCoord = pos + new Vector2Int(1, -1);
                break;
        }
    }
    void UpdateSound()
    {
        if (isSmelting)
        {
            if (!AS.isPlaying)
            {
                AS.time = (float)new System.Random().NextDouble() * AS.clip.length;
                AS.Play();
            }
        }
    }
    void OnTick()
    {
        


        Profiler.BeginSample("Refinery Tick Logic");

        if (inputInv.items[0].ID == -1 || inputInv.items[0].count < recipies.values[recipies.selectedRecipe].inCount)
        {
            isSmelting = false;
            Profiler.EndSample();
            UpdateSound();
            return;
        }
        if (!isJammed)
        {
            isSmelting = true;
            RProgress += Time.fixedDeltaTime;
        }
        if (RProgress >= RTime)
        {
            Profiler.BeginSample("AttemptSmelt");
            RefreshRecipe();
            AttemptSmelt();
            Profiler.EndSample();
        }
        if (outputInv.items[0].count > 0)
        {
            Profiler.BeginSample("AttemptOutput");
            if (OutputItem())
            {
                outputInv.RemoveItem(new int2[] { new int2(outputInv.items[0].ID, 1) }, 1.0f);
            }
            Profiler.EndSample();
        }
        Profiler.EndSample();
        UpdateSound();
    }
    void SmeltingAnimation()
    {
        float speed = Mathf.Sqrt(smeltAnimationSpeed);
        transforms[4].localPosition = basePos + (Vector3)Ocsillation(45, 0.005f,0.2f) * speed;
        transforms[2].localPosition = basePos + (Vector3)Ocsillation(7, 0.01f, 0.2f) * speed;
        transforms[3].localPosition = basePos + (Vector3)Ocsillation(12,0.01f, 0.2f) * speed;
    }
    Vector2 Ocsillation(int seed, float Strength, float speed)
    {
        return new Vector2(Mathf.Sin(speed * Time.frameCount + (seed * 1.2154f) + pos.x * (seed * 5.2154f)),
                           Mathf.Sin(speed * Time.frameCount + (seed * 2.2154f) + pos.y * (seed * 3.2154f))) * Strength;
    }
    void AttemptSmelt()
    {
        int outID = recipies.values[recipies.selectedRecipe].outputItemID;
        if (outputInv.AddItem(outID, recipies.values[recipies.selectedRecipe].outCount))
        {
            inputInv.RemoveItem(new int2[] { new int2(inputInv.items[0].ID, recipies.values[recipies.selectedRecipe].inCount) }, 1.0f);
            RProgress -= RTime;
        }
        else
        {
            isSmelting= false;
            RProgress = RTime;
        }
    }
    private void OnMouseOver()
    {
        timeHovering += Time.deltaTime;
        if (timeHovering < 1)
            return;

        if (GetComponentInChildren<DisplayRecipes>() != null)
            return;

            GameObject rec = Instantiate(RecipeDisplay, transform);
            rec.transform.rotation = Quaternion.identity;
            rec.transform.position = new Vector3(3, 3, -0.01f) + gameObject.transform.position;
            rec.GetComponent<DisplayRecipes>().type = DisplayRecipes.BuildingType.Refinery;
        
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
    public void RotateCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        SetOutput();
        FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;

    }
    public void RotateCCW()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        SetOutput();
        FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;

    }
    public void Delete()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        world.inv.AddItem((int)builds.AllBuildings[2].cost[0].x, Mathf.Clamp(builds.AllBuildings[2].count - 2, 1, int.MaxValue));
        world.inv.AddItem((int)builds.AllBuildings[2].cost[1].x, Mathf.Clamp(builds.AllBuildings[2].count - 2, 1, int.MaxValue));

        world.inv.AddItem(inputInv.items[0].ID, inputInv.items[0].count);
        world.inv.AddItem(outputInv.items[0].ID, outputInv.items[0].count);


        world.OccupiedCells.Remove(pos);
        world.OccupiedCells.Remove(pos + new Vector2(0, 1));
        world.OccupiedCells.Remove(pos + new Vector2(1, 0));
        world.OccupiedCells.Remove(pos + new Vector2(1, 1));

        builds.AllBuildings[2].count--;

        Destroy(gameObject);
    }
    bool OutputItem()
    {
        int itemID = outputInv.items[0].ID;

        GameObject OutputObj;
        world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);

        if (OutputObj != null)
            return ItemReceiver.CanObjectAcceptItem(OutputObj, itemID, Vector2Int.RoundToInt(outFromCoord));
        /*
        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
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
    public bool InputItem(int ID, int count, Vector2 inPos)
    {
        if ((inPos - inputCoord).sqrMagnitude <= 0.05f)
        {
            if (inputInv.AddItem(ID, count))
            {
                RefreshRecipe();
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
    void RefreshRecipe()
    {
        isJammed = true;
        for (int i = 0; i < recipies.values.Length; i++)
        {
            if (inputInv.items[0].ID == recipies.values[i].inputItemID)
            {
                recipies.selectedRecipe = i;
                isJammed = false;
                break;
            }
        }
    }
}