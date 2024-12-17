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

    private State _state;
    private float waitingToStartTimer = 3f;
    private float countdownToStartTimer = 5f;
    private float gamePlayingTimerMax = 20f;
    private float gamePlayingTimer;

    private void Awake()
    {
        Instance = this;
        _state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (_state)
        {
           case State.WaitingToStart:
               waitingToStartTimer -= Time.deltaTime;
               if (waitingToStartTimer < 0)
               {
                   _state = State.CountdownToStart;
                   OnStateChanged?.Invoke();
               }
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
        Debug.Log(_state);
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
    
}
