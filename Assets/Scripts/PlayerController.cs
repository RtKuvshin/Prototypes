using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        rb.AddForce(Vector3.forward * (speed * forwardInput));
    }
}
