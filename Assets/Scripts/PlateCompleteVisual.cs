using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public GameObject gameObject;
        public KitchenObjectSO kitchenObjectSo;
    }
    
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> _kitchenObjectSoGameObjectsList;

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnIngredientAdded;
        
        foreach (var kitchenObjectSo in _kitchenObjectSoGameObjectsList)
        {
            kitchenObjectSo.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObjectOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (var kitchenObjectSo in _kitchenObjectSoGameObjectsList)
        {
            if (kitchenObjectSo.kitchenObjectSo == e.addedKitchenObjectSo)
            {
                kitchenObjectSo.gameObject.SetActive(true);
            }
        }
    }
}
