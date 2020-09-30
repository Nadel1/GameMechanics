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

    [Space(20)]
    public float grappleSpeed;

    public enum State { Idle,Walking,Running,Jumping};

    [Space(20)]
    public State currentState = State.Idle;

    //private variables

    private Rigidbody rb;

    private bool jumping;
    private Vector3 moveDir=new Vector3(0,0,0);
    private Vector3 groundPos;
    private float jumpMultiplier = 1;
    private float speed;

    private float rotSpeed;
    private float secondsLeft = 0.5f;

    private Camera cam;
    private RaycastHit grapplePoint;
    private bool isGrappling;
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
            moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDir += transform.right;

        if (Input.GetKeyDown(KeyCode.LeftShift) && speed == walkSpeed)
            currentState = State.Running;
        if (Input.GetKeyUp(KeyCode.LeftShift) && speed == runSpeed)
            currentState = State.Walking;


        speed = currentState.Equals(State.Running) ? runSpeed : walkSpeed;
        if (moveDir.Equals(Vector3.zero))
        {
            rb.velocity = Vector3.zero;
        }

        if (Input.GetKeyUp(KeyCode.W) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.D))
        {
            moveDir = Vector3.zero;
            rb.velocity = Vector3.zero;
        }

       rb.MovePosition(rb.position + speed * moveDir * Time.fixedDeltaTime);
        
        if (!moveDir.Equals(Vector3.zero) && currentState != State.Idle && currentState != State.Running)
            currentState = State.Walking;
        moveDir = Vector3.zero;
        Jumping();
        rotSpeed = currentState.Equals(State.Running) ? rotSpeedRun : rotSpeedWalk;

        Quaternion rot= Quaternion.Slerp(transform.rotation,Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, rotSpeed * Input.GetAxisRaw("Mouse X"), 0f)),0.6f);
        //transform.rotation = rot;


        float h = 2 * Input.GetAxis("Mouse X");
        float v =2 * Input.GetAxis("Mouse Y");
        transform.Rotate(0, h, 0);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotSpeed * Time.fixedDeltaTime, 0), 0.5f);

       // rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, rotSpeed * Input.GetAxisRaw("Mouse X")*Time.fixedDeltaTime, 0f)));
        //transform.Rotate(0, Input.GetAxis("Mouse X") * rotSpeed * Time.fixedDeltaTime, 0);

    }

    /*
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
        if (moveDir.Equals(Vector3.zero))
        {
            rb.velocity = Vector3.zero;
        }
        rb.MovePosition(rb.position + speed* moveDir * Time.fixedDeltaTime);
        if (Input.GetKeyUp(KeyCode.W) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.D))
        {
            moveDir = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        if (!moveDir.Equals(Vector3.zero) && currentState != State.Idle && currentState != State.Running)
            currentState = State.Walking;
        moveDir = Vector3.zero;
    }
    */
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
                
                //rb.AddForce(new Vector3(0, 2*jumpMultiplier, 0) * jumpSpeed, ForceMode.Acceleration);
            }
            jumping = !jumping;
            secondsLeft = 0.5f;
        }
        groundPos = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void Turn()
    {
        
    }
   
}


/*
   private void Update()

   {
       Ray ray = cam.ScreenPointToRay(Input.mousePosition);
       if(Input.GetKeyDown(KeyCode.Mouse0)&&Physics.Raycast(ray,out grapplePoint))
       {
           isGrappling = true;
           Vector3 grappleDirection = (grapplePoint.point - transform.position);
           rb.velocity = grappleDirection.normalized * grappleSpeed;
       }

       if (Input.GetKeyUp(KeyCode.Mouse0))
           isGrappling = false;

       if (isGrappling)
       {
           transform.LookAt(grapplePoint.point);

           Vector3 grappleDirection = (grapplePoint.point - transform.position);

           if (distance < grappleDirection.magnitude)
           {
               float velocity = rb.velocity.magnitude;
               Vector3 newDirection = Vector3.ProjectOnPlane(rb.velocity, grappleDirection);

               rb.velocity = newDirection.normalized * velocity;
           }
           else
           {
               rb.AddForce(grappleDirection.normalized * grappleSpeed);
               distance = grappleDirection.magnitude;
           }
       }
       else
       {
           transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
       }
   }*/
