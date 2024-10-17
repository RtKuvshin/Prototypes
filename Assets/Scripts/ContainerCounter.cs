using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event Action OnPlayerGrabbedObject; 
    [SerializeField] private KitchenObjectSO _kitchenObjectSo;
    

    public override void Interact(Player player)
    {
        Transform kitchenObjTransform = Instantiate(_kitchenObjectSo.prefab);
            kitchenObjTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
            OnPlayerGrabbedObject?.Invoke();
    }
    
   
}
