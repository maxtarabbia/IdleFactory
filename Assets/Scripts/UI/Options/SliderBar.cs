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
            value = Mathf.Pow(math.remap(-80, 0, 0, 1,value), 2);
        }
        else
        {
            value = PlayerPrefs.GetFloat(AudioVariable);
            group.audioMixer.SetFloat(AudioVariable, math.remap(0, 1, -80, 0, Mathf.Sqrt(value)));
        }
    }
    public void UpdatePos()
    {
        group.audioMixer.SetFloat(AudioVariable, math.remap(0,1,-80,0,Mathf.Sqrt(value)));
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(AudioVariable, value);
    }
}
