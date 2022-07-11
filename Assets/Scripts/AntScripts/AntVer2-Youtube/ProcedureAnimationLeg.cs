using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureAnimationLeg : MonoBehaviour
{
    public Transform[] legTargets;
    public float stepSize = 0.15f;
    public int smoothness = 8;
    public float stepHeight = 0.15f;
    public float sphereCastRadius = 0.125f;
    public bool bodyOrientation = true;

    public float raycastRange = 1.5f;
    private Vector3[] defaultLegPositions;
    private Vector3[] lastLegPositions;
    private Vector3 lastBodyUp;
    private bool[] legMoving;
    private int nbLegs;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastBodyPos;

    private float velocityMultiplier = 15f;

    Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] res = new Vector3[2];
        res[1] = Vector3.zero;
        RaycastHit hit;
        Ray ray = new Ray(point + halfRange * up / 2f, -up);

        if (Physics.SphereCast(ray, sphereCastRadius, out hit, 2f * halfRange))
        {
            res[0] = hit.point;
            res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }

    void Start()
    {
        lastBodyUp = transform.up;

        nbLegs = legTargets.Length;
        defaultLegPositions = new Vector3[nbLegs];
        lastLegPositions = new Vector3[nbLegs];
        legMoving = new bool[nbLegs];
        for (int i = 0; i < nbLegs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].localPosition;
            lastLegPositions[i] = legTargets[i].position;
            legMoving[i] = false;
        }
        lastBodyPos = transform.position;
    }

    IEnumerator PerformStep(int index, Vector3 targetPoint)
    {
        Vector3 startPos = lastLegPositions[index];
        for (int i = 1; i <= smoothness; ++i)
        {
            legTargets[index].position = Vector3.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
            legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
            yield return new WaitForFixedUpdate();
        }
        legTargets[index].position = targetPoint;
        lastLegPositions[index] = legTargets[index].position;
        legMoving[0] = false;
    }


    void FixedUpdate()
    {
        velocity = transform.position - lastBodyPos;
        velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

        if (velocity.magnitude < 0.000025f)
            velocity = lastVelocity;
        else
            lastVelocity = velocity;


        Vector3[] desiredPositions = new Vector3[nbLegs];
        int indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < nbLegs; ++i)
        {
            desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

            float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPositions[i], transform.up).magnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
            }
        }
        for (int i = 0; i < nbLegs; ++i)
            if (i != indexToMove)
                legTargets[i].position = lastLegPositions[i];

        if (indexToMove != -1 && !legMoving[0])
        {
            Vector3 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0.0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position) + velocity * velocityMultiplier;

            Vector3[] positionAndNormalFwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange, (transform.parent.up - velocity * 100).normalized);
            Vector3[] positionAndNormalBwd = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange * (1f + velocity.magnitude), (transform.parent.up + velocity * 75).normalized);

            legMoving[0] = true;

            if (positionAndNormalFwd[1] == Vector3.zero)
            {
                StopCoroutine("PerformStep");
                StartCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
            }
            else
            {
                StopCoroutine("PerformStep");
                StartCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
            }
        }

        lastBodyPos = transform.position;
        if (nbLegs > 3 && bodyOrientation)
        {
            Vector3 v1 = legTargets[0].position - legTargets[1].position;
            Vector3 v2 = legTargets[2].position - legTargets[3].position;
            Vector3 normal = Vector3.Cross(v1, v2).normalized;
            Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
            transform.up = up;
            transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
            lastBodyUp = transform.up;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < nbLegs; ++i)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(legTargets[i].position, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), stepSize);
        }
    }
}







