using UnityEngine;
public class SaveData : MonoBehaviour
{
    public long time;
    public int seed;
    public int prestigeState;

    public Achievement[] achievements;
    public Vector2Int[] ResourceStats;
    public bool[] SelectedSkins;

    public TutorialState.State state;

    public Vector2 CamCoord;
    public float CamScale;

    public long Currency;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;
    public AssemblerData[] assemblerdata;
    public UBData[] UBdata;

    public SpeedStates speedstates;

    public Inventory worldinv;

    public bool hasDeconstructed;

    public string SaveIntoJson()
    {

        DatatoSave datatoSave = new DatatoSave();

        datatoSave.time = time;
        datatoSave.seed = seed;
        datatoSave.prestigeState = prestigeState;

        datatoSave.achievements = achievements;
        datatoSave.ResourceStats = ResourceStats;
        datatoSave.SelectedSkins = SelectedSkins;


        datatoSave.state = state;

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
        
        datatoSave.hasDeconstructed = hasDeconstructed;

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

        SelectedSkins = savedData.SelectedSkins;

        achievements= savedData.achievements;
        ResourceStats = savedData.ResourceStats;

        prestigeState = savedData.prestigeState;

        state = savedData.state;

        time = savedData.time;
        seed = savedData.seed;

        speedstates = savedData.speedstates;

        CamScale= savedData.CamScale;
        CamCoord= savedData.CamCoord;
        worldinv = savedData.worldinv;
        Currency = savedData.Currency;
        hasDeconstructed = savedData.hasDeconstructed;
        worldinv.PopulateItemIDs();
    }
}
public struct DatatoSave
{
    public long time;
    public int seed;
    public int prestigeState;

    public Achievement[] achievements;
    public Vector2Int[] ResourceStats;
    public bool[] SelectedSkins;

    public MinerData[] minerdata;
    public BeltData[] beltdata;
    public SplitterData[] splitterdata;
    public RefineryData[] refinerydata;
    public CoreData[] coredata;
    public AssemblerData[] assemblerdata;
    public UBData[] UBdata;

    public TutorialState.State state;
    public SpeedStates speedstates;

    public Vector2 CamCoord;
    public float CamScale;

    public long Currency;

    [SerializeField]
    public Inventory worldinv;

    public bool hasDeconstructed;
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
