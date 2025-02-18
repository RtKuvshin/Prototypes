using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
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

    public event Action OnLocalPlayerReadyChanged;

    private NetworkVariable<State> _state = new NetworkVariable<State>();
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(5f);
    private float gamePlayingTimerMax = 120f;
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private bool isGamePaused;
    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction+= GameInputOnPauseAction;
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        _state.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(State previousvalue, State newvalue)
    {
        OnStateChanged?.Invoke();
    }

    private void GameInputOnInteractAction()
    {
        if (_state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke();
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            _state.Value = State.CountdownToStart;
        }
    }

    [ClientRpc]
    private void SetPlayerClientRpc()
    {
        
    }

    private void GameInputOnPauseAction()
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (_state.Value)
        {
           case State.WaitingToStart:
               
               break;
           case State.CountdownToStart:
               countdownToStartTimer.Value -= Time.deltaTime;
               if (countdownToStartTimer.Value < 0)
               {
                   _state.Value = State.GamePlaying;
                   gamePlayingTimer.Value = gamePlayingTimerMax;
               }
               break;
           case State.GamePlaying:
               gamePlayingTimer.Value -= Time.deltaTime;
               if (gamePlayingTimer.Value < 0)
               {
                   _state.Value = State.GameOver;
               }
               break;
           case State.GameOver:
               break;
           
        }
        //Debug.Log(_state);
    }

    public bool IsGamePlaying()
    {
        return _state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStart()
    {
        return _state.Value == State.CountdownToStart;
    }
    public bool IsGameOver()
    {
        return _state.Value == State.GameOver;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return  gamePlayingTimer.Value / gamePlayingTimerMax;
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
