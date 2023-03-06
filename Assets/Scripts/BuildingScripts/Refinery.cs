using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

public class Refinery : MonoBehaviour
{
    WorldGeneration world;
    public Vector2Int pos;

    public Inventory inputInv;
    public Inventory outputInv;


    [SerializeField]
    public Recipes recipies;

    string recipepath = "Recipes";
    string saveExtention = "/Refinery.dat";

    Vector2Int outputCoord = new Vector2Int();
    Vector2Int inputCoord = new Vector2Int();
    Vector2Int outFromCoord = new Vector2Int();

    TickEvents tickEvents;

    public float RProgress;
    public float RTime = 1;

    bool isJammed = false;

    public int inCount;
    public int outCount;
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
    void OnTick()
    {
        inCount = inputInv.items[0].count;
        outCount = outputInv.items[0].count;


        if (inputInv.items[0].ID == -1 || inputInv.items[0].count < recipies.values[recipies.selectedRecipe].inCount)
            return;
        if (!isJammed)
            RProgress += Time.fixedDeltaTime;
        if (RProgress >= RTime)
        {
            RefreshRecipe();
            AttemptSmelt();
        }
        if (outputInv.items[0].count > 0)
        {
            if (OutputItem())
            {
                outputInv.RemoveItem(new int2[] { new int2(outputInv.items[0].ID, 1) }, 1.0f);
            }
        }
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
            RProgress = RTime;
        }
    }
    private void OnMouseOver()
    {
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
            SetOutput();
            FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
             
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            SetOutput();
            FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
             
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[2].cost[0].x, (int)builds.AllBuildings[2].cost[0].y);
            world.inv.AddItem((int)builds.AllBuildings[2].cost[1].x, (int)builds.AllBuildings[2].cost[1].y);

            world.OccupiedCells.Remove(pos);
            world.OccupiedCells.Remove(pos + new Vector2(0, 1));
            world.OccupiedCells.Remove(pos + new Vector2(1, 0));
            world.OccupiedCells.Remove(pos + new Vector2(1, 1));

            builds.AllBuildings[2].count--;
             
            Destroy(gameObject);
        }
        */
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
        world.inv.AddItem((int)builds.AllBuildings[2].cost[0].x, (int)builds.AllBuildings[2].cost[0].y);
        world.inv.AddItem((int)builds.AllBuildings[2].cost[1].x, (int)builds.AllBuildings[2].cost[1].y);

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

        GameObject OutputObj = null;
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