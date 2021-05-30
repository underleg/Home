using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputManagerMB : MonoBehaviour
{
    private static InputManagerMB instance;
    public static InputManagerMB Instance { get { return instance; } }

     // player states
    [HideInInspector]
    public bool m_moveLeft = false;
    [HideInInspector]
    public bool m_moveRight = false;
    [HideInInspector]
    public bool m_moveUp = false;
    [HideInInspector]
    public bool m_moveDown = false;
    [HideInInspector]
    public bool m_interact = false;
    [HideInInspector]
    public bool m_openInventory = false;
    [HideInInspector]
    public bool m_closeInventory = false;

    private PlayerInput m_PlayerInput;


    private void Awake()
    {
        instance = this;
        m_PlayerInput = GetComponent<PlayerInput>();
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public void OnLeftInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_moveLeft = (v > 0.2f);
    }

    public void OnRightInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_moveRight = (v > 0.2f);
    }

    public void OnUpInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_moveUp = (v > 0.2f);
    }

    public void OnDownInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_moveDown = (v > 0.2f);
    }

    public void OnInteractInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_interact = (v > 0.2f);
    }

    public void OnOpenInventoryInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_openInventory = (v > 0.2f);
    }

    public void OnCloseInventoryInputCB(InputAction.CallbackContext context)
    {
        float v = context.ReadValue<float>();
        m_closeInventory = (v > 0.2f);
    }

    public void SetToInterfaceInput()
    {
        m_PlayerInput.SwitchCurrentActionMap("InterfaceControls");
    }

    public void SetToPlayerInput()
    {
        m_PlayerInput.SwitchCurrentActionMap("CharacterController");
    }


}