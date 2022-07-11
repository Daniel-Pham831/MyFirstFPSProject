using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPumpBoost : MonoBehaviour
{
    public Rigidbody playerRigidBody;
    public float boostSpeed = 500f;


    private GrapplingGun grapplingGun;
    private bool input;
    private Vector3 boostDirection;

    private void Awake()
    {
        grapplingGun = GetComponent<GrapplingGun>();
    }

    private void Update()
    {
        MyInput();
    }


    private void FixedUpdate()
    {
        MyInput();
        if (input && IsBoostable())
        {
            boostDirection = FindDirFromPlayerToHookPoint();

            playerRigidBody.AddForce(boostDirection * boostSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else
            boostDirection = Vector3.zero;
    }

    Vector3 FindDirFromPlayerToHookPoint()
    {
        return (grapplingGun.GetGrapplePoint() - playerRigidBody.position).normalized;
    }


    void MyInput()
    {
        input = Input.GetKey(KeyCode.Space);
    }

    bool IsBoostable()
    {
        // if player is grappling -=> boostable
        return grapplingGun.IsGrappling();
    }

}
