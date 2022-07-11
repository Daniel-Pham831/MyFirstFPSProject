using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControlLongLegs : MonoBehaviour
{
    public Transform[] IKTargets;
    public Transform[] RaycastPoints;
    public Transform[] MidPoints;
    public float smoothness;

    private int nbOfLegs;
    private Vector3[] lastPosition;
    private Vector3[] nextPosition;
    private Vector3[] nextNormal;
    private bool[] isGrounded;

    private float maxDistance;
    private float stepHeight;
    private float normalAverageHeight;
    private float currentAverageHeight;
    private float unit;

    private int[] group1;
    private int[] group2;
    private Vector3 lastBodyNormal;
    private Vector3 nextBodyNormal;
    private void Start()
    {
        lastBodyNormal = transform.up;
        Init();
        StartCoroutine(MoveLegs());
    }


    private void FixedUpdate()
    {
        transform.position += transform.forward * 5f * Time.fixedDeltaTime;
        FindNextBodyNormal();

    }


  


    void MakeAStep(int index)
    {
        StopCoroutine(MoveLeg(index));
        StartCoroutine(MoveLeg(index));
    }


    IEnumerator MoveLeg(int index)
    {
        FindNextStepPosition(index);
        isGrounded[index] = false;

        for (int i = 1; i <= smoothness; ++i)
        {
            IKTargets[index].position = Vector3.Lerp(IKTargets[index].position, nextPosition[index], i / (float)(smoothness + 1f));
            IKTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight; //*
            lastPosition[index] = IKTargets[index].position;

            yield return new WaitForFixedUpdate();
        }

        IKTargets[index].up = nextNormal[index];
        MidPoints[index].up = IKTargets[index].up;
        lastPosition[index] = IKTargets[index].position;
        isGrounded[index] = true;
    }




    bool CheckDistance(int index)
    {
        Vector3 IKTargetPos = Vector3.ProjectOnPlane(IKTargets[index].position, MidPoints[index].up);
        return (IKTargets[index].position - MidPoints[index].position).sqrMagnitude >= maxDistance * maxDistance;
    }




    /*
    void FindNextBodyNormal()
    {
        Vector3 v1 = IKTargets[0].position - IKTargets[5].position;
        Vector3 v2 = IKTargets[3].position - IKTargets[2].position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyNormal, normal, 1f / (float)(smoothness + 1));
        transform.up = normal;
        transform.rotation = Quaternion.LookRotation(transform.parent.forward, normal);
        lastBodyNormal = transform.up;
    }*/
    
    void FindNextBodyNormal()
    {
        Vector3 sumNormal = Vector3.zero;
        for(int i = 0; i < nbOfLegs; i++)
        {
            sumNormal += IKTargets[i].up;
        }
        nextBodyNormal = sumNormal.normalized;
        transform.up = Vector3.Lerp(transform.up, nextBodyNormal, 1f / (float)(smoothness + 1));
        lastBodyNormal = transform.up;
    }







    void Init()
    {
        nbOfLegs = IKTargets.Length;
        FindAUnitOfAnAnt();
        FindMaxDistanceAndStepHeight();
        InitArrays();
        GetGroup1And2();
       
    }

    void GetGroup1And2()
    {
        group1 = new int[nbOfLegs / 2];
        group2 = new int[nbOfLegs / 2];
        int i1 = 0, i2 = 0;
        for (int i = 0; i < nbOfLegs; i++)
        {
            if (i % 2 == 0)
                group1[i1++] = i;
            else
                group2[i2++] = i;
        }
    }

    IEnumerator MoveLegs()
    {

        while (true)
        {
            for (int i = 0; i < nbOfLegs; i++)
            {
                if (isGrounded[i] == true)
                    IKTargets[i].position = lastPosition[i];
            }

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



    void FindAUnitOfAnAnt() // distance of (the tip of the front leg -> the mid leg)/2
    {
        unit = Vector3.Distance(IKTargets[0].position, IKTargets[1].position)/2f;
    }
    void FindMaxDistanceAndStepHeight()
    {
        maxDistance = unit;
        stepHeight = maxDistance * 0.2f;
    }

    void InitArrays()
    { 
        lastPosition = new Vector3[nbOfLegs];
        nextPosition = new Vector3[nbOfLegs];
        nextNormal = new Vector3[nbOfLegs];
        isGrounded = new bool[nbOfLegs];
        InitLastPositionAndIsGrounded();
       
    }

    
    
    void InitLastPositionAndIsGrounded()
    {
        for(int i = 0; i < nbOfLegs; i++)
        {
            lastPosition[i] = MidPoints[i].position;
            isGrounded[i] = true;
        }
    }

    void FindNextStepPosition(int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(MidPoints[index].position, (MidPoints[index].forward + MidPoints[index].up), out hit, maxDistance))
        {
            nextPosition[index] = hit.point;
            nextNormal[index] = hit.normal;
          
            return;
        }

        if (Physics.Raycast(MidPoints[index].position, (2f*MidPoints[index].forward + MidPoints[index].up), out hit, maxDistance))
        {
            nextPosition[index] = hit.point;
            nextNormal[index] = hit.normal;
            
            return;
        }

        Vector3 tempNextPoint = MidPoints[index].position + (MidPoints[index].position - IKTargets[index].position).normalized * maxDistance*0.9f;

        if (Physics.Raycast(tempNextPoint + MidPoints[index].up.normalized * maxDistance * 0.9f, MidPoints[index].up * -1, out hit, maxDistance))
        {
            nextPosition[index] = hit.point;
            nextNormal[index] = hit.normal;
         
            return;
        }

        nextPosition[index] = tempNextPoint;
        nextNormal[index] = MidPoints[index].up;



    }



    
    private void OnDrawGizmos()
    {
    }


}






/*
 

    /*
    private void Start()
    {
        
        FindMaxDistanceAndStepHeight();
        FindNormalAverageHeight();
        lastPosition = new Vector3[IKTargets.Length];
        nextPosition = new Vector3[IKTargets.Length];
        nextNormal = new Vector3[IKTargets.Length];
        
        FindLastPosition();
        SetRaycastPointsPosition();
    }
    void Update()
    {
        transform.position += transform.forward * 5f * Time.deltaTime;
   
        for (int i = 0; i < lastPosition.Length; i++)
        {
            FindNextPostion(i);
            IKTargets[i].position = lastPosition[i];
            if (CheckDistance(i))
            {
                IKTargets[i].position = nextPosition[i];
                IKTargets[i].up = nextNormal[i];
                lastPosition[i] = nextPosition[i];
            }
            
        }
        FindAverageHeight();
        SetBodyPosition();
        FindNextBodyNormal();
   
    }


    void SetBodyPosition()
    {
        if(Mathf.Abs(currentAverageHeight - normalAverageHeight) >= normalAverageHeight * 0.05f)
        {
            if (currentAverageHeight - normalAverageHeight > 0)
                transform.position += transform.up.normalized * Mathf.Abs(currentAverageHeight - normalAverageHeight);
            else if(currentAverageHeight - normalAverageHeight < 0)
                transform.position -= transform.up.normalized * Mathf.Abs(currentAverageHeight - normalAverageHeight);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for(int i = 0; i < MidPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(MidPoints[i].position, maxDistance);
        }
        
        for (int i = 0; i < nextPosition.Length; i++)
        {
            Gizmos.DrawSphere(nextPosition[i], 1f);
        }
        
    }

    void SetRaycastPointsPosition()
    {
        for(int i = 0; i < RaycastPoints.Length; i++)
        {
            RaycastPoints[i].position = MidPoints[i].position + ((MidPoints[i].up*normalAverageHeight*1.5f) + (MidPoints[i].forward*-1*maxDistance * 0.5f));
        }
    }


    void FindNextPostion(int index)
    {
        RaycastHit hit;
      
        if (Physics.Raycast(RaycastPoints[index].position, transform.forward, out hit, maxDistance))
{
    nextPosition[index] = hit.point;
    nextNormal[index] = hit.normal;
    return;
}







if (Physics.Raycast(MidPoints[index].position + transform.up * (maxDistance / 2), -transform.up, out hit, maxDistance))
{
    nextPosition[index] = hit.point;
    nextNormal[index] = hit.normal;
}




     
        
    }

    void FindNextBodyNormal()
{
    Vector3 v1 = IKTargets[0].position - IKTargets[2].position;
    Vector3 v2 = IKTargets[5].position - IKTargets[3].position;
    Vector3 normal = Vector3.Cross(v1, v2).normalized;
    //    Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
    transform.up = normal;
    transform.rotation = Quaternion.LookRotation(transform.parent.forward, normal);
    //     lastBodyUp = transform.up;
}

void FindNormalAverageHeight()
{
    Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit);
    normalAverageHeight = hit.distance;
}
void FindAverageHeight()
{
    float sum = 0;
    for (int i = 0; i < IKTargets.Length; i++)
    {
        Physics.Raycast(IKTargets[i].position + IKTargets[i].up * normalAverageHeight, (IKTargets[i].up * -1), out RaycastHit hit);
        sum += hit.distance;
    }
    currentAverageHeight = sum / IKTargets.Length;
}

void FindMaxDistanceAndStepHeight()
{
    //if scale the model -> maxDistance also scale
    maxDistance = Vector3.Distance(IKTargets[0].position, IKTargets[1].position) * 0.7f;
    stepHeight = maxDistance / 3.5f;
}

void FindLastPosition()
{
    for (int i = 0; i < lastPosition.Length; i++)
    {
        lastPosition[i] = IKTargets[i].position;
    }
}

bool CheckDistance(int index)
{
    return (IKTargets[index].position - MidPoints[index].position).sqrMagnitude >= (maxDistance * 0.5f) * (maxDistance * 0.5f);
}


*/