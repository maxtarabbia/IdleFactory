using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCam : MonoBehaviour
{
    bool FoundCam;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = FindObjectOfType<Camera>();
    }
    private void Update()
    {
        
    }


}
