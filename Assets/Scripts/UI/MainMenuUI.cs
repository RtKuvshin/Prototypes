using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayClick);
        quitButton.onClick.AddListener(QuitClick);

        Time.timeScale = 1;
    }

    private void PlayClick()
    {
        Loader.Load(Loader.Scene.GameScene);
    }
    private void QuitClick()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); 
#endif
    }
    
}
