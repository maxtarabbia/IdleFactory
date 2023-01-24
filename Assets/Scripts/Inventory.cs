using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    public Dictionary<int,string> IdNames = new Dictionary<int,string>();


    public ItemStack[] items;

    public int maxStackSize = int.MaxValue;
    public Inventory(int count)
    {
        this.items = new ItemStack[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = new ItemStack();
        }
        PopulateItemIDs();
    }

    void PopulateItemIDs()
    {
        IdNames.Add(0, "Air");
        IdNames.Add(1, "Iron Ore");
        IdNames.Add(2, "Copper Ore");
        IdNames.Add(3, "Iron Ingot");
        IdNames.Add(4, "Copper Ingot");
    }

    public bool AddItem(int ID, int count)
    {
        bool foundspot = false;
        foreach(var item in items)
        {
            if(item.ID == ID && !foundspot && item.count + count < maxStackSize)
            {
                item.count += count;
                foundspot = true;
            }
        }

        if(!foundspot)
        {
            foreach (var item in items)
            {
                if(item.ID == -1 && !foundspot)
                {
                    item.ID = ID;
                    item.count = count;
                    foundspot = true;
                }
            }
        }
        return foundspot;
    }
    public bool RemoveItem(Vector2[] IDs)
    {
        //check to make sure there are enough of each item;
        bool[] foundItems = new bool[IDs.Length];
        for (int i = 0; i < IDs.Length; i++)
        {
            foreach (var item in items)
            {
                if (item.ID == (int)IDs[i].x && item.count >= (int)IDs[i].y)
                {
                    foundItems[i] = true;
                }
            }
        }
        bool foundall = true;
        foreach(bool found in foundItems)
        {
            if (!found)
                foundall = false;
        }

        //subtract items if found spots for all
        if (foundall)
        {
            for (int i = 0; i < IDs.Length; i++)
            {
                foreach (var item in items)
                {
                    if (item.ID == (int)IDs[i].x && item.count >= (int)IDs[i].y)
                    {
                        item.count -= (int)IDs[i].y;
                    }
                }
            }
        }

        return foundall;
    }
}
public class ItemStack
{
    public int ID = -1;
    public int count = 0;
}
