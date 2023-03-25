using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    public WorldGeneration world;
    public Buildings buildings;
    public Camera_Movement cammove;
    public bool isHovered;
    GameObject SpriteGhost;
    int LastBuildableIndex;
    public bool isTouch;

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
    // Update is called once per fram
    void testToPlace()
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

        if (world.selectedBuildableIndex == 6)
        {
            Vector3 rotatedpos = Quaternion.Euler(0, 0, buildings.AllBuildings[6].rotation) * new Vector3(-3, 0, 0) + transform.position;
            Vector2 Vec2 = rotatedpos;
            if (world.OccupiedCells.ContainsKey((Vector2)(Vector2Int.RoundToInt(Vec2))))
            {
                isClear = false;
            }
        }

        for (int i = 0; i < buildings.AllBuildings[world.selectedBuildableIndex].size; i++)
        {
            for (int j = 0; j < buildings.AllBuildings[world.selectedBuildableIndex].size; j++)
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
    private void OnMouseEnter()
    {
        isHovered = true;
        if (world == null)
        {
            world = FindObjectOfType<WorldGeneration>();
            buildings = FindObjectOfType<Buildings>();
        }
        if (Input.GetMouseButton(0) && !isTouch)
        {
            Vector3 Transposition = gameObject.transform.position;

            if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)
                Transposition += new Vector3(0.5f, 0.5f, 0f);


            bool isClear = true;
            Vector2 coord;

            if (world.selectedBuildableIndex == 6)
            {
                Vector3 rotatedpos = Quaternion.Euler(0, 0, buildings.AllBuildings[6].rotation) * new Vector3(-3, 0, 0) + transform.position;
                Vector2 Vec2 = rotatedpos;
                if (world.OccupiedCells.ContainsKey((Vector2)(Vector2Int.RoundToInt(Vec2))))
                {
                    isClear = false;
                }
            }

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
            if(!isTouch) 
            InitializeSpriteGhost();
        }
    }
    Color GetColor()
    {

        bool isClear = true;
        Vector2 coord;

        if (world.selectedBuildableIndex == 6)
        {
            Vector3 rotatedpos = Quaternion.Euler(0, 0,buildings.AllBuildings[6].rotation) * new Vector3(-3, 0, 0) + transform.position;
            Vector2 Vec2 = rotatedpos;
            if (world.OccupiedCells.ContainsKey((Vector2)(Vector2Int.RoundToInt(Vec2))))
            {
                isClear = false;
            }
        }

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
        //default color is green
        Color col = new Color(0.5f, 1, 0.8f);
        if (!isClear)
        {
            //set color to red
            col = new Color(1f, 0.5f, 0.5f);
        }
        else if (!world.inv.CheckRemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost, buildings.AllBuildings[world.selectedBuildableIndex].count + 1))
        {
            //set color to yellow
            col = new Color(1f, 1f, 0.5f);
        }
        return col;
    }
    public void InitializeSpriteGhost()
    {


        Vector3 Transposition = gameObject.transform.position;

        if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0)
            Transposition += new Vector3(0.5f, 0.5f, 0f);
        SpriteGhost = new GameObject();
        SpriteGhost.name = "SpriteGhost";
        SpriteGhost.AddComponent<SpriteRenderer>();
        SpriteGhost.GetComponent<SpriteRenderer>().sprite = buildings.AllBuildings[world.selectedBuildableIndex].prefab.GetComponent<SpriteRenderer>().sprite;
        SpriteGhost.GetComponent<SpriteRenderer>().material = buildings.AllBuildings[world.selectedBuildableIndex].prefab.GetComponent<SpriteRenderer>().sharedMaterial;
        SpriteGhost.GetComponent<SpriteRenderer>().material.SetFloat("_IsGhost", 0.8f);
        SpriteGhost.GetComponent<SpriteRenderer>().material.SetColor("_Color", GetColor());
        SpriteGhost.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        SpriteGhost.GetComponent<SpriteRenderer>().sortingOrder = 1;
        SpriteGhost.transform.position = Transposition + new Vector3(0, 0, -1);
        SpriteGhost.transform.parent = gameObject.transform;
        SpriteGhost.isStatic = true;
        SpriteGhost.transform.Rotate(0, 0, buildings.AllBuildings[world.selectedBuildableIndex].rotation);
        LastBuildableIndex = world.selectedBuildableIndex;
    }
    private void OnMouseUp()
    {
        if (isTouch && Input.touches.Length > 0)
        {
            if (cammove.distanceMoved < 0.5f && cammove.timeMoving < 0.3f && Input.touches[0].phase == TouchPhase.Ended)
            {
                testToPlace();
            }
            else
            {
                
            }

        }
    }
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !isTouch && !FindObjectOfType<Controls>().areSelectedBuildings)
        {
            testToPlace();
        }
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
            if(Time.frameCount % 5 == 0 && SpriteGhost != null)
            {
                SpriteGhost.GetComponent<SpriteRenderer>().material.SetColor("_Color", GetColor());
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

        if (world.selectedBuildableIndex == 6)
        {
            Vector3 rotatedpos = Quaternion.Euler(0, 0, buildings.AllBuildings[6].rotation) * new Vector3(-3, 0, 0) + transform.position;
            world.OccupiedCells.Add((Vector2)Vector2Int.RoundToInt((Vector2)rotatedpos), instancedObj);
        }
    }
}