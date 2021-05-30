using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMB : MonoBehaviour
{
    private static InventoryMB instance;
    public static InventoryMB Instance { get { return instance; } }

    public List<GameObject> m_itemPrefabs;

    public List<GameConstants.ObjectId> m_inventoryList;

    public GameObject m_inventoryTile;

    public InventoryButtonMB m_action;
    public InventoryButtonMB m_close;

    List<GameObject> m_items = new List<GameObject>();

    float m_inventory_y_offscreen = -6.0f;
    float m_inventory_y_onscreen = 0.0f;

    int m_inventory_width = 3;
    int m_inventory_height = 3;

    int m_current_inventory_x = 0;
    int m_current_inventory_z = 0;

    public float m_inventoryTile_start_pos_x = 0.86f;
    public float m_inventoryTile_start_pos_z = -0.93f;

    public float m_inventory_pos_dx = -2.87f;
    public float m_inventory_pos_dz = 1.76f;

    float m_buttonTimeGap = 0.5f;

    float m_timer;

    bool m_show_inventory = false;

    Coroutine m_moveInventoryCoroutine = null;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Start is called before the first frame update 
    /// </summary>

    void Start()
    {
        Vector3 pos = m_inventoryTile.transform.localPosition;
        pos.x = m_inventoryTile_start_pos_x;
        pos.z = m_inventoryTile_start_pos_z;
        m_inventoryTile.transform.localPosition = pos;

        DestroyItemInstances();
        CreateItemInstances();

        if (m_items.Count > 0)
        {
            InventoryItemMB.m_selected = m_items[0].GetComponent<InventoryItemMB>();
            AlignItems();
        }
    }

    void DestroyItemInstances()
    {
        while(m_items.Count > 0)
        {
            GameObject go = m_items[0];
            m_items.RemoveAt(0);
            Destroy(go);
        }
    }

    public int CurrentItemCount()
    {
        return m_inventoryList.Count;
    }

    public int MaxItems()
    {
        return m_inventory_width * m_inventory_height;
    }

    public void AddNewItem(InteractiveObjectMB obj)
    {
        print("Adding new item");
        m_inventoryList.Add(obj.m_objectId);
        GameObject go = GetItemPrefab(obj.m_objectId);
        GameObject newGO = GameObject.Instantiate(go, this.transform);

        go.GetComponent<InventoryItemMB>().CollectedObject = obj.gameObject;
        obj.gameObject.SetActive(false);
        m_items.Add(newGO);
        AlignItems();
    }

    void CreateItemInstances()
    {
        Debug.Assert(m_inventoryList.Count <= MaxItems());
        for(int i = 0; i < m_inventoryList.Count; ++i)
        {
            GameObject go = GetItemPrefab(m_inventoryList[i]);
            GameObject newGO = GameObject.Instantiate(go, this.transform);
            m_items.Add(newGO);
        }
    }

    GameObject GetItemPrefab(GameConstants.ObjectId id)
    {
        GameObject res = null;

        for (int i = 0; res == null && i < m_itemPrefabs.Count; ++i)
        {
            if(m_itemPrefabs[i].GetComponent<InventoryItemMB>().m_objectId == id)
            {
                res = m_itemPrefabs[i];
            }
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    void AlignItems()
    {
        float x = m_inventoryTile_start_pos_x;
        float y = m_inventoryTile_start_pos_z;

        for (int i = 0; i < m_items.Count; ++i)
        {
            Vector2 v2 = GetTilePositionFromIndex(i);

            Vector3 p = m_items[i].transform.localPosition;
            p.x = v2.x;
            p.z = v2.y;
            m_items[i].transform.localPosition = p;
        }
    }

    Vector2 GetTilePositionFromIndex(int tileIdx)
    {
        int xi = (tileIdx % m_inventory_width);
        int zi = (tileIdx / m_inventory_width);

        Vector2 res = new Vector2();

        res.x = m_inventoryTile_start_pos_x + (m_inventory_pos_dx * xi);
        res.y = m_inventoryTile_start_pos_z + (m_inventory_pos_dz * zi);

        return res;
    }
       

    /// <summary>
    ///  Update is called once per frame
    /// </summary>
    void Update()
    {
       
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ReadyForInventorySelection()
    {
        return (m_show_inventory && m_moveInventoryCoroutine == null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsInventoryShowing()
    {
        return m_show_inventory;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowInventory()
    {
        if (m_show_inventory == true)
        {
            return;
        }

        m_show_inventory = true;
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

    /// <summary>
    /// 
    /// </summary>
    public void HideInventory()
    {
        if (m_show_inventory == false)
        {
            return;
        }

        m_show_inventory = false;
        if (m_moveInventoryCoroutine != null)
        {
            StopCoroutine(m_moveInventoryCoroutine);
        }
        m_moveInventoryCoroutine = null;


        Time.timeScale = 1.0f;
        m_moveInventoryCoroutine = StartCoroutine(MoveInventory(m_inventory_y_offscreen));
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandleItemSelection()
    {
        if (m_timer > 0.0f)
        {
            m_timer -= Time.unscaledDeltaTime;
        }
        else
        {
            int new_inventory_x = m_current_inventory_x;
            int new_inventory_z = m_current_inventory_z;

            bool change = false;

            if (InputManagerMB.Instance.m_moveLeft && m_current_inventory_x > 0)
            {
                change = true;
                new_inventory_x--;
            }
            else if (InputManagerMB.Instance.m_moveRight && m_current_inventory_x < m_inventory_width - 1)
            {
                change = true;
                new_inventory_x++;
            }
            if (InputManagerMB.Instance.m_moveUp && m_current_inventory_z > 0)
            {
                change = true;
                new_inventory_z--;
            }
            else if (InputManagerMB.Instance.m_moveDown && m_current_inventory_z < m_inventory_height - 1)
            {
                change = true;
                new_inventory_z++;
            }


            if (change)
            {
                int idx = new_inventory_z * m_inventory_width + new_inventory_x;

                if (idx >= 0 && idx < m_items.Count && m_items[idx] != null)
                {
                    m_current_inventory_x = new_inventory_x;
                    m_current_inventory_z = new_inventory_z;

                    Vector3 pos = m_inventoryTile.transform.localPosition;

                    pos.x = m_inventoryTile_start_pos_x + m_current_inventory_x * m_inventory_pos_dx;
                    pos.z = m_inventoryTile_start_pos_z + m_current_inventory_z * m_inventory_pos_dz;

                    m_inventoryTile.transform.localPosition = pos;
                    m_timer = 0.2f;

                    SetSelectedItem(idx);
                }
            }
        }
    }
    

    void SetSelectedItem(int idx)
    {
        InventoryItemMB.m_selected.RestoreRotation();

        InventoryItemMB.m_selected = m_items[idx].GetComponent<InventoryItemMB>();

        string s = "XXXX";
        switch (InventoryItemMB.m_selected.m_actionId)
        {
            case GameConstants.ObjectActionId.HOLD:
                s = "Hold";
                break;
            case GameConstants.ObjectActionId.USE:
                s = "Use";
                break;
            default:
                break;
        }

        m_action.SetWord(s);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveInventory(float tgt_y)
    {
        float t = 0.3f;
        float sy = transform.position.y;
        float dy = (tgt_y - sy) / t;

        Vector3 pos = transform.position;

        while (t > 0.0f)
        {
            pos.y += dy * Time.unscaledDeltaTime;
            t -= Time.unscaledDeltaTime;
            transform.position = pos;
            yield return null;
        }

        pos.y = tgt_y;
        transform.position = pos;

        m_moveInventoryCoroutine = null;
    }


    private void OnDrawGizmosSelected()
    {
        float sz = 0.2f;
        Vector3 scl = new Vector3(sz, sz, sz);
        for(int i = 0; i < m_inventory_width * m_inventory_height; ++i)
        {
            Vector2 v2 = GetTilePositionFromIndex(i);

            Vector3 v3 = new Vector3(v2.x, 1.07f, v2.y);

            if(i == 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawCube(transform.position + transform.TransformVector(v3), scl);

        }
        
    }

}
