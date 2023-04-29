using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkinButton : MonoBehaviour
{
    public TextMeshPro TMP;
    public Skin skin;
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = skin.Thumbnail;
        TMP.text = skin.name;
        GetComponent<SpriteRenderer>().material.SetFloat("_Gray", skin.isUnlocked ? 0 : 1);
    }
    private void OnMouseUp()
    {
        if (!skin.isUnlocked)
            return;

        FindObjectOfType<Skins>().ActivateSkin(skin);
    }
}
