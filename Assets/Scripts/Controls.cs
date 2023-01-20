using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    int Keystroke; 
    // Start is called before the first frame update
    void Start()
    {
        Keystroke= 0;
    }

    // Update is called once per frame
    void Update()sd
    {
        
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Keystroke= 0;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Keystroke = 1;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            Keystroke = 2;
        }
        FindObjectOfType<WorldGeneration>().selectedBuildableIndex = Keystroke;
        FindObjectOfType<Image>().sprite = FindObjectOfType<Buildings>().AllBuildings[Keystroke].prefab.GetComponent<SpriteRenderer>().sprite;

    }
}
