using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Canvas>().worldCamera = FindObjectOfType<Camera>();
    }

}
