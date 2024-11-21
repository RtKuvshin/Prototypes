using System;
using System.Collections;
using System.Collections.Generic;
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
            _kitchenObjectSOList.Add(kitchenObjectSo);
            
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs()
            {
                addedKitchenObjectSo = kitchenObjectSo
            });
            
            return true;
        }
        
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return _kitchenObjectSOList;
    }
}
