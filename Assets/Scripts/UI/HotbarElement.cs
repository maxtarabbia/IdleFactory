using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotbarElement : MonoBehaviour
{
    public GameObject sprite;
    public GameObject highlight;
    public TextMeshPro textMeshPro;

    public string text;
    private void Start()
    {
        textMeshPro.text = text;
    }
    public void Highlight(bool active)
    {
        highlight.SetActive(active);
    }
}
