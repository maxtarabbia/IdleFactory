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

    public bool holdConfirmation = false;
    public float timeToHold;
    float timeHeld;
    void Start()
    {
        if(isGrayedOut)
            setToClicked();
        AudioManager = (AudioManager == null) ? FindObjectOfType<AudioManager>() : AudioManager;
        SR = GetComponent<SpriteRenderer>();
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
        if (holdConfirmation)
        {
            SR.material.SetFloat("_RotationProg", timeHeld / timeToHold);
            timeHeld = 0;
        }
    }
    private void OnMouseDown()
    {
        if (isGrayedOut)
            return;
        if(holdConfirmation)
            timeHeld = 0.001f;
        else
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
        if (!holdConfirmation)
        {
            OnClick?.Invoke();
        }
        else
        {
            timeHeld = 0;
            SR.material.SetFloat("_RotationProg", timeHeld / timeToHold);
        }
    }
    private void OnMouseOver()
    {
        if (!holdConfirmation)
            return;
        if (timeHeld > 0)
        {
            timeHeld += Time.deltaTime;
            SR.material.SetFloat("_RotationProg", timeHeld / timeToHold);
        }
        if(timeHeld >= timeToHold)
        {
            setToClicked();
            OnClick?.Invoke();
        }
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
