using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    private Action _onCloseButtonAction;

    [SerializeField] private GameObject rebindKeyObject;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private ButtonWithText moveUpButton;
    [SerializeField] private ButtonWithText moveDownButton;
    [SerializeField] private ButtonWithText moveLeftButton;
    [SerializeField] private ButtonWithText moveRightButton;
    [SerializeField] private ButtonWithText interactButton;
    [SerializeField] private ButtonWithText interactAltButton;
    [SerializeField] private ButtonWithText pauseButton;
    [SerializeField] private ButtonWithText gamepadInteractButton;
    [SerializeField] private ButtonWithText gamepadInteractAltButton;
    [SerializeField] private ButtonWithText gamepadPauseButton;

    private void Awake()
    {
        Instance = this;
        
        sfxButton.onClick.AddListener((() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        }));
        musicButton.onClick.AddListener((() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        }));
        closeButton.onClick.AddListener(() =>
        {
            Hide(gameObject);
            _onCloseButtonAction();
        });
        
        moveUpButton.Initialize();
        moveDownButton.Initialize();
        moveLeftButton.Initialize();
        moveRightButton.Initialize();
        interactButton.Initialize();
        interactAltButton.Initialize();
        pauseButton.Initialize();
        gamepadInteractButton.Initialize();
        gamepadInteractAltButton.Initialize();
        gamepadPauseButton.Initialize();
        
        moveUpButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveUp));
        moveDownButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveDown));
        moveLeftButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveLeft));
        moveRightButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.MoveRight));
        interactButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
        interactAltButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlternate));
        pauseButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));
        gamepadInteractButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Interact));
        gamepadInteractAltButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_InteractAlternate));
        gamepadPauseButton.Button.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Pause));
        
    }

    private void UpdateVisual() 
    {
        sfxText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);
        
        moveUpButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp));
        moveDownButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown));
        moveLeftButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft));
        moveRightButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight));
        interactButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Interact));
        interactAltButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate));
        pauseButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Pause));
        gamepadInteractButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact));
        gamepadInteractAltButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate));
        gamepadPauseButton.SetText(GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause));
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnpaused += () => Hide(gameObject);
        UpdateVisual();
        Hide(rebindKeyObject);
        Hide(gameObject);
    }
    public void Show(GameObject window, Action onCloseButtonAction = null)
    {
        if(onCloseButtonAction != null) _onCloseButtonAction = onCloseButtonAction;
        window.SetActive(true);
        
        musicButton.Select();
    }
    private void Hide(GameObject window)
    {
        window.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        Show(rebindKeyObject);
        GameInput.Instance.RebindBinding(binding, () =>
        {
            UpdateVisual();
            Hide(rebindKeyObject);
        }); 
    }
}
