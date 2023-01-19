using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    WorldGeneration world;

    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {
            bool isClear = true;
            Vector2 coord;
            for (int i = 0;i < world.buildables[world.selectedBuildableIndex].size; i++)
            {
                for(int j = 0; j < world.buildables[world.selectedBuildableIndex].size; j++)
                {
                    coord = new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j);
                    if (world.OccupiedCells.ContainsKey(coord) && world.OccupiedCells[coord])
                    {
                        isClear = false; break;
                    }
                }
            }

            if (isClear && world.inv.RemoveItem(Mathf.RoundToInt(world.buildables[world.selectedBuildableIndex].costs[0].x),Mathf.RoundToInt(world.buildables[world.selectedBuildableIndex].costs[0].y)))
                placeObject();
        }
    }
    void placeObject()
    {

        GameObject instancedObj = Instantiate(world.buildables[world.selectedBuildableIndex].objectToInstance, gameObject.transform);
        instancedObj.transform.parent = null;

        for (int i = 0; i < world.buildables[world.selectedBuildableIndex].size; i++)
        {
            for (int j = 0; j < world.buildables[world.selectedBuildableIndex].size; j++)
            {
                world.OccupiedCells[new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j)] = true;
            }
        }
    }
}
