using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class TickEvents : MonoBehaviour
{
    public delegate void MyEventHandler();
    public event MyEventHandler MyEvent;
    void FixedUpdate()
    {
        if(MyEvent != null)
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
            print("Tick jammed "+ iterations+ " times.");
        }
    }
}
