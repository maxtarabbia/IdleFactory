using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public void DestroyMenu()
    {
        Destroy(gameObject.transform.parent.transform.parent.gameObject);
    }
}
