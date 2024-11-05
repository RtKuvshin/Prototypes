using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    [SerializeField] private FryingRecipeSO[] _fryingRecipeSoArray;

    private float fryingTimer;
    private FryingRecipeSO _fryingRecipeSo;

    private void Update()
    {
        if (this.HasKitchenObject())
        {
            fryingTimer += Time.deltaTime;

            if (fryingTimer > _fryingRecipeSo.fryingTimeMax)
            {
                fryingTimer = 0;
                Debug.Log("Fried!");
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
            }
            Debug.Log(fryingTimer);
        }
    }

    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _fryingRecipeSo = GetFryingRecipeSoWithInput(this.GetKitchenObject().GetKitchenObjectSO());
                }
            }
        }
        else
        {
            if (player.HasKitchenObject() == false)
            {
                this.GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSO);
        return fryingRecipeSo != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSO);
        return fryingRecipeSo.output;
    }

    private FryingRecipeSO GetFryingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSo in _fryingRecipeSoArray)
        {
            if (fryingRecipeSo.input == inputKitchenObjectSO)
            {
                return fryingRecipeSo;
            }
        }

        return null;
    }
}
