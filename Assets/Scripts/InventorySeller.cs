using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class InventorySeller : MonoBehaviour
{
    Inventory inv;
    GameObject[] itemSprites;
    public GameObject SellerButtonPrefab;
    public TMP_FontAsset fontAsset;

    int itemsPerColumn = 3;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = FindObjectOfType<Camera>();
        gameObject.transform.position = new Vector3(cam.gameObject.transform.position.x, cam.gameObject.transform.position.y, transform.position.z);
        gameObject.transform.localScale *= cam.orthographicSize;
        inv = FindObjectOfType<WorldGeneration>().inv;
        itemSprites= new GameObject[inv.items.Length];
        int curColumn = 0;
        for(int i = 0; i < inv.items.Length; i++)
        {
            if ((i) % itemsPerColumn == 0)
                curColumn++;
            if (inv.items[i].ID == -1)
                continue;
            itemSprites[i] = Instantiate(SellerButtonPrefab, transform);
            itemSprites[i].transform.localPosition = new Vector3(-6f + curColumn*2f, -1.7f + (i - curColumn*itemsPerColumn) * -1.5f, -0.5f) * 0.08f;
            itemSprites[i].transform.localScale = Vector3.one * 0.065f;
            itemSprites[i].GetComponent<ItemIconSeller>().ID = inv.items[i].ID;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i =0; i<itemSprites.Length;i++)
        {
            if (itemSprites[i] != null)
                itemSprites[i].GetComponentInChildren<TextMeshPro>().text = IntLib.IntToString(inv.items[i].count);
        }
    }
}
