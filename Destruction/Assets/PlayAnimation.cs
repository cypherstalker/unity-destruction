using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

public class PlayAnimation : MonoBehaviour
{
    [Tooltip("Amount of time to wait before playing animation")]
    public float startdelay = 2;
    [Tooltip("Name of animation to play")]
    public string animation_name;
    [Tooltip("Animator to use")]
    public Animator animator;
    [Tooltip("Layer to play animation on")]
    public int animation_layer;


    private float starttime = 0;
    private float endtime = 0;
    private bool finished = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        starttime = Time.realtimeSinceStartup;
    }


    // Update is called once per frame
    void Update()
    {
        if (!finished)
        {
            endtime = Time.realtimeSinceStartup;
            endtime = endtime - starttime;
            if (endtime > startdelay)
            {
                animator.enabled = true;
                animator.Play(animation_name, animation_layer);
                finished = true;
            }
        }
    }
}
