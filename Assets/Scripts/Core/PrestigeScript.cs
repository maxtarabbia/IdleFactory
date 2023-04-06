using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using JetBrains.Annotations;

public class PrestigeScript : MonoBehaviour
{
    public TextMeshPro CostText;
    public long PrestigeCost;
    WorldGeneration world;
    MenuButtons button;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        CostText.text = "$" + IntLib.IntToString(PrestigeCost);
        button =GetComponent<MenuButtons>();
        button.isGrayedOut = world.Currency < PrestigeCost;
        button.updateGraying();
    }
    public void Prestige()
    {
        if(world.Currency < PrestigeCost)
        {
            return;
        }

        world.Currency -= PrestigeCost;

        world.oreMap = new Dictionary<Vector2, Cell>();
        world.OccupiedCells = new Dictionary<Vector2, GameObject>();
        world.prestigeState++;
        List<GameObject> Buildings = GameObject.Find("Buildings").GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToList();
        List<GameObject> Ores = GameObject.Find("Ores").GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToList();

        world.Initialize(50);

        Buildings.RemoveRange(0, 1);

        Ores.RemoveRange(0, 1);

        for (int i = 0; i < Buildings.Count; i++)
        {
            Destroy(Buildings[i]);
        }
        for (int i = 0; i < Ores.Count; i++)
        {
            Destroy(Ores[i]);
        }
        GameObject.Find("Background").GetComponent<SpriteRenderer>().material.SetFloat("_Hue", world.prestigeState * 168.854f);

        Building[] buildings = world.gameObject.GetComponent<Buildings>().AllBuildings;
        foreach (Building building in buildings)
        {
            building.count = 0;
        }
        world.inv.AddItem(0,20);
        world.speedstates = world.defaultSpeeds;
        SpeedWarpButton[] SWBs = FindObjectsOfType<SpeedWarpButton>();
        foreach(SpeedWarpButton swb in SWBs) 
        {
            swb.Initialize();
        }

        button.isGrayedOut = world.Currency < PrestigeCost;
        button.updateGraying();
    }
    private void Update()
    {
        if(button.isGrayedOut && world.Currency >= PrestigeCost)
        {
            button.isGrayedOut= false;
            button.updateGraying();
        }
    }
}
