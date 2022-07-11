using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementPlayer : MonoBehaviour
{

    private Rigidbody rb;
    public float moveSpeed;
    public float rbDrag = 6f;
    public float gravity = 9.81f;
    public float jumpForce = 6f;
    public Transform groundCheck;
    public LayerMask WhatIsGround;


    //Rotation and look
    public Camera playerCam;
    public float sensitivity = 150f;
    private float xRot;
    private float moveSpeedMultifier = 10f;

    float x, z;

    bool isGrounded;

    Vector3 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //camera control
        MyInput();
        Look();
        ControlDrag();
        Jump();
        ApplyMoreGravity();
        Debug.Log(rb.velocity.magnitude);

    }

    private void FixedUpdate()
    {
        //camera control
        Look();
        PlayerMove();
    }



    void ControlDrag()
    {
        if (isGrounded)
            rb.drag = rbDrag;
        else
            rb.drag = 0.02f;
    }

    private void Look()
    {
        float x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= y;
        xRot = Mathf.Clamp(xRot, -90, 90);

        playerCam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * x);
    }

    void MyInput() {

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        direction = (transform.forward * z + transform.right * x).normalized;
        if (!isGrounded)
            direction = Vector3.zero;

    }
    void PlayerMove()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, WhatIsGround);

        rb.AddForce(direction * moveSpeed * moveSpeedMultifier * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
        rb.velocity += direction * moveSpeed * Time.fixedDeltaTime;
    }


    void ApplyMoreGravity()
    {

        rb.AddForce(Vector3.down * gravity);
    }


    void Jump()
    {
       
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity += Vector3.up * jumpForce;
        }
    }

}
