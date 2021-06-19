using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InventoryItemMB : MonoBehaviour
{
    static public InventoryItemMB m_selected = null;
    bool m_isSelected = true;


    public GameConstants.ObjectId m_objectId;
    public GameConstants.ObjectActionId m_actionId;
    public GameObject m_model;
    public TextMeshPro m_text;

    float m_rotationStart;

    
    public GameObject CollectedObject
    {
        get;
        set;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_rotationStart = transform.eulerAngles.z;       
    }

    // Update is called once per frame
    void Update()
    {
        if(this == m_selected)
        {
            if(!m_isSelected)
            {
                m_isSelected = true;
                m_text.alpha = 1.0f;

            }
            m_model.transform.Rotate(0, 0, 50.0f * Time.unscaledDeltaTime,  Space.Self);           
        }    
        else if (m_isSelected && m_text != null)
        {
            m_isSelected = false;
            m_text.alpha = 0.5f;
        }
    }

    public void RestoreRotation()
    {
        Vector3 rot = m_model.transform.eulerAngles;
        rot.z = m_rotationStart;
        m_model.transform.eulerAngles = rot;
    }

}
