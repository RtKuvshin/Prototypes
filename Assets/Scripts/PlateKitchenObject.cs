using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
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
            return true;
        }
        
    }
}
