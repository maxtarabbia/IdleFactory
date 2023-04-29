using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellButtonContainer : MonoBehaviour
{
    public ItemIconSeller IIS;
    private void OnMouseDown()
    {
        IIS.CloseMenu();
    }
}
