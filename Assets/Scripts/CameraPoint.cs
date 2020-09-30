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
    void FixedUpdate()
    {
        Vector3 oldPos = transform.position;
        float angle = Input.GetAxisRaw("Mouse Y");
        //angle=Mathf.Clamp(angle, -0.5f, 0.5f);
        transform.RotateAround(rotateAround.position, new Vector3(-1, 0, 0), rotSpeed * angle);
        transform.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        float height = Mathf.Clamp(transform.position.y, 1, 2.5f);
        Vector3 newPos = new Vector3(transform.position.x, height, transform.position.z);
        Debug.Log(height);
        /*
        if (Vector3.Distance(newPos, rotateAround.position) < 2.5f)
        {
            Debug.Log(Vector3.Distance(newPos, rotateAround.position));
            float yVal = newPos.y;
            newPos=(newPos-rotateAround.position).normalized* 2.5f;
            newPos = new Vector3(newPos.x, yVal, newPos.z);
            
        }*/
        if(Vector3.Distance(newPos, rotateAround.position) > 2.5f)
        {
            transform.position = newPos;
        }
        else
        {
            transform.position = oldPos;
        }

        

    }
}
