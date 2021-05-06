using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class MBThoughtBubble : MonoBehaviour
{
    public enum ThoughtBubbleSM
    {
        INVISIBLE,
        APPEARING,
        MESSAGE,
        ELLIPSE,
        DISAPPEARING
    };

    ThoughtBubbleSM m_state = ThoughtBubbleSM.INVISIBLE;
    Camera cam;

    public GameObject m_bubble1;
    public GameObject m_bubble2;
    public GameObject m_mainBubble;
    public TextMeshPro m_textMP;
    public float m_floatMovement = 0.1f;

    float m_mainBubbleDefaultY;

    float m_thoughtShowTime = 2.5f;

    bool m_elipsing = true;
    float m_timer = 0.0f;

    string m_thoughtString = "";

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        Assert.IsNotNull(m_bubble1);
        Assert.IsNotNull(m_bubble2);
        Assert.IsNotNull(m_mainBubble);

        m_mainBubbleDefaultY = m_mainBubble.transform.localPosition.y;

        Assert.IsNotNull(m_textMP);
        m_textMP.SetText(m_thoughtString);

        SetState(ThoughtBubbleSM.INVISIBLE);

    }

    private void Update()
    {
        m_timer += Time.deltaTime;

        switch (m_state)
        {
            case ThoughtBubbleSM.INVISIBLE:
                break;
            case ThoughtBubbleSM.APPEARING:
                BobThoughtBubbles();
                HandleAppearingState();
                break;
            case ThoughtBubbleSM.MESSAGE:
                BobThoughtBubbles();
                HandleMessageState();              
                break;
            case ThoughtBubbleSM.ELLIPSE:
                BobThoughtBubbles();
                HandleEllipseState();
                break;
            case ThoughtBubbleSM.DISAPPEARING:
                BobThoughtBubbles();
                HandleDisappearingState();
                break;
        }
    }

    void HandleAppearingState()
    {
        float timeGap = 0.2f;
        if (m_timer > timeGap * 3)
        {
            SetState(ThoughtBubbleSM.ELLIPSE);
        }
        else if( m_timer > timeGap * 2)
        {
            m_bubble1.GetComponent<Renderer>().enabled = true;
            m_bubble2.GetComponent<Renderer>().enabled = true;
        }
        else if(m_timer > timeGap)
        {
            m_bubble1.GetComponent<Renderer>().enabled = true;
        }
    }

    void HandleMessageState()
    {
        if(m_timer > this.m_thoughtShowTime)
        {
            SetState(ThoughtBubbleSM.DISAPPEARING);
        }
    }


    void HandleEllipseState()
    {
        if (m_elipsing)
        {
            if(m_timer > 0.8f)
            {
                m_timer -= 0.8f;

                if(m_thoughtString == "")
                {
                    m_thoughtString = ".";
                }
                else if(m_thoughtString == ".")
                {
                    m_thoughtString = "..";
                }
                else if (m_thoughtString == "..")
                {
                    m_thoughtString = "...";
                }
                else 
                {
                    m_thoughtString = "";
                }

                m_textMP.SetText(m_thoughtString);
            }
        }
    }

    void HandleDisappearingState()
    {
        float timeGap = 0.2f;
        if (m_timer > timeGap * 3)
        {
            SetState(ThoughtBubbleSM.INVISIBLE);         
        }
        else if (m_timer > timeGap * 2)
        {
            m_bubble2.GetComponent<Renderer>().enabled = false;
            m_mainBubble.GetComponent<Renderer>().enabled = false;
            m_textMP.GetComponent<Renderer>().enabled = false;
        }
        else if (m_timer > timeGap)
        {
            m_mainBubble.GetComponent<Renderer>().enabled = false;
            m_textMP.GetComponent<Renderer>().enabled = false;

        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);     
    }

    // move bubble around gently
    void BobThoughtBubbles()
    {
        Vector3 p1 = m_bubble1.transform.localPosition;
        p1.x = Mathf.Sin(Time.time / 2.0f) * m_floatMovement;
        m_bubble1.transform.localPosition = p1;

        Vector3 p2 = m_bubble2.transform.localPosition;
        p2.x = Mathf.Cos(Time.time / 3.0f) * m_floatMovement;
        m_bubble2.transform.localPosition = p2;

        Vector3 p3 = m_mainBubble.transform.localPosition;
        p3.x = Mathf.Sin(Time.time / 4.0f) * m_floatMovement / 2.0f;
        p3.y = m_mainBubbleDefaultY + Mathf.Cos(Time.time / 2.0f) * m_floatMovement / 2.0f;
        m_mainBubble.transform.localPosition = p3;
    }


    // set the m_state of the thought bubble
    void SetState(ThoughtBubbleSM newState)
    {   
        switch (newState)
        {
            case ThoughtBubbleSM.INVISIBLE:
                m_bubble1.GetComponent<Renderer>().enabled = false;
                m_bubble2.GetComponent<Renderer>().enabled = false;
                m_mainBubble.GetComponent<Renderer>().enabled = false;
                m_textMP.GetComponent<Renderer>().enabled = false;
                this.m_thoughtString = "";
                m_textMP.SetText(m_thoughtString);
                break;
            case ThoughtBubbleSM.APPEARING:
                m_bubble1.GetComponent<Renderer>().enabled = false;
                m_bubble2.GetComponent<Renderer>().enabled = false;
                m_mainBubble.GetComponent<Renderer>().enabled = false;
                break;
            case ThoughtBubbleSM.MESSAGE:              
                m_bubble1.GetComponent<Renderer>().enabled = true;
                m_bubble2.GetComponent<Renderer>().enabled = true;
                m_mainBubble.GetComponent<Renderer>().enabled = true;
                m_textMP.GetComponent<Renderer>().enabled = true;
                m_textMP.SetText(m_thoughtString);
                break;

            case ThoughtBubbleSM.ELLIPSE:
                if (m_state == ThoughtBubbleSM.INVISIBLE)
                {
                    this.m_thoughtString = "...";
                    SetState(ThoughtBubbleSM.APPEARING); // recursion to show bubble first
                    newState = ThoughtBubbleSM.APPEARING;
                }
                else
                {
                    m_bubble1.GetComponent<Renderer>().enabled = true;
                    m_bubble2.GetComponent<Renderer>().enabled = true;
                    m_mainBubble.GetComponent<Renderer>().enabled = true;
                    m_textMP.GetComponent<Renderer>().enabled = true;
                }
                break;
            case ThoughtBubbleSM.DISAPPEARING:
                break;
        }


        this.m_timer = 0.0f;
        this.m_state = newState;

    }


    public void ShowThought(string msg)
    {
        m_thoughtString = msg;
        SetState(ThoughtBubbleSM.MESSAGE);
    }

    public void ShowThinking()
    {
        m_thoughtString = "";
        SetState(ThoughtBubbleSM.ELLIPSE);
    }

}
