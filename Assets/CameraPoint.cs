using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private float rotSpeed = 1;

    public Transform rotateAround;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Input.GetAxis("Mouse Y");
        Mathf.Clamp(angle, -0.5f, 0.5f);
        Debug.Log(angle);
        transform.RotateAround(rotateAround.position, new Vector3(-1, 0, 0), 5 * angle);

        transform.LookAt(rotateAround.position);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-rotSpeed * Input.GetAxis("Mouse Y"),0f , 0f));
    }
}
