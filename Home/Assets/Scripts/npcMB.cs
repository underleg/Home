using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcMB : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;
    // [SerializeField] private Rigidbody m_rigidBody = null;

    public enum NPCState
    {
        Undefined,
        Idle,
        Wave,
        Wander,
        Wander_thinking,
        Talking,
        Listening
    };

    NPCState m_state = NPCState.Undefined;

    float m_timer;

    float m_targetDirection = 180.0f;

    Node m_currentNode;
    Node m_targetNode;


    // Start is called before the first frame update
    void Start()
    {
        if (!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        
        InitState(NPCState.Idle);
        
    }


    void InitState(NPCState newState)
    {
        switch (newState)
        {
            case NPCState.Idle:
                m_animator.SetFloat("MoveSpeed", 0.0f);
                break;
            case NPCState.Wave:
                m_animator.SetFloat("MoveSpeed", 0.0f);
                m_timer = 3.0f; // 3 seconds 
                m_animator.SetTrigger("Wave");
                break;
            case NPCState.Wander:
                m_timer = 3.0f; // 3 seconds 
                m_animator.SetFloat("MoveSpeed", 0.2f);
                m_currentNode = GridMB.Instance.GetNodeFromPosition(transform.position);
                m_targetNode = GridMB.Instance.PickRandomNearbyOpenNode(m_currentNode, 1);
                break;
            default:
                break;
        }

        m_state = newState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (!m_animator) return;

        m_animator.SetBool("Grounded", true);

        int r;
        
        switch (m_state)
        {
            case NPCState.Idle:
                r = Random.Range(0, 60);
                if (r == 1)
                {
                    InitState(NPCState.Wave);
                }
                else if(r == 2)
                {
                    InitState(NPCState.Wander);
                }
                break;

            case NPCState.Wave:
                m_timer -= Time.deltaTime;
                if (m_timer < 0.0f) // todo - replace with events in animator
                {
                    InitState(NPCState.Idle);
                }
                break;

            case NPCState.Wander:       
                r = Random.Range(0, 60);
                if (r == 1)
                {
                    m_targetDirection += 30.0f;
                }
                else if(r == 2)
                {
                    m_targetDirection -= 30.0f;
                }

                m_timer -= Time.deltaTime;
                if (m_timer < 0.0f) // todo - replace with events in animator
                {
                    this.transform.position = this.m_targetNode.m_pos;
                    InitState(NPCState.Idle);
                }

                break;

            default:
                break;
        }

        HandleTurning();
    }




    void HandleTurning()
    {
        //m_animator.SetFloat("MoveSpeed", 0.2f);

        Vector3 playerRot = transform.localEulerAngles;

        float degrees = 0.0f;

        if (Mathf.Abs(playerRot.y - m_targetDirection) < 5.0f)
        {
            degrees = m_targetDirection - playerRot.y;
        }
        else if (playerRot.y < m_targetDirection)
        {
            degrees -= 10.0f * Time.deltaTime;
        }
        else if (playerRot.y > m_targetDirection)
        {
            degrees += 10.0f * Time.deltaTime;
        }

        transform.Rotate(0, degrees, 0);
    }

}
