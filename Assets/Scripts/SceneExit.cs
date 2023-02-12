using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExit : MonoBehaviour
{
    string SceneToLoad;
    float progress = 0;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerPrefs.SetInt("isLoaded", 0);
        DontDestroyOnLoad(transform.gameObject);
        SceneToLoad = PlayerPrefs.GetString("Level", "MainScene");
        SceneManager.LoadSceneAsync(SceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        StateSaveLoad SSL = FindObjectOfType<StateSaveLoad>();
        if (FindObjectOfType<WorldGeneration>())
        {
            if(SSL.ticksToJam > 0)
            {
                progress = (float)SSL.ticksToJam / SSL.totalTicks;
                print(progress);
            }
            else if(SSL.ticksToJam == 0)
            {
                PlayerPrefs.SetInt("isLoaded", 1);
                Destroy(transform.gameObject);
            }
        }
    }
}
