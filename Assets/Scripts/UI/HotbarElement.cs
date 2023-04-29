using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HotbarElement : MonoBehaviour
{
    public GameObject sprite;
    public GameObject highlight;
    public TextMeshPro textMeshPro;

    public int ID;

    public string text;
    private void Start()
    {
        textMeshPro.text = text;
    }
    public void Highlight(bool active)
    {
        highlight.SetActive(active);
    }
    private void OnMouseDown()
    {
        FindObjectOfType<WorldGeneration>().setBuildableIndex(ID);
    }
}
