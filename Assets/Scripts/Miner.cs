using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Miner : MonoBehaviour
{
    public Dictionary<Vector2,Cell> worldmap;
    public Sprite sprite;
    public Vector2 pos;
    public int coveredTileID = -1;
    SpriteRenderer spriteRenderer;
    VisualEffect effect;
    World world;
    float miningProgress;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Initialize()
    {
        world = FindObjectOfType<World>();
        worldmap = world.map;
        gameObject.transform.localScale = Vector3.one * world.scale;

        effect = GetComponent<VisualEffect>();
        

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 1;

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
        
    }
    private void FixedUpdate()
    {
        if (miningProgress >= 1)
        {
            MineItem();
            miningProgress = 0;
        }
        if (coveredTileID != 0)
        {
            miningProgress += 0.01f;
        }
        print(miningProgress);
    }
    void MineItem()
    {
        effect.Play();
    }
}
