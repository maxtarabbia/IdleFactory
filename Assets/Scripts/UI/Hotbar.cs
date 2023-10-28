using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Hotbar : MonoBehaviour
{

    public GameObject hotbarIconPrefab;

    Buildings buildings;
    WorldGeneration world;
    public TextMeshPro MITMP;
    public TextMeshPro BuildingnameTMP;

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
            HE[i].text = "(" + (i+1) + ")";
            HE[i].ElementIndex = i;
            hotbarIcons[i].transform.localPosition = new Vector3(-iconOffset + (i+0.5f) * spreadDist, 0, 0);
        }
    }
    public void HighlightItem(int ID)
    {
        HE[lastID].Highlight(false);
        HE[ID].Highlight(true);
        BuildingnameTMP.text = buildings.AllBuildings[ID].name;
        lastID = ID;

        ChechForError();
    }
    void ChechForError()
    {
        MITMP.text = string.Empty;
        int[] IDs = buildings.AllBuildings[lastID].cost.Select(obj => obj.x).ToArray();
        List<int> missingIDs = new List<int>();
        for(int i = 0; i < IDs.Length;i++)
        {
            if(!world.inv.CheckRemoveItem(new int2[] {new int2(IDs[i],1)},1f))
            {
                missingIDs.Add(IDs[i]);
            }
        }
        for (int i = 0; i < missingIDs.Count; i++)
        {
            MITMP.text += "Missing " + world.items[missingIDs[i]].name +"s!\n";
            MITMP.faceColor = Color.white;
            timeSincePopUp = 0;
        }
    }
    private void Update()
    {
        timeSincePopUp += Time.deltaTime;
        Color32 col = MITMP.faceColor;
        if (col.a <= 0)
            return;
        if (timeSincePopUp < 1f)
            return;
        
        col.a = (byte)Mathf.Clamp((float)(256-(timeSincePopUp - 1)*512),0,256);
        MITMP.faceColor = col;
    }
}
