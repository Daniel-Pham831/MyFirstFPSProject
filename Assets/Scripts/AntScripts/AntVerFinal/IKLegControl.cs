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



    //1 Unit of an Ant  for measure
    private float unit;
    public float percent = 0.5f;
    public int nbOfRays = 4;
    private Vector3 testPoints;
    private Vector3 pointToRaycast;
    private Vector3 currentNormal;
    private Vector3 nextPoint;
    private Vector3 nextNormal;


    private void Start()
    {
      
        FindAnAntUnit();
        FindTheNormalVector();
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
        FindAnAntUnit();
        FindTheNormalVector();
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
        }*/
        //body orientation
        // lastBodyPos = antBody.position;


        /*
        if (bodyOrientation)
        {
            Physics.Raycast(antBody.position, -antBody.up, out RaycastHit hitInfo);
            antBody.up = hitInfo.normal;
            
        }
        */


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
           
        }
        */
        Physics.Raycast(antBody.position, antBody.forward, out RaycastHit hit2, Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * 2.5f);
        antBody.up = Vector3.Lerp(antBody.up, hit2.normal, Time.fixedDeltaTime * 30/smoothness);

        antBody.up = Vector3.Lerp(antBody.up, nextNormal, Time.fixedDeltaTime * 30 / smoothness);




       
        
        currentAverageHeight = GetAverageHeight1();
        if (currentAverageHeight == 0) return;

        if (Mathf.Abs(normalAverageHeight - currentAverageHeight) >= normalAverageHeight * 0.01f)
        {
            Vector3 newAntBodyPosition = antBody.position;
            if ((normalAverageHeight -currentAverageHeight) > 0)//move the body up
            {
                newAntBodyPosition += antBody.up * Mathf.Abs(currentAverageHeight - normalAverageHeight);
            }
            else if ((normalAverageHeight - currentAverageHeight) < 0)
            {
                newAntBodyPosition -= antBody.up * Mathf.Abs(currentAverageHeight - normalAverageHeight);
            }
            else
            {
                newAntBodyPosition = antBody.position;
            }
            antBody.position = Vector3.Lerp(antBody.position, newAntBodyPosition, Time.fixedDeltaTime * 10f);
        }

        //   antBody.position = Vector3.zero;
       // antBody.localPosition = Vector3.Lerp(antBody.localPosition, newAntBodyPosition, 1);
        
    }


    void FindTheNormalVector()
    {
        pointToRaycast = antBody.position + (antBody.forward * unit*1.5f);
        RaycastHit hit;
        Physics.Raycast(antBody.position, antBody.up * -1, out hit, normalAverageHeight * 1.2f);
        Vector3 point = hit.point + hit.normal * -1f * normalAverageHeight*2f;
        Physics.Raycast(pointToRaycast, (point - pointToRaycast).normalized, out hit, normalAverageHeight * 1.6f);

        nextPoint = hit.point;
        nextNormal = hit.normal;
    }

    /* this is good enought
    void FindTheNormalVector() //normal vector of the surface that the ant is standing on
    {
        float radius = unit;
        testPoints = new Vector3[nbOfRays];
        Transform antBodytemp = antBody;
        float angleToRotate = 2 * Mathf.PI / nbOfRays;
        for(int i = 0; i < nbOfRays; i++)
        {
            testPoints[i] = antBody.position + (antBodytemp.forward * radius);
            antBodytemp.Rotate(Vector3.up  *angleToRotate* Mathf.Rad2Deg);
        }

    }
    */

    void FindAnAntUnit()
    {
        unit = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percent;
      
    }


    int[] GetAllGroundedLegs()
    {
       
        int counter = 0;
        for(int i = 0; i < isGrounded.Length; i++)
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
        for(int i = 0; i < groundedLegIndexList.Length; i++)
        {
            sum += Mathf.Abs((transform.GetChild(groundedLegIndexList[i]).position.y - antBody.position.y));
        }
        return sum / groundedLegIndexList.Length;
    }




    float GetAverageHeight()
    {
        float sum = 0;
        for(int i = 0; i < transform.childCount; i++)
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
        for(int i = 0; i < group.Length; i++)
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
        for(int i = 0; i < transform.childCount; i++)
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
        
        for (int i = 0;i< targetsList.Length; i++)
        {
            Vector3 temp = targetsList[i].position + targetsList[i].up.normalized * normalAverageHeight;
            if (Physics.Raycast(temp, -targetsList[i].up , out RaycastHit hitInfo))
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
        return (Vector3.SqrMagnitude(transform.GetChild(index).position - targetsList[index].position) >= maxDistance* maxDistance);
    }

    void GetMaxDistanceAndStepHeight()
    {
        maxDistance = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percentDistance;
        stepHeight = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percentStepHeight;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount; i++)
            Gizmos.DrawWireSphere(transform.GetChild(i).position, maxDistance);
        */
        Gizmos.color = Color.red;
        Gizmos.DrawRay(nextPoint, nextNormal * 3f);


        Gizmos.color = Color.green;
       // Gizmos.DrawRay(antBody.position, antBody.forward * Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position)*2.5f) ;


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



    //1 Unit of an Ant  for measure
    private float unit;
    public float percent = 0.5f;
    public int nbOfRays = 4;
    private Vector3 testPoints;
    private Vector3 pointToRaycast;
    private Vector3 currentNormal;
    private Vector3 nextPoint;
    private Vector3 nextNormal;


    private void Start()
    {
      
        FindAnAntUnit();
        FindTheNormalVector();
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
        FindAnAntUnit();
        FindTheNormalVector();
        GetMaxDistanceAndStepHeight();
        SetSmoothnessRelativeToMoveSpeed();
    }
    private void FixedUpdate()
    {


    }

    void SetBodyPositionAndOrientation()
{
    Physics.Raycast(antBody.position, antBody.forward, out RaycastHit hit2, Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * 2.5f);
    antBody.up = Vector3.Lerp(antBody.up, hit2.normal, Time.fixedDeltaTime * 30 / smoothness);

    antBody.up = Vector3.Lerp(antBody.up, nextNormal, Time.fixedDeltaTime * 30 / smoothness);

    Physics.Raycast(antBody.position, -antBody.up, out RaycastHit hit, Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * 2.5f);
    antBody.up = Vector3.Lerp(antBody.up, hit.normal, Time.fixedDeltaTime * 30 / smoothness);




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
        antBody.position = Vector3.Lerp(antBody.position, newAntBodyPosition, Time.fixedDeltaTime * 10f);
    }

    //   antBody.position = Vector3.zero;
    // antBody.localPosition = Vector3.Lerp(antBody.localPosition, newAntBodyPosition, 1);

}


void FindTheNormalVector()
{
    pointToRaycast = antBody.position + (antBody.forward * unit);
    RaycastHit hit;
    Physics.Raycast(antBody.position, antBody.up * -1, out hit, normalAverageHeight * 1.2f);
    Vector3 point = hit.point + hit.normal * -1f * normalAverageHeight;
    Physics.Raycast(pointToRaycast, (point - pointToRaycast).normalized, out hit, normalAverageHeight * 1.6f);

    nextPoint = hit.point;
    nextNormal = hit.normal;
}


void FindAnAntUnit()
{
    unit = Vector3.Distance(tipOfMidLeg.position, upperOfMidLeg.position) * percent;

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
    Gizmos.color = Color.red;
    Gizmos.DrawRay(nextPoint, nextNormal * 3f);


    Gizmos.color = Color.green;


}
}

 
 
 */