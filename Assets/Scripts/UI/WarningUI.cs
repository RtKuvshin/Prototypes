using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;

    private void Start()
    {
        _stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
        ToggleVisual(false);
    }

    private void StoveCounterOnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        float burnProgressAmount = 0.2f;
        bool show = _stoveCounter.IsFired() && e.progressNormalized >= burnProgressAmount;
        ToggleVisual(show);
    }

    private void ToggleVisual(bool value)
    {
        gameObject.SetActive(value);
    }   
}
