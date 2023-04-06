using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class AudioManager : MonoBehaviour
{
    public AudioSource HoverSource;
    public AudioSource PlaySource;
    [SerializeField]
    bool isUnloaded;

    public AudioMixerGroup mixerGroup;
    public string[] audioParams;

    // Start is called before the first frame update
    void Start()
    {
        isUnloaded = false;
        DontDestroyOnLoad(this);
        SceneManager.sceneUnloaded += OnSceneUnload;


        for(int i = 0; i < audioParams.Length; i++)
        {
            mixerGroup.audioMixer.SetFloat(audioParams[i], math.remap(0, 3, -40, 20, PlayerPrefs.GetFloat(audioParams[i], 0f)));
        }
    }
    public void PlayHover()
    {
        HoverSource.Play();
    }
    public void PlayPlay()
    {
        PlaySource.Play();
    }
    void OnSceneUnload(Scene current)
    {
        isUnloaded = true;
    }
    private void Update()
    {
        if (!isUnloaded)
            return;
        if (!HoverSource.isPlaying && !PlaySource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
