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
        Head_to_Target,
        Talking,
        Listening
    };

    NPCState m_state = NPCState.Undefined;
    int m_substate = 0;

    float m_timer;

    float m_targetDirection = 180.0f;

    Node m_currentNode;
    Node m_targetNode;

    List<Node> m_path;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_animator) 
        { 
            m_animator = gameObject.GetComponent<Animator>(); 
        }
        
        InitState(NPCState.Idle);
        
    }

    // Initialise local state
    void InitState(NPCState newState)
    {
        m_state = newState;
        m_substate = 0;

        switch (newState)
        {
            case NPCState.Idle:
                InitIdle();
                break;

            case NPCState.Wave:
                InitWave();
                break;

            case NPCState.Wander:
                InitWander();
                break;

            case NPCState.Head_to_Target:
                InitHeadToTarget();
                break;

            default:
                break;
        }    
    }

    private void FixedUpdate()
    {
        if (!m_animator) return;

        m_animator.SetBool("Grounded", true);

        switch (m_state)
        {
            case NPCState.Idle:
                HandleIdle();
                break;

            case NPCState.Wave:
                HandleWave();
                break;

            case NPCState.Wander:
                HandleWander();
                break;

            case NPCState.Head_to_Target:
                HandleHeadToTarget();
                break;

            default:
                break;
        }

        HandleTurning();
    }

    /// WANDER STATE ///
    private void InitWander()
    {
        m_timer = 3.0f; // 3 seconds 
        m_animator.SetFloat("MoveSpeed", 0.4f);
        m_currentNode = GridMB.Instance.GetNodeFromPosition(transform.position);
        m_targetNode = GridMB.Instance.PickRandomNearbyOpenNode(m_currentNode, 1);

        // have we moved?
        if (m_currentNode == m_targetNode)
        {
            InitState(NPCState.Idle);
        }
        else
        {
            transform.LookAt(m_targetNode.m_pos);
        }
    }

    private void HandleWander()
    {
        m_timer -= Time.deltaTime;
        if (m_timer < 0.0f) // todo - replace with events in animator
        {
            this.transform.position = this.m_targetNode.m_pos;
            InitState(NPCState.Idle);
        }
    }

    /// HEAD TO TARGET STATE ///
    private void InitHeadToTarget()
    {
        GameObject tgt = GameObject.Find("TestTarget");

        float dbp = Utils.DistBetweenPoints(transform.position, tgt.transform.position);
        if ( dbp < Consts.closeEnough)
        {
            InitState(NPCState.Idle);
        }
        else
        {
            m_path = PathFinderMB.Instance.FindPath(transform.position, tgt.transform.position);
        }
    }

    private void HandleHeadToTarget()
    {
        // 0 = turn to node
        switch (m_substate)
        {
            case 0: // turn and look
                m_animator.SetFloat("MoveSpeed", 0.4f);
                transform.LookAt(m_path[0].m_pos);
                m_timer = 1.0f;
                m_substate++;
                break;

            case 1: // wait
                m_timer -= Time.deltaTime;
                if (m_timer < 0.0f) // todo - replace with events in animator
                {
                    m_substate++;
                }
                break;

            case 2: // move
                this.transform.position = m_path[0].m_pos;
                if (AtTargetNode(m_path[0]))
                {
                    this.transform.position = m_path[0].m_pos;
                    m_path.RemoveAt(0);

                    if (m_path.Count == 0) // arrived
                    {
                        InitState(NPCState.Idle);
                    }
                    else
                    {
                        m_substate = 0;
                    }
                }
                break;

            default:
                Debug.LogError("bad state");
                m_substate = 0;
                break;
        }
    }


    /// WAVE STATE ///
    private void InitWave()
    {
        m_animator.SetFloat("MoveSpeed", 0.0f);
        m_timer = 3.0f; // 3 seconds 
        m_animator.SetTrigger("Wave");
    }

    void HandleWave()
    {
        m_timer -= Time.deltaTime;
        if (m_timer < 0.0f) // todo - replace with events in animator
        {
            InitState(NPCState.Idle);
        }
    }

    /// IDLE STATE ///
    private void InitIdle()
    {
        m_animator.SetFloat("MoveSpeed", 0.0f);
    }

    void HandleIdle()
    {
        int r = Random.Range(0, 60);
        if (r == 1)
        {
            InitState(NPCState.Wave);
        }
        else if (r == 2)
        {
            InitState(NPCState.Head_to_Target);
        }
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



    bool AtTargetNode(Node n)
    {
        if (n == null)
        {
            Debug.LogError("bad param");
            return true;
        }
        else
        {
            Vector3 d = n.m_pos - transform.position;
            bool res = (d.magnitude < 0.1f);
            return res;
        }
    }

}
