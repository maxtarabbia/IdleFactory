using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSellButton : MonoBehaviour
{
    WorldGeneration world;
    public int ID;
    public int count;
    private void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        gameObject.GetComponentInChildren<Canvas>().overrideSorting = true;
    }
    public void Sell()
    {
       bool abletoSell = world.inv.RemoveItem(new Vector2[] {new Vector2(ID + 1,count)},1f);
        if (abletoSell)
        {
            print("sold: " + count + " of " + ID);
            world.Currency += (count * world.CurrencyRates[ID]);
            world.GetComponent<StateSaveLoad>().Save();
        }
    }
}
