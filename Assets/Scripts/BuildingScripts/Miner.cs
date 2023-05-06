using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.VFX;

public class Miner : MonoBehaviour
{
    public Dictionary<Vector2, Cell> worldmap;
    public Vector2 pos;
    public int[] coveredTileID;



    public bool hasRotated = false;

    WorldGeneration world;

    GameObject OutputObj;

    TickEvents tickEvents;

    Vector3 basePos;
    Transform[] transforms;

    AudioSource AS;

    public float miningProgress;
    public float secondsPerItem = 4;

    bool isOnOre = false;

    int calls;

    ResourceStats stats;

    Inventory inv;

    public GameObject Frame;
    public GameObject Hopper;
    public GameObject Drill;

    public ParticleSystem oreUIPS;

    public Material[] Oremats;
    //Start is called before the first frame update
    void Awake()
    {
        stats = FindObjectOfType<ResourceStats>();
        coveredTileID = Enumerable.Repeat(-1, 4).ToArray();
        transforms = GetComponentsInChildren<Transform>();
        basePos = transforms[1].localPosition;
        //gameObject.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), Mathf.Round(gameObject.transform.position.z));
    }
    private void Start()
    {

        AS = GetComponent<AudioSource>();
    }
    void Initialize()
    {
        inv = new Inventory(1);
        inv.maxStackSize= 1;
        inv.PopulateItemIDs();
        world = FindObjectOfType<WorldGeneration>();
        worldmap = world.oreMap;
        gameObject.transform.localScale = Vector3.one;

        FindObjectOfType<Skins>().Setskin(Skin.SkinType.Miner, gameObject);


        secondsPerItem = world.speedstates.MinerInfo.speed;

        pos = gameObject.transform.position;
        pos += new Vector2(-0.5f, -0.5f);
        try
        {
            coveredTileID[0] = world.oreMap[pos].ID;
            coveredTileID[1] = world.oreMap[pos + new Vector2(0, 1)].ID;
            coveredTileID[2] = world.oreMap[pos + new Vector2(1, 0)].ID;
            coveredTileID[3] = world.oreMap[pos + new Vector2(1, 1)].ID;
        }
        catch
        {
            if (!world.oreMap.ContainsKey(pos))
            {
                world.SetDefaultCell(pos);
            }
            if (!world.oreMap.ContainsKey(pos + new Vector2(0, 1)))
            {
                world.SetDefaultCell(pos + new Vector2(0, 1));
            }
            if (!world.oreMap.ContainsKey(pos + new Vector2(1, 0)))
            {
                world.SetDefaultCell(pos + new Vector2(1, 0));
            }
            if (!world.oreMap.ContainsKey(pos + new Vector2(1, 1)))
            {
                world.SetDefaultCell(pos + new Vector2(1, 1));
            }
        }
        checkForOre();

        tickEvents = world.GetComponent<TickEvents>();
        tickEvents.MyEvent += OnTick;
        FindObjectOfType<StateSaveLoad>().Save();

    }
    public GameObject RecipeDisplay;
    float timeHovering;
    private void OnMouseOver()
    {
        timeHovering += Time.deltaTime;
        if (timeHovering < 1)
            return;

        if (GetComponentInChildren<DisplayRecipes>() != null)
            return;

        GameObject rec = Instantiate(RecipeDisplay, transform);
        rec.transform.rotation = Quaternion.identity;
        rec.transform.position = new Vector3(3, 2, -0.01f) + gameObject.transform.position;
        rec.GetComponent<DisplayRecipes>().type = DisplayRecipes.BuildingType.Miner;

    }
    private void OnMouseExit()
    {
        timeHovering = 0;
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }

    }
    // On Hover Events

       /* if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
            FindObjectOfType<Buildings>().AllBuildings[0].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            FindObjectOfType<Buildings>().AllBuildings[0].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[0].cost[0].x, (int)builds.AllBuildings[0].cost[0].y);

            world.OccupiedCells.Remove(pos);
            world.OccupiedCells.Remove(pos + new Vector2(0, 1));
            world.OccupiedCells.Remove(pos + new Vector2(1, 0));
            world.OccupiedCells.Remove(pos + new Vector2(1, 1));

            builds.AllBuildings[0].count--;

            Destroy(gameObject);
        }
       */
    
    public void RotateCW()
    {
        OutputObj = null;
        gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        FindObjectOfType<Buildings>().AllBuildings[0].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        UpdateEffectRotation();
        hasRotated= true;
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }
    }
    void UpdateEffectRotation()
    {
        
    }
    public void RotateCCW()
    {
        try
        {
            Destroy(GetComponentInChildren<DisplayRecipes>().gameObject);
        }
        catch { }
        OutputObj = null;
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        hasRotated = true;
        UpdateEffectRotation();
        FindObjectOfType<Buildings>().AllBuildings[0].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
    }
    public void Delete()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        world.inv.AddItem(builds.AllBuildings[0].cost[0].x, builds.AllBuildings[0].cost[0].y * Mathf.Clamp(builds.AllBuildings[0].count - 2,1,int.MaxValue));

        world.OccupiedCells.Remove(pos);
        world.OccupiedCells.Remove(pos + new Vector2(0, 1));
        world.OccupiedCells.Remove(pos + new Vector2(1, 0));
        world.OccupiedCells.Remove(pos + new Vector2(1, 1));

        builds.AllBuildings[0].count--;

        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        tickEvents.MyEvent -= OnTick;
    }
    // Update is called once per frame
    void Update()
    {
        if(coveredTileID[0] == -1)
        {
            Initialize();
        }
        if(isOnOre)
        {

            MiningAnimation();
        }
    }
    
    void OnTick()
    {
        Profiler.BeginSample("Miner Tick Logic");
        if (this == null)
        {
            tickEvents.MyEvent -= OnTick;
            return;
        }

        if (isOnOre)
        {
            if (miningProgress >= secondsPerItem)
            {
                MineItem();
                miningProgress -= secondsPerItem;
            }
            miningProgress += Time.fixedDeltaTime;
        }
        if (inv.items[0].count > 0)
        {
            if (OutputItem(inv.items[0].ID))
            {
                inv.RemoveItem(new int2[] { new int2(inv.items[0].ID, 1) }, 1);
            }
        }
        Profiler.EndSample();
    }
    void MiningAnimation()
    {
        transforms[1].localPosition = basePos + new Vector3(Mathf.Sin(Time.frameCount*0.0984f + pos.x*85.584f), Mathf.Sin(Time.frameCount * 0.132f + pos.y * 85.584f)) * 0.005f;
        transforms[2].localPosition = basePos + new Vector3(Mathf.Sin(Time.frameCount * 0.186f + pos.x * 1.4f), Mathf.Sin(Time.frameCount * 0.1f+ pos.x * 12.544f)) * 0.01f;
        transforms[3].Rotate(Vector3.forward * (200/secondsPerItem) * Time.deltaTime);
    }
    void MineItem()
    {
        if(calls > 40)
        {
            checkForOre();
        }
        if(calls > 60)
        {

            checkForOre();
            print("Broken Miner at:" + pos + "\nWith: " + coveredTileID[0] + "," + coveredTileID[1] + "," + coveredTileID[2] + "," + coveredTileID[3]);
            isOnOre = false;
            calls = 0;
            return;
        }
        float randfloat = (float)new System.Random().NextDouble();
        int minedItemID = coveredTileID[Mathf.FloorToInt(randfloat * 4)];
        switch (minedItemID)
        {
            case 0: //blank tile
                /*
                calls++;
                MineItem();
                */
                break;
            case 1: //iron ore
                calls = 0;
                stats.Additem(0, 1);
                if (!OutputItem(0))
                {
                    if (!inv.AddItem(0, 1))
                    { 
                    world.inv.AddItem(0, 1);
                    
                    GetComponent<ParticleSystemRenderer>().material = Oremats[0];
                        if (PlayerPrefs.GetInt("isLoaded") == 1)
                        {
                            oreUIPS.Play(false);
                        }
                    }
                }
                break;
            case 2: //copper ore
                calls = 0;
                stats.Additem(1, 1);
                if (!OutputItem(1))
                {
                    if (!inv.AddItem(1, 1))
                    {
                        world.inv.AddItem(1, 1);
                        GetComponent<ParticleSystemRenderer>().material = Oremats[1];
                        if (PlayerPrefs.GetInt("isLoaded") == 1)
                        {
                            oreUIPS.Play(false);
                        }
                    }
                }
                break;
        }
        
    }
    void checkForOre()
    {
        isOnOre = false;
        foreach (int tile in coveredTileID)
        {
            if (tile > 0)
                isOnOre= true;
            
        }
        if (isOnOre)
        {
            AS.time = (float)new System.Random().NextDouble() * AS.clip.length;
            AS.Play();
        }
    }
    bool OutputItem(int itemID)
    {
        Vector2 outputCoord = new Vector2();
        Vector2 OutFromCoord = new Vector2();
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2(0, -1);
                OutFromCoord = pos;
                break;
            case 90:
                outputCoord = pos + new Vector2(2, 0);
                OutFromCoord = pos + new Vector2(1, 0);
                break;
            case 180:
                outputCoord = pos + new Vector2(1, 2);
                OutFromCoord = pos + new Vector2(1, 1);
                break;
            case 270:
                outputCoord = pos + new Vector2(-1, 1);
                OutFromCoord = pos + new Vector2(0,1);
                break;
        }

        bool canAccept = false;
        if (OutputObj == null)
            world.OccupiedCells.TryGetValue(outputCoord, out OutputObj);
        if(OutputObj != null)
            canAccept = ItemReceiver.CanObjectAcceptItem(OutputObj,itemID,Vector2Int.RoundToInt(OutFromCoord));

        return canAccept;


        /*
        if (OutputObj != null)
        {
            Belt beltScript = OutputObj.GetComponent<Belt>();
            if (beltScript != null)
            {
                if (beltScript.inputItem(itemID, 0.5f))
                {
                    return true;
                }
            }
            Splitter splitterscript = OutputObj.GetComponent<Splitter>();
            if (splitterscript != null)
            {
                if (OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z == 90 || OutputObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z == -270)
                {
                    if (splitterscript.inputItem(itemID, 0))
                    {
                        return true;
                    }
                }

            }
            Refinery refineryScript = OutputObj.GetComponent<Refinery>();
            if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1, OutFromCoord))
                {
                    return true;
                }
            }
            Assembler assembler = OutputObj.GetComponent<Assembler>();
            if (assembler != null)
            {
                if (assembler.InputItem(itemID, 1, OutFromCoord))
                {
                    return true;
                }
            }
            Core corescript = OutputObj.GetComponent<Core>();
            if (corescript != null)
            {
                corescript.InputItem(itemID);
                return true;
            }

        }
        return false;
        */
    }
}
