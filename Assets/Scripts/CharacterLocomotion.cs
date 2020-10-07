using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterLocomotion : MonoBehaviour
{
   

    [Header("Movement")]
    [SerializeField]
    [Tooltip("Movement speed of the player")]
    [Range(50,Mathf.Infinity)]
    private float moveSpeed = 100;

    [SerializeField]
    [Tooltip("Multiplier, depending on wether or not the player is jumping or running")]
    [Range(0.1f, 10)]
    private float multiplier = 1;
    
    //checks if the player is currently on the ground
    private bool grounded;

    
    [Header("Jumping")]
    [Space(5)]
    [SerializeField]
    [Tooltip("Seconds the player is being accelerated upwards")]
    private float secondsLeft = 1f;

    [SerializeField]
    [Tooltip("Force of the jump")]
    private float jumpingForce = 8;

    [Header("Turning")]
    [Space(5)]
    [SerializeField]
    [Tooltip("Speed at which the player rotates")]
    private float turnSpeed = 15;
    private Camera mainCamera;

    [SerializeField]
    [Tooltip("Amount of time it takes for the player to fully aim")]
    private float aimDuration = 0.3f;

    [Header("Aiming and Grappling Hook")]
    [Space(5)]
    [SerializeField]
    [Tooltip("Aniamtion layer of the aiming")]
    private Rig aimLayer;

    [SerializeField]
    [Tooltip("Direct access to the currently equipped gun")]
    private RaycastWeapon weapon;

    
    [SerializeField]
    [Tooltip("Direct access to the hook itself")]
    public Transform Hook;

    float currentDistance;

    private float animationSpeed = 1;

    private bool gliding = false;

    //checks wether or not the player is hooked
    private bool hooked;
    private float distToGround;

    //used to draw the grappling hook
    private LineRenderer lr;

    //postion of the hook for convenience
    private Vector3 grapplePoint;

    private Transform gunTip, camera;

    //used for the grapple mechanic
    private SpringJoint joint;

    private bool crouching = false;

    private bool isRunning = false;
    //original amount of seconds left so that each jump is being accelerated for the same amount of time
    private float origin;

    //keeps track of wether or not the player is currently jumping
    private bool isjumping = false;

    //private variables needed for the movement and animation of the player
    private Animator animator;
    private Vector2 input;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        grounded = true;
        origin = secondsLeft;

        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        lr = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        GetInput();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        Movement();
        Jumping();
        Turning();
        GrapplingHook();
        Animate();

     
    }

    private void LateUpdate()
    {
        if (hooked)
            DrawRope();
        if (Input.GetMouseButton(1))
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }


    }

    private void GrapplingHook()
    {
        if(hooked)
            StartGrapple();
        if (!hooked && Input.GetMouseButtonDown(0))
        {
            weapon.StartFiring();
            
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
                StopGrapple();
        }
    }


    void StartGrapple()
    {
        
        
        grapplePoint = weapon.hitInfo.point;

        rb.drag = 20;
        rb.angularDrag = 20;
        joint = this.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(this.transform.position, grapplePoint);

       // joint.connectedBody = Hook.GetComponent<Rigidbody>();

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring =1000f;
        joint.damper = 60f;
        joint.massScale = 10f;
   
    }

    void DrawRope()
    {
        lr.enabled = true;
        lr.SetVertexCount(2);
        lr.SetPosition(0, weapon.hookOrigin.transform.position);
        lr.SetPosition(1, Hook.transform.position);
    }

    void StopGrapple()
    {
        SpringJoint[] joints = GetComponents<SpringJoint>();
        rb.drag = 1;
        rb.angularDrag = 1;
        foreach (SpringJoint obj in joints)
            Destroy(obj);
        lr.positionCount = 0;
        lr.enabled = false;
        hooked = false;
    }
    private void Turning()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

    }
    private void GetInput()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
        if (grounded&&Input.GetKey(KeyCode.LeftControl))
        {
            crouching = true;
        }
           
        if (grounded&&Input.GetKeyUp(KeyCode.LeftControl))
            crouching = false;
        if (!grounded && Input.GetKey(KeyCode.LeftControl))
            gliding = true;
        if ((!grounded && Input.GetKeyUp(KeyCode.LeftControl))||grounded)
            gliding = false;

    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !isjumping)
        {
            isjumping = true;
            grounded = false;
            animator.SetBool("jumping", true);
            while (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                rb.AddForce(new Vector3(0, 1, 0) * jumpingForce, ForceMode.Acceleration);
            }
            secondsLeft = origin;
            
            
        }
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        animationSpeed = isRunning ? 1.5f : 1;
        if (crouching)
        {
            //rb.velocity += rb.transform.forward * 10;
            rb.AddForce(Vector3.down * Time.deltaTime * 30000,ForceMode.Acceleration);
            rb.AddForce(transform.forward * 10000*Time.deltaTime,ForceMode.Acceleration);
        }

        int multiplierY;
        multiplierY = crouching ? 0 : 1;
        multiplier = grounded ? 1 : 10;
        rb.AddForce(transform.forward * input.y * moveSpeed*100*multiplier*Time.fixedDeltaTime*multiplierY);
        rb.AddForce(transform.right * input.x * moveSpeed*100*multiplier*Time.fixedDeltaTime);

        if (gliding)
        {
            rb.drag = (10 - Vector3.Dot(mainCamera.transform.forward, Vector3.down) * 5);
            Debug.Log(Vector3.Dot(mainCamera.transform.forward, Vector3.down));
        }
        else
            rb.drag = 1;

        if (!grounded && !isjumping)
        {
            animator.SetBool("falling", true);
            Debug.Log("falling");
        }

    }

    private void Animate()
    {
        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputY", input.y);
        animator.speed = animationSpeed;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            animator.SetBool("falling", false);
            animator.SetBool("jumping", false);
            grounded = true;
            isjumping = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            secondsLeft = origin;
        }
    }

    public void SetHooked(bool hooked)
    {
        this.hooked = hooked;
    }
}
