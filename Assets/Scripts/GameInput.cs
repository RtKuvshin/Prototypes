using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    public event Action OnInteractAction; 
    public event Action OnInteractAlternateAction;
    public event Action OnPauseAction;
    
    private PlayerInputSystem playerInputSystem;
    
    private void Awake()
    {
        Instance = this;
        
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.Player.Enable();
        
        playerInputSystem.Player.Interact.performed += InteractOnperformed;
        playerInputSystem.Player.InteractAlternate.performed += InteractAlternateOnperformed;
        playerInputSystem.Player.Pause.performed += PauseOnperformed;
    }

    private void OnDestroy()
    {
        playerInputSystem.Player.Interact.performed -= InteractOnperformed;
        playerInputSystem.Player.InteractAlternate.performed -= InteractAlternateOnperformed;
        playerInputSystem.Player.Pause.performed -= PauseOnperformed;
        
        playerInputSystem.Dispose();
    }

    private void PauseOnperformed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke();
    }

    private void InteractAlternateOnperformed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke();
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
