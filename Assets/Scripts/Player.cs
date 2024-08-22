using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool hasKey;
    public bool HasKey
    {
        get => hasKey;
        private set => hasKey = value;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Key key))
        {
            Destroy(key.gameObject);
            HasKey = true;
        }
    }
}
