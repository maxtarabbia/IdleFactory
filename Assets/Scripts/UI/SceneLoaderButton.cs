using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    [SerializeField]
    public string SceneString;
    public static string SceneToLoad; 
    private void Start()
    {
        SceneToLoad = SceneString;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    public void LoadMainScene()
    {
        PlayerPrefs.SetString("Level", SceneToLoad);
        SceneManager.LoadScene("LoadingScene");
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("Seed", new System.Random().Next(9999));
        string path = Application.persistentDataPath + "/Saves";
        System.IO.File.Delete(path + "/Save1.dat");
        LoadMainScene();
    }
}
