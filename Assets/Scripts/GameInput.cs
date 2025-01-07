using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause
    }
    
    public static GameInput Instance { get; private set; }
    public event Action OnInteractAction; 
    public event Action OnInteractAlternateAction;
    public event Action OnPauseAction;

    private const string BINDINGS = "Bindings";
    
    private PlayerInputSystem playerInputSystem;
    
    private void Awake()
    {
        Instance = this;
        
        playerInputSystem = new PlayerInputSystem();

        if (PlayerPrefs.HasKey(BINDINGS))
        {
            playerInputSystem.LoadBindingOverridesFromJson(PlayerPrefs.GetString(BINDINGS));
        }
        
        playerInputSystem.Player.Enable();
        
        playerInputSystem.Player.Interact.performed += InteractOnperformed;
        playerInputSystem.Player.InteractAlternate.performed += InteractAlternateOnperformed;
        playerInputSystem.Player.Pause.performed += PauseOnperformed;
        
        //Debug.Log(GetBindingText(Binding.Pause));
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

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                return playerInputSystem.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return playerInputSystem.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return playerInputSystem.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return playerInputSystem.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputSystem.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputSystem.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputSystem.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputSystem.Player.Disable();

        InputAction inputAction;
        int bindingIndex;
        
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = playerInputSystem.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = playerInputSystem.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = playerInputSystem.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = playerInputSystem.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputSystem.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputSystem.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputSystem.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            playerInputSystem.Player.Enable();
            onActionRebound();
            
            PlayerPrefs.SetString(BINDINGS,playerInputSystem.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        } ).Start();
    }
}
