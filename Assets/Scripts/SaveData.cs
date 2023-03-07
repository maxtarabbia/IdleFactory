using UnityEngine;
public class SaveData : MonoBehaviour
{
    public long time;
    public int seed;

    public Vector2 CamCoord;
    public float CamScale;

    public int Currency;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;
    public AssemblerData[] assemblerdata;
    public UBData[] UBdata;

    public SpeedStates speedstates;

    public Inventory worldinv;

    public string SaveIntoJson()
    {

        DatatoSave datatoSave = new DatatoSave();

        datatoSave.time = time;
        datatoSave.seed = seed;

        datatoSave.UBdata = UBdata;
        datatoSave.minerdata= minerdata;
        datatoSave.beltdata= beltdata;
        datatoSave.splitterdata= splitterdata;
        datatoSave.coredata= coredata;
        datatoSave.refinerydata= refinerydata;
        datatoSave.assemblerdata= assemblerdata;
        datatoSave.Currency= Currency;
        datatoSave.CamCoord = CamCoord;
        datatoSave.CamScale= CamScale;
        datatoSave.worldinv= worldinv;

        datatoSave.speedstates= speedstates;
        

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
        assemblerdata= savedData.assemblerdata;
        UBdata = savedData.UBdata;


        time = savedData.time;
        seed = savedData.seed;

        speedstates = savedData.speedstates;

        CamScale= savedData.CamScale;
        CamCoord= savedData.CamCoord;
        worldinv = savedData.worldinv;
        Currency = savedData.Currency;
        worldinv.PopulateItemIDs();
    }
}
public struct DatatoSave
{
    public long time;
    public int seed;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;
    public AssemblerData[] assemblerdata;
    public UBData[] UBdata;

    public SpeedStates speedstates;

    public Vector2 CamCoord;
    public float CamScale;

    public int Currency;

    [SerializeField]
    public Inventory worldinv;
}
[System.Serializable]
public class MinerData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public float Speed;
}
[System.Serializable]
public class UBData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public int itemID;
    public float Speed;
}
[System.Serializable]
public class BeltData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public int itemID;
    public float Speed;
}
[System.Serializable]
public class AssemblerData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public Inventory inv1;
    public Inventory inv2;
    public float Speed;
}
[System.Serializable]
public class RefineryData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public Inventory inv1;
    public Inventory inv2;
    public float Speed;
}
[System.Serializable]
public class SplitterData
{
    public Vector2 Position;
    public int Rotation;
    public float Progress;
    public int itemID;
    public float Speed;
}
[System.Serializable]
public class CoreData
{
    public Vector2 Position;
}
