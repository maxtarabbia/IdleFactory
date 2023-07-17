using System.Collections;
using System.IO;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{

    public void Toggle()
    {
        Screen.fullScreen = !Screen.fullScreen;
        print("toggled");
    }
}
