using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class SliderBar: MonoBehaviour
{
    public string AudioVariable = "MasterVol";

    public AudioMixerGroup group;

    public float value;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(AudioVariable))
        {
            group.audioMixer.GetFloat(AudioVariable, out value);
            value = math.remap(-80, 20, 0, 3, value);
        }
        else
        {
            value = PlayerPrefs.GetFloat(AudioVariable);
            group.audioMixer.SetFloat(AudioVariable, math.remap(0, 3, -80, 20, value));
        }
    }
    public void UpdatePos()
    {
        group.audioMixer.SetFloat(AudioVariable, math.remap(0,3,-80,20,value));
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(AudioVariable, value);
    }
}
