using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



public class PlayerMB : MonoBehaviour
{
    private static PlayerMB instance;
    public static PlayerMB Instance { get { return instance; } }

    public Vector3 forward;


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
            if(contact.otherCollider.gameObject.name != "Ground")
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
        
        this.forward= transform.position;
        forward += new Vector3(Mathf.Sin(playerRot.y), 0f, Mathf.Cos(playerRot.y)) * 0.25f;
    }

    void OnDrawGizmosSelected()
    {
        UpdateForwardVector();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, forward);


    }
}
