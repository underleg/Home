using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMB : MonoBehaviour
{
    public GameObject m_followCharacter = null;
    Vector3 m_characterOffset = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        if(m_followCharacter != null)
        {
            m_characterOffset = m_followCharacter.transform.position - transform.position;          
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // update camera position to follow character
        if (m_followCharacter != null)
        {
            transform.position = m_followCharacter.transform.position - m_characterOffset;
        }
    }
}
