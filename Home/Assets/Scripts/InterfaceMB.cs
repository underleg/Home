using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceMB : MonoBehaviour
{
    public List<GameObject> m_items;
    public GameObject m_inventory;
    public GameObject m_inventoryTile;


    float m_inventory_y_offscreen = -6.0f;
    float m_inventory_y_onscreen = 0.0f;

    int m_inventory_width = 3;
    int m_inventory_height = 3;

    int m_current_inventory_x = 0;
    int m_current_inventory_z = 0;

    float m_inventoryTile_start_pos_x;
    float m_inventoryTile_start_pos_z;

    public List<Vector2> m_itemsPositions;



    public float m_inventory_pos_dx = -2.87f;
    public float m_inventory_pos_dz = 1.76f;

    float m_buttonTimeGap = 0.5f;

    float m_timer;



    bool m_show_inventory = false;

    Coroutine m_moveInventoryCoroutine = null;

    
    // Start is called before the first frame update
    void Start()
    {
        m_inventoryTile_start_pos_x = m_inventoryTile.transform.localPosition.x;
        m_inventoryTile_start_pos_z = m_inventoryTile.transform.localPosition.z;
        InventoryItemMB.m_selected = m_items[0].GetComponent<InventoryItemMB>();
        
        for(int i = 0; i < m_items.Count; ++i)
        {
            m_itemsPositions.Add(new Vector2(m_items[i].transform.localPosition.x, m_items[i].transform.localPosition.z));
        }

        AlignItems();
    }

    void AlignItems()
    {
        for(int i = 0; i < m_items.Count;  ++i)
        {
            Vector3 p = m_items[i].transform.localPosition;
            p.x = m_itemsPositions[i].x;
            p.z = m_itemsPositions[i].y;
            m_items[i].transform.localPosition = p;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            print("show inventory");
            m_show_inventory = !m_show_inventory;
            if (m_moveInventoryCoroutine != null)
            {
                StopCoroutine(m_moveInventoryCoroutine);
            }
            m_moveInventoryCoroutine = null;


            if (m_show_inventory)
            {
                Time.timeScale = 0.0f;
                m_timer = m_buttonTimeGap;
                m_moveInventoryCoroutine = StartCoroutine(MoveInventory(m_inventory_y_onscreen));
            }
            else
            {
                Time.timeScale = 1.0f;
                m_moveInventoryCoroutine = StartCoroutine(MoveInventory(m_inventory_y_offscreen));
            }
        }
        else
        {
            if (m_show_inventory && m_moveInventoryCoroutine == null)
            {
                HandleItemSelection();
            }
        }
    }

    void HandleItemSelection()
    {
        if (m_timer > 0.0f)
        {
            m_timer -= Time.unscaledDeltaTime;
        }
        else
        {
            bool change = false;
            if (Input.GetKey(KeyCode.LeftArrow) && m_current_inventory_x > 0)
            {
                change = true;
                m_current_inventory_x--;                
            }
            else if (Input.GetKey(KeyCode.RightArrow) && m_current_inventory_x < m_inventory_width - 1)
            {
                change = true;
                m_current_inventory_x++;
            }
            if (Input.GetKey(KeyCode.UpArrow) && m_current_inventory_z > 0)
            {
                change = true;
                m_current_inventory_z--;
            }
            else if (Input.GetKey(KeyCode.DownArrow) && m_current_inventory_z < m_inventory_height - 1)
            {
                change = true;
                m_current_inventory_z++;
            }
            if (change)
            {
                Vector3 pos = m_inventoryTile.transform.localPosition;

                pos.x = m_inventoryTile_start_pos_x + m_current_inventory_x * m_inventory_pos_dx;
                pos.z = m_inventoryTile_start_pos_z + m_current_inventory_z * m_inventory_pos_dz;

                m_inventoryTile.transform.localPosition = pos;
                m_timer = 0.2f;

                int idx = m_current_inventory_z * m_inventory_width + m_current_inventory_x;
                InventoryItemMB.m_selected = m_items[idx].GetComponent<InventoryItemMB>();


            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveInventory(float tgt_y)
    {
        float t = 0.3f;
        float sy = m_inventory.transform.position.y;
        float dy = (tgt_y - sy) / t;

        Vector3 pos = m_inventory.transform.position;

        while (t > 0.0f)
        {
            pos.y += dy * Time.unscaledDeltaTime;
            t -= Time.unscaledDeltaTime;
            m_inventory.transform.position = pos;
            yield return null;   
        }

        pos.y = tgt_y;
        m_inventory.transform.position = pos;

        m_moveInventoryCoroutine = null;
    }

  
}