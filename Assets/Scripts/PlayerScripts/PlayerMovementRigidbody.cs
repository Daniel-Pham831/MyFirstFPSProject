using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{

    //movement 

    public float moveSpeed = 5f;
    public float runSpeedPercent = 1.3f;
    public float jumpForce = 15f;
    public float gravity = 9.81f;
    public float slopeLimit = 50f;
    public float slidingSpeed = 1f;
    public Camera playerCam;
    public Transform groundCheck;
    public float groundCheckDistance = 0.3f;
    public LayerMask WhatIsGround;




    private float crouchHeight, normalHeight;
    private bool isRunning, isCrouching, isGrounded;
    private Vector3 direction;
    private Rigidbody rb;
    private RaycastHit slopeHit;
    private Vector3 slidingDirection;
    private float finalSpeed;
    private CapsuleCollider capsule;
    private float rbDrag = 5;
    private float moveSpeedMultifier = 10f;

    //Rotation and look
    public float sensitivity = 50f;
    private float xRot;

    float x;
    float z;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        normalHeight = capsule.height;
        crouchHeight = normalHeight * 0.5f;
    }

    void Update()
    {

        MyInput();
        
        //slope handling
        OnSlope();
        OnSlopeRedirection();


        //jumping and Crouch
        CheckOnGround();
        Jump();
        Crouch();


        //camera control
        Look();


        ControlDrag();
        SpeedModifier();

       /*
        Vector2 speed = new Vector2(rb.velocity.x, rb.velocity.z);
        Debug.Log(speed.magnitude);
       */
    }

    private void FixedUpdate()
    {
      
        //camera control
        Look();

        ApplyMoreGravity();
        Move();        
    }

    void MyInput()
    {

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        direction = (transform.forward * z + transform.right * x).normalized;

        if (!isGrounded)
            direction = direction * 0.05f;
    }
    void Move()
    {
        // rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
       
        //rb.AddForce(direction * moveSpeed * moveSpeedMultifier * Time.fixedDeltaTime, ForceMode.VelocityChange);


        rb.AddForce(slidingDirection*0.5f + direction * finalSpeed * moveSpeedMultifier * Time.fixedDeltaTime, ForceMode.VelocityChange);

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


    void Jump()
    {
        CheckOnGround();
        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            rb.velocity += Vector3.up * jumpForce;
        }
    }


 
    void ApplyMoreGravity()
    {
        if(!isGrounded)
            rb.velocity += Vector3.down * gravity ;
    }

   

    void CheckOnGround()
    {
        SetGroundCheck();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, WhatIsGround);
    }

    void SetGroundCheck()
    {
        groundCheck.position = new Vector3(capsule.transform.position.x, capsule.transform.position.y - capsule.height / 2, capsule.transform.position.z);
    }
    void ControlDrag()
    {
        if (isGrounded)
            rb.drag = rbDrag;
        else
            rb.drag = 0.02f;
    }

    bool OnSlope()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, GetPlayerHeight() * 2f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                
                return true;
            }
            else
            {
                
                return false;
            }
        }
        return false;
    }
    float GetPlayerHeight()
    {
        return transform.localScale.y;
    }

    void OnSlopeRedirection()
    {
        if (!(Vector3.Angle(Vector3.up, slopeHit.normal) <= slopeLimit))
        {
            Vector3 normal = slopeHit.normal;
            Vector3 c = Vector3.Cross(Vector3.up, normal);
            Vector3 u = Vector3.Cross(c, normal);
            slidingDirection = u * 4f;
        }
        else
        {
            slidingDirection = Vector3.zero;
        }
    }


    void SpeedModifier()
    {
        float currentSpeed = moveSpeed;
        float runSpeed = moveSpeed * runSpeedPercent;
        float tempSpeedHolder;
        //running goes faster
        if (isRunning)
            currentSpeed = runSpeed;
        else
            currentSpeed = moveSpeed;

        tempSpeedHolder = currentSpeed;
        //Crouching goes slower
        if (isCrouching)
            currentSpeed = currentSpeed * 0.6f;
        else
            currentSpeed = tempSpeedHolder;

        tempSpeedHolder = currentSpeed;

        


        finalSpeed = currentSpeed;
    }


    void Crouch()
    {
        if (isCrouching)
        {
            capsule.height = crouchHeight;
        }
        else
        {
            capsule.height = normalHeight;
        }
    }

}
