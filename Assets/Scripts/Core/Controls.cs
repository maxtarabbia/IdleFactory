using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    WorldGeneration world;
    public Image UIsprite;
    public TextMeshProUGUI UIText;
    
    public GameObject PauseMenu;
    public GameObject InventoryMenu;

    public bool areSelectedBuildings;
    TickEvents events;
    // Start is called before the first frame update
    void Start()
    {
        events = GetComponent<TickEvents>();
        world = FindObjectOfType<WorldGeneration>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject IM = GameObject.Find("World Inv(Clone)");
        GameObject PM = GameObject.Find("PauseMenu(Clone)");
        if (PM != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject.Find("ResumeButton").GetComponent<ResumeButton>().DestroyMenu();
            }
            return;
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            world.setBuildableIndex(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            world.setBuildableIndex(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            world.setBuildableIndex(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            world.setBuildableIndex(3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            world.setBuildableIndex(4);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            world.setBuildableIndex(5);
        }
        if (Input.GetKey(KeyCode.Alpha7))
        {
            world.setBuildableIndex(6);
        }
        if (Input.GetKey(KeyCode.F))
        {
            if(Input.GetKey(KeyCode.Alpha1))
            {
                events.TickJam(100);
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                events.TickJam(200);
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                events.TickJam(300);
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                events.TickJam(400);
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                events.TickJam(500);
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                events.TickJam(600);
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                events.TickJam(700);
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                events.TickJam(800);
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                events.TickJam(900);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.KeypadPlus))
            {
                world.Currency += 100_000;
            }
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<StateSaveLoad>().Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            //GetComponent<StateSaveLoad>().Load();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (IM == null)
            {
                Instantiate(PauseMenu);
            }
            else
            {
                Destroy(IM);
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(IM == null)
            {
                Instantiate(InventoryMenu);
            }
            else
            {
                Destroy(IM);
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            var objs = FindObjectsOfType<hoveringSprites>();
            areSelectedBuildings = false;
            foreach (var obj in objs)
            { 
                if (obj.isSelected)
                {
                    obj.isSelected = false;
                    obj.Unhover(true);
                }
                
            }
        }

    }
}
