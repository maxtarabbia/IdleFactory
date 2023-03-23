using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hotbar : MonoBehaviour
{

    public GameObject hotbarIconPrefab;

    Buildings buildings;
    WorldGeneration world;
    public TextMeshPro TMP;

    float timeSincePopUp = 10;

    HotbarElement[] HE;

    int lastID = 1;

    float spreadDist = 1.3f;
    float iconOffset;

    GameObject[] hotbarIcons;
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();

        buildings = FindObjectOfType<Buildings>();
        hotbarIcons = new GameObject[buildings.AllBuildings.Length];
        iconOffset = spreadDist * hotbarIcons.Length / 2;
        HE = new HotbarElement[buildings.AllBuildings.Length];
        for(int i = 0; i< hotbarIcons.Length; i++)
        {
            hotbarIcons[i] = Instantiate(hotbarIconPrefab, transform);
            HE[i] = hotbarIcons[i].GetComponent<HotbarElement>();
            if (buildings.AllBuildings[i].size == 1)
                HE[i].sprite.transform.localScale = Vector3.one;
            HE[i].sprite.GetComponent<SpriteRenderer>().sprite = buildings.AllBuildings[i].prefab.GetComponent<SpriteRenderer>().sprite;
            hotbarIcons[i].transform.localPosition = new Vector3(-iconOffset + (i+0.5f) * spreadDist, 0, 0);
        }
    }
    public void HighlightItem(int ID)
    {
        HE[lastID].Highlight(false);
        HE[ID].Highlight(true);
        lastID = ID;
        ChechForError();
    }
    void ChechForError()
    {
        if(!world.inv.CheckRemoveItem(buildings.AllBuildings[lastID].cost, buildings.AllBuildings[lastID].count + 1))
        {
            TMP.text = "Missing Items!";
            TMP.faceColor = Color.white;
            timeSincePopUp = 0;
        }
    }
    private void Update()
    {
        timeSincePopUp += Time.deltaTime;
        Color32 col = TMP.faceColor;
        if (col.a <= 0)
            return;
        if (timeSincePopUp < 0.5f)
            return;
        
        col.a = (byte)Mathf.Clamp((float)(256-(timeSincePopUp- 0.5)*512),0,256);
        TMP.faceColor = col;
    }
}
