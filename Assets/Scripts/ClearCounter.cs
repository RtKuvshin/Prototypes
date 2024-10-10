using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSo;
    [SerializeField] private Transform counterTopPoint;
    public void Interact()
    {
        Transform kitchenObjTransform = Instantiate(_kitchenObjectSo.prefab, counterTopPoint);
        kitchenObjTransform.localPosition = Vector3.zero;
        
        Debug.Log(kitchenObjTransform.GetComponent<KitchenObject>().GetKitchenObjectSO().objName);
    }
}
