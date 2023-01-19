using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {
            placeObject();
        }
    }
    void placeObject()
    {
        WorldGeneration world = FindObjectOfType<WorldGeneration>();

        GameObject instancedObj = Instantiate(world.selectedObject, gameObject.transform);
        instancedObj.transform.parent = null;
    }
}
