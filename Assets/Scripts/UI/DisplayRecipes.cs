using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayRecipes : MonoBehaviour
{
    public GameObject recipePrefab;
    WorldGeneration world;
    public Sprite singleArrow;
    public TextMeshPro TMP;
    public enum BuildingType
    {
        Refinery,
        Assembler,
        Miner,
        Belt
    }
    public BuildingType type;
    private void Start()
    {
        world = FindObjectOfType<WorldGeneration>();
        if(type == BuildingType.Refinery)
        {
            TMP.text = "Speed: " + IntLib.RoundToString(1 / world.speedstates.RefineryInfo.speed);
            Refinery assembler = GetComponentInParent<Refinery>();
            Refinery.Recipes rec = assembler.recipies;
            for (int i = 0; i < rec.values.Length; i ++)
            {
                GameObject recipe = Instantiate(recipePrefab, transform);
                recipe.transform.localPosition = new Vector3(0, 0.28f - i * 0.22f, 0);
                recipe.transform.localScale = Vector3.one * 0.17f;
                SingleRecipe SR = recipe.GetComponent<SingleRecipe>();
                SR.GetComponentInChildren<SpriteRenderer>().sprite = singleArrow;
                SR.In1.GetComponent<SpriteRenderer>().sprite = getSprite(rec.values[i].inputItemID);
                SR.In1.GetComponentInChildren<TextMeshPro>().text = getval(rec.values[i].inCount);
                SR.Out.GetComponent<SpriteRenderer>().sprite = getSprite(rec.values[i].outputItemID);
                SR.Out.GetComponentInChildren<TextMeshPro>().text = getval(rec.values[i].outCount);
            }
        }
        else if(type == BuildingType.Assembler)
        {
            TMP.text = "Speed: " + IntLib.RoundToString(1 / world.speedstates.RefineryInfo.speed);
            Assembler assembler = GetComponentInParent<Assembler>();
            Assembler.Recipes rec = assembler.recipies;
            for (int i = 0; i < rec.values.Length; i += 2)
            {
                GameObject recipe = Instantiate(recipePrefab, transform);
                recipe.transform.localPosition = new Vector3(0, 0.35f - i * 0.12f, 0);
                recipe.transform.localScale = Vector3.one * 0.17f;
                SingleRecipe SR = recipe.GetComponent<SingleRecipe>();
                SR.In1.GetComponent<SpriteRenderer>().sprite = getSprite(rec.values[i].inputItemID);
                SR.In1.GetComponentInChildren<TextMeshPro>().text = getval(rec.values[i].inCount);
                SR.In2.GetComponent<SpriteRenderer>().sprite = getSprite(rec.values[i].inputItemID2);
                SR.In2.GetComponentInChildren<TextMeshPro>().text = getval(rec.values[i].inCount2);
                SR.Out.GetComponent<SpriteRenderer>().sprite = getSprite(rec.values[i].outputItemID);
                SR.Out.GetComponentInChildren<TextMeshPro>().text = getval(rec.values[i].outCount);
            }
        }
        else if(type == BuildingType.Miner)
        {
            TMP.gameObject.transform.localPosition = new Vector3(0, 0,-0.0001f);
            GetComponent<SpriteRenderer>().material.SetVector("_Size", new Vector2(0.33f, 0.15f));
            TMP.text = "Speed: " + IntLib.RoundToString(1/world.speedstates.MinerInfo.speed);
        }
        else if(type == BuildingType.Belt)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Size", new Vector2(0.33f, 0.15f));
            TMP.gameObject.transform.localPosition = new Vector3(0, 0, -0.0001f);
            TMP.text = "Speed: " + IntLib.RoundToString(1/world.speedstates.BeltInfo.speed);
        }
    }
    Sprite getSprite(int ID)
    {
        if(ID == -1) return null;

        return world.items[ID].sprite;
    }
    string getval(int count)
    {
        if (count == 0) return string.Empty;
        return count.ToString();
    }
}
