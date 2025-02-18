using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
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
        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenManagerOnLocalPlayerReadyChanged;
        KitchenGameManager.Instance.OnStateChanged += KitchenManagerOnStateChanged;
        Hide();
    }

    private void KitchenManagerOnStateChanged()
    {
        if (KitchenGameManager.Instance.IsCountdownToStart())
        {
            Hide();
        }
    }

    private void KitchenManagerOnLocalPlayerReadyChanged()
    {
        if (KitchenGameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
        
    }
}
