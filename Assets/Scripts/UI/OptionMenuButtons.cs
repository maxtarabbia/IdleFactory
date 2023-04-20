using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuButtons : MonoBehaviour
{
    public GameObject AAButton;
    public GameObject keybinds;
    public GameObject AudioSliderPrefab;
    public GameObject SkinsPrefab;

    GameObject GraphicsButtons;
    GameObject keyBindButtons;
    GameObject AudioSliders;
    GameObject Skins;
    void Start()
    {
        AddAAOPtions();
    }
    public void AddKeybinds()
    {
        Destroy(AudioSliders);
        Destroy(keyBindButtons);
        Destroy(GraphicsButtons);
        Destroy(Skins);
        keyBindButtons = Instantiate(keybinds, transform);
        keyBindButtons.transform.localPosition = new Vector3(0.0f, -0.1f, -0.01f);
        keyBindButtons.transform.localScale *= 0.08f;
    }
    public void AddAAOPtions()
    {
        Destroy(AudioSliders);
        Destroy(GraphicsButtons);
        Destroy(keyBindButtons);
        Destroy(Skins);
        GraphicsButtons = Instantiate(AAButton, transform);
        GraphicsButtons.transform.localScale *= 0.09f;
        GraphicsButtons.transform.localPosition = new Vector3(0, -0.15f, -0.01f);
    }
    public void AddAudioSliders()
    {
        Destroy(AudioSliders);
        Destroy(GraphicsButtons);
        Destroy(keyBindButtons);
        Destroy(Skins);
        AudioSliders = Instantiate(AudioSliderPrefab, transform);
        AudioSliders.transform.localScale *= 0.08f;
        AudioSliders.transform.localPosition = new Vector3(-0.05f, -0.06f, -0.01f);
    }
    public void AddSkinOptions()
    {
        Destroy(AudioSliders);
        Destroy(GraphicsButtons);
        Destroy(keyBindButtons);
        Destroy(Skins);
        Skins = Instantiate(SkinsPrefab, transform);
        Skins.transform.localScale *= 0.09f;
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
