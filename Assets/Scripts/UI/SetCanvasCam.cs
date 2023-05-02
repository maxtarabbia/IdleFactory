using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCam : MonoBehaviour
{
    bool FoundCam;
    Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        if(canvas.isRootCanvas)
        {
            canvas.worldCamera = FindObjectOfType<Camera>();
        }
        else
        {
            canvas.overrideSorting = true;
        }

    }
    private void Update()
    {
        
    }


}
