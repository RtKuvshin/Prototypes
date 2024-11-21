using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private PlateIconSingleUI iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnIngredientAdded;
    }

    private void PlateKitchenObjectOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate.transform)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        foreach (var kitchenObjectSo in _plateKitchenObject.GetKitchenObjectSOList() )
        {
            PlateIconSingleUI iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.SetImage(kitchenObjectSo);
        }
    }
    
}
