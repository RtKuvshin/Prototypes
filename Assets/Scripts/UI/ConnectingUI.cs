using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayerOnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailToJoinGame += KitchenGameMultiplayerOnFailToJoinGame;
        Hide();
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailToJoinGame -= KitchenGameMultiplayerOnFailToJoinGame;
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayerOnTryingToJoinGame; 
    }

    private void KitchenGameMultiplayerOnFailToJoinGame()
    {
        Hide();
    }

    private void KitchenGameMultiplayerOnTryingToJoinGame()
    {
        Show();
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
