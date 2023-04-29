using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class TickEvents : MonoBehaviour
{
    public delegate void TickEventHandler();
    public event TickEventHandler MyEvent;
    private void Start()
    {
        Time.fixedDeltaTime = 0.1f;
    }
    void FixedUpdate()
    {

        if (MyEvent != null)
            MyEvent();
    }
    public void TickJam(int iterations)
    {
        if (MyEvent != null)
        {
            for(int i = 0; i < iterations; i++)
            {
                MyEvent();
                
            }
            //print("Tick jammed "+ iterations+ " times.");
        }
    }
    public void TickJam(long iterations)
    {
        if (MyEvent != null)
        {
            for (int i = 0; i < iterations; i++)
            {
                MyEvent();

            }
            //print("Tick jammed "+ iterations+ " times.");
        }
    }
}
