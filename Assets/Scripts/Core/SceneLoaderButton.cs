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

        //print("attempting to Launch: " + SceneToLoad);
        PlayerPrefs.SetString("Level", SceneToLoad);

        SceneManager.LoadScene("LoadingScene");

    }
    public void NewGame()
    {
        string path = Application.persistentDataPath + "Assets/Saves";
        System.IO.File.Delete(path + "/Save1.dat");
        LoadMainScene();
    }
}
