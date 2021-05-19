using UnityEngine;

public class InteractiveObjectMB : MonoBehaviour
 {
    public enum PlayerState
    {
        undefined,
        Player_outside_active_zone,
        Player_inside_active_zone,
        Player_inside_interactive_zone
    };

    protected PlayerState m_playerState = PlayerState.undefined;

    public float m_outerRadius = 2.0f; // player can turn on / pick up etc
    public float m_innerRadius = 0.4f; // object is close enough to player to attract attention

    public GameConstants.ObjectId m_objectId = GameConstants.ObjectId.NONE;

    protected bool m_collectable = false;
 
    // Use this for initialization
    virtual protected void Start()
    {
    }

    // Update is called once per frame
    virtual protected void Update()
    {

    }

    public bool IsCollectable()
    {
        return m_collectable;
    }

    
    protected bool UpdatePlayerState()
    {
        bool res = true; // true means stqate changed

        Vector3 toPlayer = PlayerMB.Instance.transform.position - transform.position;
        float dist = toPlayer.magnitude;

        Debug.Assert(m_innerRadius < m_outerRadius, "");

        switch (m_playerState)
        {
            case PlayerState.undefined:
                if(dist < m_innerRadius)
                    m_playerState = PlayerState.Player_inside_interactive_zone;
                else if(dist < m_outerRadius)
                    m_playerState = PlayerState.Player_inside_active_zone;
                else
                    m_playerState = PlayerState.Player_outside_active_zone;
                break;

            case PlayerState.Player_outside_active_zone:
                if (dist < m_innerRadius)
                    m_playerState = PlayerState.Player_inside_interactive_zone;
                else if (dist < m_outerRadius)
                    m_playerState = PlayerState.Player_inside_active_zone;
                else
                    res = false;
                break;

            case PlayerState.Player_inside_active_zone:
                if (dist < m_innerRadius)
                {
                    if (IsPlayerFacingObject())
                    {
                        m_playerState = PlayerState.Player_inside_interactive_zone;
                        PlayerMB.Instance.InteractiveObject = this;
                    }
                    }
                else if (dist >= m_outerRadius)
                    m_playerState = PlayerState.Player_outside_active_zone;
                else
                    res = false;
                break;

            case PlayerState.Player_inside_interactive_zone:
                if (dist >= m_outerRadius)
                {
                    m_playerState = PlayerState.Player_outside_active_zone;
                    if (PlayerMB.Instance.InteractiveObject == this)
                    {
                        PlayerMB.Instance.InteractiveObject = null;
                    }
                }
                else if (dist >= m_innerRadius || IsPlayerFacingObject() == false)
                {
                    m_playerState = PlayerState.Player_inside_active_zone;
                    if (PlayerMB.Instance.InteractiveObject == this)
                    {
                        PlayerMB.Instance.InteractiveObject = null;
                    }
                }
                else
                    res = false;
                break;
        }


        return res;
    }

    public bool IsPlayerFacingObject()
    {
        bool res = false;

        Vector3 pVec = PlayerMB.Instance.transform.position;
        pVec.y = transform.position.y;
        Vector3 pFor = PlayerMB.Instance.transform.forward * 0.2f;
        pFor.y = transform.position.y;
        pFor += pVec;

        Vector3 v1 = pVec - transform.position;
        
        Vector3 v2 = pFor - transform.position;
        
        if (v2.sqrMagnitude < v1.sqrMagnitude)
        {
            res = true;
        }


        return res;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_outerRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_innerRadius);

        // draw m_forward line
        if (PlayerMB.Instance != null)
        {
            Vector3 pVec = PlayerMB.Instance.transform.position;
            pVec.y = transform.position.y;
            Vector3 pFor = PlayerMB.Instance.transform.forward * 0.2f;
            pFor.y = transform.position.y;
            pFor += pVec;

            Vector3 v1 = pVec - transform.position;

            Vector3 v2 = pFor - transform.position;

            Gizmos.DrawLine(pFor, pVec);
        }
    }


    protected void HandleShake(Animator a)
    {
        if (UpdatePlayerState())
        {
            if (m_playerState == PlayerState.Player_inside_active_zone)
            {
                a.Play("InteractiveShake");
            }
            else if (m_playerState == PlayerState.Player_inside_interactive_zone)
            {
                a.Play("ActiveShake");
            }
            else
            {
                a.Play("Idle");
            }
        }
    }
}