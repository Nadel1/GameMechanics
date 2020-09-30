using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    [Header("Movement attributes")]
    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character moves while walking")]
    private float walkSpeed = 5;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character moves while running")]
    private float runSpeed = 10;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character rotates while walking")]
    private float rotSpeedWalk = 5;

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

    public enum State { Idle, Walking, Running, Jumping };

    [Space(20)]
    public State currentState = State.Idle;

    //private variables

    //private Rigidbody rb;

    public CharacterController controller;
    public float speed;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform cam;
    
 
    public float gravity = -9.81f;
    Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public LayerMask groundMask;
    public float jumpHeight = 3;

    bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {

        // rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float hoizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hoizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);
    }
}