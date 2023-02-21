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
        if(miners.Length == 0 )
            return;
        
        if(world.Currency < CurrentCost)
        {
            return;
        }
        world.speedstates.MinerInfo.speed = world.speedstates.MinerInfo.speed / world.speedstates.MinerInfo.speedScale;
        foreach (Miner miner in miners)
        {
            miner.secondsPerItem = world.speedstates.MinerInfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(world.speedstates.MinerInfo.costScale * CurrentCost));
        world.speedstates.MinerInfo.cost = CurrentCost;
        CostText.text = "$" +IntLib.IntToString(CurrentCost);
        FindObjectOfType<StateSaveLoad>().Save();
    }
    public void SpeedupRefinery()
    {
        Refinery[] refineries = FindObjectsOfType<Refinery>();
        if (refineries.Length == 0)
            return;

        if (world.Currency < CurrentCost)
        {
            return;
        }
        world.speedstates.RefineryInfo.speed = world.speedstates.RefineryInfo.speed / world.speedstates.RefineryInfo.speedScale;
        foreach (Refinery refinery in refineries)
        {
            refinery.RTime = world.speedstates.RefineryInfo.speed;
        }
        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(world.speedstates.RefineryInfo.costScale * CurrentCost));
        world.speedstates.RefineryInfo.cost = CurrentCost;
        CostText.text = "$" + IntLib.IntToString(CurrentCost);
        FindObjectOfType<StateSaveLoad>().Save();
    }
    public void SpeedupBelt()
    {


        if (world.Currency < CurrentCost)
        {
            return;
        }
        
        Belt[] belts = FindObjectsOfType<Belt>();
        world.speedstates.BeltInfo.speed = world.speedstates.BeltInfo.speed / world.speedstates.BeltInfo.speedScale;
        foreach (Belt belt in belts)
        {
            belt.timeTotravel = world.speedstates.BeltInfo.speed;
        }
        Splitter[] splitters = FindObjectsOfType<Splitter>();
        foreach (Splitter splitter in splitters)
        {
            splitter.timeTotravel = world.speedstates.BeltInfo.speed;
        }

        world.Currency -= CurrentCost;
        CurrentCost = (Mathf.RoundToInt(world.speedstates.BeltInfo.costScale * CurrentCost));
        CostText.text = "$" + IntLib.IntToString(CurrentCost);
        world.speedstates.BeltInfo.cost = CurrentCost;

        if (belts.Length != 0)
        FindObjectOfType<StateSaveLoad>().Save();
    }
}
