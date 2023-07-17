using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public static class SteamAchievements
{
    public static bool IsSteamInitialized()
    {
        return SteamManager.Initialized;
    }

    public static bool SetStat(string Statname, int amount)
    {
        if(!SteamManager.Initialized)
            return false;

        int CurrentStat;
        if(!SteamUserStats.GetStat(Statname, out CurrentStat))
            return false;

        if(CurrentStat < amount)
            SteamUserStats.SetStat(Statname,amount);

        SteamUserStats.StoreStats();
        return true;
    }
    public static bool AchievementReached(string AchName)
    {
        if (!SteamManager.Initialized)
            return false;

        bool isCompleted;
        if(!SteamUserStats.GetAchievement(AchName, out isCompleted))
            return false;

        if (!isCompleted)
            SteamUserStats.SetAchievement(AchName);

        SteamUserStats.StoreStats();
        return true;
    }

}
