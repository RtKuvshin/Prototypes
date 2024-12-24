using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }
    
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI musicText;

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
        closeButton.onClick.AddListener(Hide);
    }

    private void UpdateVisual()
    {
        sfxText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnpaused += Hide;
        UpdateVisual();
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
