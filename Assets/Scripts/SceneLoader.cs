using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    public string SceneString;
    public static string SceneToLoad; 
    private void Start()
    {
        SceneToLoad = SceneString;
    }
    public void LoadMainScene()
    {

        print("attempting to Launch: " + SceneToLoad);
        PlayerPrefs.SetString("Level", SceneToLoad);

        SceneManager.LoadScene("LoadingScene");

        SceneManager.UnloadSceneAsync("MenuScene");
    }
    public void NewGame()
    {
        string path = Application.persistentDataPath + "Assets/Saves";
        System.IO.File.Delete(path + "/Save1.dat");
        LoadMainScene();
    }
}
