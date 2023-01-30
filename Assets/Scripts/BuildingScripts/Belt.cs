using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Belt : MonoBehaviour
{
    WorldGeneration world;
    Vector2 pos;
    float timeTotravel = 0.3f;
    Vector2 itemID;

    Vector2 outputCoord = new Vector2();

    GameObject sprite;

    public Sprite[] spriteAssets;

     public Sprite[] BeltRotations;
    // Start is called before the first frame update
    void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        pos = transform.position;
        //x is ID
        //y is time spent on belt


        sprite = new GameObject("Sprite");
        sprite.transform.position = pos + new Vector2(0, 0);
        sprite.transform.parent = gameObject.transform;
        sprite.transform.eulerAngles = (Vector3.zero);
        sprite.AddComponent<SpriteRenderer>();
        sprite.GetComponent<SpriteRenderer>().sortingLayerName = "Particles";

        itemID = new Vector2(-1, 0);
        UpdateBeltInput();
        UpdateAdjacentBelts();

       
    }
    public void UpdateAdjacentBelts()
    {
        GameObject[] cells = new GameObject[4];
        world.OccupiedCells.TryGetValue(pos + new Vector2(0, 1), out cells[0]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(0, -1), out cells[1]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(1, 0), out cells[2]);
        world.OccupiedCells.TryGetValue(pos + new Vector2(-1, 0), out cells[3]);

        foreach (GameObject cell in cells)
        {
            if (cell != null)
            {
                Belt belt;
                if (cell.TryGetComponent<Belt>(out belt))
                {
                    cell.GetComponent<Belt>().UpdateBeltInput();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       // UpdateSprites();
    }
    public void UpdateBeltInput()
    {
        Vector2 Offset = Vector2.zero;
        switch ((int)gameObject.transform.rotation.eulerAngles.z)
        {
            case 0:
                Offset = new Vector2(-1, 0);
                break;
            case 90:
                Offset = new Vector2(0, -1);
                break;
            case 180:
                Offset = new Vector2(1, 0);
                break;
            case 270:
                Offset = new Vector2(0, 1);
                break;
        }
        outputCoord = pos + new Vector2(Offset.x,Offset.y);
        GameObject rearObj;
        GameObject leftObj;
        GameObject rightObj;

        Vector2 location;
        location = (Vector2)(Quaternion.Euler(0, 0, 180) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out rearObj);
        location = (Vector2)(Quaternion.Euler(0, 0, 90) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out leftObj);
        location = (Vector2)(Quaternion.Euler(0, 0, 270) * Offset) + pos;
        location.x = Mathf.Round(location.x); location.y = Mathf.Round(location.y);
        world.OccupiedCells.TryGetValue(location, out rightObj);

        bool rearOcc = false;
        bool leftOcc = false;
        bool rightOcc = false;

        if (rearObj != null)
            rearOcc = rearObj.GetComponent<Belt>() != null || rearObj.GetComponent<Splitter>() != null;
        if (leftObj != null)
            leftOcc = leftObj.GetComponent<Belt>() != null || leftObj.GetComponent<Splitter>() != null;
        if(rightObj != null)
            rightOcc = rightObj.GetComponent<Belt>() != null || rightObj.GetComponent<Splitter>() != null;

        if (rearOcc && rearObj.GetComponent<Belt>() != null)
            rearOcc = rearObj.GetComponent<Belt>().outputCoord == pos;
        if (leftOcc && leftObj.GetComponent<Belt>() != null)
            leftOcc = leftObj.GetComponent<Belt>().outputCoord == pos;
        if (rightOcc && rightObj.GetComponent<Belt>() != null)
            rightOcc = rightObj.GetComponent<Belt>().outputCoord == pos;


        if (!rearOcc && (leftOcc ^ rightOcc))
        {
            if (rightOcc) 
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[1];
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[2];
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = BeltRotations[0];
        }


    }
    private void FixedUpdate()
    {
        UpdateSpritePositions(true);
    }
    void UpdateSpritePositions(bool moveForward)
    {
        if (itemID.x != -1)
        {
            if (moveForward)
                itemID.y += Time.fixedDeltaTime;
            sprite.GetComponent<SpriteRenderer>().sprite = spriteAssets[(int)itemID.x - 1];
            sprite.transform.localPosition = new Vector3(0.5f - (itemID.y / timeTotravel), 0, itemID.y);
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().sprite = null;
        }
        if (itemID.y >= timeTotravel)
        {
            if (OutputItem((int)itemID.x))
            {
                sprite.GetComponent<SpriteRenderer>().sprite = null;
                itemID = new Vector2(-1, 0);
            }
            else
            {
                itemID.y = timeTotravel;
            }
        }
    }
    public bool inputItem(int initemID, float time)
    {
            if (itemID.x == -1)
            {
                itemID.y = time * timeTotravel;
                itemID.x = initemID;
                UpdateSpritePositions(false);


                return true;

            }
        return false;
    }
    bool OutputItem(int itemID)
    {

        GameObject cellObj;
        world.OccupiedCells.TryGetValue(outputCoord, out cellObj);
        if (cellObj != null)
        {
            Belt beltscript = cellObj.GetComponent<Belt>();
            Refinery refineryScript= cellObj.GetComponent<Refinery>();
            Splitter splitter = cellObj.GetComponent<Splitter>();
            Core core = cellObj.GetComponent<Core>();
            if (beltscript != null)
            {
                float spot = 0;
                if(Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 90 || Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 270)
                {
                    spot = 0.3f;
                }
                else if(Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 180)
                {
                    spot= 0.9f;
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
            else if (splitter != null)
            {
                if (Mathf.Abs(cellObj.transform.rotation.eulerAngles.z - gameObject.transform.rotation.eulerAngles.z) == 0)
                {
                    if (splitter.inputItem(itemID,0))
                    {
                        return true;
                    }
                }

            }
            else if (core != null)
            {
                core.InputItem(itemID);
                return true;
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
            UpdateBeltInput();
            UpdateAdjacentBelts();

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
            FixRotations();
            UpdateBeltInput();
            UpdateAdjacentBelts();
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            Buildings builds = FindObjectOfType<Buildings>();
            world.inv.AddItem((int)builds.AllBuildings[1].cost[0].x, (int)builds.AllBuildings[1].cost[0].y);

            world.OccupiedCells.Remove(pos);

            Destroy(gameObject);
        }
    }
    void FixRotations()
    {
        FindObjectOfType<Buildings>().AllBuildings[1].rotation = (int)gameObject.transform.rotation.eulerAngles.z;
        sprite.transform.eulerAngles = (Vector3.zero);
    }
}
