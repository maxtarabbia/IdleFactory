using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class SaveData : MonoBehaviour
{

    public Vector2 CamCoord;
    public float CamScale;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;

    public string SaveIntoJson()
    {

        DatatoSave datatoSave = new DatatoSave();

        datatoSave.minerdata= minerdata;
        datatoSave.beltdata= beltdata;
        datatoSave.splitterdata= splitterdata;
        datatoSave.coredata= coredata;
        datatoSave.refinerydata= refinerydata;

        datatoSave.CamCoord = CamCoord;
        datatoSave.CamScale= CamScale;

        string savedString = string.Empty;


        savedString += JsonUtility.ToJson(datatoSave);

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

        CamScale= savedData.CamScale;
        CamCoord= savedData.CamCoord;
    }


}
[System.Serializable]
public struct DatatoSave
{
    [SerializeField] public MinerData[] minerdata;
    [SerializeField] public BeltData[] beltdata;
    [SerializeField] public SplitterData[] splitterdata;
    [SerializeField] public RefineryData[] refinerydata;
    [SerializeField] public CoreData[] coredata;

    public Vector2 CamCoord;
    public float CamScale;
}
[System.Serializable]
public class MinerData
{
    [SerializeField] public Vector2 Position;
    [SerializeField] public int Rotation;
    [SerializeField] public int Progress;
}
[System.Serializable]
public class BeltData
{
    [SerializeField] public Vector2 Position;
    [SerializeField] public int Rotation;
    [SerializeField] public float Progress;
    [SerializeField] public int itemID;
}
[System.Serializable]
public class RefineryData
{
    [SerializeField] public Vector2 Position;
    [SerializeField] public int Rotation;
    [SerializeField] public float Progress;
    [SerializeField] public Inventory inv1;
    [SerializeField] public Inventory inv2;
}
[System.Serializable]
public class SplitterData
{
    [SerializeField] public Vector2 Position;
    [SerializeField] public int Rotation;
    [SerializeField] public float Progress;
    [SerializeField] public int itemID;
}
[System.Serializable]
public class CoreData
{
    [SerializeField] public Vector2 Position;
}
