using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
[System.Serializable]
public class Inventory
{

    [SerializeField]
    public ItemStack[] items;
    [SerializeField]
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
    public void PopulateItemIDs()
    {
    }

    public bool AddItem(int ID, int count)
    {
        if(ID == -1)
            return true;
        bool foundspot = false;
        foreach(var item in items)
        {
            if(item.ID == ID && !foundspot && item.count + count <= maxStackSize)
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
                    SortInv();
                }
            }
        }
        return foundspot;
    }
    public void SortInv()
    {
        List<ItemStack> sorteditems = items.OrderBy(i => i.ID).ToList();
        items = sorteditems.OrderBy(i => i.ID == -1).ToArray();
        //items = sorteditems.ToArray();
    }
    public int ForceRemoveItem(int2 ID)
    {
        int itemcount = ID.y;
        foreach (var item in items)
        {
            if (item.ID != ID.x)
                continue;
            if (ID.y > item.count)
            {
                itemcount = item.count;
            }
            item.count -= itemcount;
            checkvoids();
            return itemcount;
        }
        checkvoids();
        return 0;
    }
    public bool RemoveItem(int2[] IDs, float multiplier)
    {
        //check to make sure there are enough of each item;
        bool[] foundItems = new bool[IDs.Length];
        for (int i = 0; i < IDs.Length; i++)
        {
            foreach (var item in items)
            {
                if (item.ID == IDs[i].x && item.count >= IDs[i].y * multiplier)
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
                    if (item.ID == (int)IDs[i].x && item.count >= (int)IDs[i].y * multiplier)
                    {
                        item.count -= (int)(IDs[i].y * multiplier);
                    }
                }
            }
        }
        checkvoids();
        return foundall;
    }
    public int GetCount(int ID)
    {
       foreach(ItemStack item in items)
        {
            if(item.ID == ID) return item.count;
        }
       return 0;
    }
    public bool CheckRemoveItem(int2[] IDs, float multiplier)
    {
        //check to make sure there are enough of each item;
        bool[] foundItems = new bool[IDs.Length];
        for (int i = 0; i < IDs.Length; i++)
        {
            foreach (var item in items)
            {
                if (item.ID == IDs[i].x && item.count >= IDs[i].y * multiplier)
                {
                    foundItems[i] = true;
                }
            }
        }
        bool foundall = true;
        foreach (bool found in foundItems)
        {
            if (!found)
                foundall = false;
        }
        return foundall;
    }
    void checkvoids()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i].count == 0)
            {
                items[i].ID = -1;
            }
        }
    }
    public void Clear()
    {
        foreach(ItemStack item in items)
        {
            item.ID = -1;
            item.count = 0;
        }
    }
}
[System.Serializable]
public class ItemStack
{
    public int ID = -1;
    public int count = 0;
}
