using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
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
