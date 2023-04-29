using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class IntLib
{
    public static string RoundToString (float x)
    {
        float o =  Mathf.Round(x * 100)/100;
        return o.ToString();
    }
    public static string IntToString(int value)
    {
        string output = value.ToString();
        int leng = output.Length;
        if (leng > 9)
        {
            output = Math.Round(value/100_000_000f)/10f  + "B";
            return output;
        }
        if (leng > 6)
        {
            output = Math.Round(value / 100_000f) / 10f + "M";
            return output;
        }
        if (leng > 3)
        {
            output = Math.Round(value / 100f) / 10f + "K";
            return output;
        }

        return output;
    }
    public static string IntToString(long value)
    {
        string output = value.ToString();
        int leng = output.Length;
        if (leng > 9)
        {
            output = Math.Round(value / 100_000_000f) / 10f + "B";
            return output;
        }
        if (leng > 6)
        {
            output = Math.Round(value / 100_000f) / 10f + "M";
            return output;
        }
        if (leng > 3)
        {
            output = Math.Round(value / 100f) / 10f + "K";
            return output;
        }

        return output;
    }
}
