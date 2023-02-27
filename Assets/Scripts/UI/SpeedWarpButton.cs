using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedWarpButton : MonoBehaviour
{

    public TextMeshPro CostText;
    int CurrentCost = 1;
    WorldGeneration world;
    public BuildingType selectedbuild;
    public enum BuildingType
    {
        Miner,
        Belt,
        Refinery
    }
    private void Start()
    {
        world = FindObjectOfType<WorldGeneration>();


        if (selectedbuild == BuildingType.Miner)
        {
            CurrentCost = world.speedstates.MinerInfo.cost;
            GetComponentInChildren<TextMeshPro>().text = "Miner Speed +" + Mathf.RoundToInt((world.speedstates.MinerInfo.speedScale - 1.0f)*100) + "%";
        }
        if (selectedbuild == BuildingType.Belt)
        {
            CurrentCost = world.speedstates.BeltInfo.cost;
            GetComponentInChildren<TextMeshPro>().text = "Belt Speed +" + Mathf.RoundToInt((world.speedstates.BeltInfo.speedScale - 1.0f)*100) + "%";
        }
        if (selectedbuild == BuildingType.Refinery)
        {
            CurrentCost = world.speedstates.RefineryInfo.cost;
            GetComponentInChildren<TextMeshPro>().text = "Smelt Speed +" + Mathf.RoundToInt((world.speedstates.RefineryInfo.speedScale - 1.0f)*100) + "%";
        }




        CostText.text = "$" + IntLib.IntToString(CurrentCost);
    }
    public void SpeedupMiner()
    {
        Miner[] miners  = FindObjectsOfType<Miner>();
        speedinfo refinfo = world.speedstates.MinerInfo;
        
        if(world.Currency < CurrentCost)
        {
            return;
        }
        refinfo.speed = 1 / (1 / refinfo.speed + (refinfo.speedScale - 1));
        foreach (Miner miner in miners)
        {
            miner.secondsPerItem = refinfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(refinfo.costScale * CurrentCost));
        refinfo.cost = CurrentCost;
        CostText.text = "$" +IntLib.IntToString(CurrentCost);
        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.MinerInfo = refinfo;
        print("Miner speed set to:" + 1 / refinfo.speed);
    }
    public void SpeedupRefinery()
    {
        Refinery[] refineries = FindObjectsOfType<Refinery>();
        speedinfo refinfo = world.speedstates.RefineryInfo;

        if (world.Currency < CurrentCost)
        {
            return;
        }
        refinfo.speed = 1/(1 / refinfo.speed + (refinfo.speedScale - 1));
        foreach (Refinery refinery in refineries)
        {
            refinery.RTime = refinfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(refinfo.costScale * CurrentCost));
        refinfo.cost = CurrentCost;
        CostText.text = "$" + IntLib.IntToString(CurrentCost);
        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.RefineryInfo = refinfo;
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
        refinfo.speed = 1 / (1 / refinfo.speed + (refinfo.speedScale - 1));
        foreach (Belt belt in belts)
        {
            belt.timeTotravel = refinfo.speed;
        }
        Splitter[] splitters = FindObjectsOfType<Splitter>();
        foreach (Splitter splitter in splitters)
        {
            splitter.timeTotravel = refinfo.speed;
        }

        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(refinfo.costScale * CurrentCost));
        CostText.text = "$" + IntLib.IntToString(CurrentCost);
        refinfo.cost = CurrentCost;
        print("Belt speed set to:" + 1 / refinfo.speed);

        if (belts.Length != 0)
        FindObjectOfType<StateSaveLoad>().Save();
        world.speedstates.BeltInfo= refinfo;

    }
}
