using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuButtons : MonoBehaviour
{
    public GameObject FullscreenToggleButton;
    void Start()
    {
        AddFullScreenButton();
    }
    void AddFullScreenButton()
    {
        GameObject button = Instantiate(FullscreenToggleButton,transform);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(transform.parent.transform.parent.gameObject);
        }
    }
}
