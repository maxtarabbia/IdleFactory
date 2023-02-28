using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.Profiling;
using System.Linq;
using UnityEngine.UI;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector2, Cell> oreMap = new Dictionary<Vector2, Cell>();
    int Spawnsize = 20;

    public int Seed = 42;

    public Sprite Blank;
    public Sprite Iron_Ore;
    public Sprite Copper_Ore;
    public Sprite OOB;

    public int selectedBuildableIndex;

    public Canvas UICanvas;

    public Image UIImage;
    public TextMeshProUGUI UIText;

    public Inventory inv;
    public int[] CurrencyRates;
    public int Currency;

    public Vector2 CamCoord;
    public Vector2 CamSize;

    public int OccCellcount;
    [SerializeField]
    public SpeedStates speedstates;

    [SerializeField]
    public Dictionary<Vector2, GameObject> OccupiedCells = new Dictionary<Vector2, GameObject>();

    public bool GodMode;

    public GameObject OOBprefab;

    void Start()
    {
        SetInventory();
    }
    public void setBuildableIndex(int index)
    {
        selectedBuildableIndex = index;
        UIImage.sprite = GetComponent<Buildings>().AllBuildings[index].prefab.GetComponent<SpriteRenderer>().sprite;
        UIText.text = GetComponent<Buildings>().AllBuildings[index].name;
    }
    public void SetInventory()
    {
        inv = new Inventory(5);
        if (GodMode)
        {
            inv.AddItem(1, 5000);
            inv.AddItem(2, 5000);
            inv.AddItem(3, 5000);
            inv.AddItem(4, 5000);
            Currency = 5000000;
        }
        else
        {
            inv.AddItem(1, 5);
            inv.AddItem(2, 0);
        }
    }
    public void Initialize(int size)
    {

        OccupiedCells.Clear();
        oreMap.Clear();
        
        int offset = size/ 2;
        for (int x = 0; x < size; x++)
        {
            for(int y =0; y < size; y++)
            {
                SetDefaultCell(new Vector2(x - offset, y - offset));
            }
        }
        UpdateNewBlocks();
    }
    public void UpdateNewBlocks()
    {
        foreach(var cell in oreMap)
        {
            cell.Value.setToDelete = true;
        }


        Profiler.BeginSample("Checking For Now Blocks");

        Vector2 startingcoord = CamCoord - (CamSize);
        startingcoord = startingcoord + new Vector2(-1, -1);
        startingcoord.x = Mathf.Round(startingcoord.x);
        startingcoord.y = Mathf.Round(startingcoord.y);
        for(int x = 0; x < (CamSize.x+1) * 2; x++)
        {
            for (int y = 0; y < (CamSize.y+1) * 2; y++)
            {
                if(!oreMap.ContainsKey(new Vector2(x,y)+startingcoord))
                {
                    Profiler.BeginSample("Making New Block");
                    SetDefaultCell(new Vector2(x, y) + startingcoord);
                    Profiler.EndSample();
                }
                else
                {
                    if(!oreMap[(new Vector2(x, y) + startingcoord)].gameobject.activeInHierarchy)
                        SetDefaultCell(new Vector2(x, y) + startingcoord);
                    oreMap[(new Vector2(x, y) + startingcoord)].setToDelete = false;
                }
            }
        }
        Profiler.BeginSample("Culling Blocks");
        List<Cell> keys = oreMap.Values.ToList();
        Profiler.BeginSample("looping Blocks");
        foreach (var key in keys)
        {
            if (key.setToDelete)
            {
                key.gameobject.gameObject.SetActive(false);
            }
        }
        Profiler.EndSample();
        Profiler.EndSample();
        Profiler.EndSample();
    }
    public void SetDefaultCell(Vector2 position)
    {
        
        if (!oreMap.ContainsKey(position))
        {
            Profiler.BeginSample("Setting ore");
            float scale = 0.05f;
            Cell newcell = new Cell();
            if (noise.cnoise(new Vector2(Seed, Seed) + position * scale) > 0.55)
            {
                newcell.ID = 1;
                newcell.name = "Iron Ore";

            }
            else if (noise.cnoise(new Vector2(50 + Seed * 20, Seed - 85) + position * scale) > 0.55)
            {
                newcell.ID = 2;
                newcell.name = "Copper Ore";
            }
            else
            {
                newcell.ID = 0;
                newcell.name = "Air";
            }
            float dist = Vector2.Distance(math.clamp(position, new float2(-100, -100), new float2(100, 100)), position) + noise.cnoise(new Vector2(Seed * -23, Seed - 800) + position * scale * 1f) * 4;
            if (dist > 20)
            {
                newcell.ID = 3;
                newcell.name = "OOB Block";
                newcell.dist = dist-20;
            }
            Profiler.EndSample();
            if (dist > 32)
            {
                
                return;
            }

            oreMap.Add(position, newcell);
            Profiler.BeginSample("Creating new Block GameObject");
            oreMap[position].gameobject = GenerateCell(position);
            Profiler.EndSample();
        }
        else 
        {
            oreMap[position].gameobject.gameObject.SetActive(true);
        }



    }
    void Update()
    {
        if(UICanvas == null)
        {
            UICanvas = FindObjectOfType<Canvas>();
        }
        //print("oremapcount: " + oreMap.Count + "\nOccCellCount: " + OccupiedCells.Count);
    }
    void FixedUpdate()
    {
        OccCellcount = OccupiedCells.Count;
        UpdateUI();
    }
    Vector2[] CountCosts()
    {
        Buildings builds = GetComponent<Buildings>();
        Vector2[] costs = new Vector2[builds.AllBuildings[selectedBuildableIndex].cost.Length];
        
        for(int i = 0; i < builds.AllBuildings[selectedBuildableIndex].cost.Length; i++)
        {
            costs[i].x = builds.AllBuildings[selectedBuildableIndex].cost[i].x;
            costs[i].y = builds.AllBuildings[selectedBuildableIndex].cost[i].y * (builds.AllBuildings[selectedBuildableIndex].count + 1);
        }
        return costs;
    }
    void UpdateUI()
    {
        string UItext = new string("");
        var tmpUI = UICanvas.GetComponentInChildren<TextMeshProUGUI>();

        Vector2[] costs = CountCosts();

        foreach(var item in inv.items)
        {
            if (item.ID != -1)
            {
                UItext = UItext + inv.IdNames[item.ID] + ": " + IntLib.IntToString(item.count);
                for(int i = 0; i < costs.Length; i++)
                {
                    if (costs[i].x == item.ID)
                    {
                        UItext = UItext + " - " + IntLib.IntToString(Mathf.RoundToInt(costs[i].y));
                    }
                }
                UItext = UItext + "\n";
            }

        }
        UItext += "\n$" + IntLib.IntToString(Currency);

        tmpUI.text = UItext;
       
    }
    GameObject GenerateCell(Vector2 position)
    {
        Profiler.BeginSample("Creating Gameobject");
        int ID = oreMap[position].ID;
        SpriteRenderer SR;
        GameObject cell;
        if (ID ==3)
        {
            cell = Instantiate(OOBprefab);
            SR = cell.GetComponent<SpriteRenderer>();
            cell.GetComponent<WallObject>().dist = oreMap[position].dist;
        }
        else
        {
            cell = new GameObject();
            cell.AddComponent<ObjectPlacement>();
            SR = cell.AddComponent<SpriteRenderer>();
        }

        cell.transform.position = position;
        cell.transform.position += new Vector3(0, 0, 10);
        cell.transform.localScale = Vector3.one;
        cell.transform.parent = gameObject.transform.GetChild(1);
        cell.name = oreMap[position].name;

        BoxCollider2D boxCol = cell.AddComponent<BoxCollider2D>();
        boxCol.edgeRadius = 0.0f;
        boxCol.size = new Vector2(1f,1f);
        SR.sortingLayerName = "Ores";

        switch(ID)
        {
            case 0:
                SR.sprite = Blank; 
                break;
            case 1:
                SR.sprite = Iron_Ore;
                break;
            case 2:
                SR.sprite = Copper_Ore;
                break;
            case 3:
                SR.sprite = OOB;
                break;
        }
        Profiler.EndSample();
        return cell;
    }
}
[Serializable]
public class Buildable
{
    public GameObject objectToInstance;
    public Vector2[] costs = new Vector2[1];
    public int size;

    public Buildable(GameObject gameObject, Vector2[] IDcosts, int size)
    {
        this.size= size;
        this.objectToInstance = gameObject;
        IDcosts = costs;
    }
}

public class Cell
{
    public GameObject gameobject;
    public int ID;
    public string name;
    public bool setToDelete;
    public float dist;
}

