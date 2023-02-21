using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySeller : MonoBehaviour
{
    Inventory inv;
    int[] stacks;
    GameObject[] itemSprites;
    public Sprite[] spriteAssets;
    public GameObject SellerButtonPrefab;
    ItemSeller[] SellerButtons;
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
            itemSprites[i].AddComponent<SpriteRenderer>();
            itemSprites[i].GetComponent<SpriteRenderer>().sprite = spriteAssets[inv.items[i].ID - 1];
            itemSprites[i].GetComponent<SpriteRenderer>().sortingLayerID = gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            itemSprites[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
            //itemSprites[i].transform.parent = transform;
            itemSprites[i].transform.parent = transform;
            itemSprites[i].transform.localPosition = new Vector3(-4.6f, 2.8f - i * 1.1f, -0.5f) * 0.08f;
            itemSprites[i].transform.localScale = Vector3.one * 0.08f;

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
        
    }
    class ItemSeller
    {
        public int ID;
        public GameObject[] SellButtons;
        public GameObject ButtonPrefab;
        int buttoncount = 7;
        public GameObject parent;

        public void SetButtons()
        {
            SellButtons = new GameObject[buttoncount];
            for(int i =0;i < buttoncount;i++)
            {
                SellButtons[i] = Instantiate(ButtonPrefab);
                SellButtons[i].transform.parent = parent.transform;
                SellButtons[i].transform.localPosition = new Vector3(1.4f + i * 1.2f, 0);
                SellButtons[i].transform.localScale = Vector3.one * 1.7f;
                SellButtons[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
                SellButtons[i].GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                SellButtons[i].GetComponent<ItemSellButton>().ID = ID;
                SellButtons[i].GetComponent<ItemSellButton>().count = (int)Mathf.Pow(10, i);
                
            }
        }
    }
}
