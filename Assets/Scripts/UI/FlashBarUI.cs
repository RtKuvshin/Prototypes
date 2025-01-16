using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";
    
    [SerializeField] private StoveCounter _stoveCounter;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
        _animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounterOnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        float burnProgressAmount = 0.2f;
        bool show = _stoveCounter.IsFired() && e.progressNormalized >= burnProgressAmount; 
        _animator.SetBool(IS_FLASHING, show);
    }
}
