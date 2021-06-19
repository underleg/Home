using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcMB : MovingObjectMB
{
    [SerializeField] private Animator m_animator = null;
    // [SerializeField] private Rigidbody m_rigidBody = null;

    public enum NPCState
    {
        Undefined,
        Idle,
        Wander,
        Wander_thinking,
        Head_to_Target,
        Talking,
        Listening,
        Face_player
    };

    NPCState m_state = NPCState.Undefined;
    int m_substate = 0;
    
    float m_timer;

    float m_targetDirection = 180.0f;

    Node m_currentNode;
    Node m_targetNode;

    List<Node> m_path;
 
    float m_turnSpeed = 2.0f;

    public float m_moveSpeed = 1.5f;

    public Vector3 m_forward;

    public MBThoughtBubble m_thoughtBubble;

    bool m_pauseWalk = false;

    float m_walkSpeed = 0.4f;

    Collider m_myCollider;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        m_animator = gameObject.GetComponent<Animator>(); 
        InitState(NPCState.Idle);

        m_myCollider = gameObject.GetComponent<Collider>();


    }

    private void Awake()
    {
        UpdateForwardVector();
    }

    override protected void Update()
    {
        base.Update();
        UpdateForwardVector();
    }

    void UpdateForwardVector()
    {
        Vector3 rot = transform.localEulerAngles;
        rot *= Mathf.Deg2Rad;

        this.m_forward = transform.position;
        m_forward += new Vector3(Mathf.Sin(rot.y), 0f, Mathf.Cos(rot.y));
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
       
            case NPCState.Wander:
                InitWander();
                break;

            case NPCState.Face_player:
                InitFacePlayer();
                break;

            default:
                break;
        }    
    }

    private void FixedUpdate()
    {
        if (!m_animator) return;

        {
            m_animator.SetBool("Grounded", true);

            switch (m_state)
            {
                case NPCState.Idle:
                    HandleIdle();
                    break;

               case NPCState.Wander:
                    break;

                case NPCState.Face_player:
                    HandleFacePlayer();
                    break;

                default:
                    break;
            }
        }

    }

    /// <summary>
    /// WANDER STATE 
    /// </summary>
    private void InitWander()
    {
        m_animator.SetFloat("MoveSpeed", m_walkSpeed);
        m_currentNode = GridMB.Instance.GetNodeFromPosition(transform.position);
      
        m_targetNode = GridMB.Instance.PickRandomNearbyOpenNode(m_currentNode, 10);
        PathRequestManagerMB.RequestPath(transform.position, m_targetNode.m_pos, OnWanderPathFound);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPath"></param>
    /// <param name="pathSuccessful"></param>
    public void OnWanderPathFound(List<Node> newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            m_path = newPath;
            //targetIndex = 0;
            StopCoroutine("FollowWanderPath");
            StartCoroutine("FollowWanderPath");
        }
        else
        {
            InitState(NPCState.Idle);
        }
    }

    /// <summary>
    /// coroutine to carry out wnader state
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowWanderPath()
    {
        while (true)
        {      
            if (Utils.VectorsCloseToEqual(transform.position, m_path[0].m_pos))               
            {
                m_path.RemoveAt(0);
                if (m_path.Count == 0)
                {
                    InitState(NPCState.Idle);
                    yield break;
                }
            }

            TurnToFaceTarget(new Vector3(
                m_path[0].m_pos.x,
                transform.position.y,
                m_path[0].m_pos.z));


            if (GiveWay(m_path[0].m_pos) == false)
            {
                m_animator.SetFloat("MoveSpeed", m_walkSpeed);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    m_path[0].m_pos,
                    m_moveSpeed * Time.deltaTime);
            }
            else
            {
                m_animator.SetFloat("MoveSpeed", 0.0f);
            }    

           
            yield return null;
        }
    }

    bool GiveWay(Vector3 tgt)
    {
        /*
        bool res = false;
        
        Vector3 step = (tgt - transform.position).normalized;
        Vector3 pos = transform.position;
        pos.y -= 0.5f;
        m_myCollider.enabled = false; // disalbe our own collider

        RaycastHit hit;

        if(Physics.SphereCast(pos, 0.01f, transform.forward, out hit) == true)
        {
            print("collison ");
            m_pauseWalk = true;
            res = true;
        }
        m_myCollider.enabled = true;
  
        return res;
        */
        return false;
    }


    void TurnToFaceTarget(Vector3 tgt)
    {
        Vector3 fwd = (m_forward - transform.position).normalized;
        tgt = (tgt - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = Quaternion.LookRotation(tgt);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_turnSpeed);
    }

    bool IsFacingTarget(Vector3 tgt)
    {
        Vector3 fwd = (m_forward - transform.position).normalized;
        tgt = (tgt - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = Quaternion.LookRotation(tgt);
  
        float angle = Quaternion.Angle(transform.rotation, lookRotation);

        return (angle < 5.0f);
    }

    /// IDLE STATE ///
    private void InitIdle()
    {
        m_animator.SetFloat("MoveSpeed", 0.0f);
    }

    void HandleIdle()
    {
        int r = Random.Range(0, 120);
        if (r == 2 || r == 3)
        {
            InitState(NPCState.Wander);
        }
        else if(r == 5)
        {
            
            InitState(NPCState.Face_player);
        }
    }

    // Face player
    private void InitFacePlayer()
    {
        m_animator.SetFloat("MoveSpeed", 0.0f);
    }

    void HandleFacePlayer()
    {
        Vector3 plyr = PlayerMB.Instance.transform.position;

        TurnToFaceTarget(plyr);

        switch(m_substate)
        {
            case 0:
                if (IsFacingTarget(plyr))
                {
                    m_thoughtBubble.ShowThought("Hiya!");
                    m_animator.SetTrigger("Wave");
                    m_substate++;
                    m_timer = 3.0f;
                }
                break;

            case 1:
                m_timer -= Time.deltaTime;
                if (m_timer < 0.0f)
                {
                    InitState(NPCState.Idle);
                }
                break;
            default:
                break;
        }
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(m_path != null && m_path.Count > 1)
        {
            Vector3 stt = transform.position;
            for(int i = 0; i < m_path.Count; ++i)
            {
                Gizmos.DrawLine(stt, m_path[i].m_pos);
                stt = m_path[i].m_pos;
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, m_forward);
    }


}
