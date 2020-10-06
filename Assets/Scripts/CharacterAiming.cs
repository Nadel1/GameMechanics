
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
    private Vector3 grapplePoint;
    public Transform gunTip, camera;
    private SpringJoint joint;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        lr = GetComponent<LineRenderer>();
    }


    private void FixedUpdate()
    {
        
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
        
        if (!hooked&&Input.GetMouseButtonDown(0))
        {
            weapon.StartFiring();
            StartGrapple();
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
                StopGrapple();
        }
    }

    void StartGrapple()
    {
        transform.position = Vector3.MoveTowards(transform.position, Hook.transform.position,5);
        grapplePoint = Hook.position;
            joint = this.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(this.transform.position, grapplePoint);


            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;


            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        
            
        
       
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
        Destroy(joint);
        lr.positionCount = 0;
        lr.enabled = false;
        hooked = false;
    }
    // Update is called once per frame
   // void FixedUpdate()
   // {
        /*
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

        if (hooked)
        {

            aimLayer.weight = 1;

            lr.SetVertexCount(2);
            lr.SetPosition(0, weapon.hookOrigin.transform.position);
            lr.SetPosition(1, Hook.transform.position);
            currentDistance = Vector3.Distance(transform.position, Hook.position);
            //transform.position = Vector3.MoveTowards(transform.position, Hook.transform.position, Time.fixedDeltaTime * 10);
            GetComponent<Rigidbody>().AddForce((Hook.position - transform.position).normalized * 1000, ForceMode.Acceleration);

            // GetComponent<Rigidbody>().useGravity = false;
            if (currentDistance < 1 || Input.GetMouseButtonUp(1))
            {

                weapon.ReturnHook();
                hooked = false;
            }
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = true;
            lr.SetVertexCount(0);
        }
        */

   // }

    private void LateUpdate()
    {
        if(hooked)
            DrawRope();
        //late update from here on
        if (Input.GetMouseButton(1))
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }

        
    }
    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 1f);
    }
}
