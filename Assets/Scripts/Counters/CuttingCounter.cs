using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO _cutKitchenObjectSO; 
    public override void Interact(Player player)
    {
        if (HasKitchenObject() == false)
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
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

    public override void InteractAlternate(Player player)
    {
        if (this.HasKitchenObject())
        {
            this.GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(_cutKitchenObjectSO, this);
        }
    }
}
