using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialState : MonoBehaviour
{
    float timeSinceEnd;
    public bool isoverRide = false;
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
                textMeshProUGUI.text = "Use Mouse1 to place a Miner on some iron. The \"(#)\" indicates the number key to select it. \nuse WASD to move around and scroll wheel to zoom";
                break;
            case State.Belt:
                textMeshProUGUI.text = "Place a belt. You'll have to mine some copper first. Use mouse2 to delete constructed buildings";
                break;
            case State.Refinery:
                textMeshProUGUI.text = "Place a refinery. The cost is displayed in the top left next to the item quantities";
                break;
            case State.Splitter:
                textMeshProUGUI.text = "Refine a few iron ingot and belt them into a HUB. then place a splitter\nYou can hold CTRL and hover over buildings to select multiple and delete them all at once.";
                break;
            case State.Assembler:
                textMeshProUGUI.text = "You'll need some steel and copper ingots to make assemblers. once you've refined some steel by running iron ingots through a refinery, place an assembler";
                break;
            case State.Underground:
                textMeshProUGUI.text = "Underground belts will also require steel. place one down to see how it works";
                break;
            case State.Inventory:
                textMeshProUGUI.text = "You can use \"E\" to open the inventory where you can sell items and upgrade various buildings";
                break;
            case State.Done:
                textMeshProUGUI.text = "You have completed the base tutorial!";
                timeSinceEnd = 0;
                break;
        }
    }
    private void Update()
    {
        if(currentState == State.Done && !isoverRide)
        {
            timeSinceEnd += Time.deltaTime;
            if (timeSinceEnd > 5)
                textMeshProUGUI.text = string.Empty;
        }
    }
}
