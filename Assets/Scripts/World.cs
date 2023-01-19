using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class World : MonoBehaviour
{
    public Dictionary<Vector2, Cell> map = new Dictionary<Vector2, Cell>();
    int Spawnsize = 100;

    public int Seed = 42;

    public float scale = 1f;
    public Sprite Blank;
    public Sprite Iron_Ore;
    public Sprite Copper_Ore;
    // Start is called before the first frame update
    void Start()
    {
        Initialize(Spawnsize);
    }
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
        map.Add(position, newcell);
        GenerateCell(position);


    }
    // Update is called once per frame
    void Update()
    {

    }
    GameObject GenerateCell(Vector2 position)
    {
        GameObject cell = new GameObject();
        cell.transform.position = position * scale;
        cell.transform.localScale = new Vector3(scale,scale,scale);
        cell.transform.parent = gameObject.transform;
        cell.name = map[position].name;
        int ID = map[position].ID;
        SpriteRenderer SR = cell.AddComponent<SpriteRenderer>();
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

public class Cell
{
    public int ID;
    public string name;
}
