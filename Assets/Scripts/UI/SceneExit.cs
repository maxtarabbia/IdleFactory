using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class SceneExit : MonoBehaviour
{
    string SceneToLoad;
    float progress = 0;
    SpriteRenderer LbSR;
    int Phase = 0;
    AsyncOperation asyncLoad;
    // Start is called before the first frame update
    void Awake()
    {
        SetLBMaterial();
        PlayerPrefs.SetInt("isLoaded", 0);
        DontDestroyOnLoad(transform.gameObject);
        SceneToLoad = PlayerPrefs.GetString("Level", "MainScene");
    }
    bool SetCam()
    {
        if (SceneToLoad == "MainScene")
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            Camera[] cam = FindObjectsOfType<Camera>();
            foreach (Camera c in cam)
            {
                if (c.transform.parent != null)
                    if (c.transform.parent.name == "MasterObject")
                    {
                        canvas.worldCamera = c;
                        return true;
                    }
            }

            return false;
        }
        return true;
    }
    void SetLBMaterial()
    {
        SpriteRenderer[] SRs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sr in SRs)
        {
            if(sr.name == "LoadingBar")
                LbSR = sr;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Phase == 0)
        {
            asyncLoad = SceneManager.LoadSceneAsync(SceneToLoad);
            Phase = 1;
        }
        if(Phase == 1)
        {
            if (asyncLoad.isDone)
                Phase = 2;
        }
        if (Phase == 2)
        {
            if (SetCam())
                Phase = 3;

        }
        if (SceneToLoad == "MainScene")
        { 
            StateSaveLoad SSL = FindObjectOfType<StateSaveLoad>();
            if (FindObjectOfType<WorldGeneration>())
            {
                if (SSL.ticksToJam > 0)
                {
                    progress = 1f - (float)SSL.ticksToJam / SSL.totalTicks;
                    if (LbSR != null)
                    {
                        float BarProg = 0.15f + progress * 0.85f;
                        LbSR.material.SetFloat("_LoadingProgress", BarProg);
                        //print(BarProg);
                    }

                }
                else if (SSL.ticksToJam == 0)
                {
                    PlayerPrefs.SetInt("isLoaded", 1);
                    Destroy(transform.gameObject);
                }
            }
        }
        else if(Phase== 3)
        {
            Destroy(transform.gameObject);
        }
    }
}
