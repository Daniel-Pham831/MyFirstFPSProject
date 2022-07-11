using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //public float secondsAliveAfterImpact;
    public GameObject thingToDestroy;
    private Vector3 fireDir;
   
    private bool move;
    private Rigidbody rb;
    private float timeStart;
    private float secondsAlive = 10f;
    private float secondsAliveAfterImpact = 0f;

    private float bulletSpeed;
    public float maxBulletSpeed = 1000f;
    public float minBulletSpeed = 100f;

    //trail of the bullet
    private TrailRenderer trailRenderer;
    public float maxTrailTime = 0.2f;
    public float minTrailTime = 0.05f;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(thingToDestroy, secondsAliveAfterImpact);
    }

    public void SetMove(bool value)
    {
        move = value;
    }
    public void SetBulletSpeed(float value)
    {
        if (value <= minBulletSpeed) value = minBulletSpeed;
        if (value >= maxBulletSpeed) value = maxBulletSpeed;

        bulletSpeed = value;
        //make the trailTime longer when bulletspeed is low , and vice versa
        float bulletSpeedGap = Mathf.Abs(minBulletSpeed-maxBulletSpeed);
        float trailsTimeGap = Mathf.Abs(minTrailTime - maxTrailTime);
        //this math makes time becomes smaller when speed is high
        //I spent 1.5h just to find this line :) 
        trailRenderer.time = ((1 - ((value - minBulletSpeed) / bulletSpeedGap)) * trailsTimeGap) + minTrailTime;


        Debug.Log(trailRenderer.time);
    }

    public void SetFireDirection(Vector3 value)
    {
        fireDir = value;
        thingToDestroy.transform.forward = fireDir;
    }
    private void Update()
    {
       

        if (move) //fire once 
        {
            timeStart = Time.time + secondsAlive;
            rb.AddForce(fireDir * bulletSpeed * 10f * Time.deltaTime, ForceMode.Impulse);
            move = false;
        }
        
        if (Time.time > timeStart)
        {
            Destroy(thingToDestroy);
            return;
        }
        

    }
}
