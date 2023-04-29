using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class MenuButtons : MonoBehaviour
{
    public UnityEvent OnClick;
    SpriteRenderer SR;

    AudioManager AudioManager;
    public bool playclick = true;

    public bool isGrayedOut = false;
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();

        AudioManager = (AudioManager == null) ? FindObjectOfType<AudioManager>() : AudioManager;
        if (isGrayedOut)
            setToClicked();
        else
            setToDefault();
    }
    private void OnMouseEnter()
    {
        if (isGrayedOut)
            return;
        AudioManager = (AudioManager == null) ? FindObjectOfType<AudioManager>() : AudioManager;
        AudioManager.PlayHover();
        setToHovered();
    }
    private void OnMouseExit()
    {
        if (isGrayedOut)
            return;
        setToDefault();
    }
    private void OnMouseDown()
    {
        if (isGrayedOut)
            return;
        setToClicked();
    }
    private void OnMouseUp()
    {
        if (isGrayedOut)
            return;
        AudioManager = FindObjectOfType<AudioManager>();
        if (playclick)
            AudioManager.PlayPlay();
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
        SR.material.SetColor("_MainColor", new Color(0.7f, 0.7f, 0.7f));
        SR.material.SetColor("_OutlineColor", new Color(0.2f, 0.2f, 0.2f));
        SR.material.SetFloat("_ShadowDist", 0.03f);
    }
    public void updateGraying()
    {
        if (isGrayedOut)
        {
            setToClicked();
        }
        else
        {
            setToDefault();
        }
    }
}
