using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[Serializable]
public class Keybind
{
    public string control;
    public string key;
}

public class ScrollingKeyBinds : MonoBehaviour
{
    public GameObject keybindPrefab;

    public Keybind[] binds;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<binds.Length; i++)
        {
            GameObject GM =  Instantiate(keybindPrefab, transform);
            GM.transform.localPosition += new Vector3(0, i * -0.5f + 2f, 0);
            GM.GetComponentInChildren<SetKeybind>().Control = binds[i].control;
            GM.GetComponentInChildren<SetKeybind>().DefaultValue = binds[i].key;
            GM.GetComponent<TextMeshPro>().text = binds[i].control;
        }
    }

    // Update is called once per frame
    private void OnMouseOver()
    {
        gameObject.transform.position += new Vector3(0, Input.mouseScrollDelta.x, 0);
    }
}
