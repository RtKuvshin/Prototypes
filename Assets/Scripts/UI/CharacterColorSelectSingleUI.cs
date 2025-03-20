using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image _image;
    [SerializeField] private Image selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener( (() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        }));
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
        _image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -=
            KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
    }

    private void KitchenGameMultiplayerOnPlayerDataNetworkListChanged()
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.gameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.gameObject.SetActive(false);
        }
    }
}
