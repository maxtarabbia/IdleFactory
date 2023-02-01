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

    Vector2 outputCoord = new Vector2();
    Vector2 inputCoord= new Vector2();

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

        inputInv.maxStackSize = 10;
        outputInv.maxStackSize = 10;
        SetOutput();
    }

    void FixedUpdate()
    {
        OnTick();

        inCount = inputInv.items[0].count;
        outCount = outputInv.items[0].count;
    }
    void SetOutput()
    {
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                outputCoord = pos + new Vector2(-1, 0);
                inputCoord = pos + new Vector2(2, 1);
                break;
            case 90:
                outputCoord = pos + new Vector2(1, -1);
                inputCoord =  pos + new Vector2(0, 2);
                break;
            case 180:
                outputCoord = pos + new Vector2(2, 1);
                inputCoord = pos + new Vector2(-1, 0);
                break;
            case 270:
                outputCoord = pos + new Vector2(0, 2);
                inputCoord = pos + new Vector2(1, -1);
                break;
        }
    }
    void OnTick()
    {

        if (inputInv.items[0].ID == -1 || inputInv.items[0].count == 0)
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
            SetOutput();
            FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            SetOutput();
            FindObjectOfType<Buildings>().AllBuildings[2].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        }
        if (Input.GetKey(KeyCode.Delete))
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

        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltScript = cellObj.GetComponent<Belt>();
            Refinery refineryScript = cellObj.GetComponent<Refinery>();
            if (beltScript != null)
            {
                if (beltScript.inputItem(itemID, 0.5f))
                {
                    return true;
                }
            }
            else if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1,pos))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool InputItem(int ID, int count, Vector2 inPos)
    {
        if ((inPos - inputCoord).sqrMagnitude <= 0.05f)
        {
            return inputInv.AddItem(ID, count);
        }
        else
        {
            return false;
        }
    }
}
