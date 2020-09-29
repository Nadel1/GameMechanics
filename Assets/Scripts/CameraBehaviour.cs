using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform cameraPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, cameraPoint.position, 0.5f);
        transform.rotation = Quaternion.Euler(cameraPoint.rotation.eulerAngles.x, cameraPoint.rotation.eulerAngles.y, 0);
    }
}
