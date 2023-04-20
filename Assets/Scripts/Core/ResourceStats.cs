using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ResourceStats : MonoBehaviour
{
    WorldGeneration world;
    TickEvents Ticks;
    public List<Vector2Int> createdItemsThisTick = new List<Vector2Int>();
    public List<Vector2Int> createdItemsTotal = new List<Vector2Int>();

    public Achievement[] achievements;
    public TextMeshProUGUI TMPGUI;

    private void Start()
    {
        world = GetComponent<WorldGeneration>();
        Ticks = world.GetComponent<TickEvents>();
        Ticks.MyEvent += OnTick;
        foreach(Achievement achievement in achievements)
        {
            if(achievement.hasBeenAchieved && achievement.UnlockSkinID != -1)
                world.GetComponent<Skins>().allSkins[achievement.UnlockSkinID].isUnlocked= true;
        }
    }
    void OnTick()
    {
        bool foundspot;
        for(int i = 0; i< createdItemsThisTick.Count; i++) 
        {
            foundspot= false;
            for (int j = 0; j < createdItemsTotal.Count; j++)
            {
                if (createdItemsThisTick[i].x == createdItemsTotal[j].x)
                {
                    createdItemsTotal[j] += new Vector2Int(0, createdItemsThisTick[i].y);
                    foundspot= true;
                    break;
                }
            }
            if (!foundspot)
            {
                createdItemsTotal.Add(createdItemsThisTick[i]);
            }
        }
        createdItemsThisTick.Clear();
        CheckForAchievements();
    }
    private void Update()
    {
        TMPGUI.color -= new Color(0, 0, 0, Time.deltaTime/2);
    }
    void CheckForAchievements()
    {
        foreach(Achievement achievement in achievements)
        {
            for(int i = 0; i < createdItemsTotal.Count;i++)
            {
                if (createdItemsTotal[i].x == achievement.ItemID && createdItemsTotal[i].y >= achievement.ItemCount && !achievement.hasBeenAchieved)
                {
                    achievement.hasBeenAchieved = true;
                    if (achievement.UnlockSkinID != -1)
                    {
                        world.GetComponent<Skins>().allSkins[achievement.UnlockSkinID].isUnlocked = true;
                        TMPGUI.text = achievement.name + " has been achieved!\n" + world.GetComponent<Skins>().allSkins[achievement.UnlockSkinID].name + " has been unlocked!";
                    }
                    else
                    {
                        TMPGUI.text = achievement.name + " has been achieved!";
                    }
                    TMPGUI.color = new Color(1f, 1f, 1f, 1f);
                    print(achievement.name + " has been achieved!");
                }
            }
        }
    }
    public void Additem(Vector2Int input)
    {
        for(int i = 0; i<createdItemsThisTick.Count; i++)
        {
            if (input.x == createdItemsThisTick[i].x)
            {
                createdItemsThisTick[i] += new Vector2Int(0, input.y);
                return;
            }

        }
        createdItemsThisTick.Add(input);
    }
    public void Additem(int x, int y)
    {
        for (int i = 0; i < createdItemsThisTick.Count; i++)
        {
            if (x == createdItemsThisTick[i].x)
            {
                createdItemsThisTick[i] += new Vector2Int(0, y);
                return;
            }

        }
        createdItemsThisTick.Add(new Vector2Int(x,y));
    }
}
[Serializable]
public class Achievement
{
    public string name;
    public int ItemID;
    public int ItemCount;
    public bool hasBeenAchieved;
    public int UnlockSkinID = -1;
}

