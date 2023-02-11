using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class hoveringSprites : MonoBehaviour
{
    public Vector2 size;
    public Sprite CornerSprite;
    GameObject[] sprites = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            InitializeSprite(i);
            sprites[i].transform.Rotate(new Vector3(0,0,i*90));
        }
        sprites[0].transform.position += new Vector3((size.x - 1) / -2, (size.y - 1)/2,0);
        sprites[1].transform.position += new Vector3((size.x - 1) / -2, (size.y - 1) / -2, 0);
        sprites[2].transform.position += new Vector3((size.x-1) / 2, (size.y - 1) / -2, 0);
        sprites[3].transform.position += new Vector3((size.x-1) / 2,( size.y - 1)/ 2, 0);
        
    }
    void InitializeSprite(int ID)
    {
        sprites[ID] = new GameObject();
        sprites[ID].transform.position = transform.position;
        sprites[ID].transform.parent = transform;
        SpriteRenderer SR = sprites[ID].AddComponent<SpriteRenderer>();
        SR.sprite = CornerSprite;

        SR.sortingLayerName = "UI";
        sprites[ID].SetActive(false);

    }
    private void OnMouseEnter()
    {
        foreach(GameObject sprite in sprites)
        {
            sprite.SetActive(true);
        }
    }
    private void OnMouseExit()
    {
        foreach (GameObject sprite in sprites)
        {
            sprite.SetActive(false);
        }
    }
}
