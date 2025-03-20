using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : Visible
{
    [SerializeField] private int playerIndex;
    [SerializeField] private TextMeshPro readyText;
    [SerializeField] private PlayerVisual _playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            kickButton.gameObject.SetActive(false);
        }
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientID);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReadyOnReadyChanged;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        if (playerIndex == 0)
        {
            kickButton.gameObject.SetActive(false);
        }
        
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReadyOnReadyChanged;
    }

    private void CharacterSelectReadyOnReadyChanged()
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayerOnPlayerDataNetworkListChanged()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            
            readyText.gameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID));
            _playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }
}

