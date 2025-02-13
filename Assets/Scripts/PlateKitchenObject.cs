using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO addedKitchenObjectSo;
    }
    
    [SerializeField] private List<KitchenObjectSO> _validKitchenObjectSOList;
    private List<KitchenObjectSO> _kitchenObjectSOList = new List<KitchenObjectSO>();

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
    {
        if (!_validKitchenObjectSOList.Contains(kitchenObjectSo)) return false;
        
        if (_kitchenObjectSOList.Contains(kitchenObjectSo))
        {
            return false;
        }
        else
        {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(kitchenObjectSo));
            
            return true;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitcheObjectSOIndex)
    {
        AddIngredientClientRpc(kitcheObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSo);
            
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs()
        {
            addedKitchenObjectSo = kitchenObjectSo
        });
    }
    
    
    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return _kitchenObjectSOList;
    }
    
    
}
