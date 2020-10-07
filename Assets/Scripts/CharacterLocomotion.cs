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

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
        GetInput();
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
        /*
        
        grapplePoint = weapon.hitInfo.point;

        
        joint = this.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(this.transform.position, grapplePoint);

        //joint.connectedBody = Hook.GetComponent<Rigidbody>();

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring =20.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
       
        
    */
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

    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !isjumping)
        {
            isjumping = true;
            grounded = false;
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
        multiplier = grounded ? 1 : isRunning ? 2 : 0.5f;
        

        rb.AddForce(transform.forward * input.y * moveSpeed*multiplier*Time.deltaTime);
        rb.AddForce(transform.right * input.x * moveSpeed*multiplier*Time.deltaTime);
    }
    private void Animate()
    {
        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputY", input.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
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
