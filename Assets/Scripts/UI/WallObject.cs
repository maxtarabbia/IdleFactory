using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    SpriteRenderer SR;
    public Material wallmat;
    public float dist;
    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        
        SR.material = wallmat;
        SR.material.SetFloat("_dist", dist);


    }

}
