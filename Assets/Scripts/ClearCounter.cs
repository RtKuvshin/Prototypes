using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSo;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private ClearCounter secondCounter;
    [SerializeField] private bool isTesting;
    
    private KitchenObject _kitchenObject;
    public void Interact()
    {
        if (_kitchenObject == null)
        {
            Transform kitchenObjTransform = Instantiate(_kitchenObjectSo.prefab, counterTopPoint);
            kitchenObjTransform.GetComponent<KitchenObject>().SetClearCounter(this);
        }
        else
        {
            Debug.Log(_kitchenObject.GetClearCounter());
        }
    }

    private void Update()
    {
        if (isTesting && Input.GetKeyDown(KeyCode.F))
        {
            if(_kitchenObject != null)
            {
              _kitchenObject.SetClearCounter(secondCounter);  
            }
        }
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
}
