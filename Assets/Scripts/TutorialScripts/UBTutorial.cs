using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UBTutorial : MonoBehaviour
{
    Vector3 basePos;
    public TextMeshPro TextBox;
    float timeSinceStart;
    public List<string> strings;
    public GameObject arrow;
    public bool doTutorial;

    TutorialState tutstate;

    int currentstate;
    // Start is called before the first frame update
    void Start()
    {
        tutstate = FindObjectOfType<TutorialState>();

        if (tutstate.currentState == TutorialState.State.Underground && tutstate.isTutorialActive == false)
            doTutorial = true;
        if (doTutorial)
        {
            arrow.SetActive(true);
            currentstate = 0;
            TextBox.gameObject.SetActive(true);
            tutstate.isTutorialActive = true;
        }
        basePos = arrow.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!doTutorial)
            return;


        TextBox.gameObject.transform.rotation = Quaternion.identity;

        arrow.transform.localPosition = basePos + new Vector3(Mathf.Sin(Time.time * 4) / 8, 0, 0);

        timeSinceStart += Time.deltaTime;

        switch (currentstate)
        {
            case 0:
                TextBox.text = strings[0];
                if (timeSinceStart > 3)
                {
                    currentstate = 1;
                    timeSinceStart = 0;
                    tutstate.isTutorialActive = true;
                }
                break;
            case 1:
                TextBox.text = strings[1];
                if (timeSinceStart > 3)
                {
                    currentstate = 2;
                    timeSinceStart = 0;
                }
                break;
            case 2:

                TextBox.text = strings[2];
                if (timeSinceStart > 3)
                {
                    currentstate = 5;
                    timeSinceStart = 0;
                }
                break;
            case 5:
                tutstate.setState(TutorialState.State.Inventory);
                tutstate.isTutorialActive = false;
                TextBox.text = string.Empty;
                arrow.SetActive(false);
                doTutorial = false;

                break;
        }

    }
    private void OnDestroy()
    {
        if (doTutorial)
            tutstate.isTutorialActive = false;
    }
}
