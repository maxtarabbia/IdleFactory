using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class MenuButton : MonoBehaviour
{
    public UnityEvent OnClick;
    SpriteRenderer SR;

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        setToDefault();
    }
    private void OnMouseEnter()
    {
        setToHovered();
    }
    private void OnMouseExit()
    {
        setToDefault();
    }
    private void OnMouseDown()
    {
        setToClicked();
    }
    private void OnMouseUp()
    {
        setToHovered();
        OnClick?.Invoke();
    }
    void setToHovered()
    {
        SR.material.SetColor("_MainColor", new Color(0.8f, 0.8f, 0.8f));
        SR.material.SetColor("_OutlineColor", new Color(0.3f, 0.3f, 0.3f));
        SR.material.SetFloat("_ShadowDist", 0.03f);
    }
    void setToClicked()
    {
        SR.material.SetColor("_MainColor", new Color(0.5f, 0.5f, 0.5f));
        SR.material.SetColor("_OutlineColor", new Color(0.2f, 0.2f, 0.2f));
        SR.material.SetFloat("_ShadowDist", 0.01f);
    }
    void setToDefault()
    {
        SR.material.SetColor("_MainColor", new Color(0.6f, 0.6f, 0.6f));
        SR.material.SetColor("_OutlineColor", new Color(0.2f, 0.2f, 0.2f));
        SR.material.SetFloat("_ShadowDist", 0.03f);
    }
}
