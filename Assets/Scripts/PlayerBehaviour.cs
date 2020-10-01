using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    [Header("Phyiscs attributes")]
    [SerializeField]
    [Tooltip("Used in order to smoothen the turn of the character")]
    [Range(0, 1)]
    private float turnSmoothTime = 0.1f;



    [SerializeField]
    [Tooltip("Gravitational force that will be applied")]
    private float gravity = -9.81f;

    [SerializeField]
    [Tooltip("Distance between ground and bottom of player")]
    private float groundDistance = 0.4f;

    [SerializeField]
    [Tooltip("All objects on this layer will be seen as ground objects")]
    public LayerMask groundMask;

    [SerializeField]
    [Tooltip("holder of the groundcheck object which checks wether or not the character is grounded")]
    private Transform groundCheck;


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
    [Tooltip("Speed at which the character jumps")]
    private float maxJump = 3;


    [SerializeField]
    [Tooltip("Maximum amount of jumps the player can execute without touching the floor")]
    private int jumpsAllowed = 2;

    public GameObject hookHolder;

    public GameObject hook;

    public float hookTravelSpeed;
    public float playerTravelSpeed;
    public static bool fired;
    public bool hooked;

    public float maxDistance;
    private float currentDistance;

    //internal variable that holds gravity amount since gravity gets disabled during the hooking
    private float restoreGrav;

    //internal variable that holds how many jumps the player is still allowed to execute
    private int currentJumps;
    
    //holds the character contorller
    private CharacterController controller;

    //holds wether or not the character is touching the ground, used for jumping
    private bool isGrounded;

    //holder of the velocity applied due to gravity
    private Vector3 velocity;

    //internal variable for speed as the movement speed changes between walking and running
    private float speed;

    //internal physics variable which determines how smoothly the character will be turned
    private float turnSmoothVelocity;

    //holder of the camera which is used for the rotation
    private Transform cam;

    

    //internal variable for the jumpheight as it differs depending on the movement speed
    private float jumpHeight;

    private Vector3 moveTo = Vector3.zero;

    // Start is called before the first frame update
    void Start() { 

        //the necessary holder variables are initialised
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        jumpHeight=maxJump;
        restoreGrav = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        //adjust the speeds and jump height depending on if the player is running
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        jumpHeight = speed == runSpeed ? maxJump : maxJump * 0.5f;

        //check wether or not the character is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //restes the y-velocity in case character is grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            currentJumps = jumpsAllowed;
        }

        //retrieval of input (wasd-keys)
        float hoizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hoizontal, 0, vertical).normalized;

        //calculation of the angle of the character depending on the camera
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //only move the character when the direction magnitude does not equal 0
        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

       
        

        //jump mechanic by increasing the y-velocity
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded|| currentJumps > 0)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                currentJumps--;
            }
            
        }

        

        if (Input.GetMouseButtonDown(0) && !fired)
        {
            fired = true;

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit && hitInfo.transform.gameObject.tag == "Hookable")
            {
                Debug.DrawRay(hookHolder.transform.position, hitInfo.point, Color.red, 50, false);
                moveTo = (hitInfo.point- hookHolder.transform.position).normalized;
            }
        }


        if (fired&&!hooked)
        {
            //hook.transform.Translate(-Vector3.forward * Time.deltaTime * hookTravelSpeed);
            
            hook.transform.Translate(moveTo*Time.deltaTime* hookTravelSpeed);
            currentDistance = Vector3.Distance(transform.position, hook.transform.position);
            if (currentDistance >= maxDistance)
                ReturnHook();
        }

        if (hooked)
        {
            transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, playerTravelSpeed);
            float distanceToHook = Vector3.Distance(transform.position, hook.transform.position);

            gravity = 0;

            if (distanceToHook < 1)
                ReturnHook();
        }
        else
        {
            gravity = restoreGrav;
        }

        //apply gravity and movement of player
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);
    }

    private void ReturnHook()
    {
        hook.transform.position = hookHolder.transform.position;
        fired = false;
        hooked = false;
        moveTo = Vector3.zero;
    }

    private void MoveHook()
    {
        //hook.transform.Translate(-Vector3.forward * Time.deltaTime * hookTravelSpeed);
        hook.GetComponent<Rigidbody>().velocity = Vector3.forward * hookTravelSpeed;
    }
}