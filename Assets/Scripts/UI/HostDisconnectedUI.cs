using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        }));
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientID)
    {
        if (clientID == NetworkManager.ServerClientId)
        {
            Show();
        }
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManagerOnClientDisconnectCallback;
    }
}
