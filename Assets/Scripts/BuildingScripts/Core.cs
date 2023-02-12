using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Core : MonoBehaviour
{
    Vector2 pos;
    WorldGeneration world;
    void Start()
    {
        pos = gameObject.transform.position;
        pos += new Vector2(-0.5f, -0.5f);
        world = FindObjectOfType<WorldGeneration>();
        FindObjectOfType<StateSaveLoad>().Save();
    }
    public void InputItem(int ID)
    {
        world.inv.AddItem(ID, 1);
    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            /*
            Buildings builds = FindObjectOfType<Buildings>();
            //world.inv.AddItem((int)builds.AllBuildings[4].cost[0].x, (int)builds.AllBuildings[4].cost[0].y);

            world.OccupiedCells.Remove(pos);
            world.OccupiedCells.Remove(pos + new Vector2(0, 1));
            world.OccupiedCells.Remove(pos + new Vector2(1, 0));
            world.OccupiedCells.Remove(pos + new Vector2(1, 1));

            builds.AllBuildings[4].count--;
            FindObjectOfType<StateSaveLoad>().Save();
            Destroy(gameObject);
            */
        }
    }
    public void Delete()
    {
        Buildings builds = FindObjectOfType<Buildings>();
        //world.inv.AddItem((int)builds.AllBuildings[4].cost[0].x, (int)builds.AllBuildings[4].cost[0].y);

        world.OccupiedCells.Remove(pos);
        world.OccupiedCells.Remove(pos + new Vector2(0, 1));
        world.OccupiedCells.Remove(pos + new Vector2(1, 0));
        world.OccupiedCells.Remove(pos + new Vector2(1, 1));

        builds.AllBuildings[4].count--;
        Destroy(gameObject);
    }
}
