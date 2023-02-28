using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class InventorySeller : MonoBehaviour
{
    Inventory inv;
    int[] stacks;
    GameObject[] itemSprites;
    public Sprite[] spriteAssets;
    public GameObject SellerButtonPrefab;
    ItemSeller[] SellerButtons;
    public TMP_FontAsset fontAsset;
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = FindObjectOfType<Camera>();
        gameObject.transform.position = new Vector3(cam.gameObject.transform.position.x, cam.gameObject.transform.position.y, transform.position.z);
        gameObject.transform.localScale *= cam.orthographicSize;
        inv = FindObjectOfType<WorldGeneration>().inv;
        stacks = new int[inv.items.Length];
        itemSprites= new GameObject[inv.items.Length];
        SellerButtons = new ItemSeller[inv.items.Length];
        for(int i = 0; i < inv.items.Length; i++)
        {
            if (inv.items[i].ID == -1)
                continue;
            stacks[i] = inv.items[i].count;
            itemSprites[i] = new GameObject();
            itemSprites[i].name = inv.IdNames[inv.items[i].ID] + " icon";
            SpriteRenderer SR = itemSprites[i].AddComponent<SpriteRenderer>();
            SR.sprite = spriteAssets[inv.items[i].ID - 1];
            SR.sortingLayerID = gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            SR.sortingOrder = 3;
            itemSprites[i].transform.parent = transform;
            itemSprites[i].transform.localPosition = new Vector3(-4.8f, 2.8f - i * 1.0f, -0.5f) * 0.08f;
            itemSprites[i].transform.localScale = Vector3.one * 0.08f;

            GameObject textsprite = new GameObject();
            textsprite.name = "itemCount";
            textsprite.transform.parent = itemSprites[i].transform;
            textsprite.transform.localScale = itemSprites[i].transform.localScale;
            textsprite.transform.localPosition = new Vector3(1.5f,0,0);
            TextMeshPro textmesh = textsprite.AddComponent<TextMeshPro>();
            textmesh.text = IntLib.IntToString(inv.items[i].count);
            textmesh.font = fontAsset;
            textmesh.fontSize = 55;
            textmesh.color = Color.black;
            SortingGroup SG = textsprite.AddComponent<SortingGroup>();
            SG.sortingLayerName = "UI";
            SG.sortingOrder = 3;

            SellerButtons[i] = new ItemSeller();
            SellerButtons[i].ButtonPrefab = SellerButtonPrefab;
            SellerButtons[i].parent = itemSprites[i];
            SellerButtons[i].ID = inv.items[i].ID - 1;
            SellerButtons[i].SetButtons();
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
    class ItemSeller
    {
        public int ID;
        public GameObject[] SellButtons;
        public GameObject ButtonPrefab;
        int buttoncount = 6;
        public GameObject parent;

        public void SetButtons()
        {
            SellButtons = new GameObject[buttoncount];
            for(int i =0;i < buttoncount;i++)
            {
                SellButtons[i] = Instantiate(ButtonPrefab);
                SellButtons[i].transform.parent = parent.transform;
                SellButtons[i].transform.localPosition = new Vector3(3f + i * 1.2f, 0);
                SellButtons[i].transform.localScale = Vector3.one * 1.7f;
                SellButtons[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
                SellButtons[i].GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                SellButtons[i].GetComponent<ItemSellButton>().ID = ID;
                SellButtons[i].GetComponent<ItemSellButton>().count = (int)Mathf.Pow(10, i);
                
            }
        }
    }
}
