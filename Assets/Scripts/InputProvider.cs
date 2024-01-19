using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : SingletonMonoBehaviour<InputProvider>
{
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction dashAction;
    [SerializeField] private InputAction eKeyAction;
    [HideInInspector] public Vector2 axis;
    [HideInInspector] public bool isSpaceKeyPressing;

    public void Init()
    {
        axis = Vector2.zero;

        moveAction.performed += context => axis = context.ReadValue<Vector2>();
        moveAction.canceled += context => axis = context.ReadValue<Vector2>();

        dashAction.performed += _ => isSpaceKeyPressing = true;
        dashAction.canceled += _ => isSpaceKeyPressing = false;
        
        moveAction.Enable();
        dashAction.Enable();
    }
}