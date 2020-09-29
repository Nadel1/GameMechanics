using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform cameraPoint;
    public Transform rotateAround;
    void FixedUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, cameraPoint.position,0.3f);
        transform.LookAt(rotateAround.position);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
