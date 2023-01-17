using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public Dictionary<Vector2,Cell> worldmap;
    public Sprite sprite;
    public Vector2 pos;
    public int coveredTileID = -1;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Initialize()
    {
        World world = FindObjectOfType<World>();
        worldmap = world.map;
        SpriteRenderer SR = gameObject.AddComponent<SpriteRenderer>();

        gameObject.transform.localScale = Vector3.one * world.scale;
        SR.sprite = sprite;
        pos = gameObject.transform.position / world.scale;
        coveredTileID = world.map[pos].ID;
    }

    // Update is called once per frame
    void Update()
    {
        if(coveredTileID == -1)
        {
            Initialize();
        }
        print(coveredTileID);
    }
}
