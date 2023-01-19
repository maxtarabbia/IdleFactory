using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    public ItemStack[] items;
    public bool isSpace = true;

    public Inventory(int count)
    {
        this.items = new ItemStack[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = new ItemStack();
        }
    }

    public bool AddItem(int ID, int count)
    {
        bool foundspot = false;
        if(!isSpace)
        {
            return false;
        }
        else
        {
            foreach(var item in items)
            {
                if(item.ID == ID && !foundspot)
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
                        refreshSpace();
                    }
                }
            }
            return true;

        }
    }
    void refreshSpace()
    {
        isSpace = false;
        foreach(var item in items)
        {
            if(item.count == 0)
            {
                isSpace= true;
                return;
            }
        }
    }
}
public class ItemStack
{
    public int ID = -1;
    public int count = 0;
}
