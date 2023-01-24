using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Miner : MonoBehaviour
{
    public Dictionary<Vector2,Cell> worldmap;
    public Vector2 pos;
    public int[] coveredTileID;

    Vector3 basePos;

    VisualEffect effect;
    WorldGeneration world;


    Transform[] transforms;

    int miningProgress;
    int ticksToMine = 100;

    bool isOnOre = false;

     public Sprite[] OreSprites;
     //Start is called before the first frame update
    void Start()
    {
        coveredTileID = Enumerable.Repeat(-1, 4).ToArray();
        transforms = GetComponentsInChildren<Transform>();
        basePos = transforms[1].localPosition;
        //gameObject.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), Mathf.Round(gameObject.transform.position.z));
    }
    void Initialize()
    {
        world = FindObjectOfType<WorldGeneration>();
        worldmap = world.oreMap;
        gameObject.transform.localScale = Vector3.one;

        effect = GetComponent<VisualEffect>();


        pos = gameObject.transform.position;
        pos += new Vector2(-0.5f,-0.5f);
        coveredTileID[0] = world.oreMap[pos].ID;
        coveredTileID[1] = world.oreMap[pos + new Vector2(0,1)].ID;
        coveredTileID[2] = world.oreMap[pos + new Vector2(1,0)].ID;
        coveredTileID[3] = world.oreMap[pos + new Vector2(1,1)].ID;

        checkForOre();

    }
    // On Hover Events
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[0].cost[0].x, (int)builds.AllBuildings[0].cost[0].y);

            world.OccupiedCells.Remove(pos);
            world.OccupiedCells.Remove(pos + new Vector2(0,1));
            world.OccupiedCells.Remove(pos + new Vector2(1,0));
            world.OccupiedCells.Remove(pos + new Vector2(1,1));

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(coveredTileID[0] == -1)
        {
            Initialize();
        }
        
    }
    private void FixedUpdate()
    {
        if (isOnOre)
        {
            if (miningProgress >= ticksToMine)
            {
                MineItem();
                miningProgress = 0;
            }
            MiningAnimation();
            miningProgress += 1;
        }
        
    }
    void MiningAnimation()
    {
        transforms[1].localPosition = basePos + new Vector3(Random.value - 0.5f, Random.value - 0.5f) * 0.01f;
        transforms[2].localPosition = basePos + new Vector3(Random.value - 0.5f, Random.value - 0.5f) * 0.02f;
        transforms[3].Rotate(Vector3.forward * (200/ticksToMine));
    }
    void MineItem()
    {
        int minedItemID = coveredTileID[Mathf.FloorToInt(Random.value * 4)];
        switch (minedItemID)
        {
            case 0: //blank tile
                MineItem();
                break;
            case 1: //iron ore

                if (!OutputItem(1))
                { 
                    world.inv.AddItem(1, 1);
                    effect.SetTexture("spriteTex", OreSprites[0].texture);
                    effect.Play();
                }
                break;
            case 2: //copper ore

                if (!OutputItem(2))
                {
                    world.inv.AddItem(2, 1);
                    effect.SetTexture("spriteTex", OreSprites[1].texture);
                    effect.Play();
                }
                break;
        }
        
    }
    void checkForOre()
    {
        foreach (int tile in coveredTileID)
        {
            if (tile != 0)
                isOnOre= true;
            
        }
    }
    bool OutputItem(int itemID)
    {
        Vector2 outputCoord = new Vector2();
        switch (gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2(0, -1);
                break;
            case 90:
                outputCoord = pos + new Vector2(2, 0);
                break;
            case 180:
                outputCoord = pos + new Vector2(1, 2);
                break;
            case 270:
                outputCoord = pos + new Vector2(-1, 1);
                break;
        }
        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltScript = cellObj.GetComponent<Belt>();
            Refinery refineryScript = cellObj.GetComponent<Refinery>();
            if (beltScript != null)
            {
                if (beltScript.inputItem(itemID, 1))
                {
                    return true;
                }
            }
            else if(refineryScript != null)
            {
                if(refineryScript.InputItem(itemID,1))
                {
                    return true;
                }
            }
        }
        return false;

    }
}
