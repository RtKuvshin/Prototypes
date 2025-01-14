using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Key keyMoveUp;
    [SerializeField] private Key keyMoveDown;
    [SerializeField] private Key keyMoveLeft;
    [SerializeField] private Key keyMoveRight;
    [SerializeField] private Key keyInteract;
    [SerializeField] private Key keyInteractAlt;
    [SerializeField] private Key keyPause;
    [SerializeField] private Key keyGamepadInteract;
    [SerializeField] private Key keyGamepadInteractAlt;
    [SerializeField] private Key keyGamepadPause;

    private void UpdateVisual()
    {
        keyMoveUp.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp));
        keyMoveDown.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
        keyMoveLeft.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft));
        keyMoveRight.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight));
        keyInteract.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Interact));
        keyInteractAlt.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate));
        keyPause.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Pause));
        keyGamepadInteract.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact));
        keyGamepadInteractAlt.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate));
        keyGamepadPause.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause));
    }

    private void Start()
    {
        GameInput.Instance.OnBindRebind += UpdateVisual;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManagerOnStateChanged;
        
        UpdateVisual();
        Show();
    }

    private void KitchenGameManagerOnStateChanged()
    {
        if (KitchenGameManager.Instance.IsCountdownToStart())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
