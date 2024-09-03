using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool hasKey;
    private bool isNearKey;
    private Key _key;
    public bool HasKey
    {
        get => hasKey;
        private set => hasKey = value;
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Key key))
        {
            isNearKey = true;
            _key = key;
            Tutorial.Instance.ChangeStep(TutorialSteps.Grab);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Key key))
        {
            isNearKey = false;
            _key = null;
        }
    }

    private void Update()
    {
        if (isNearKey && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(_key.gameObject);
            isNearKey = false;
            hasKey = true;
            Tutorial.Instance.ChangeStep(TutorialSteps.Exit);
        } 
    }
}
