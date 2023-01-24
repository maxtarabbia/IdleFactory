using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class Refinery : MonoBehaviour
{
    WorldGeneration world;
    Vector2 pos;

    Inventory inputInv;
    Inventory outputInv;

    int RProgress;
    int RTime = 50;

    public int inCount;
    public int outCount;

    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        pos = gameObject.transform.position;
        pos += new Vector2(-0.5f, -0.5f);
        inputInv = new Inventory(1);
        outputInv = new Inventory(1);
    }

    void FixedUpdate()
    {
        OnTick();

        inCount = inputInv.items[0].count;
        outCount = outputInv.items[0].count;
    }
    void OnTick()
    {

        if (inputInv.items[0].ID == -1)
            return;

        RProgress++;
        if (RProgress >= RTime)
        {
            int inID = inputInv.items[0].ID;
            int outID = inID + 2;
            if (outputInv.AddItem(outID, 1))
            {

                inputInv.RemoveItem(new Vector2[] {new Vector2 (inputInv.items[0].ID, 1)});
                RProgress = 0;
            }
            else
            {
                RProgress = RTime;
            }
        }
        if (outputInv.items[0].count > 0)
        {
            if (OutputItem())
            {
                outputInv.RemoveItem(new Vector2[] { new Vector2(outputInv.items[0].ID, 1) });
            }
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[2].cost[0].x, (int)builds.AllBuildings[2].cost[0].y);
            world.inv.AddItem((int)builds.AllBuildings[2].cost[1].x, (int)builds.AllBuildings[2].cost[1].y);

            world.OccupiedCells.Remove(pos);
            world.OccupiedCells.Remove(pos + new Vector2(0, 1));
            world.OccupiedCells.Remove(pos + new Vector2(1, 0));
            world.OccupiedCells.Remove(pos + new Vector2(1, 1));

            Destroy(gameObject);
        }
    }
    bool OutputItem()
    {
        int itemID = outputInv.items[0].ID;
        Vector2 outputCoord = new Vector2();
        switch (gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2(-1, 0);
                break;
            case 90:
                outputCoord = pos + new Vector2(1, -1);
                break;
            case 180:
                outputCoord = pos + new Vector2(2, 1);
                break;
            case 270:
                outputCoord = pos + new Vector2(0, 2);
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
            else if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool InputItem(int ID, int count)
    {
        
        return inputInv.AddItem(ID, count);
    }
}
