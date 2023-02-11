using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.Profiling;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector2, Cell> oreMap = new Dictionary<Vector2, Cell>();
    int Spawnsize = 20;

    public int Seed = 42;

    public Sprite Blank;
    public Sprite Iron_Ore;
    public Sprite Copper_Ore;

    public int selectedBuildableIndex;

    public Canvas UICanvas;

    public Inventory inv;

    public Vector2 CamCoord;
    public Vector2 CamSize;

    public int OccCellcount;

    [SerializeField]
    public Dictionary<Vector2, GameObject> OccupiedCells = new Dictionary<Vector2, GameObject>();

    public bool GodMode;

    void Start()
    {
        SetInventory();
       // Initialize(Spawnsize);
    }
    public void SetInventory()
    {
        inv = new Inventory(4);
        if (GodMode)
        {
            inv.AddItem(1, 5000);
            inv.AddItem(2, 5000);
            inv.AddItem(3, 5000);
            inv.AddItem(4, 5000);
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
            }
        }

        Profiler.EndSample();
    }
    public void SetDefaultCell(Vector2 position)
    {
        float scale = 0.1f;
        Cell newcell = new Cell();
        if (noise.cnoise(new Vector2(Seed, Seed) + position * scale) > 0.4)
        {
            newcell.ID = 1;
            newcell.name = "Iron Ore";
            
        }
        else if (noise.cnoise(new Vector2(Seed *20, Seed - 85) + position * scale) > 0.5)
        {
            newcell.ID = 2;
            newcell.name = "Copper Ore";
        }
        else
        {
            newcell.ID = 0;
            newcell.name = "Air";
        }
        oreMap.Add(position, newcell);
        Profiler.BeginSample("Creating new Block GameObject");
        GenerateCell(position);
        Profiler.EndSample();

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
                UItext = UItext + inv.IdNames[item.ID] + ": " + item.count;
                for(int i = 0; i < costs.Length; i++)
                {
                    if (costs[i].x == item.ID)
                    {
                        UItext = UItext + " - " + costs[i].y;
                    }
                }
                UItext = UItext + "\n";
            }

        }

        tmpUI.text = UItext;
       
    }
    GameObject GenerateCell(Vector2 position)
    {
        GameObject cell = new GameObject();
        cell.transform.position = position;
        cell.transform.localScale = Vector3.one;
        cell.transform.parent = gameObject.transform.GetChild(1);
        cell.name = oreMap[position].name;
        int ID = oreMap[position].ID;
        SpriteRenderer SR = cell.AddComponent<SpriteRenderer>();
        cell.AddComponent<ObjectPlacement>();
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
        }
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
    public int ID;
    public string name;
}
