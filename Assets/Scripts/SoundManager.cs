using System;
using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClipRefsSO _audioClipRefsSo;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
        CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
        Player.Instance.OnPickedSomething += PlayerOnPickedSomething;
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
        PlaySound(_audioClipRefsSo.objectPickup, Player.Instance.transform.position);
    }

    private void CuttingCounterOnAnyCut(object sender, EventArgs e)
    {
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

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(_audioClipRefsSo.footstep, position, volume);
    }
}
