using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingObject : MonoBehaviour
{
    Vector3 basePos;
    float baseValue;
    public SliderBar bar;
    private void Start()
    {
        transform.localPosition = new Vector3(Mathf.Clamp(bar.value*3, 0, 3), 0, -0.001f);
    }
    private void OnMouseDrag()
    {
        int width = Screen.currentResolution.width;
        width = Screen.mainWindowDisplayInfo.width;
        width = Screen.width;
        Debug.Log("width is: " + width);
        float calculatedValue = Mathf.Clamp(baseValue + (-basePos.x + Input.mousePosition.x) / (width/5.5f), 0, 1);
        bar.value = calculatedValue;
        transform.localPosition = new Vector3(calculatedValue*3, 0, -0.001f);
        bar.UpdatePos();
    }
    private void OnMouseDown()
    {
        baseValue = bar.value;
        basePos = Input.mousePosition;
    }

}
