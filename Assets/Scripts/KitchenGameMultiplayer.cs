using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO _kitchenObjectListSo;
    private void Awake()
    {
        Instance = this;
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

    private int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
    {
        return _kitchenObjectListSo.kitchenObjectSOList.IndexOf(kitchenObjectSo);
    }

    private KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
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
}
