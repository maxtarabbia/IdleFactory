using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReceiver : MonoBehaviour
{

    Miner minerScript;
    Belt beltScript;
    Refinery refineryScript;
    Splitter splitterScript;
    Core coreScript;
    Assembler assemblerScript;
    UnderGroundBelt UBscript;
    public enum BuilableType
    {
        None,
        Miner,
        Belt,
        Refinery,
        Splitter,
        Core,
        Assembler,
        Underground
    }
    
    BuilableType type;
    public bool CanAcceptItem(int item, Vector2Int InputPos)
    {
        if (type == BuilableType.None)
        {
            if (TryGetComponent(out minerScript))
                type = BuilableType.Miner;
            if (TryGetComponent(out beltScript))
                type = BuilableType.Belt;
            if (TryGetComponent(out refineryScript))
                type = BuilableType.Refinery;
            if (TryGetComponent(out splitterScript))
                type = BuilableType.Splitter;
            if (TryGetComponent(out coreScript))
                type = BuilableType.Core;
            if (TryGetComponent(out assemblerScript))
                type = BuilableType.Assembler;
            if (TryGetComponent(out UBscript))
                type = BuilableType.Underground;
        }
        bool canOutput = false;
        switch (type)
        {
            case BuilableType.Miner:
                canOutput = false;
                break;
            case BuilableType.Belt:
                canOutput = beltScript.inputItem(item, InputPos);
                break;
            case BuilableType.Refinery:
                canOutput = refineryScript.InputItem(item, 1, InputPos);
                break;
            case BuilableType.Splitter:
                canOutput = splitterScript.inputItem(item, InputPos);
                break;
            case BuilableType.Core:
                canOutput = true;
                coreScript.InputItem(item);
                break;
            case BuilableType.Assembler:
                canOutput = assemblerScript.InputItem(item,1,InputPos);
                break;
            case BuilableType.Underground:
                canOutput = UBscript.inputItem(item, InputPos);
                break;
        }



        // Your code to check if the item can be accepted
        return canOutput;
    }

    public static bool CanObjectAcceptItem(GameObject obj, int item, Vector2Int InputPos)
    {
        ItemReceiver receiver = obj.GetComponent<ItemReceiver>();
        if (receiver != null)
        {
            return receiver.CanAcceptItem(item, InputPos);
        }
        return false;
    }
}
