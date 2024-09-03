using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private float turnSpeed;
    
    private AudioSource audioSource;
    private Vector3 moveDirection;
    private Animator animator;
    private Quaternion rotation;
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        Movement();
        Rotation();
        Animation();
    }

    private void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        moveDirection.Set(horizontalInput, 0, verticalInput);
        moveDirection.Normalize();
    }
    private void Rotation()
    {
        Vector3 lookDirection = Vector3.RotateTowards(transform.forward, moveDirection, turnSpeed * Time.deltaTime, 0);
        rotation = Quaternion.LookRotation(lookDirection);
    }
    private void Animation()
    {
        bool hasVerticalInput =  !Mathf.Approximately(verticalInput, 0f) ;
        bool hasHorizontalInput = !Mathf.Approximately(horizontalInput, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        animator.SetBool(IS_WALKING, isWalking);
        
        PlayFootsteps(isWalking);
    }

    private void PlayFootsteps(bool isWalking)
    {
        if (isWalking)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
    private void OnAnimatorMove()
    {
            rb.MovePosition(rb.position + moveDirection* animator.deltaPosition.magnitude);
            rb.MoveRotation(rotation);
    }
    
}
