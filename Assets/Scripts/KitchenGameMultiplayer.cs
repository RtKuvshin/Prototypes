using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }
    public static bool playMultiplayer;
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer"; 

    public event Action OnTryingToJoinGame;
    public event Action OnFailToJoinGame;
    public event Action OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectListSO _kitchenObjectListSo;
    [SerializeField] private List<Color> playerColorList;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        
        PlayerPrefs.SetString(PLAYER_NAME_MULTIPLAYER, playerName);
    }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_NAME_MULTIPLAYER, "PlayerName " + Random.Range(0, 1000));
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnListChanged;
    }

    private void Start()
    {
        if (!playMultiplayer)
        {
            StartHost();
            Loader.LoadNetwork(Loader.Scene.GameScene); 
        }
    }

    private void PlayerDataNetworkListOnListChanged(NetworkListEvent<PlayerData> changeevent)
    {
        OnPlayerDataNetworkListChanged?.Invoke();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerServerOnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManagerServerOnClientDisconnectCallback(ulong client)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientID == client)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManagerOnClientConnectedCallback(ulong obj)
    {
        playerDataNetworkList.Add(new PlayerData{clientID = obj, colorId = GetFirstUnusedColorId() });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse arg2)
    {
        //Debug.Log(NetworkManager.Singleton.ConnectedClientsIds.Count);
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            arg2.Approved = false;
            arg2.Reason = "Game Has Already Started!";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            arg2.Approved = false;
            arg2.Reason = "Game Is Full";
            return;
        }
        
        arg2.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke();
        
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerClientOnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerClientOnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManagerClientOnClientConnectedCallback(ulong obj)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerData;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerId = playerId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManagerClientOnClientDisconnectCallback(ulong obj)
    {
        OnFailToJoinGame ?.Invoke();
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo,
        IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSoIndex(kitchenObjectSo), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex,
        NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSo = GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        Transform kitchenObjTransform = Instantiate(kitchenObjectSo.prefab);
        
        NetworkObject kitchenObjectNetworkObject = kitchenObjTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        
        KitchenObject kitchenObject = kitchenObjTransform.GetComponent<KitchenObject>();
        
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
    {
        return _kitchenObjectListSo.kitchenObjectSOList.IndexOf(kitchenObjectSo);
    }

    public KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
    {
        return _kitchenObjectListSo.kitchenObjectSOList[kitchenObjectSoIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorID)
    {
        return playerColorList[colorID];
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientID == clientId)
            {
                return playerData;
            }
        }

        return default;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientID == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }

        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    public void KickPlayer(ulong clientID)
    {
        NetworkManager.Singleton.DisconnectClient(clientID);
        NetworkManagerServerOnClientDisconnectCallback(clientID);
    }
}
