using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    StateSaveLoad SSL;

    public TextMeshProUGUI textMeshProUGUI;
    int Minutes;
    int Hours;
    // Start is called before the first frame update
    void Awake()
    {
        
        SetLBMaterial();
        PlayerPrefs.SetInt("isLoaded", 0);
        DontDestroyOnLoad(transform.gameObject);
        SceneToLoad = PlayerPrefs.GetString("Level", "MainScene");
    }
    void SetMinHours(int ticks)
    {
        int seconds = Mathf.RoundToInt(SSL.totalTicks * Time.fixedDeltaTime);
        Minutes = (seconds / 60)%60;
        Hours = (seconds / 60) / 60;
        textMeshProUGUI.text = "Simulating " + Hours + ":" + Minutes.ToString().PadLeft(2,'0');
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
            if(SSL == null)
            {
                SSL = FindObjectOfType<StateSaveLoad>();
            }
            if (FindObjectOfType<WorldGeneration>())
            {
                if(textMeshProUGUI.text == "")
                    SetMinHours((int)SSL.totalTicks);
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
