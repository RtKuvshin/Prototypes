using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter _baseCounter;
    [SerializeField] private GameObject[] _visualGameObjectArray;
    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += PlayerOnAnyPlayerSpawned;
        }
    }

    private void PlayerOnAnyPlayerSpawned()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= PlayerOnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += PlayerOnSelectedCounterChanged;
        }
    }

    private void PlayerOnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == _baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in _visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in _visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
