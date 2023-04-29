using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetCamAA : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetAA.AAtype AA = (SetAA.AAtype)PlayerPrefs.GetInt("AA");
        var camdata = GetComponent<Camera>().GetUniversalAdditionalCameraData();
        switch (AA)
        {
            case SetAA.AAtype.None:
                camdata.antialiasing = AntialiasingMode.None;
                break;
            case SetAA.AAtype.FXAA:
                camdata.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                break;
            case SetAA.AAtype.SMAA:
                camdata.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
        }
    }


}
