using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOptionsMenu : MonoBehaviour
{
    public int layerorder;
    public string RenderLayer;
    public GameObject OptionsMenuPrefab;

    public void OpenOptions()
    {
        GameObject Instance = Instantiate(OptionsMenuPrefab);


    }

}
