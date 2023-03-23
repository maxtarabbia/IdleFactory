using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialState : MonoBehaviour
{
    public enum State
    {
        Miner,
        Belt,
        Refinery,
        Splitter,
        Assembler,
        Underground,
        Inventory,
        Done
    }
    public State currentState;
    public bool isTutorialActive;

    public TextMeshProUGUI textMeshProUGUI;
    private void Start()
    {
        setState(currentState);
    }
    public void setState(State state)
    {
        currentState = state;
        switch (state)
        {
            case State.Miner:
                textMeshProUGUI.text = "Use Mouse1 to place a Miner(1) on some iron. The \"(#)\" indicates the number key to select it";
                break;
            case State.Belt:
                textMeshProUGUI.text = "Place a belt(2). You'll have to mine some copper first. Use mouse2 to delete constructed buildings";
                break;
            case State.Refinery:
                textMeshProUGUI.text = "Place a refinery(3). It'll cost 5 iron and 5 copper";
                break;
            case State.Splitter:
                textMeshProUGUI.text = "Refine a few iron ingot and belt them into a HUB(5). then place a splitter(4)\nYou can hold CTRL and hover over buildings to select multiple and delete them all at once.";
                break;
            case State.Assembler:
                textMeshProUGUI.text = "You'll need some steel and copper ingots to make assemblers. once you've refined some steel by running iron ingots through a refinery, place an assembler(6)";
                break;
            case State.Underground:
                textMeshProUGUI.text = "Underground belts(7) will also require steel. place one down to see how it works";
                break;
            case State.Inventory:
                textMeshProUGUI.text = "You can use \"E\" to open the inventory where you can sell items and upgrade various buildings";
                break;
            case State.Done:
                textMeshProUGUI.text = "You have completed the base tutorial!";
                break;
        }
    }
}
