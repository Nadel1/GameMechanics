using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;

    public float walkSpeed;
    public float runSpeed;
    public float rotSpeed;

    private bool running = false;
    private Vector3 moveDir=new Vector3(0,0,0);
    // Start is called before the first frame update
    void Start()
    {
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

        rb.MovePosition(transform.position + walkSpeed * moveDir * Time.fixedDeltaTime);

        rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + new Vector3(0f, rotSpeed * Input.GetAxis("Mouse X"), 0f));
        moveDir = Vector3.zero;
    }
}
