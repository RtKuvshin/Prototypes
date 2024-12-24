using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;

    public new static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    
    public event Action OnCut;
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;
    
    [FormerlySerializedAs("_cutKitchenObjectSO")] [SerializeField] private CuttingRecipeSO[] _cuttingRecipeObjectSOArray;

    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    
                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                this.GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (this.HasKitchenObject() && HasRecipeWithInput(this.GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            OnCut?.Invoke();
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(this.GetKitchenObject().GetKitchenObjectSO());
            
            OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSo.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
            {
                ///Debug.Log(cuttingProgress + " : " + cuttingRecipeSo.cuttingProgressMax);
                KitchenObjectSO outputKitchenObjectSo = GetOutputForInput(this.GetKitchenObject().GetKitchenObjectSO());
                this.GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(inputKitchenObjectSO);
        return cuttingRecipeSo != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSo = GetCuttingRecipeSoWithInput(inputKitchenObjectSO);
        return cuttingRecipeSo.output;
    }

    private CuttingRecipeSO GetCuttingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSo in _cuttingRecipeObjectSOArray)
        {
            if (cuttingRecipeSo.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSo;
            }
        }

        return null;
    }
    
    
}
