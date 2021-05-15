using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InventoryItemMB : MonoBehaviour
{
    static public InventoryItemMB m_selected = null;

    public GameConstants.ObjectId m_objectId;
    public GameObject m_model;
    public TextMeshPro m_text;

    float m_rotationStart;

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
            m_model.transform.Rotate(0, 0, 50.0f * Time.unscaledDeltaTime,  Space.Self);           
        }    
    }

    public void RestoreRotation()
    {
        Vector3 rot = m_model.transform.eulerAngles;
        rot.z = m_rotationStart;
        m_model.transform.eulerAngles = rot;
    }
}
