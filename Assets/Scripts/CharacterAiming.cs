
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;



public class CharacterAiming : MonoBehaviour
{
    public float turnSpeed = 15;
    private Camera mainCamera;
    public float aimDuration = 0.3f;

    public Rig aimLayer;

    public RaycastWeapon weapon;

    public bool hooked;
    public Transform Hook;

    float currentDistance;

    private float distToGround;

    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        lr = Hook.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

        if (hooked)
        {
            aimLayer.weight = 1;
            
            lr.SetVertexCount(2);
            lr.SetPosition(0, weapon.hookOrigin.transform.position);
            lr.SetPosition(1, Hook.transform.position);
            currentDistance = Vector3.Distance(transform.position, Hook.position);
            transform.position = Vector3.MoveTowards(transform.position, Hook.transform.position, Time.fixedDeltaTime * 10);
            GetComponent<Rigidbody>().useGravity = false;
            if (currentDistance < 1)
            {
                
                weapon.ReturnHook();
            }
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = true;
            lr.SetVertexCount(0);
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }

        if (Input.GetMouseButtonDown(0))
        {
            weapon.StartFiring();
        }
        if (!Input.GetMouseButtonDown(0))
        {
           
        }
    }
    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 1f);
    }
}
