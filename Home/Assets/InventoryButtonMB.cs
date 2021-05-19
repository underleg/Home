using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryButtonMB : MonoBehaviour
{
    public GameObject m_buttonImage;
    public TextMeshPro m_word;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWord(string s)
    {
        m_word.SetText(s);
    }
}
