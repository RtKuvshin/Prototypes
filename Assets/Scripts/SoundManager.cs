using System;
using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    
    [SerializeField] private AudioClipRefsSO _audioClipRefsSo;

    private float volume;

    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
        CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
        //Player.Instance.OnPickedSomething += PlayerOnPickedSomething;
        BaseCounter.OnAnyObjectPlaced += BaseCounterOnAnyObjectPlaced;
        TrashCounter.OnAnyObjectTrashed += TrashCounterOnAnyObjectTrashed;
    }

    private void TrashCounterOnAnyObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(_audioClipRefsSo.trash, trashCounter.transform.position);
    }

    private void BaseCounterOnAnyObjectPlaced(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(_audioClipRefsSo.objectDrop, baseCounter.transform.position);
    }

    private void PlayerOnPickedSomething()
    {
        //PlaySound(_audioClipRefsSo.objectPickup, Player.Instance.transform.position);
    }

    private void CuttingCounterOnAnyCut(object sender, EventArgs e)
    {
        //Debug.Log(transform.position);
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(_audioClipRefsSo.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManagerOnRecipeFailed()
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(_audioClipRefsSo.deliveryFail, deliveryCounter.transform.position);
        
    }
    private void DeliveryManagerOnRecipeSuccess()
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(_audioClipRefsSo.deliverySuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier);
    }

    public void PlayFootstepsSound(Vector3 position, float volumeMultiplier)
    {
        PlaySound(_audioClipRefsSo.footstep, position, volumeMultiplier * volume);
    }
    public void PlayCountdownSound(int index)
    {
        PlaySound(_audioClipRefsSo.warning[index], Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(_audioClipRefsSo.warning, position);
    }


    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1f)
        {
            volume = 0;
        }
        
        PlayerPrefs.SetFloat(SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
