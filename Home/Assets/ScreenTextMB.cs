using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenTextMB : MonoBehaviour
{
    private static ScreenTextMB instance;
    public static ScreenTextMB Instance { get { return instance; } }


    TextMeshPro m_text;

    float m_timer = 0.0f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timer > 0.0f)
        {
            m_timer -= Time.deltaTime;
            if(m_timer <= 0.0f)
            {
                m_text.SetText("");
            }
        }
        
    }

    public void SetText(string s, float displayTime = 4.0f)
    {
        m_text.SetText(s);
        m_timer = displayTime;
    }
}
