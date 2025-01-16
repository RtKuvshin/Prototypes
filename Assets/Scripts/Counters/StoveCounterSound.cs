using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    private AudioSource _audioSource;
    private float warningSoundTimer;
    private bool playWarningSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
        _stoveCounter.OnProgressChange += StoveCounterOnProgressChange;
    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0)
            {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;
                SoundManager.Instance.PlayWarningSound(_stoveCounter.transform.position);
            }
        }
    }

    private void StoveCounterOnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        float burnProgressAmount = 0.2f;
        playWarningSound = _stoveCounter.IsFired() && e.progressNormalized >= burnProgressAmount;
    }

    private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state is StoveCounter.State.Frying or StoveCounter.State.Fried;

        if (playSound)
        {
            var volume =SoundManager.Instance.GetVolume();
            _audioSource.volume = volume;
            _audioSource.Play();
        }
        else
        {
            _audioSource.Pause();
        }
    }
}
