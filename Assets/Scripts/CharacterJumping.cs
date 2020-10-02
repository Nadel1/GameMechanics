using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJumping : MonoBehaviour
{
    public float secondsLeft = 0.5f;
    public float jumpingForce = 4;
    private Rigidbody rb;
    private float origin;
    private float distToGround;

    private void Start()
    {
        origin = secondsLeft;
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                rb.AddForce(new Vector3(0, 1, 0)*jumpingForce, ForceMode.Acceleration);
            }
        }
        if (isGrounded())
        {
            secondsLeft = origin;
        }
        
    }
}
