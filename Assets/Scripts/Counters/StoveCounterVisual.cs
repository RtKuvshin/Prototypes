using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    [SerializeField] private GameObject stoveGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        _stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
    }

    private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool showVisual = e.state is StoveCounter.State.Frying or StoveCounter.State.Fried;
        stoveGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
