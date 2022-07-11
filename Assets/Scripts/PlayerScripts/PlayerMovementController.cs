using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public CharacterController control;
    public float moveSpeed;
    public float runSpeedPercent;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundCheckDistance;
    public float jumpHeight = 3f;
    public LayerMask WhatIsGround;
    public float slideSpeedPercent;
    bool isGrounded;
    bool isRunning;
    bool isCrouching;
    float finalSpeed;
    float crouchHeight;
    float normalHeight;
    float originStepOffset;


    Vector3 slidingDirection;
    Vector3 vel;
    Vector3 move;

    RaycastHit slopeHit;

    //Rotation and look
    public Camera playerCam;
    public float sensitivity = 50f;
    private float xRot;




    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        normalHeight = control.height;
        crouchHeight = normalHeight * 0.6f;
        originStepOffset = control.stepOffset;


    }

    // Update is called once per frame
    void Update()
    {
       
        MyInput();
        Look();
        Jump();
        Crouch();
        SpeedModifier();
        ApplyGravity();
    }
    private void FixedUpdate()
    {
        //make player slide off a slope when the slope angle is > slopelimit
        if (OnSlope())
        {
            move += (slidingDirection*slideSpeedPercent);
        }

        Look();
        control.Move(move * finalSpeed * Time.deltaTime);
    }
    private void LateUpdate()
    {
        CheckOnGround();
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if ((Vector3.Angle(Vector3.up, hit.normal) > control.slopeLimit))
        {
            Vector3 normal = hit.normal;
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
        //On air slows you down
        if (!isGrounded)
            currentSpeed = currentSpeed * 0.6f;
        else
            currentSpeed = tempSpeedHolder;


        finalSpeed = currentSpeed;
    }   

    void SetGroundCheck()
    {
        groundCheck.position = new Vector3(control.transform.position.x, control.transform.position.y - control.height / 2, control.transform.position.z);
       
    }
    
    void ApplyGravity()
    {
        vel.y += gravity * Time.deltaTime;
        control.Move(vel * Time.deltaTime);
    }

    void SetStepOffset()
    {
        if (!isGrounded)
        {
            control.stepOffset = 0f;
        }
        else
        {
            control.stepOffset = originStepOffset;
        }
    }
   
    void CheckOnGround()
    {
        SetGroundCheck();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, WhatIsGround);
        if (isGrounded && vel.y < 0f)
        {
            vel.y = -2f;
        }

        //fix bug where move jumping near an object
        SetStepOffset();
      
    }
  

    void MyInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        move = (transform.forward * z + transform.right * x);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
       
    }
    void Crouch()
    {
        if (isCrouching)
        {
            control.height = crouchHeight;
        }
        else
        {
            control.height = normalHeight;
        }
    }

    

    bool OnSlope()
    {
        
        if (Physics.Raycast(transform.position,Vector3.down,out slopeHit,GetPlayerHeight()*2f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }

    float GetPlayerHeight()
    {
        return transform.localScale.y;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,Vector3.down*10f);
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




}

