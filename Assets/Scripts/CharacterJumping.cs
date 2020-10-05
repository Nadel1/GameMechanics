﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJumping : MonoBehaviour
{
    public float secondsLeft = 0.5f;
    public float jumpingForce = 4;
    private Rigidbody rb;
    private float origin;
    private float distToGround;
    private Vector3 startPos;
    public float maxDistance = 5;
    private bool isjumping = false;
    private bool isGrounded;

    private void Start()
    {
        origin = secondsLeft;
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&isGrounded&&!isjumping)
        {
            isjumping = true;
            isGrounded = false;
            while (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                rb.AddForce(new Vector3(0, 1, 0) * jumpingForce, ForceMode.Acceleration);

            }
        }
  
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Environment")
        {
            isGrounded = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("grounded!");
            isjumping = false;
            secondsLeft = origin;
        }
    }
}
