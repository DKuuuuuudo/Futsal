using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public float speed = 6.0F;       //���s���x
    public float jumpSpeed = 8.0F;   //�W�����v��
    public float gravity = 20.0F;    //�d�͂̑傫��

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float h, v;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(h, 0, v);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

}
