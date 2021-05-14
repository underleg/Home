using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckMB : InteractiveObjectMB
{
    public Animator m_animator;


    // Start is called before the first frame update
    override protected void Start()
    {
        m_collectable = true;
    }

    // Update is called once per frame
    override protected void Update()
    {
        if(UpdatePlayerState())
        {
            if(m_playerState == PlayerState.Player_inside_active_zone)
            {
                m_animator.Play("InteractiveShake");
            }
            else if (m_playerState == PlayerState.Player_inside_interactive_zone)
            {
                m_animator.Play("ActiveShake");
            }
            else
            {
                m_animator.Play("Idle");
            }
        }
    }
}
