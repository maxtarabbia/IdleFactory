using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class StateSaveLoad : MonoBehaviour
{

    string path = "/Saves";
    // Start is called before the first frame update

    public bool SaveNext = false;

    public List<MinerData> minerData = new List<MinerData>();
    public List<BeltData> beltData = new List<BeltData>();
    public List<RefineryData> refineryData = new List<RefineryData>();
    public List<SplitterData> splitterData = new List<SplitterData>();
    public List<CoreData> coreData = new List<CoreData>();
    public List<AssemblerData> assemblerData = new List<AssemblerData>();
    public List<UBData> UBdata = new List<UBData>();



    Buildings buildings;
    WorldGeneration world;

    public long totalTicks;
    public long ticksToJam;
    long ticksAtATime = 5000;

    int MaxHours = 4;

    SaveData saveData;
    void Start()
    {
        buildings = GetComponent<Buildings>();
        world = GetComponent<WorldGeneration>();
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            
        }
        path = Application.persistentDataPath + path;
        if (File.Exists(path + "/Save.dat"))
        {
            Load();
        }
    }
    public void LateSave()
    {
        SaveNext = true;
    }
    public void Save()
    {
        Profiler.BeginSample("Searching all builds");
        saveData = GetComponent<SaveData>();
        string stringdata;
        Transform[] Transforms = GameObject.Find("Buildings").GetComponentsInChildren<Transform>();
        List<GameObject> childObjects = new List<GameObject>();
        foreach(Transform child in Transforms)
        {
            if (child.gameObject.transform.parent == GameObject.Find("Buildings").transform)
            {
                childObjects.Add(child.gameObject);
            }
        }
        Profiler.EndSample();
        Profiler.BeginSample("Assemble Building Data");
        SaveData data = SerializeBuilding(childObjects.ToArray());
        Profiler.EndSample();
        Profiler.BeginSample("Collecting rest of data");
        WorldGeneration world = FindObjectOfType<WorldGeneration>();
        data.worldinv = world.inv;

        data.speedstates = world.speedstates;

        data.SelectedSkins = FindObjectOfType<Skins>().allSkins.Select(o => o.isSelected).ToArray();

        Camera cam = FindObjectOfType<Camera>();
        data.CamScale = cam.orthographicSize;
        data.CamCoord = cam.gameObject.transform.localPosition;
        data.prestigeState = world.prestigeState;

        data.state = FindObjectOfType<TutorialState>().currentState;

        data.achievements = FindObjectOfType<ResourceStats>().achievements;
        data.ResourceStats = FindObjectOfType<ResourceStats>().createdItemsTotal.ToArray();

        data.time = Gettime();
        data.seed = world.Seed;
        data.Currency = world.Currency;
        Profiler.EndSample();
        Profiler.BeginSample("convert to json");
        stringdata = JsonUtility.ToJson(data);
        Profiler.EndSample();

        Profiler.BeginSample("Writing to path");
        Directory.CreateDirectory(path);
        System.IO.File.WriteAllText(path + "/Save.dat", stringdata);
        PlayerPrefs.Save();
        SaveNext = false;
        Debug.Log("DebugLog: Saved Game to: " + path);

        Profiler.EndSample();
    }
    public SaveData SerializeBuilding(GameObject[] go)
    {
        minerData = new List<MinerData>();
        beltData = new List<BeltData>();
        refineryData= new List<RefineryData>();
        splitterData= new List<SplitterData>();
        coreData= new List<CoreData>();
        assemblerData= new List<AssemblerData>();
        UBdata = new List<UBData>();

        

        string outputstring = new string("");


        foreach(GameObject thisObj in go)
        {
            if (thisObj.TryGetComponent(out Miner miner))
            {
                MinerData minerdat = new MinerData();
                minerdat.Position = miner.pos;
                minerdat.Rotation = Mathf.RoundToInt(miner.gameObject.transform.eulerAngles.z);
                minerdat.Progress = miner.miningProgress;
                minerdat.Speed = miner.Speed;
                minerData.Add(minerdat);
            }
            else if(thisObj.TryGetComponent(out Belt belt))
            {
                BeltData beltdat = new BeltData();
                beltdat.Position = belt.transform.position;
                beltdat.Rotation = Mathf.RoundToInt(belt.gameObject.transform.eulerAngles.z);
                beltdat.Progress = belt.itemID.y;
                beltdat.itemID = Mathf.RoundToInt(belt.itemID.x);
                beltdat.Speed = belt.Speed;
                beltData.Add(beltdat);
            }
            else if (thisObj.TryGetComponent(out Refinery refinery))
            {
                RefineryData refinerydat = new RefineryData();
                refinerydat.Position = refinery.pos;
                refinerydat.Rotation = Mathf.RoundToInt(refinery.gameObject.transform.eulerAngles.z);
                refinerydat.inv1 = refinery.inputInv;
                refinerydat.inv2 = refinery.outputInv;
                refinerydat.Progress = refinery.RProgress;
                refinerydat.Speed = refinery.Speed;
                refineryData.Add(refinerydat);
            }
            else if (thisObj.TryGetComponent(out Splitter splitter))
            {
                SplitterData splitterdat = new SplitterData();
                splitterdat.Position = splitter.transform.position;
                splitterdat.Rotation = Mathf.RoundToInt(splitter.gameObject.transform.eulerAngles.z);
                splitterdat.Progress = splitter.itemsID[0].y;
                splitterdat.itemID = Mathf.RoundToInt(splitter.itemsID[0].x);
                splitterdat.Speed = splitter.Speed;
                splitterData.Add(splitterdat);
            }
            else if(thisObj.TryGetComponent(out Core core))
            {
                CoreData coredat = new CoreData();
                coredat.Position = core.gameObject.transform.position;
                coreData.Add(coredat);
            }
            else if(thisObj.TryGetComponent(out UnderGroundBelt underGroundBelt))
            {
                UBData UBdat = new UBData
                {
                    Position = underGroundBelt.transform.position,
                    Rotation = Mathf.RoundToInt(underGroundBelt.gameObject.transform.eulerAngles.z),
                    Progress = underGroundBelt.itemID.y,
                    itemID = Mathf.RoundToInt(underGroundBelt.itemID.x),
                    Speed = underGroundBelt.Speed
                };
                UBdata.Add(UBdat);
            }
            else if (thisObj.TryGetComponent(out Assembler assembler))
            {
                AssemblerData assemblerdat = new AssemblerData
                {
                    Position = assembler.pos,
                    Rotation = Mathf.RoundToInt(assembler.gameObject.transform.eulerAngles.z),
                    inv1 = assembler.inputInv,
                    inv2 = assembler.outputInv,
                    Progress = assembler.RProgress,
                    Speed = assembler.Speed
                };
                assemblerData.Add(assemblerdat);
            }
        }
        SaveData saveData = GetComponent<SaveData>();

       
        saveData.UBdata = UBdata.ToArray();
        saveData.minerdata = minerData.ToArray();
        saveData.beltdata= beltData.ToArray();
        saveData.refinerydata= refineryData.ToArray();
        saveData.splitterdata= splitterData.ToArray();
        saveData.coredata= coreData.ToArray();
        saveData.assemblerdata= assemblerData.ToArray();

        return saveData;
    }
    public void Load()
    {
        saveData = GetComponent<SaveData>();
        saveData.LoadFromJson(System.IO.File.ReadAllText(path + "/Save.dat"));

        if (saveData.minerdata == null)
        {
            return;
        }
        clearMap();

        WorldGeneration world = FindObjectOfType<WorldGeneration>();

        PlayerPrefs.SetInt("Seed", saveData.seed);
        world.prestigeState = saveData.prestigeState;
        world.Initialize(24, saveData.seed);
        world.SetInventory();

        FindObjectOfType<Skins>().SetSelected(saveData.SelectedSkins);

        foreach (MinerData minerData in saveData.minerdata)
        {
            GameObject newMiner = PlaceObjectManual(minerData.Position, 0, minerData.Rotation);
            newMiner.GetComponent<Miner>().Speed = minerData.Speed;
            newMiner.GetComponent<Miner>().miningProgress = minerData.Progress;
        }
        foreach (UBData UBdata in saveData.UBdata)
        {
            GameObject newUGBelt = PlaceObjectManual(UBdata.Position, 6, UBdata.Rotation);
            newUGBelt.GetComponent<UnderGroundBelt>().itemID.x = UBdata.itemID;
            newUGBelt.GetComponent<UnderGroundBelt>().itemID.y = UBdata.Progress;
            newUGBelt.GetComponent<UnderGroundBelt>().Speed = UBdata.Speed;
            newUGBelt.GetComponent<UnderGroundBelt>().world = world;
        }
        foreach (BeltData beltData in saveData.beltdata)
        {
            GameObject newBelt = PlaceObjectManual(beltData.Position, 1, beltData.Rotation);
            newBelt.GetComponent<Belt>().itemID.x = beltData.itemID;
            newBelt.GetComponent<Belt>().itemID.y = beltData.Progress;
            newBelt.GetComponent<Belt>().Speed = beltData.Speed;
            newBelt.GetComponent<Belt>().world = world;
        }
        foreach (SplitterData splitterData in saveData.splitterdata)
        {
            GameObject newSplitter = PlaceObjectManual(splitterData.Position,3, splitterData.Rotation);
            newSplitter.GetComponent<Splitter>().itemsID[0].x = splitterData.itemID;
            newSplitter.GetComponent<Splitter>().itemsID[0].y = splitterData.Progress;
            newSplitter.GetComponent<Splitter>().Speed = splitterData.Speed;
            newSplitter.GetComponent<Splitter>().world = world;

        }
        foreach (RefineryData refineryData in saveData.refinerydata)
        {
            GameObject newRefinery = PlaceObjectManual(refineryData.Position,2, refineryData.Rotation);
            newRefinery.GetComponent<Refinery>().RProgress = refineryData.Progress;
            newRefinery.GetComponent<Refinery>().inputInv = refineryData.inv1;
            newRefinery.GetComponent<Refinery>().outputInv = refineryData.inv2;
            newRefinery.GetComponent<Refinery>().Speed = refineryData.Speed;

        }
        foreach (CoreData coreData in saveData.coredata)
        {
            GameObject newCore = PlaceObjectManual(coreData.Position + new Vector2(-.5f,-.5f), 4, 0);
        }
        foreach (AssemblerData assemblerData in saveData.assemblerdata)
        {
            GameObject newAssembler = PlaceObjectManual(assemblerData.Position, 5, assemblerData.Rotation);
            newAssembler.GetComponent<Assembler>().RProgress = assemblerData.Progress;
            newAssembler.GetComponent<Assembler>().inputInv = assemblerData.inv1;
            newAssembler.GetComponent<Assembler>().outputInv = assemblerData.inv2;
            newAssembler.GetComponent<Assembler>().Speed = assemblerData.Speed;

        }
        FindObjectOfType<Camera>().transform.position = new Vector3(saveData.CamCoord.x, saveData.CamCoord.y, -10);
        FindObjectOfType<Camera>().orthographicSize = saveData.CamScale;

        world.inv = saveData.worldinv;
        world.Currency = saveData.Currency;
        world.Seed = saveData.seed;

        ResourceStats RS = FindObjectOfType<ResourceStats>();
        RS.createdItemsTotal = saveData.ResourceStats.ToList();
        RS.achievements = saveData.achievements;

        world.speedstates = saveData.speedstates;

        FindObjectOfType<TutorialState>().currentState = saveData.state;

        FindObjectOfType<Camera_Movement>().updateCamSize();
        //FindObjectOfType<WorldGeneration>().Initialize(24);

        long oldtime = saveData.time;
        long seconds = (Gettime() - oldtime);
        if (seconds < 0)
        {
            seconds = 0;
            print("?????");
        }
        else if(GameObject.Find("LoadingScreen"))
        {
            PlayerPrefs.SetInt("isloaded", 1);
        }
        int TPS = Mathf.RoundToInt(1 / Time.fixedDeltaTime);
        print("Simulating " + seconds + " seconds\nActually simulating " + Mathf.Clamp((int)seconds, 5, 3600 * MaxHours) + " seconds");
        ticksToJam = Mathf.Clamp((int)seconds,5,3600 * MaxHours) * TPS;
        totalTicks = ticksToJam;
        world.inv.SortInv();

    }
    long Gettime()
    {
        return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
    }
    void clearMap()
    {
        Transform[] BuildingMaster = GameObject.Find("Buildings").GetComponentsInChildren<Transform>(true);
        Transform[] OreMaster = GameObject.Find("Ores").GetComponentsInChildren<Transform>(true);

        List<GameObject> objsToDelete = new List<GameObject>();
        foreach (Transform t in BuildingMaster)
        {
            if(t.parent == GameObject.Find("Buildings").transform)
            {
                objsToDelete.Add(t.gameObject);
            }

        }
        foreach (Transform t in OreMaster)
        {
            if (t.parent == GameObject.Find("Ores").transform)
            {
                objsToDelete.Add(t.gameObject);
            }
        }
        foreach(GameObject obj in objsToDelete)
        {
            Destroy(obj);
        }


    }
    private void LateUpdate()
    {
        if(ticksToJam != 0)
        {
            int optimaltick = Mathf.RoundToInt(MathF.Ceiling(totalTicks / 100f));
            if (ticksAtATime != optimaltick)
            {
                ticksAtATime = optimaltick;
                print("ticks At a time: "+ ticksAtATime);
            }
            if (ticksToJam > ticksAtATime)
            {
                FindObjectOfType<TickEvents>().TickJam(ticksAtATime);
                ticksToJam -= ticksAtATime;
            }
            else
            {
                FindObjectOfType<TickEvents>().TickJam(ticksToJam);
                ticksToJam= 0;
            }
        }
        if(SaveNext)
        {
            asyncsave();
        }
    }
    void asyncsave()
    {
        Profiler.BeginSample("Saving");
        Save();
        //await Task.Run(() => Save());
        Profiler.EndSample();
    }
    public GameObject PlaceObjectManual(Vector3 pos, int BuildableIndex, int Rotation)
    {



        Vector3 Transposition = pos;
        if (buildings.AllBuildings[BuildableIndex].size % 2 == 0) Transposition += new Vector3(0.5f, 0.5f, 0f);

        buildings.AllBuildings[BuildableIndex].count++;

        GameObject instancedObj = Instantiate(buildings.AllBuildings[BuildableIndex].prefab);
        instancedObj.transform.position = Transposition + new Vector3(0, 0, 9);
        instancedObj.transform.parent = transform.GetChild(0);
        instancedObj.isStatic = true;
        instancedObj.transform.Rotate(0, 0, Rotation);


        for (int i = 0; i < buildings.AllBuildings[BuildableIndex].size; i++)
        {
            for (int j = 0; j < buildings.AllBuildings[BuildableIndex].size; j++)
            {
                world.OccupiedCells[new Vector2(pos.x + i, pos.y + j)] = instancedObj;
            }
        }

        if (BuildableIndex == 6)
        {
            Vector3 rotatedpos = Quaternion.Euler(0, 0, Rotation) * new Vector3(-3, 0, 0) + pos;
            Vector2 Vec2 = rotatedpos;
            world.OccupiedCells.Add((Vector2)Vector2Int.RoundToInt(Vec2), instancedObj);
        }

        return instancedObj;
    }
    public void DeleteSave()
    {
        System.IO.File.Delete(path + "/Save.dat");
    }
}
