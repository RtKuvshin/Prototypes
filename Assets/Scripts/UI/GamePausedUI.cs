using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePausedUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener((() =>
        {
            KitchenGameManager.Instance.TogglePauseGame();
        }));
        mainMenuButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        }));
        optionsButton.onClick.AddListener((() =>
        {
            Hide();
            OptionsUI.Instance.Show(OptionsUI.Instance.gameObject, Show);
        }));
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnLocalGamePaused += KitchenLocalGameManagerOnLocalGamePaused;
        KitchenGameManager.Instance.OnLocalGameUnpaused += KitchenLocalGameManagerOnLocalGameUnpaused;
        
        Hide();
    }

    private void KitchenLocalGameManagerOnLocalGameUnpaused()
    {
        Hide();
    }

    private void KitchenLocalGameManagerOnLocalGamePaused()
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        
        resumeButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
