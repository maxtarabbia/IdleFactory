using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public GameObject PauseMenu;
    private void OnMouseUp()
    {
        Instantiate(PauseMenu);
    }
    public void DisplayeMenu()
    {
        //Instantiate(PauseMenu);
    }
}
