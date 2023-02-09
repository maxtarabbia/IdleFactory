using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    int Keystroke;
    WorldGeneration world;
    Image UIsprite;
    Buildings buildings;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        UIsprite = FindObjectOfType<Image>();
        Keystroke = 0;
        buildings = FindObjectOfType<Buildings>();
    }

    // Update is called once per frame
    void Update()
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
        if (Input.GetKey(KeyCode.Alpha4))
        {
            Keystroke = 3;
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            Keystroke = 4;
        }
        if (Input.GetKey(KeyCode.F))
        {
            TickEvents events = GetComponent<TickEvents>();
            events.TickJam(10);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<StateSaveLoad>().Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<StateSaveLoad>().Load();
        }
        world.selectedBuildableIndex = Keystroke;
        UIsprite.sprite = buildings.AllBuildings[Keystroke].prefab.GetComponent<SpriteRenderer>().sprite;

    }
}
