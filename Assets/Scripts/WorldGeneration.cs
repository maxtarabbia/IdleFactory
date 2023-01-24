using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public Dictionary<Vector2, Cell> oreMap = new Dictionary<Vector2, Cell>();
    int Spawnsize = 100;

    public int Seed = 42;

    public Sprite Blank;
    public Sprite Iron_Ore;
    public Sprite Copper_Ore;

    [SerializeField]
    //public Buildable[] buildables;
    public int selectedBuildableIndex;

    public Canvas UICanvas;

    public Inventory inv;

    public Dictionary<Vector2, GameObject> OccupiedCells = new Dictionary<Vector2, GameObject>();

    

    void Start()
    {
        inv = new Inventory(3);
        inv.AddItem(1, 500);
        inv.AddItem(2, 500);

        //setBuildingPrices();

        Initialize(Spawnsize);
    }
    /*
    public void setBuildingPrices()
    {
        foreach (var building in buildables)
        {
            Vector2[] costs = new Vector2[1];
            if (building.size == 1)
            {
                costs[0] = new Vector2(1, 5);
            }
            else
            {
                costs[0] = new Vector2 (2, 5);
            }
            building.costs = costs;
        }
    }
    */
    public void Initialize(int size)
    {
        int offset = size/ 2;
        for (int x = 0; x < size; x++)
        {
            for(int y =0; y < size; y++)
            {
                SetDefaultCell(new Vector2(x - offset, y - offset));
            }
        }
    }
    public void SetDefaultCell(Vector2 position)
    {
        float scale = 0.1f;
        Cell newcell = new Cell();
        if (noise.cnoise(new Vector2(Seed, Seed) + position * scale) > 0.5)
        {
            newcell.ID = 1;
            newcell.name = "Iron Ore";
            
        }
        else if (noise.cnoise(new Vector2(Seed *20, Seed - 85) + position * scale) > 0.6)
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
        GenerateCell(position);


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
    }
    void UpdateUI()
    {
        string UItext = new string("");
        var tmpUI = UICanvas.GetComponentInChildren<TextMeshProUGUI>();

        foreach(var item in inv.items)
        {
            if (item.ID != -1)
            {
                UItext = UItext + "\n" + inv.IdNames[item.ID] + ": " + item.count;
            }

        }

        tmpUI.text = UItext;
       // tmpUI.text = inv.IdNames[inv.items[0].ID] + ": " + inv.items[0].count + "\n" + inv.IdNames[inv.items[1].ID] + ": " + inv.items[1].count;
    }
    GameObject GenerateCell(Vector2 position)
    {
        GameObject cell = new GameObject();
        cell.transform.position = position;
        cell.transform.localScale = Vector3.one;
        cell.transform.parent = gameObject.transform;
        cell.name = oreMap[position].name;
        int ID = oreMap[position].ID;
        SpriteRenderer SR = cell.AddComponent<SpriteRenderer>();
        cell.AddComponent<ObjectPlacement>();
        BoxCollider2D boxCol = cell.AddComponent<BoxCollider2D>();
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
