using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class SpeedWarpButton : MonoBehaviour
{

    public TextMeshPro LabelText;
    public TextMeshPro ButtonText;
    long CurrentCost = 1;
    WorldGeneration world;
    public BuildingType selectedbuild;
    int Level=0;

    MenuButtons button;
    public enum BuildingType
    {
        Miner,
        Belt,
        Refinery
    }
    private void Start()
    {
        button = GetComponent<MenuButtons>();
        
        world = FindObjectOfType<WorldGeneration>();

        UpdatePrices();

    }
    public void UpdatePrices()
    {
        if (selectedbuild == BuildingType.Miner)
        {
            float startspeed = world.defaultSpeeds.MinerInfo.speed;
            float curspeed = world.speedstates.MinerInfo.speed;

            Level =1 + Mathf.RoundToInt((curspeed-startspeed) / world.defaultSpeeds.MinerInfo.speedScale);

            CurrentCost = world.speedstates.MinerInfo.cost;
            LabelText.text = "Miner Speed\nLevel " + Level + "\n(" + Mathf.RoundToInt(100 + (curspeed - startspeed)/startspeed * 100) + "%)";
            ButtonText.text = "$" + IntLib.IntToString(CurrentCost) + "\n(+" + Mathf.RoundToInt(world.speedstates.MinerInfo.speedScale/startspeed * 100) + "%)";
        }
        if (selectedbuild == BuildingType.Belt)
        {
            float startspeed = world.defaultSpeeds.BeltInfo.speed;
            float curspeed = world.speedstates.BeltInfo.speed;

            Level = 1 + Mathf.RoundToInt((curspeed - startspeed) / world.defaultSpeeds.BeltInfo.speedScale);

            CurrentCost = world.speedstates.BeltInfo.cost;
            LabelText.text = "Belt Speed\nLevel " + Level + "\n(" + Mathf.RoundToInt(100 + (curspeed - startspeed)/startspeed * 100) + "%)";
            ButtonText.text = "$" + IntLib.IntToString(CurrentCost) + "\n(+" + Mathf.RoundToInt(world.speedstates.BeltInfo.speedScale/startspeed * 100) + "%)";
        }
        if (selectedbuild == BuildingType.Refinery)
        {
            float startspeed = world.defaultSpeeds.RefineryInfo.speed;
            float curspeed = world.speedstates.RefineryInfo.speed;

            Level = 1 + Mathf.RoundToInt((curspeed - startspeed) / world.defaultSpeeds.RefineryInfo.speedScale);

            CurrentCost = world.speedstates.RefineryInfo.cost;
            LabelText.text = "Refinery Speed\nLevel " + Level + "\n(" + Mathf.RoundToInt(100 + (curspeed - startspeed)/startspeed * 100) + "%)";
            ButtonText.text = "$" + IntLib.IntToString(CurrentCost) + "\n(+" + Mathf.RoundToInt(world.speedstates.RefineryInfo.speedScale/startspeed * 100) + "%)";
        }

        UpdateButtonGraying();


        //LabelText.text = "$" + IntLib.IntToString(CurrentCost);
    }
    public void SpeedupMiner()
    {
        Miner[] miners  = FindObjectsOfType<Miner>();
        speedinfo refinfo = world.speedstates.MinerInfo;
        if(world.Currency < CurrentCost)
        {
            return;
        }
        refinfo.speed += refinfo.speedScale;
        foreach (Miner miner in miners)
        {
            miner.Speed = refinfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (long)(math.round(refinfo.costScale * CurrentCost));
        refinfo.cost = CurrentCost;

        

        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.MinerInfo = refinfo;
        UpdatePrices();
        print("Miner speed set to:" + refinfo.speed);
    }
    public void SpeedupRefinery()
    {
        Refinery[] refineries = FindObjectsOfType<Refinery>();
        speedinfo refinfo = world.speedstates.RefineryInfo;

        if (world.Currency < CurrentCost)
        {
            return;
        }
        refinfo.speed += refinfo.speedScale;
        foreach (Refinery refinery in refineries)
        {
            refinery.Speed = refinfo.speed;
        }
        Assembler[] assemblers = FindObjectsOfType<Assembler>();
        foreach(Assembler assembler in assemblers)
        {
            assembler.Speed = refinfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (long)(math.round(refinfo.costScale * CurrentCost));
        refinfo.cost = CurrentCost;



        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.RefineryInfo = refinfo;
        UpdatePrices();
        print("Refinery speed set to:" + 1 / refinfo.speed);
    }
    public void SpeedupBelt()
    {

        speedinfo refinfo = world.speedstates.BeltInfo;
        if (world.Currency < CurrentCost)
        {
            return;
        }
        
        Belt[] belts = FindObjectsOfType<Belt>();
        refinfo.speed += refinfo.speedScale;
        foreach (Belt belt in belts)
        {
            belt.Speed = refinfo.speed;
        }
        Splitter[] splitters = FindObjectsOfType<Splitter>();
        foreach (Splitter splitter in splitters)
        {
            splitter.Speed = refinfo.speed;
        }
        UnderGroundBelt[] UBS = FindObjectsOfType<UnderGroundBelt>();
        foreach (UnderGroundBelt ub in UBS)
        {
            ub.Speed = refinfo.speed;
        }

        world.Currency -= CurrentCost;
        CurrentCost = (long)(math.round(refinfo.costScale * CurrentCost));


        refinfo.cost = CurrentCost;
        print("Belt speed set to:" + 1 / refinfo.speed);

        if (belts.Length != 0)
        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.BeltInfo= refinfo;
        UpdatePrices();

    }
    void UpdateButtonGraying()
    {
        if(world.Currency < CurrentCost)
        {
            button.isGrayedOut = true;
            button.updateGraying();
        }
    }
    private void Update()
    {
        if(world.Currency >= CurrentCost)
        {
            if (button.isGrayedOut)
            {
                button.isGrayedOut = false;
                button.updateGraying();
            }
        }
    }
}
