using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuButtons : MonoBehaviour
{
    public GameObject FullscreenToggleButton;
    public GameObject AAButton;
    void Start()
    {
        AddFullScreenButton();
        AddAAOPtions();
    }
    void AddAAOPtions()
    {
        GameObject buttons = Instantiate(AAButton, transform);
        buttons.transform.localScale *= 0.1f;
        buttons.transform.localPosition = new Vector3(0, -0.1f, -0.01f);
    }
    void AddFullScreenButton()
    {
        GameObject button = Instantiate(FullscreenToggleButton,transform);
        button.transform.localPosition = new Vector3(0, 0.05f, -0.01f);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
    public void CloseMenu()
    {
        Destroy(transform.parent.transform.parent.gameObject);
    }
    private void OnMouseDown()
    {
        CloseMenu();
    }
}
