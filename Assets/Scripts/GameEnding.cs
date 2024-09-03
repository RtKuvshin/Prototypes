using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnding : MonoBehaviour
{
    [SerializeField] private CanvasGroup caughtPanel;
    [SerializeField] private CanvasGroup exitPanel;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AudioSource exitAudio;
    [SerializeField] private AudioSource caughtAudio;
    private bool hasAudioPlayed;
    private float timer;
    private float delay = 2f;
    private bool isPlayerAtExit;
    private bool isPlayerCaught;
    
    
    
    private void EndLevel(CanvasGroup canvasGroup, bool doRestart, AudioSource audioSource)
    {
        PlayAudio(audioSource);
        
        
        timer += Time.deltaTime;
        if (timer < fadeDuration)
        {
            canvasGroup.alpha = timer / fadeDuration;
        }
        else if (timer > fadeDuration + delay) 
        {
            if (doRestart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
    private void Update()
    {
        if (isPlayerAtExit)
        {
            EndLevel(exitPanel, false, exitAudio);
        }
        else if (isPlayerCaught)
        {
            EndLevel(caughtPanel, true, caughtAudio);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) )
        {
            if (player.HasKey)
            {
                isPlayerAtExit = true;   
            }
            else
            {
                Tutorial.Instance.ChangeStep(TutorialSteps.NeedKey);
            }
        }
    }

    public void CaughtPlayer()
    {
        isPlayerCaught = true;
    }

    private void PlayAudio(AudioSource audioSource)
    {
        if (!hasAudioPlayed)
        {
            audioSource.Play();
            hasAudioPlayed = true;
        }
    }
}
