﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    Dictionary<Vector2, Cell> map = new Dictionary<Vector2, Cell>();
    int Spawnsize = 10;
    float scale = 3f;
    public Sprite ground;
    public Sprite Smeltery;
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
        Cell newcell = new Cell();
        newcell.direction = 0;
        if (Random.value > 0.3)
        {
            newcell.ID = 0;
            newcell.name = "Empty";
        }
        else
        {
            newcell.ID = 1;
            newcell.name = "Smeltery";
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

        switch(ID)
        {
            case 0:
                SR.sprite = ground; 
                break;
            case 1:
                SR.sprite = Smeltery;
                break;
        }

        return cell;
    }
    public void RefreshCells()
    { 
        
    }
}

public class Cell
{
    public int direction;
    public int ID;
    public string name;
}
