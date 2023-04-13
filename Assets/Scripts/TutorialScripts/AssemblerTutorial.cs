using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssemblerTutorial : MonoBehaviour
{
    Vector3 basePos;
    public TextMeshPro TextBox;
    float timeSinceState;
    float timeSinceStart;
    public List<string> strings;
    public bool doTutorial;
    public GameObject arrow;

    TutorialState tutstate;

    int currentstate;
    // Start is called before the first frame update
    void Start()
    {
        tutstate = FindObjectOfType<TutorialState>();

        if (tutstate.currentState == TutorialState.State.Assembler && tutstate.isTutorialActive == false)
            doTutorial = true;
        if (doTutorial)
        {
            currentstate = 0;
            TextBox.gameObject.SetActive(true);
            tutstate.isTutorialActive = true;
        }
        arrow.SetActive(true);
        basePos = arrow.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if (!doTutorial)
        {
            if (timeSinceStart >= 2)
                arrow.SetActive(false);
            else
                arrow.transform.localPosition = basePos + new Vector3(Mathf.Sin(Time.time * 4) / 12, 0, 0);
            return;
        }

        arrow.transform.localPosition = basePos + new Vector3( Mathf.Sin(Time.time * 4) / 12, 0,0);

        TextBox.gameObject.transform.rotation = Quaternion.identity;

        timeSinceState += Time.deltaTime;

        switch (currentstate)
        {
            case 0:
                TextBox.text = strings[0];
                if (timeSinceState > 5)
                {
                    currentstate = 1;
                    timeSinceState = 0;
                    tutstate.isTutorialActive = true;
                }
                break;
            case 1:
                TextBox.text = strings[1];
                if (timeSinceState > 5)
                {
                    currentstate = 2;
                    timeSinceState = 0;
                }
                break;
            case 2:

                TextBox.text = strings[2];
                if (timeSinceState > 5)
                {
                    currentstate = 5;
                    timeSinceState = 0;
                }
                break;
            case 5:
                tutstate.setState(TutorialState.State.Underground);
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
