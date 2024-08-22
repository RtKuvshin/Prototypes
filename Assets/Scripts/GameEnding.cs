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
    private float timer;
    private float delay = 2f;
    private bool isPlayerAtExit;
    private bool isPlayerCaught;
    
    
    private void EndLevel(CanvasGroup canvasGroup, bool doRestart)
    {
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
                #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
                #else
                    Application.Quit();
                #endif
            }
        }
    }
    private void Update()
    {
        if (isPlayerAtExit)
        {
            EndLevel(exitPanel, false);
        }
        else if (isPlayerCaught)
        {
            EndLevel(caughtPanel, true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && player.HasKey)
        {
            isPlayerAtExit = true;
        }
    }

    public void CaughtPlayer()
    {
        isPlayerCaught = true;
    }
}
