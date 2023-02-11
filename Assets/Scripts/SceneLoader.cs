using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string SceneToLoad;
    public void LoadMainScene()
    {

        print("attempting to Launch: " + SceneToLoad);


        SceneManager.LoadScene(SceneToLoad);

        SceneManager.UnloadSceneAsync("MenuScene");
    }
    private void OnMouseDown()
    {
            LoadMainScene();
    }
}
