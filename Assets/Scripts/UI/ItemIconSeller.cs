using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemIconSeller : MonoBehaviour
{
    Inventory inv;
    public TextMeshPro TMP;
    public int ID;

    public GameObject BackgroundBox;
    public GameObject buttonPrefab;
    GameObject[] buttons;
    int buttonCount = 7;
    SpriteRenderer SR;

    WorldGeneration world;
    void Start()
    {
        BackgroundBox.SetActive(false);
        world = FindObjectOfType<WorldGeneration>();
        SR = GetComponent<SpriteRenderer>();
        SR.sprite = SR.sprite = world.items[ID].sprite;
        inv = world.inv;
        TMP.text = IntLib.IntToString(inv.GetCount(ID));
    }
    private void Update()
    { 
        TMP.text = IntLib.IntToString(inv.GetCount(ID));
    }
    public void CloseMenu()
    {
        SR.sortingOrder = 3;
        transform.position -= new Vector3(0, 0, -0.02f);
        BackgroundBox.SetActive(false);
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons = null;
        return;
    }
    void Openmenu()
    {
        SR.sortingOrder = 4;
        transform.position += new Vector3(0,0,-0.02f);
        buttons = new GameObject[buttonCount];
        BackgroundBox.SetActive(true);
        for (int i = 0; i < buttonCount; i++)
        {
            buttons[i] = Instantiate(buttonPrefab);
            buttons[i].transform.parent = transform;
            buttons[i].transform.localPosition = new Vector3(1.8f + i * 1.25f,-0.37f, -0.02f);
            buttons[i].transform.localScale = Vector3.one * 1.7f;
            buttons[i].GetComponent<ItemSellButton>().ID = ID;
            buttons[i].GetComponent<ItemSellButton>().count = (int)Mathf.Pow(10, i);
        }
    }
    private void OnMouseDown()
    {
        if (buttons == null)
        {
            Openmenu();
        }
        else
        {
            CloseMenu();
        }

    }
}
