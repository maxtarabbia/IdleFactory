using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour
{
    WorldGeneration world;
    Vector2 pos;
    float timeTotravel = 1f;
    Vector2[] itemIDs;

    Vector2[] OutputPos;
    int OutIter = 0;

    int capacity = 3;

    GameObject[] sprites;

    public Sprite[] spriteAssets;
    // Start is called before the first frame update
    void Start()
    {
        OutputPos = new Vector2[3];
        world = FindObjectOfType<WorldGeneration>();
        pos = transform.position;
        //x is ID
        //y is time spent on belt
        itemIDs = new Vector2[capacity];
        sprites = new GameObject[capacity];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = new GameObject("Sprite " + i);
            sprites[i].transform.position = pos + new Vector2(-0.33333f + 0.3f * i, 0);
            sprites[i].transform.parent = gameObject.transform;
            sprites[i].transform.eulerAngles = (Vector3.zero);
            sprites[i].AddComponent<SpriteRenderer>();
            sprites[i].GetComponent<SpriteRenderer>().sortingLayerName = "Particles";

            itemIDs[i] = new Vector2(-1, 0);
        }
        sprites[1].GetComponent<SpriteRenderer>().enabled = false;
        sprites[0].GetComponent<SpriteRenderer>().enabled = false;
        FixOutputs();
    }
    void FixOutputs()
    {
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {

            case 0:
                OutputPos[0] = pos + new Vector2(0, 1);
                OutputPos[1] = pos + new Vector2(-1, 0);
                OutputPos[2] = pos + new Vector2(0, -1);
                break;
            case 90:
                OutputPos[0] = pos + new Vector2(-1, 0);
                OutputPos[1] = pos + new Vector2(0, -1);
                OutputPos[2] = pos + new Vector2(1, 0);
                break;
            case 180:
                OutputPos[0] = pos + new Vector2(0, -1);
                OutputPos[1] = pos + new Vector2(1, 0);
                OutputPos[2] = pos + new Vector2(0, 1);
                break;
            case 270:
                OutputPos[0] = pos + new Vector2(1, 0);
                OutputPos[1] = pos + new Vector2(0, 1);
                OutputPos[2] = pos + new Vector2(-1, 0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprites();
    }
    void UpdateSprites()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (itemIDs[i].x != -1)
            {
                sprites[i].GetComponent<SpriteRenderer>().sprite = spriteAssets[(int)itemIDs[i].x - 1];

                sprites[i].transform.localPosition = new Vector2(0.65f + -itemIDs[i].y, 0);
            }
        }
    }
    private void FixedUpdate()
    {
        UpdateSpritePositions(true);
    }
    void UpdateSpritePositions(bool moveForward)
    {
        for (int i = 0; i < itemIDs.Length; i++)
        {
            if (itemIDs[i].x != -1)
            {
                sprites[i].GetComponent<SpriteRenderer>().sprite = spriteAssets[(int)itemIDs[i].x - 1];
                if (moveForward)
                    itemIDs[i].y += Time.fixedDeltaTime;
                if (itemIDs[i].y > 1 - i * 0.33333f * timeTotravel)
                {
                    if (i != 0 && itemIDs[i - 1].x == -1)
                    {
                        itemIDs[i - 1] = itemIDs[i];
                        itemIDs[i] = new Vector2(-1, 0);
                        UpdateSprites();
                    }
                    else
                    {
                        itemIDs[i].y = 1 - i * 0.333333f * timeTotravel;
                    }
                }



            }
            else
            {
                sprites[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        if (itemIDs[0].y >= timeTotravel)
        {
            if (OutputItem((int)itemIDs[0].x))
            {
                ShiftForward();
                sprites[0].GetComponent<SpriteRenderer>().sprite = null;
            }
            else
            {
                itemIDs[0].y = timeTotravel;
            }
        }
    }
    void ShiftForward()
    {
        Vector2[] oldIDs = itemIDs;
        itemIDs[0] = oldIDs[1];
        itemIDs[1] = oldIDs[2];
        itemIDs[2] = new Vector2(-1, 0);
    }
    public bool inputItem(int itemID)
    {
        if (itemIDs[2].x == -1)
        {
            itemIDs[2].y = 0;
            itemIDs[2].x = itemID;
            UpdateSpritePositions(false);
            return true;

        }
        return false;
    }
    bool OutputItem(int itemID)
    {
        OutIter += (int)Random.Range(-0.5f, 1.49999f);
        if(OutIter >= 3)
        {
            OutIter -= 3;
        }

        Vector2 outputCoord = OutputPos[OutIter];
        
        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltscript = cellObj.GetComponent<Belt>();
            Refinery refineryScript = cellObj.GetComponent<Refinery>();
            if (beltscript != null)
            {
                int spot = 2;
                if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 90 || Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 270)
                {
                    spot = 1;
                }
                else if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 180)
                {
                    spot = 0;
                }
                if (beltscript.inputItem(itemID, spot))
                {
                    OutIter++;
                    return true;
                }
            }
            else if (refineryScript != null)
            {
                if (refineryScript.InputItem(itemID, 1))
                {
                    OutIter++;
                    return true;
                }
            }
        }
        return false;

    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, -90f));
            FixRotations();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            FixRotations();
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[3].cost[0].x, (int)builds.AllBuildings[3].cost[0].y);

            world.OccupiedCells.Remove(pos);

            Destroy(gameObject);
        }
    }
    void FixRotations()
    {
        FindObjectOfType<Buildings>().AllBuildings[3].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].transform.eulerAngles = (Vector3.zero);
        }
        FixOutputs();
    }
}
