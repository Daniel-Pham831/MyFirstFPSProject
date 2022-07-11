using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigLegMovementScript : MonoBehaviour
{
    public Transform upperBoneFromIK; //for distance
    public Transform targetPos;
    public bool isMove;
    public bool isReadyToMove;
    public bool isMidPoint;
   // private float smoothTime = 5f;
    private Vector3 newTargetPos = Vector3.zero;
    private Vector3 midPointBetweenCurrentAndTarget = Vector3.zero;
    private float maxDistance;
   // private float moveSpeed = 0f;

    private void Awake()
    {
        FindMaxAndMinDistance();
    }

    void Update()
    {
        if (transform.position == newTargetPos)
        {
            if (MoveLeg() == null) return;
            StopCoroutine(MoveLeg());
            isMove = false;
        }
    }
    public IEnumerator MoveLeg()
    {
        isMove = true;
        newTargetPos = targetPos.position + (targetPos.position - transform.position).normalized * maxDistance;
        midPointBetweenCurrentAndTarget = targetPos.position;
        midPointBetweenCurrentAndTarget.y = targetPos.position.y + maxDistance * 0.1f;
        while (true)
        {
          
            transform.position = Vector3.MoveTowards(transform.position, newTargetPos, Time.deltaTime* Vector3.Distance(transform.position, newTargetPos) * Random.Range(.1f, .4f));         
            yield return null;
        }
    }

    

    public float GetCurrentDistance()
    {
        return Vector3.Distance(transform.position, targetPos.position);
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public void SetMaxDistance(float value)
    {
        maxDistance = value;
    }
    public void MoveLegUp(float amount)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + amount, transform.position.z);
    }

    void FindMaxAndMinDistance()
    {
        maxDistance = Vector3.Distance(targetPos.position, upperBoneFromIK.position) * 1.2f;
    }

    float CheckDistance(Vector3 current,Vector3 target)
    {
        return Vector3.Distance(current, target);
    }

    
    
}
