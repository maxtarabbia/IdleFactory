using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    public AudioClip[] music;
    public float timeToPlay;

    public AudioMixerGroup AM;

    public int selectedSource;

    GameObject[] sources;
    // Start is called before the first frame update
    void Start()
    {
        sources = new GameObject[music.Length];
        for (int i = 0; i< music.Length; i++) 
        {
            sources[i] = new GameObject(music[i].name);
            sources[i].transform.parent = transform;

            AudioSource AS = sources[i].AddComponent<AudioSource>();
            AS.clip = music[i];

            AS.outputAudioMixerGroup = AM;
        }
        selectedSource = Random.Range(0,sources.Length);
        timeToPlay = Random.Range(10, 30);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        timeToPlay -= Time.fixedDeltaTime;

        if (timeToPlay > 0)
            return;

        selectedSource++;

        if (selectedSource >= sources.Length)
            selectedSource = 0;

        //selectedSource = Random.Range(0, music.Length - 1);
        sources[selectedSource].GetComponent<AudioSource>().Play();
        timeToPlay = Random.Range(10, 30) + sources[selectedSource].GetComponent<AudioSource>().clip.length;
    }
}
