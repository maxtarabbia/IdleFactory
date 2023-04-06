using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsRadioButtons : MonoBehaviour
{
    public OptionMenuButtons menuController;

    public void SetGraphics()
    {
        menuController.AddAAOPtions();
    }
    public void SetAudio()
    {
        menuController.AddAudioSliders();
    }
    public void SetKeybinds()
    {
        menuController.AddKeybinds();
    }
}
