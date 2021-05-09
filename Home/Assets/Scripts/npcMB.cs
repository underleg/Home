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
        Wave,
        Wander,
        Wander_thinking,
        Head_to_Target,
        Talking,
        Listening
    };

    NPCState m_state = NPCState.Undefined;
    
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

 
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        m_animator = gameObject.GetComponent<Animator>(); 
        InitState(NPCState.Idle);
        
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
                break;

            default:
                break;
        }

    }

    /// <summary>
    /// WANDER STATE 
    /// </summary>
    private void InitWander()
    {
        m_animator.SetFloat("MoveSpeed", 0.4f);
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
        m_animator.SetFloat("MoveSpeed", 0.4f);
     
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
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    m_path[0].m_pos,
                    m_moveSpeed * Time.deltaTime);
            }

           
            yield return null;
        }
    }

    bool GiveWay(Vector3 tgt)
    {
        bool res = false;
        
        Vector3 step = (tgt - transform.position).normalized;
        Vector3 pos = transform.position;
        for (int i = 1; res == false && i <= 2; ++i)
        {
            Vector3 newPos = pos + step;
            QueryTriggerInteraction query = new QueryTriggerInteraction();
            if (Physics.Linecast(pos, newPos, 7, query))
            {
                print("collison " + query.ToString());
                m_pauseWalk = true;
                res = true;
            }
        }
        
        return res;
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
        int r = Random.Range(0, 120);
        if (r == 1)
        {
            InitState(NPCState.Wave);
        }
        else if (r == 2 || r == 3)
        {
            InitState(NPCState.Wander);
        }
        else if(r == 4)
        {
            m_thoughtBubble.ShowThought("...");
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

        Gizmos.DrawLine(transform.position, m_forward);
    }


}
