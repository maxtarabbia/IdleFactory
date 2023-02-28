using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchVisualizer : MonoBehaviour
{
    public GameObject prefab;
    List<GameObject> sprites = new List<GameObject>();
    void Update()
    {
        foreach(GameObject sprite in sprites)
        {
            Destroy(sprite);
        }
        sprites.Clear();
        if(Input.touchCount > 0)
        {
            
            for(int i = 0; i < Input.touchCount; i++)
            {
                sprites.Add(Instantiate(prefab));
                sprites[i].transform.parent = transform;
                sprites[i].transform.localScale = Vector3.one * 60;
                RectTransform rect = sprites[i].GetComponent<RectTransform>();
                Vector2 pos = Input.touches[i].position;

                pos -= new Vector2(Screen.width /2, Screen.height /2);

                rect.localPosition= pos;
            }
        }

    }
}
