using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public GameObject InventoryMenu;
    private void OnMouseDown()
    {
        GameObject IM = GameObject.Find("World Inv(Clone)");
        if (IM == null)
        {
            Instantiate(InventoryMenu);
        }
        else
        {
            Destroy(IM);
        }
    }
}
