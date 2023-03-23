using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarElement : MonoBehaviour
{
    public GameObject sprite;
    public GameObject highlight;
    public void Highlight(bool active)
    {
        highlight.SetActive(active);
    }
}
