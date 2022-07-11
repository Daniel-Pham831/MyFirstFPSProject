using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMovementScript : MonoBehaviour
{
    public Transform rootOfTheLeg;
    public Transform footTarget;
    public Transform target;
    public float smoothMoveTime = 15f;


    private float maxDistance;
    private float minDistance;
    private float originYpos;
    private float yOffSetFootStep;
    private bool flag = false;

    void Awake()
    {
        footTarget.position = new Vector3(transform.position.x, footTarget.position.y, transform.position.z);
        originYpos = transform.position.y;
        yOffSetFootStep = Mathf.Abs(footTarget.position.y - transform.position.y)*0.5f;
        findMaxDistance();
    }
    void Update()
    {
        //get the distance between the IK bone and the target
        Vector3 currentPos = transform.position;
        currentPos.y = 0;
        Vector3 targetPos = footTarget.position;
        targetPos.y = 0;
        float distance = Vector3.Distance(currentPos, targetPos);



        Vector3 targetMovePos = new Vector3(targetPos.x, originYpos, targetPos.z);
        if (distance >= maxDistance) // distance is bigger than maxdistance => turn flag on and lift the leg up
        {
            flag = true;
         
            transform.position = new Vector3(transform.position.x, transform.position.y + yOffSetFootStep, transform.position.z);
            
        }



        if (flag && distance >= minDistance) // move the leg to the targetPos
        {
            
            transform.position = Vector3.Lerp(transform.position, targetMovePos, Time.deltaTime* 100f);
            
        }
        else
        {
            flag = false;
        }

    }


    void findMaxDistance()
    {
        float distance = 0;
        
        Transform current = rootOfTheLeg;
        Transform next = rootOfTheLeg.GetChild(0);
        while (current != null)
        {
            distance += Vector3.Distance(current.transform.position, next.transform.position);
            current = next;
            if (current.childCount == 0) break;
            next = current.GetChild(0);

            if (current == target) break;
        }

        maxDistance = distance * 0.65f;
        minDistance = distance * 0.1f;
    }

}
