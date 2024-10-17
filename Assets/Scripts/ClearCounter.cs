using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSo;
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
}
