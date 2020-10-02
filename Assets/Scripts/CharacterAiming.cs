
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
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);

        if (hooked)
        {
            currentDistance = Vector3.Distance(transform.position, Hook.position);
            if (currentDistance > 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, Hook.transform.position, Time.fixedDeltaTime * 10);
               // GetComponent<Rigidbody>().AddForce((Hook.transform.position - transform.position).normalized * 100, ForceMode.Acceleration);
            }
            else
            {
                weapon.ReturnHook();
            }
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
           // weapon.StopFiring();
        }
    }
}
