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
    Buildings buildings;
    public GameObject PauseMenu;
    public GameObject InventoryMenu;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        //UIsprite = GameObject.Find("InventoryDisplay").GetComponent<Image>();
        //UIText = GameObject.Find("SelectedBuildingText").GetComponent<TextMeshPro>();
        buildings = FindObjectOfType<Buildings>();
    }

    // Update is called once per frame
    void Update()
    {
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
            TickEvents events = GetComponent<TickEvents>();
            events.TickJam(10);
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
            Instantiate(PauseMenu);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameObject IM = GameObject.Find("World Inv(Clone)");
            if(IM == null)
            {
                Instantiate(InventoryMenu);
            }
            else
            {
                Destroy(IM);
            }
        }


    }
}
