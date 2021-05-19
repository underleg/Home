using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TelephoneMB : InteractiveObjectMB
{
    public Animator m_animator;


    // Start is called before the first frame update
    override protected void Start()
    {
    }

    // Update is called once per frame
    override protected void Update()
    {
        HandleShake(m_animator);       
    }
}

