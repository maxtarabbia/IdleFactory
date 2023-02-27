using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleBuildable : MonoBehaviour
{
    WorldGeneration world;

    private void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
    }
    // Update is called once per frame
    private void OnMouseDown()
    {
        if(Input.touchCount == 1)
        {
            int curInt = world.selectedBuildableIndex;
            if(curInt + 1 == FindObjectOfType<Buildings>().AllBuildings.Length)
            {
                world.setBuildableIndex(0);
            }
            else
            {
                world.setBuildableIndex(curInt + 1);
            }
        }
    }
}
