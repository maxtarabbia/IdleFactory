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
       bool abletoSell = world.inv.RemoveItem(new int2[] {new int2(ID,count)},1f);
        if (abletoSell)
        {
            world.Currency += (count * world.items[ID].value);
            world.GetComponent<StateSaveLoad>().Save();
        }
    }
}
