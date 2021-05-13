using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemMB : MonoBehaviour
{
    static public InventoryItemMB m_selected = null;

    float m_rotationStart;

    // Start is called before the first frame update
    void Start()
    {
        m_rotationStart = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(this == m_selected)
        {
            float curY = transform.eulerAngles.y;

            transform.Rotate(0, 0, 50.0f * Time.unscaledDeltaTime,  Space.Self);
            
        }    
    }
}
