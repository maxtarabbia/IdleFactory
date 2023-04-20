using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skins : MonoBehaviour
{
    public Skin[] allSkins;
    public static void ActivateSkin(Skin skin, GameObject go)
    {

       

        switch (skin.type)
        {
            case (Skin.SkinType.Miner):
                Miner miner = go.GetComponent<Miner>();

                miner.Frame.GetComponent<SpriteRenderer>().sprite = skin.sprites[0];
                miner.Drill.GetComponent<SpriteRenderer>().sprite = skin.sprites[1];
                miner.Hopper.GetComponent<SpriteRenderer>().sprite = skin.sprites[2];
                
                break;
            case (Skin.SkinType.Refinery):
                Refinery refinery = go.GetComponent<Refinery>();

                refinery.RefBase.GetComponent<SpriteRenderer>().sprite = skin.sprites[0];
                refinery.RefElements.GetComponent<SpriteRenderer>().sprite = skin.sprites[1];
                refinery.SmokeStack.GetComponent<SpriteRenderer>().sprite = skin.sprites[2];
                
                break;
            case (Skin.SkinType.Belt):
                Belt belt = go.GetComponent<Belt>();
                Splitter splitter = go.GetComponent<Splitter>();
                UnderGroundBelt UB = go.GetComponent<UnderGroundBelt>();
                if (belt != null)
                {
                    belt.BeltRotations[0] = skin.sprites[0];
                    belt.BeltRotations[1] = skin.sprites[1];
                    belt.BeltRotations[2] = skin.sprites[2];
                    belt.UpdateBeltInput();
                }
                if(splitter != null)
                {
                    splitter.GetComponent<SpriteRenderer>().sprite = skin.sprites[3];
                    splitter.splitterTop.GetComponent<SpriteRenderer>().sprite = skin.sprites[4];
                }
                if(UB != null)
                {
                    UB.GetComponent<SpriteRenderer>().sprite = skin.sprites[5];
                    UB.InputTop.GetComponent<SpriteRenderer>().sprite = skin.sprites[6];
                    UB.OutputGO.GetComponent<SpriteRenderer>().sprite = skin.sprites[7];
                    UB.OutputTop.GetComponent<SpriteRenderer>().sprite= skin.sprites[8];
                }
                
                break;
            case (Skin.SkinType.Assembler):
                Assembler assembler = go.GetComponent<Assembler>();
                assembler.GetComponent<SpriteRenderer>().sprite = skin.sprites[0];
                
                break;
        }
    }
    public void SetSelected(bool[] booleans)
    {
        for(int i =0; i< allSkins.Length; i++)
        {
            if(booleans.Length >= i)
            {
                allSkins[i].isSelected = booleans[i];
            }
        }
    }
    public void ActivateSkin(Skin skin)
    {


        foreach(Skin skinToDesel in allSkins)
        {
            if(skinToDesel.type == skin.type)
            {
                skinToDesel.isSelected = false;
            }
        }

        skin.isSelected = true;

        switch (skin.type)
        {
            case (Skin.SkinType.Miner):
                foreach (Miner miner in FindObjectsOfType<Miner>())
                {
                    ActivateSkin(skin, miner.gameObject);
                }
                break;
            case (Skin.SkinType.Refinery):
                foreach (Refinery refinery in FindObjectsOfType<Refinery>())
                {
                    ActivateSkin(skin, refinery.gameObject);
                }
                break;
            case (Skin.SkinType.Belt):
                
                foreach (Belt belt in FindObjectsOfType<Belt>())
                {
                    ActivateSkin(skin, belt.gameObject);
                }
                foreach (Splitter splitter in FindObjectsOfType<Splitter>())
                {
                    ActivateSkin(skin, splitter.gameObject);
                }
                foreach (UnderGroundBelt UGB in FindObjectsOfType<UnderGroundBelt>())
                {
                    ActivateSkin(skin, UGB.gameObject);
                }
                break;
            case (Skin.SkinType.Assembler):
                foreach (Assembler assembler in FindObjectsOfType<Assembler>())
                {
                    ActivateSkin(skin, assembler.gameObject);
                }
                break;
        }
    }
    public void Setskin(Skin.SkinType type, GameObject go)
    {
        for(int i = 0; i < allSkins.Length; i++)
        {
            if (allSkins[i].type == type && allSkins[i].isSelected)
                ActivateSkin(allSkins[i], go);
        }
    }
}
[Serializable]
public class Skin
{
    public string name;
    public Sprite Thumbnail;

    public Sprite[] sprites;
    public enum SkinType{
        Miner,
        Refinery,
        Belt,
        Assembler
    }
    public SkinType type;

    public bool isSelected;
    public bool isUnlocked;
}
