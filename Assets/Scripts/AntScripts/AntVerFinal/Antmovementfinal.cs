using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antmovementfinal : MonoBehaviour
{
    public float speedPercent; //min -> max
    public Transform IKLegs;
    private float moveSpeed;
    private float minSpeedPercent = 0f;
    private float maxSpeedPercent = 10f;
    private float minSpeed;
    private float maxSpeed;
    private float normalSpeed; //this mean when speedPercent == 1



    Vector3 position;
    Vector3 velocity;
    Vector3 desiredDirection;
    float angle;
  //  public float maxSpeed = 8;
    public float steerStrength = 10;
    public float wanderStrength = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        SetMoveSpeedDefault();
        SetDesiredDirection(); 
    }

    void Update()
    {
        SetMoveSpeed();
        if (moveSpeed <= minSpeed)
            moveSpeed = minSpeed;
        else if (moveSpeed >= maxSpeed)
            moveSpeed = maxSpeed;

        // transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * moveSpeed);

        /*
        position = transform.position;
        Vector3 randomUnitSphere = Random.insideUnitSphere;
        randomUnitSphere.y = 0; 
        desiredDirection = (desiredDirection + randomUnitSphere * wanderStrength).normalized;


        Vector3 desiredVelocity = desiredDirection * moveSpeed;
        Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, moveSpeed);
        position += velocity * Time.deltaTime;

        angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, angle, 0));
        *//*
        if (desiredDirection != transform.forward)
        {
            transform.forward = Vector3.Lerp(transform.forward, transform.right, Time.deltaTime);
        }
        else
        {
            SetDesiredDirection();
        }*/
       
    }
    void SetDesiredDirection()
    {
        bool random = (Random.Range(0f, 1f) < 50);
        if (random)
        {
            desiredDirection = transform.right;
        }
        else
            desiredDirection = -transform.right;


    }

    void SetMoveSpeedDefault()
    {
        float sum = 0;
        for (int i = 0; i < IKLegs.childCount; i++)
        {
            sum += Vector3.Magnitude(IKLegs.GetChild(i).position - transform.position); //*
        }
        normalSpeed = (sum / IKLegs.childCount);
        moveSpeed = (sum / IKLegs.childCount) * speedPercent;
        maxSpeed = (sum / IKLegs.childCount) * maxSpeedPercent;
        minSpeed = (sum / IKLegs.childCount) * minSpeedPercent;
    }

    void SetMoveSpeed()
    {
        moveSpeed = normalSpeed * speedPercent;
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetMaxSpeedPerCent()
    {
        return maxSpeedPercent;
    }
    public float GetMinSpeedPerCent()
    {
        return minSpeedPercent;
    }
    public float GetSpeedPerCent()
    {
        return speedPercent;
    }

  
}
