using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public event Action OnLocalGamePaused;
    public event Action OnLocalGameUnpaused;
    public event Action OnLocalPlayerReadyChanged;
    public event Action OnMultiplayerGamePaused;
    public event Action OnMultiplayerGameUnpaused;

    [SerializeField] private Transform playerPrefab;
    
    private NetworkVariable<State> _state = new NetworkVariable<State>();
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(5f);
    private float gamePlayingTimerMax = 120f;
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private bool isLocalGamePaused;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerPausedDictionary = new Dictionary<ulong, bool>();
    private bool autoTestGamePausedState;

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
        isGamePaused.OnValueChanged += OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
        }
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong obj)
    {
        autoTestGamePausedState = true;
    }

    private void OnValueChanged(bool previousvalue, bool newvalue)
    {
        Time.timeScale = isGamePaused.Value ? 0 : 1;
        if (isGamePaused.Value)
        {
            OnMultiplayerGamePaused?.Invoke();
        }
        else
        {
            OnMultiplayerGameUnpaused?.Invoke();
        }
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

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }

    public bool IsGamePlaying()
    {
        return _state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStart()
    {
        return _state.Value == State.CountdownToStart;
    }

    public bool IsWaitingToStart()
    {
        return _state.Value == State.WaitingToStart;
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
        isLocalGamePaused = !isLocalGamePaused;
        
        if (isLocalGamePaused)
        {
            GamePauseServerRpc();
            OnLocalGamePaused?.Invoke();
        }
        else
        {
            GameUnpauseServerRpc();
            OnLocalGameUnpaused?.Invoke();
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void GamePauseServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    } 
    [ServerRpc (RequireOwnership = false)]
    private void GameUnpauseServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        bool allClientsReady = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }

        isGamePaused.Value = false;
    }
    
}
