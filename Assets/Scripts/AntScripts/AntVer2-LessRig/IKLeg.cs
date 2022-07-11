using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLeg : MonoBehaviour
{
    public Transform rootPoint;
    public Transform midPoint;
    public Transform targetPoint;
    public bool isMove;
    public bool moveAble;


    private float speed;
    private float distanceLimit;
    private float maxDistance;
    public Transform midPointInAir;

    public bool isMid;



    private void Update()
    {
         

        if (Vector3.Distance(transform.position, midPointInAir.position) <= 0.2f)
        {
            isMid = true;
        }
        else
            isMid = false;

        if (!isMid)
            transform.position = Vector3.MoveTowards(transform.position, midPointInAir.position, (Time.deltaTime * Mathf.Abs(speed))*4f);
        else
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, (Time.deltaTime * Mathf.Abs(speed))*4f);




        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.1f)
        {
            isMove = false;
            moveAble = false;
        }
    }

    public void MakeAStep()
    {
        
    }

    IEnumerator StepUp()
    {
        yield return null;
    }

    IEnumerator StepDown()
    {
        yield return null;
    }

    public void SetLegPos()
    {
        transform.position = targetPoint.position;
    }



    void MakeLegMoveUpdate()
    {
        
        while (Vector3.Distance(transform.position, targetPoint.position) >= 0.1f)
        {
            isMove = true;
            // transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed);
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * speed * 0.5f); 
        }
        isMove = false;
        moveAble = false;
    }


    public IEnumerator MoveLeg(float moveSpeed)
    {
        yield return null;
        isMove = true;
       
        float speed = (Time.deltaTime * moveSpeed) + (Time.deltaTime * Mathf.Abs(moveSpeed) * 2f);
       
        while (Vector3.Distance(transform.position, targetPoint.position) >= 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed);
           
            yield return null;
        }
        isMove = false;
    }
    public void StartMoveLeg(float moveSpeed)
    {
        StopCoroutine("MoveLeg");
        StartCoroutine(MoveLeg(moveSpeed));
    }
    public bool CheckDistance()
    {
        return Vector3.Distance(targetPoint.position, transform.position) >= maxDistance;
    }
    public void SetDistance(float value)
    {
        maxDistance = value;
        distanceLimit = maxDistance * 1.8f;
    }
    public void SetSpeed(float value)
    {
        speed = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPoint.position, maxDistance);
    }

}
