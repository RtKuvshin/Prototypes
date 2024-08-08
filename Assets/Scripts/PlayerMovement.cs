using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private float turnSpeed;
    
    private Vector3 moveDirection;
    private Animator animator;
    private Quaternion rotation;
    private Rigidbody rb;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        moveDirection.Set(horizontalInput, 0, verticalInput);
        moveDirection.Normalize();

        bool hasVerticalInput =  !Mathf.Approximately(verticalInput, 0f) ;
        bool hasHorizontalInput = !Mathf.Approximately(horizontalInput, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        animator.SetBool(IS_WALKING, isWalking);

        Vector3 lookDirection = Vector3.RotateTowards(transform.forward, moveDirection, turnSpeed * Time.deltaTime, 0);
        rotation = Quaternion.LookRotation(lookDirection);
    }

    private void OnAnimatorMove()
    {
            rb.MovePosition(rb.position + moveDirection* animator.deltaPosition.magnitude);
            rb.MoveRotation(rotation);
    }
    
}
