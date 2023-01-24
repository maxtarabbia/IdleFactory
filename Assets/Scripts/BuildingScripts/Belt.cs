using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Belt : MonoBehaviour
{
    WorldGeneration world;
    Vector2 pos;
    float timeTotravel = 1f;
    Vector2[] itemIDs;



    int capacity = 3;

    GameObject[] sprites;

    public Sprite[] spriteAssets;
    // Start is called before the first frame update
    void Start()
    {
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
            sprites[i].AddComponent<SpriteRenderer>();
            sprites[i].GetComponent<SpriteRenderer>().sortingLayerName = "Particles";

            itemIDs[i] = new Vector2(-1, 0);
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
                if(moveForward)
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
    public bool inputItem(int itemID, int Position)
    {
            if (itemIDs[Position].x == -1)
            {
                itemIDs[Position].y = (2 - Position) * 0.333f;
                itemIDs[Position].x = itemID;
                UpdateSpritePositions(false);


                return true;

            }
        return false;
    }
    bool OutputItem(int itemID)
    {
        Vector2 outputCoord = new Vector2();
        switch(gameObject.transform.rotation.eulerAngles.z)
        {

            case 0:
                outputCoord = pos + new Vector2(-1, 0);
                break;
            case 90:
                outputCoord = pos + new Vector2(0, -1);
                break;
            case 180:
                outputCoord = pos + new Vector2(1, 0);
                break;
            case 270:
                outputCoord = pos + new Vector2(0, 1);
                break;
        }
        GameObject cellObj = null;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltscript = cellObj.GetComponent<Belt>();
            Refinery refineryScript= cellObj.GetComponent<Refinery>();
            if (beltscript != null)
            {
                int spot = 2;
                if(Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 90 || Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 270)
                {
                    spot = 1;
                }
                else if(Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 180)
                {
                    spot= 0;
                }
                if (beltscript.inputItem(itemID, spot))
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
            print("Deleting Belt");
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[1].cost[0].x, (int)builds.AllBuildings[1].cost[0].y);

            world.OccupiedCells.Remove(pos);

            Destroy(gameObject);
        }
    }
}
