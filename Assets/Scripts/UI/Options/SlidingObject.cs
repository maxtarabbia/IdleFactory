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
        transform.localPosition = new Vector3(Mathf.Clamp(bar.value, 0, 3), 0, -0.001f);
    }
    private void OnMouseDrag()
    {
        float calculatedValue = Mathf.Clamp(baseValue + (-basePos.x + Input.mousePosition.x) / 100, 0, 3);
        bar.value = calculatedValue;
        transform.localPosition = new Vector3(calculatedValue, 0, -0.001f);
        bar.UpdatePos();
    }
    private void OnMouseDown()
    {
        baseValue = bar.value;
        basePos = Input.mousePosition;
    }

}
