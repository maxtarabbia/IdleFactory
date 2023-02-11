using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateSaveLoad : MonoBehaviour
{
    GameObject objectToSave;
    string path = "Assets/Saves";
    // Start is called before the first frame update

    public List<MinerData> minerData = new List<MinerData>();
    public List<BeltData> beltData = new List<BeltData>();
    public List<RefineryData> refineryData = new List<RefineryData>();
    public List<SplitterData> splitterData = new List<SplitterData>();
    public List<CoreData> coreData = new List<CoreData>();

    Buildings buildings;
    WorldGeneration world;

    int ticksToJam;

    SaveData saveData;
    void Start()
    {
        buildings = GetComponent<Buildings>();
        world = GetComponent<WorldGeneration>();
        path = Application.persistentDataPath + path;
        if(System.IO.File.Exists(path + "/Save1.dat"))
        {
            Load();
        }
    }

    public void Save()
    {
        saveData = GetComponent<SaveData>();
        objectToSave = GameObject.Find("MasterObject");
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
        SaveData data = SerializeBuilding(childObjects.ToArray());

        data.worldinv = FindObjectOfType<WorldGeneration>().inv;

        Camera cam = FindObjectOfType<Camera>();
        data.CamScale = cam.orthographicSize;
        data.CamCoord = cam.gameObject.transform.localPosition;

        data.time = Gettime();

        stringdata = JsonUtility.ToJson(data);

      //  data = "Assets/Prefabs/" + objectToSave.name + ".prefab";

        Directory.CreateDirectory(path);
        System.IO.File.WriteAllText(path + "/Save1.dat", stringdata);
    }
    public SaveData SerializeBuilding(GameObject[] go)
    {
        minerData = new List<MinerData>();
        beltData = new List<BeltData>();
        refineryData= new List<RefineryData>();
        splitterData= new List<SplitterData>();
        coreData= new List<CoreData>();


        string outputstring = new string("");


        foreach(GameObject thisObj in go)
        {
            if (thisObj.TryGetComponent<Miner>(out Miner miner))
            {
                MinerData minerdat = new MinerData();
                minerdat.Position = miner.pos;
                minerdat.Rotation = Mathf.RoundToInt(miner.gameObject.transform.eulerAngles.z);
                minerdat.Progress = miner.miningProgress;
                minerData.Add(minerdat);
            }
            else if(thisObj.TryGetComponent<Belt>(out Belt belt))
            {
                BeltData beltdat = new BeltData();
                beltdat.Position = belt.transform.position;
                beltdat.Rotation = Mathf.RoundToInt(belt.gameObject.transform.eulerAngles.z);
                beltdat.Progress = belt.itemID.y;
                beltdat.itemID = Mathf.RoundToInt(belt.itemID.x);
                beltData.Add(beltdat);
            }
            else if (thisObj.TryGetComponent<Refinery>(out Refinery refinery))
            {
                RefineryData refinerydat = new RefineryData();
                refinerydat.Position = refinery.pos;
                refinerydat.Rotation = Mathf.RoundToInt(refinery.gameObject.transform.eulerAngles.z);
                refinerydat.inv1 = refinery.inputInv;
                refinerydat.inv2 = refinery.outputInv;
                refinerydat.Progress = refinery.RProgress;
                refineryData.Add(refinerydat);
            }
            else if (thisObj.TryGetComponent<Splitter>(out Splitter splitter))
            {
                SplitterData splitterdat = new SplitterData();
                splitterdat.Position = splitter.transform.position;
                splitterdat.Rotation = Mathf.RoundToInt(splitter.gameObject.transform.eulerAngles.z);
                splitterdat.Progress = splitter.itemID.y;
                splitterdat.itemID = Mathf.RoundToInt(splitter.itemID.x);
                splitterData.Add(splitterdat);
            }
            else if(thisObj.TryGetComponent(out Core core))
            {
                CoreData coredat = new CoreData();
                coredat.Position = core.gameObject.transform.position;
                coreData.Add(coredat);
            }
        }
        SaveData saveData = GetComponent<SaveData>();


        saveData.minerdata = minerData.ToArray();
        saveData.beltdata= beltData.ToArray();
        saveData.refinerydata= refineryData.ToArray();
        saveData.splitterdata= splitterData.ToArray();
        saveData.coredata= coreData.ToArray();

        return saveData;
    }
    public void Load()
    {
        saveData = GetComponent<SaveData>();
        saveData.LoadFromJson(System.IO.File.ReadAllText(path + "/Save1.dat"));

        if (saveData.minerdata == null)
        {
            return;
        }
        clearMap();

        FindObjectOfType<WorldGeneration>().Initialize(24);
        FindObjectOfType<WorldGeneration>().SetInventory();

        foreach (MinerData minerData in saveData.minerdata)
        {
            GameObject newMiner = PlaceObjectManual(minerData.Position, 0, minerData.Rotation);
            newMiner.GetComponent<Miner>().miningProgress = minerData.Progress;
        }
        foreach (BeltData beltData in saveData.beltdata)
        {
            GameObject newBelt = PlaceObjectManual(beltData.Position, 1, beltData.Rotation);
            newBelt.GetComponent<Belt>().itemID.x = beltData.itemID;
            newBelt.GetComponent<Belt>().itemID.y = beltData.Progress;
            newBelt.GetComponent<Belt>().world = FindObjectOfType<WorldGeneration>();
     //       newBelt.GetComponent<Belt>().UpdateSpritePositions(false);
        }
        foreach (SplitterData splitterData in saveData.splitterdata)
        {
            GameObject newSplitter = PlaceObjectManual(splitterData.Position,3, splitterData.Rotation);
            newSplitter.GetComponent<Splitter>().itemID.x = splitterData.itemID;
            newSplitter.GetComponent<Splitter>().itemID.y = splitterData.Progress;
            newSplitter.GetComponent<Splitter>().world = FindObjectOfType<WorldGeneration>();

        }
        foreach (RefineryData refineryData in saveData.refinerydata)
        {
            GameObject newRefinery = PlaceObjectManual(refineryData.Position,2, refineryData.Rotation);
            newRefinery.GetComponent<Refinery>().RProgress = Mathf.RoundToInt(refineryData.Progress);
            newRefinery.GetComponent<Refinery>().inputInv = refineryData.inv1;
            newRefinery.GetComponent<Refinery>().outputInv = refineryData.inv2;
        }
        foreach (CoreData coreData in saveData.coredata)
        {
            GameObject newCore = PlaceObjectManual(coreData.Position + new Vector2(-.5f,-.5f), 4, 0);
        }
        FindObjectOfType<Camera>().transform.position = new Vector3(saveData.CamCoord.x, saveData.CamCoord.y, -10);
        FindObjectOfType<Camera>().orthographicSize = saveData.CamScale;

        FindObjectOfType<WorldGeneration>().inv = saveData.worldinv;

        int oldtime = saveData.time;
        int ticks = (Gettime() - oldtime);
        if (ticks < 0)
        {
            ticks += 216000;
            print("boosted time by 216000");
        }
        ticksToJam = ticks * 50;
    }
    int Gettime()
    {
        return DateTime.Now.Second + DateTime.Now.Minute * 60 + DateTime.Now.Hour * 3600;
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
            if (ticksToJam > 100)
            {
                FindObjectOfType<TickEvents>().TickJam(100);
                ticksToJam -= 100;
            }
            else
            {
                FindObjectOfType<TickEvents>().TickJam(ticksToJam);
                ticksToJam= 0;
            }
        }
    }
    public GameObject PlaceObjectManual(Vector3 pos, int BuildableIndex, int Rotation)
    {
        Vector3 Transposition = pos;
        if (buildings.AllBuildings[BuildableIndex].size % 2 == 0) Transposition += new Vector3(0.5f, 0.5f, 0f);

        buildings.AllBuildings[BuildableIndex].count++;

        GameObject instancedObj = Instantiate(buildings.AllBuildings[BuildableIndex].prefab);
        instancedObj.transform.position = Transposition + new Vector3(0, 0, -1);
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
        return instancedObj;
    }
}
