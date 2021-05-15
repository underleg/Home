using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



public class PlayerMB : MonoBehaviour
{
    private static PlayerMB instance;
    public static PlayerMB Instance { get { return instance; } }

    public Vector3 m_forward;

    MBThoughtBubble thoughtBubble;

    private void Awake()
    {
        UpdateForwardVector();

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        thoughtBubble = GameObject.Find("thought").GetComponent<MBThoughtBubble>();
        Assert.IsNotNull(thoughtBubble);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateForwardVector();


    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject.name != "Ground")
            {
                thoughtBubble.ShowThought("Ouch!");
            }
        }
        if (collision.relativeVelocity.magnitude > 2)
        {
        }
    }

    void UpdateForwardVector()
    {
        Vector3 playerRot = transform.localEulerAngles;
        playerRot *= Mathf.Deg2Rad;

        this.m_forward = transform.position;
        m_forward += new Vector3(Mathf.Sin(playerRot.y), 0f, Mathf.Cos(playerRot.y));
    }

    void OnDrawGizmosSelected()
    {
        UpdateForwardVector();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, m_forward);
    }

    public InteractiveObjectMB InteractiveObject
    {
        get;
        set;
    }


    public bool CanPickUpInteractiveObject()
    {
        bool res = false;

        if( InteractiveObject != null && 
            InteractiveObject.IsCollectable() && 
            InventoryMB.Instance.CurrentItemCount() < InventoryMB.Instance.MaxItems() )
        {
            res = true;
        }

        return res;
    }

    public void PickUp()
    {
        if (CanPickUpInteractiveObject())
        {
            InventoryMB.Instance.AddNewItem(InteractiveObject);
            InteractiveObject = null;
        }
    }
}
