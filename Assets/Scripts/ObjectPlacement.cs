using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    WorldGeneration world;
    Buildings buildings;

    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        buildings= FindObjectOfType<Buildings>();
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {

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
                }
            }

            if (isClear && world.inv.RemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost, buildings.AllBuildings[world.selectedBuildableIndex].count + 1))
                placeObject();
        }
    }
    private void OnMouseEnter()
    {
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

            if (isClear && world.inv.RemoveItem(buildings.AllBuildings[world.selectedBuildableIndex].cost,1.0f))
                placeObject();
        }
    }
    void placeObject()
    {

        Vector3 Transposition = gameObject.transform.position;
        if (buildings.AllBuildings[world.selectedBuildableIndex].size % 2 == 0) Transposition += new Vector3(0.5f, 0.5f, 0f);

        buildings.AllBuildings[world.selectedBuildableIndex].count++;

        GameObject instancedObj = Instantiate(buildings.AllBuildings[world.selectedBuildableIndex].prefab);
        instancedObj.transform.position = Transposition + new Vector3(0,0,-1);
        instancedObj.transform.parent = transform.parent;
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
