using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
   [SerializeField] private float speed = 5f;
   private InputActions inputActions;
   private Rigidbody rb;
   private void Start()
   {
      rb = GetComponent<Rigidbody>();
      inputActions = new InputActions();
      inputActions.VehicleController.Move.Enable();
   }

   private void Update()
   {
      
      Vector2 inputVector = inputActions.VehicleController.Move.ReadValue<Vector2>();
      inputVector = inputVector.normalized;
      
      Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
      //transform.Translate(moveDirection * (speed * Time.deltaTime));
      rb.AddForce(Vector3.forward * speed);

   }

   
}
