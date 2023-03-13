using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetAA : MonoBehaviour
{
    public enum AAtype
    {
        None,
        FXAA,
        SMAA
    }
    public AAtype AA;
    public void activateAA()
    {
        var camdata = Camera.main.GetUniversalAdditionalCameraData();
        switch (AA)
        {
            case AAtype.None:
                camdata.antialiasing = AntialiasingMode.None;
                break;
            case AAtype.FXAA:
                camdata.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                break;
            case AAtype.SMAA:
                camdata.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                break;
        }

    }
}
