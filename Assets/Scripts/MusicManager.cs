using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance{ get; private set;}
    
    private const string MUSIC_VOLUME = "MusicVolume";
    
    private float volume;
    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 0.4f);
        _audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1f)
        {
            volume = 0;
        }

        _audioSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }
    

    public float GetVolume()
    {
        return volume;
    }
}
