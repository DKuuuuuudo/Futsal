using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Vector3 targetPos;

    void Start()
    {
        targetPos = this.transform.position;
    }

    void Update()
    {

        Camera.main.transform.position += this.transform.position - targetPos;
        targetPos = this.transform.position;

        if (Input.GetMouseButton(1))
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            Camera.main.transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * 400f);
        }
    }
}

