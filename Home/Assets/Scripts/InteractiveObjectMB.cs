using System.Collections;
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

    public float m_outerRadius = 3.0f; // player can turn on / pick up etc
    public float m_innerRadius = 1.0f; // object is close enough to player to attract attention

 
    // Use this for initialization
    virtual protected void Start()
    {
    }

    // Update is called once per frame
    virtual protected void Update()
    {

    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_outerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_innerRadius);

        // draw m_forward line
        /*
        Vector3 playerRot = PlayerMB.Instance.transform.localEulerAngles;
        playerRot *= Mathf.Deg2Rad;
        Vector3 playerForward = PlayerMB.Instance.transform.position;
        playerForward += new Vector3(Mathf.Cos(playerRot.y), 0f, Mathf.Cos(playerRot.y));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(PlayerMB.Instance.transform.position, playerForward);
        */


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
                    if(IsPlayerFacingObject())
                        m_playerState = PlayerState.Player_inside_interactive_zone;
                }
                else if (dist >= m_outerRadius)
                    m_playerState = PlayerState.Player_outside_active_zone;
                else
                    res = false;
                break;

            case PlayerState.Player_inside_interactive_zone:
                if (dist >= m_outerRadius)
                    m_playerState = PlayerState.Player_outside_active_zone;
                else if (dist >= m_innerRadius || IsPlayerFacingObject() == false)
                    m_playerState = PlayerState.Player_inside_active_zone;
                else
                    res = false;
                break;
        }


        return res;
    }

    public bool IsPlayerFacingObject()
    {
        bool res = false;

        Vector3 v1 = PlayerMB.Instance.transform.position - transform.position;
        
        Vector3 v2 = PlayerMB.Instance.m_forward - transform.position;
        
        if(v2.sqrMagnitude < v1.sqrMagnitude)
        {
            res = true;
        }


        return res;
    }


}