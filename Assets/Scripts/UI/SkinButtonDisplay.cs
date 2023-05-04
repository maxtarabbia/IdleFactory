using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinButtonDisplay : MonoBehaviour
{
    public Skin.SkinType skinType;
    public GameObject SkinButtonPrefab;
    Skin[] skins;
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<Skins>() == null)
            return;
        skins = FindObjectOfType<Skins>().allSkins.Where(obj => obj.type == skinType).ToArray();
        for(int i =0; i <skins.Length; i++)
        {
            GameObject skinBut = Instantiate(SkinButtonPrefab, transform);
            skinBut.GetComponentInChildren<SkinButton>().skin = skins[i];
            skinBut.transform.localScale = Vector3.one * 0.9f;
            skinBut.transform.localPosition = new Vector3(skins.Length * -0.6f + i * 1.2f + 0.6f, -1f, -0.001f);
        }
    }

}
