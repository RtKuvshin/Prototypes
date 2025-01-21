using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    public static KitchenGameManager Instance { get; private set; }

    public event Action OnStateChanged;
    public event Action OnGamePaused;
    public event Action OnGameUnpaused;

    private State _state;
    private float countdownToStartTimer = 5f;
    private float gamePlayingTimerMax = 120f;
    private float gamePlayingTimer;
    private bool isGamePaused;

    private void Awake()
    {
        Instance = this;
        _state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction+= GameInputOnPauseAction;
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
    }

    private void GameInputOnInteractAction()
    {
        if (_state == State.WaitingToStart)
        {
            _state = State.CountdownToStart;
            OnStateChanged?.Invoke();
        }
    }

    private void GameInputOnPauseAction()
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (_state)
        {
           case State.WaitingToStart:
               
               break;
           case State.CountdownToStart:
               countdownToStartTimer -= Time.deltaTime;
               if (countdownToStartTimer < 0)
               {
                   _state = State.GamePlaying;
                   gamePlayingTimer = gamePlayingTimerMax;
                   OnStateChanged?.Invoke();
               }
               break;
           case State.GamePlaying:
               gamePlayingTimer -= Time.deltaTime;
               if (gamePlayingTimer < 0)
               {
                   _state = State.GameOver;
                   OnStateChanged?.Invoke();
               }
               break;
           case State.GameOver:
               break;
           
        }
        //Debug.Log(_state);
    }

    public bool IsGamePlaying()
    {
        return _state == State.GamePlaying;
    }

    public bool IsCountdownToStart()
    {
        return _state == State.CountdownToStart;
    }
    public bool IsGameOver()
    {
        return _state == State.GameOver;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return  gamePlayingTimer / gamePlayingTimerMax;
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1;
        if (isGamePaused)
        {
            OnGamePaused?.Invoke();
        }
        else
        {
            OnGameUnpaused?.Invoke();
        }
    }
    
}
