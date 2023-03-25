using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ItemSellButton : MonoBehaviour
{
    WorldGeneration world;
    public int ID;
    public int count;
    private void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = IntLib.IntToString(count);
        gameObject.GetComponentInChildren<Canvas>().overrideSorting = true;
    }
    public void Sell()
    {
        int SellCount = world.inv.ForceRemoveItem(new int2(ID,count));
        world.Currency += (SellCount * world.items[ID].value * world.prestigeState);
        world.GetComponent<StateSaveLoad>().Save();
    }
}
