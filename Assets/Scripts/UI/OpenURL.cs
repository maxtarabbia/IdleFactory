using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class OpenURL : MonoBehaviour
{
    public string url;
    public void OpenULR()
    {
        if(SteamManager.Initialized)
        {
            //Steamworks.SteamUserStats.SetAchievement("");
        }
        Application.OpenURL(url);
    }
}
