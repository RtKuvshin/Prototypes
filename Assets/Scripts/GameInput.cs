using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event Action OnInteractAction; 
    private PlayerInputSystem playerInputSystem;

    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        playerInputSystem.Player.Interact.performed += InteractOnperformed;
    }

    private void InteractOnperformed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke();
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputSystem.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
