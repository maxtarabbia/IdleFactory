using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomConsole
{
    public class Console
    {
        
        public static void Print(string message)
        {
            TutorialState TS = Object.FindObjectOfType<TutorialState>();
            TS.textMeshProUGUI.text = message;
        }
    }
}
