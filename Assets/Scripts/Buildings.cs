using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    [SerializeField]
    public Building[] AllBuildings;
    void Start()
    {
    
    }
}
[Serializable]
public class Building
{
    public string name;
    public GameObject prefab;
    public int size;
    public Vector2[] cost;
    public int rotation;
    public Building(GameObject prefab, string name, int size, Vector2[] cost, int rotation)
    {
        this.prefab = prefab;
        this.name = name;
        this.size = size;
        this.cost = cost;
        this.rotation = rotation;
    }
}
