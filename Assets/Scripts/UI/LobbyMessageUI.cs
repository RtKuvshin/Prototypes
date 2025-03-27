using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }
    

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailToJoinGame += KitchenGameMultiplayerOnFailToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenLobbyOnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenLobbyOnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenLobbyOnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenLobbyOnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenLobbyOnJoinFailed; 
        Hide();
    }

    private void KitchenLobbyOnJoinFailed()
    {
        ShowMessage("Failed To Join Lobby :(");
    }

    private void KitchenLobbyOnQuickJoinFailed()
    {
        ShowMessage("Couldn't find a Lobby to quick join :(");
    }

    private void KitchenLobbyOnJoinStarted()
    {
        ShowMessage("Joining Lobby...");
    }

    private void KitchenLobbyOnCreateLobbyFailed()
    {
        ShowMessage("Failed To Create Lobby :(");
    }

    private void KitchenLobbyOnCreateLobbyStarted() 
    {
        ShowMessage("Creating Lobby...");
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailToJoinGame -= KitchenGameMultiplayerOnFailToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenLobbyOnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenLobbyOnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenLobbyOnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenLobbyOnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenLobbyOnJoinFailed; 
    }

    private void KitchenGameMultiplayerOnFailToJoinGame()
    {
        ShowMessage(NetworkManager.Singleton.DisconnectReason == ""
            ? "Failed To Connect!"
            : NetworkManager.Singleton.DisconnectReason);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }
}
