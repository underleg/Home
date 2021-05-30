using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceMB : MonoBehaviour
{
    public InventoryMB m_inventory;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if(m_inventory.IsInventoryShowing())
        {
            if(InputManagerMB.Instance.m_closeInventory)
            {
                InputManagerMB.Instance.SetToPlayerInput();
                m_inventory.HideInventory();
            }
            else
            {
                m_inventory.HandleItemSelection();
            }
        }
        else
        {
            if (InputManagerMB.Instance.m_openInventory)
            {
                InputManagerMB.Instance.SetToInterfaceInput();
                m_inventory.ShowInventory();
            }
        }


    }
}