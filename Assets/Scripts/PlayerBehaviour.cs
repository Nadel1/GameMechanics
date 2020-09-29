using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    
    [Header("Movement attributes")]
    [SerializeField]
    [Range(1,50)]
    [Tooltip("Speed at which the character moves while walking")]
    private float walkSpeed=5;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character moves while running")]
    private float runSpeed=10;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character rotates while walking")]
    private float rotSpeedWalk=5;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character rotates while running")]
    private float rotSpeedRun = 2;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character jumps")]
    private float jumpSpeed = 20;

    [SerializeField]
    private enum State { Idle,Walking,Running,Jumping};

    [SerializeField]
    private State currentState = State.Idle;

    //private variables
    private Rigidbody rb;
    private bool jumping;
    private Vector3 moveDir=new Vector3(0,0,0);
    private Vector3 groundPos;
    private float jumpMultiplier = 1;
    private float speed;

    private float rotSpeed;
    private float secondsLeft = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckInput();
        Jumping();
        Turn();
    }


    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.W))
            moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDir += transform.right;

        if (Input.GetKeyDown(KeyCode.LeftShift)&&speed==walkSpeed)
            currentState = State.Running;
        if (Input.GetKeyUp(KeyCode.LeftShift) && speed == runSpeed)
                currentState = State.Walking;
        

        speed = currentState.Equals(State.Running) ? runSpeed : walkSpeed;
        Debug.Log(speed);
        rb.MovePosition(transform.position + speed* moveDir * Time.fixedDeltaTime);
        if (!moveDir.Equals(Vector3.zero) && currentState != State.Idle && currentState != State.Running)
            currentState = State.Walking;
        moveDir = Vector3.zero;
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumping = !jumping;

        }

        if (jumping)
        {
            if (currentState.Equals(State.Running))
                jumpMultiplier = 2f;
            else
                jumpMultiplier = 1f;


            while (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                rb.AddForce(new Vector3(0, 2*jumpMultiplier, 0) * jumpSpeed, ForceMode.Acceleration);
            }
            jumping = !jumping;
            secondsLeft = 0.5f;
        }
        groundPos = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void Turn()
    {
        rotSpeed = currentState.Equals(State.Running) ? rotSpeedRun : rotSpeedWalk;

        rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + new Vector3(0f, rotSpeed * Input.GetAxis("Mouse X"), 0f));
    }

}
