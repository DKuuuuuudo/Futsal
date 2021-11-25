using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = target.transform.position;
    }

    void Update()
    {
        transform.position += target.transform.position - targetPos;
        targetPos = target.transform.position;

        if (Input.GetMouseButton(1))
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = Input.GetAxis("Mouse Y");
            transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * 200f);
        }
    }
}

