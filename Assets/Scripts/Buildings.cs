using System;
using Unity.Mathematics;
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
    public int2[] cost;
    public int rotation;
    public int count;
    public Building(GameObject prefab, string name, int size, int2[] cost, int rotation)
    {
        this.prefab = prefab;
        this.name = name;
        this.size = size;
        this.cost = cost;
        this.rotation = rotation;
        count = 0;
    }
}
