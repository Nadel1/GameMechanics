using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    
    [Header("Movement attributes")]
    [SerializeField]
    [Range(1,50)]
    [Tooltip("Speed at which the character moves while walking")]
    private float walkSpeed=10;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character moves while running")]
    private float runSpeed=10;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character rotates")]
    public float rotSpeed=1;

    [SerializeField]
    [Range(1, 50)]
    [Tooltip("Speed at which the character jumps")]
    public float jumpSpeed = 20;


    //private variables
    private Rigidbody rb;
    private bool running = false;
    private bool jumping;
    private bool grounded;
    private Vector3 moveDir=new Vector3(0,0,0);
    private Vector3 groundPos;
    private int gravity;

    private float secondsLeft = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = grounded ? 0 : 1;
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

  

        rb.MovePosition(transform.position + walkSpeed * moveDir * Time.fixedDeltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumping = !jumping;

        }

        if (jumping)
        {
            while (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                rb.AddForce(new Vector3(0,2,0)*jumpSpeed, ForceMode.Acceleration);
            }
            jumping = !jumping;
            secondsLeft = 0.5f;
        }
        rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + new Vector3(0f, rotSpeed * Input.GetAxis("Mouse X"), 0f));
        moveDir = Vector3.zero;
        groundPos = new Vector3(transform.position.x, 0f, transform.position.z);

    }

   

}