/*
 
 
 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegControl : MonoBehaviour
{
    //for maxDistance and StepHeight
    public Transform tipOfMidLeg;
    public Transform upperOfMidLeg;
    private float percentDistance = 0.55f; // 0 -> 1
    private float percentStepHeight = 0.08f;// 0 -> 1
    private float maxDistance;
    private float stepHeight;
    //Targets list
    public Transform[] targetsList;

    //How fast the leg move when make a step , min = 0 (mean intance) , max = 20;
    private float moveSpeed;
    private float minSpeed = 1;
    private float maxSpeed = 30;
    private int smoothness = 5;


    //for body orientation
    public Transform antBody;
    private Vector3 lastBodyUp;
    public bool bodyOrientation = false;
    private Vector3 lastBodyPos;
    private float normalAverageHeight;
    private float currentAverageHeight;



    //Leg groups
    private int[] group1;
    private int[] group2;
    private bool[] isGrounded;


    private void Start()
    {
       // lastBodyPos = antBody.position;
      //  lastBodyUp = antBody.up;
        isGrounded = new bool[transform.childCount];

        GetGroup1And2();
        SetAllGrounded(true);
        SetSmoothnessRelativeToMoveSpeed();

        //for body position and orientation
        normalAverageHeight = GetAverageHeight1();

        StartCoroutine(MoveLegs());
    }

    private void Update()
    {
        GetMaxDistanceAndStepHeight();
        SetSmoothnessRelativeToMoveSpeed();
    }
    private void FixedUpdate()
    {

        //update leg targets and their orientation
      //  UpdateLegTargets();
        SetBodyPositionAndOrientation();


        /*
        //body orientation
        lastBodyPos = antBody.position;
        if (bodyOrientation)
        {
            Vector3 v1 = transform.GetChild(0).position - transform.GetChild(5).position;
        
            Vector3 v2 = transform.GetChild(3).position - transform.GetChild(2).position;
            Vector3 normal = Vector3.Cross(v1, v2).normalized;
            Vector3 up = normal;
            antBody.up = up;
            antBody.rotation = Quaternion.LookRotation(antBody.forward, up);
            lastBodyUp = antBody.up;
        }
        //body orientation
        // lastBodyPos = antBody.position;


        /*
        if (bodyOrientation)
        {
            Physics.Raycast(antBody.position, -antBody.up, out RaycastHit hitInfo);
            antBody.up = hitInfo.normal;
            
        }
        


    }

    void SetBodyPositionAndOrientation()
{
    //for every grounded legs , their average height relative to the body == normalAverageHeight;



    /*
    if (bodyOrientation)
    {
        Vector3 v1 = transform.GetChild(0).position - transform.GetChild(5).position;

        Vector3 v2 = transform.GetChild(3).position - transform.GetChild(2).position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = normal;
        antBody.up = up;
        rotation = Quaternion.LookRotation(antBody.forward, up);
    }

    Physics.Raycast(antBody.position, -antBody.up, out RaycastHit hit);
    antBody.up = Vector3.Lerp(antBody.up, hit.normal, Time.fixedDeltaTime * 10f);


    currentAverageHeight = GetAverageHeight1();
    if (currentAverageHeight == 0) return;

    if (Mathf.Abs(normalAverageHeight - currentAverageHeight) >= normalAverageHeight * 0.01f)
    {
        Vector3 newAntBodyPosition = antBody.position;
        if ((normalAverageHeight - currentAverageHeight) > 0)//move the body up
        {
            newAntBodyPosition += antBody.up * Mathf.Abs(currentAverageHeight - normalAverageHeight);
            Debug.Log("cong");
        }
        else if ((normalAverageHeight - currentAverageHeight) < 0)
        {
            newAntBodyPosition -= antBody.up * Mathf.Abs(currentAverageHeight - normalAverageHeight);
            Debug.Log("tru");
        }
        else
        {
            newAntBodyPosition = antBody.position;
        }
        antBody.position = Vector3.Lerp(antBody.position, newAntBodyPosition, Time.fixedDeltaTime * 20f);
    }

    //   antBody.position = Vector3.zero;
    // antBody.localPosition = Vector3.Lerp(antBody.localPosition, newAntBodyPosition, 1);

}

int[] GetAllGroundedLegs()
{

    int counter = 0;
    for (int i = 0; i < isGrounded.Length; i++)
    {
        if (isGrounded[i])
            counter++;
    }
    int[] groundedLegs = new int[counter];
    counter = 0;
    for (int i = 0; i < isGrounded.Length; i++)
    {
        if (isGrounded[i])
            groundedLegs[counter++] = i;
    }
    return groundedLegs;
}

float GetAverageHeight(int[] groundedLegIndexList)
{
    if (groundedLegIndexList.Length == 0) return 0;
    float sum = 0;
    for (int i = 0; i < groundedLegIndexList.Length; i++)
    {
        sum += Mathf.Abs((transform.GetChild(groundedLegIndexList[i]).position.y - antBody.position.y));
    }
    return sum / groundedLegIndexList.Length;
}




float GetAverageHeight()
{
    float sum = 0;
    for (int i = 0; i < transform.childCount; i++)
    {
        sum += Mathf.Abs((transform.GetChild(i).position.y - antBody.position.y));
    }
    return sum / transform.childCount;
}

float GetAverageHeight1()
{
    Physics.Raycast(antBody.position, -antBody.up, out RaycastHit hit);
    return Vector3.Distance(antBody.position, hit.point);
}

IEnumerator MoveLegs()
{

    while (true)
    {
        if (CheckDistanceAll(group1) && AllGrouded(group2))
            MakeAStepAll(group1);

        if (CheckDistanceAll(group2) && AllGrouded(group1))
            MakeAStepAll(group2);



        yield return new WaitForSeconds(Time.fixedDeltaTime);
    }
}
bool AllGrouded(int[] group)
{
    for (int i = 0; i < group.Length; i++)
    {
        if (!isGrounded[group[i]])
            return false;
    }
    return true;
}
bool CheckDistanceAll(int[] group)
{
    for (int i = 0; i < group.Length; i++)
    {
        if (!CheckDistance(group[i]))
            return false;
    }
    return true;
}

void MakeAStepAll(int[] group)
{
    for (int i = 0; i < group.Length; i++)
    {
        MakeAStep(group[i]);
    }
}

void SetAllGrounded(bool value)
{
    for (int i = 0; i < isGrounded.Length; i++)
        isGrounded[i] = value;
}

void GetGroup1And2()
{
    group1 = new int[transform.childCount / 2];
    group2 = new int[transform.childCount / 2];
    int i1 = 0, i2 = 0;
    for (int i = 0; i < transform.childCount; i++)
    {
        if (i % 2 == 0)
            group1[i1++] = i;
        else
            group2[i2++] = i;
    }
}




void MakeAStep(int index)
{
    StopCoroutine(MoveLeg(index));//this prevent it to start another coroutine
    StartCoroutine(MoveLeg(index));
}

IEnumerator MoveLeg(int index)
{

    // Vector3 targetPoint = FindTargetPosition(index); // fix sau
    Vector3 targetPoint = targetsList[index].position;
    isGrounded[index] = false;

    for (int i = 1; i <= smoothness; ++i)
    {
        transform.GetChild(index).position = Vector3.Lerp(transform.GetChild(index).position, targetPoint, i / (float)(smoothness + 1f));
        transform.GetChild(index).position += antBody.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight; //*
        yield return new WaitForFixedUpdate();
    }


    isGrounded[index] = true;
}

void UpdateLegTargets()
{

    for (int i = 0; i < targetsList.Length; i++)
    {
        Vector3 temp = targetsList[i].position + targetsList[i].up.normalized * normalAverageHeight;
        if (Physics.Raycast(temp, -targetsList[i].up, out RaycastHit hitInfo))
        {
            targetsList[i].position = hitInfo.point;
            targetsList[i].up = hitInfo.normal;
            transform.GetChild(i).up = hitInfo.normal;
        }
    }
}


void GetMoveSpeed()
{
    moveSpeed = antBody.GetComponent<Antmovementfinal>().GetMoveSpeed();
}

void SetSmoothnessRelativeToMoveSpeed() //fast speed  means smoothness goes down
{
    GetMoveSpeed();
    float currentSpeedPercent = antBody.GetComponent<Antmovementfinal>().GetSpeedPerCent();

    if (currentSpeedPercent >= 0.1f && currentSpeedPercent <= 0.5f)
        smoothness = 15;
    else if (currentSpeedPercent > 0.6f && currentSpeedPercent <= 1f)
        smoothness = 12;
    else if (currentSpeedPercent > 1f && currentSpeedPercent <= 1.5f)
        smoothness = 10;
    else if (currentSpeedPercent > 1.5f && currentSpeedPercent <= 2f)
        smoothness = 8;
    else if (currentSpeedPercent > 2f && currentSpeedPercent <= 2.5f)
        smoothness = 6;
    else if (currentSpeedPercent > 3)
        smoothness = 4;
    else
        smoothness = 1;
}
Vector3 FindTargetPosition(int index)
{
    Vector3 targetPositionResult;
    Vector3 temp;
    Vector3 dir = (targetsList[index].position - transform.GetChild(index).position).normalized;
    temp = dir * maxDistance + targetsList[index].position;
    //temp.y = temp.y + antBody.position.y; //*


    Physics.Raycast(temp, -antBody.up, out RaycastHit hitInfo); //*
                                                                // Physics.Raycast(temp, -targetsList[index].up, out RaycastHit hitInfo);
                                                                //   Physics.Raycast(temp)
    targetPositionResult = hitInfo.point;



    return targetPositionResult;
}

bool CheckDistance(int index)
{
    return (Vector3.Distance(transform.GetChild(index).position, targetsList[index].position) >= maxDistance);
}

void GetMaxDistanceAndStepHeight()
{
    maxDistance = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percentDistance;
    stepHeight = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percentStepHeight;
}

private void OnDrawGizmos()
{
    Gizmos.color = Color.green;
    for (int i = 0; i < transform.childCount; i++)
        Gizmos.DrawWireSphere(transform.GetChild(i).position, maxDistance);

    Gizmos.DrawRay(antBody.position, antBody.up * 20f);
}
}

 
 
 
 
 */