using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    WorldGeneration world;
    Buildings buildings;

    public bool isHovered;
    GameObject SpriteGhost;
    int LastBuildableIndex;
    void Start()
    {

    }
    private void OnMouseExit()
    {
        isHovered = false;
        if (SpriteGhost != null)
        {
            DestroyImmediate(SpriteGhost);
        }
    }
    // Update is called once per frame
    void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (world == null)
            {
                world = FindObjectOfType<WorldGeneration>();
                buildings = FindObjectOfType<Buildings>();
            }
            Vector3 Transposition = gameObject.transform.position;

            if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)
                Transposition += new Vector3(0.5f, 0.5f, 0f);
            

            bool isClear = true;
            Vector2 coord;
            
            for (int i = 0;i < buildings.AllBuildings[world.selectedBuildableIndex].size; i++)
            {
                for(int j = 0; j < buildings.AllBuildings[world.selectedBuildableIndex].size; j++)
                {
                    coord = new Vector2(transform.position.x + i, transform.position.y + j);
                    //if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)  coord += new Vector2(0.5f, 0.5f);
                    if (world.OccupiedCells.ContainsKey(coord))
                    {
                        isClear = false; 
                        print("Cell is Occupied by " + world.OccupiedCells[coord].gameObject.name);
                        break;
                    }
                    if (world.oreMap[coord].ID == 3)
                    {
                        isClear = false;
                        print("Cell is occupied by the edge of the world");
                        break;
                    }
                }
            }

            if (isClear && world.inv.RemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost, buildings.AllBuildings[world.selectedBuildableIndex].count + 1))
                placeObject();
        }
    }
    private void OnMouseEnter()
    {
        isHovered= true;
        if (world == null)
        {
            world = FindObjectOfType<WorldGeneration>();
            buildings = FindObjectOfType<Buildings>();
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 Transposition = gameObject.transform.position;

            if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)
                Transposition += new Vector3(0.5f, 0.5f, 0f);


            bool isClear = true;
            Vector2 coord;

            for (int i = 0; i < buildings.AllBuildings[world.selectedBuildableIndex].size; i++)
            {
                for (int j = 0; j < buildings.AllBuildings[world.selectedBuildableIndex].size; j++)
                {
                    coord = new Vector2(transform.position.x + i, transform.position.y + j);
                    //if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)  coord += new Vector2(0.5f, 0.5f);
                    if (world.OccupiedCells.ContainsKey(coord))
                    {
                        isClear = false; break;
                    }
                }
            }
            if (isClear && world.inv.RemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost, buildings.AllBuildings[world.selectedBuildableIndex].count + 1))
                placeObject();
        }
        else
        {
            InitializeSpriteGhost();
        }
    }
    public void InitializeSpriteGhost()
    {

        bool isClear = true;
        Vector2 coord;

        for (int i = 0; i < buildings.AllBuildings[world.selectedBuildableIndex].size; i++)
        {
            for (int j = 0; j < buildings.AllBuildings[world.selectedBuildableIndex].size; j++)
            {
                coord = new Vector2(transform.position.x + i, transform.position.y + j);
                //if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)  coord += new Vector2(0.5f, 0.5f);
                if (world.OccupiedCells.ContainsKey(coord))
                {
                    isClear = false;
                    break;
                }
                if (world.oreMap.ContainsKey(coord) && world.oreMap[coord].ID == 3)
                {
                    isClear = false;
                    break;
                }
            }
        }
        Color col = new Color(0.5f, 1, 0.8f);
        if (!isClear)
        {
            col = new Color(1f, 0.5f, 0.5f);
        }
        else if(!world.inv.CheckRemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost, buildings.AllBuildings[world.selectedBuildableIndex].count + 1))
        {
            col = new Color(1f, 1f, 0.5f);
        }
        Vector3 Transposition = gameObject.transform.position;

        if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)
            Transposition += new Vector3(0.5f, 0.5f, 0f);
        SpriteGhost = new GameObject();
        SpriteGhost.name = "SpriteGhost";
        SpriteGhost.AddComponent<SpriteRenderer>();
        SpriteGhost.GetComponent<SpriteRenderer>().sprite = buildings.AllBuildings[world.selectedBuildableIndex].prefab.GetComponent<SpriteRenderer>().sprite;
        SpriteGhost.GetComponent<SpriteRenderer>().material = buildings.AllBuildings[world.selectedBuildableIndex].prefab.GetComponent<SpriteRenderer>().sharedMaterial;
        SpriteGhost.GetComponent<SpriteRenderer>().material.SetFloat("_IsGhost", 0.8f);
        SpriteGhost.GetComponent<SpriteRenderer>().material.SetColor("_Color", col);
        SpriteGhost.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        SpriteGhost.GetComponent<SpriteRenderer>().sortingOrder = 1;
        SpriteGhost.transform.position = Transposition + new Vector3(0, 0, -1);
        SpriteGhost.transform.parent = gameObject.transform;
        SpriteGhost.isStatic = true;
        SpriteGhost.transform.Rotate(0, 0, buildings.AllBuildings[world.selectedBuildableIndex].rotation);
        LastBuildableIndex = world.selectedBuildableIndex;
    }
    private void Update()
    {
        if (world != null)
        {
            if (world.selectedBuildableIndex != LastBuildableIndex)
            {
                ResetSprite();
            }
            if(Input.GetKeyDown(KeyCode.R) && SpriteGhost != null)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SpriteGhost.transform.Rotate(new Vector3(0f, 0f, 90f));
                }
                else
                {
                    SpriteGhost.transform.Rotate(new Vector3(0f, 0f, -90f));
                }

                buildings.AllBuildings[world.selectedBuildableIndex].rotation = (int)SpriteGhost.transform.rotation.eulerAngles.z;
                ResetSprite();
            }
            
        }
    }
    public void ResetSprite()
    {
        if (SpriteGhost != null)
        {
            DestroyImmediate(SpriteGhost);
            InitializeSpriteGhost();
        }
    }
    
    void placeObject()
    {
        Vector3 Transposition = gameObject.transform.position;
        if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0) Transposition += new Vector3(0.5f, 0.5f, 0f);

        buildings.AllBuildings[world.selectedBuildableIndex].count++;

        GameObject instancedObj = Instantiate(buildings.AllBuildings[world.selectedBuildableIndex].prefab);
        instancedObj.transform.position = Transposition + new Vector3(0,0,-1f);
        instancedObj.transform.parent = transform.parent.parent.GetChild(0);
        instancedObj.isStatic= true;
        instancedObj.transform.Rotate(0, 0, buildings.AllBuildings[world.selectedBuildableIndex].rotation);
        

        for (int i = 0; i < buildings.AllBuildings[world.selectedBuildableIndex].size; i++)
        {
            for (int j = 0; j < buildings.AllBuildings[world.selectedBuildableIndex].size; j++)
            {
                world.OccupiedCells[new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j)] = instancedObj;
            }
        }
    }
}