using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject target;
    private Vector3 targetPos;

    void Start()
    {
        if (photonView.IsMine)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            targetPos = target.transform.position;
            this.transform.LookAt(target.transform);
        }
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

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }
}

