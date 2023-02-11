using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SaveData : MonoBehaviour
{
    public int time;

    public Vector2 CamCoord;
    public float CamScale;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;

    public Inventory worldinv;

    public string SaveIntoJson()
    {

        DatatoSave datatoSave = new DatatoSave();

        datatoSave.time = time;

        datatoSave.minerdata= minerdata;
        datatoSave.beltdata= beltdata;
        datatoSave.splitterdata= splitterdata;
        datatoSave.coredata= coredata;
        datatoSave.refinerydata= refinerydata;

        datatoSave.CamCoord = CamCoord;
        datatoSave.CamScale= CamScale;
        datatoSave.worldinv= worldinv;

        

        string savedString = string.Empty;


        savedString += JsonUtility.ToJson(datatoSave,true);

        print(savedString);

        return savedString;
    }
    public void LoadFromJson(string json)
    {
        DatatoSave savedData = (DatatoSave)JsonUtility.FromJson(json, typeof(DatatoSave));
        minerdata = savedData.minerdata;
        beltdata= savedData.beltdata;
        splitterdata= savedData.splitterdata;
        refinerydata= savedData.refinerydata;
        coredata= savedData.coredata;

        time = savedData.time;

        CamScale= savedData.CamScale;
        CamCoord= savedData.CamCoord;
        worldinv = savedData.worldinv;
        worldinv.PopulateItemIDs();
    }
}
public struct DatatoSave
{
    public int time;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;

    public Vector2 CamCoord;
    public float CamScale;
    [SerializeField]
    public Inventory worldinv;
}
[System.Serializable]
public class MinerData
{
    public Vector2 Position;
    public int Rotation;
    public int Progress;
}
[System.Serializable]
public class BeltData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public int itemID;
}
[System.Serializable]
public class RefineryData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public Inventory inv1;
    public Inventory inv2;
}
[System.Serializable]
public class SplitterData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public int itemID;
}
[System.Serializable]
public class CoreData
{
    public Vector2 Position;
}
