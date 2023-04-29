using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetKeybind : MonoBehaviour
{
    public string Control;
    bool Iswaiting = false;
    public TextMeshPro TMP;
    public string DefaultValue;

    void Start()
    {
        if(!PlayerPrefs.HasKey(Control))
        {
            PlayerPrefs.SetString(Control, DefaultValue);
        }
        TMP.text = PlayerPrefs.GetString(Control, DefaultValue);
        checkTextSize();
    }

    private void OnMouseOver()
    {
        if(Input.anyKeyDown && Iswaiting)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    TMP.text = key.ToString();
                    PlayerPrefs.SetString(Control, key.ToString());
                    checkTextSize();
                    Iswaiting = false;
                }
            }
        }
    }
    void checkTextSize()
    {
        TMP.fontSize = (TMP.text.Length > 7) ? 30 : 40;
    }
    public void Setwaiting()
    {
        Iswaiting= true;
    }
}
