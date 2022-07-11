using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkLegController : MonoBehaviour
{

    private int nbOfLegs;
    private int smoothness = 5;
    


    //For finding an unit , if scale up the model
    private float unit; // Distance between the antBody and the ground
    private float maxDistance;
    private float stepHeight;
    

    //For controlling the IKtargets
    public Transform[] IKTargets;
    public Transform[] movePoints;
    public Transform[] rayCastPoints;
    private Vector3[] nextPoints;
    private Vector3[] nextNormals;
    private bool[] isGrounded;
    private int[] group1;
    private int[] group2;

    private Vector3[] testPos;
    private Vector3[] testDir;

    //for bodyposition and orientation
    private float normalDistance;
    private Vector3 bodyUp;
    private Vector3 bodyRight;
    private Vector3 bodyForward;
    private Quaternion bodyRotation;
    private Vector3 currentLegCenter;
    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
   
        MoveLegs();   
        UpdateAntBodyPositionAndOrientation();

    }

    void Init()
    {
        nbOfLegs = IKTargets.Length;
        isGrounded = new bool[nbOfLegs];
        nextPoints = new Vector3[nbOfLegs];
        nextNormals = new Vector3[nbOfLegs];
        testPos = new Vector3[nbOfLegs];
        testDir = new Vector3[nbOfLegs];

        for (int i = 0; i < nbOfLegs; i++)
        {
            nextNormals[i] = movePoints[i].up;
            nextPoints[i] = movePoints[i].position;
            IKTargets[i].rotation = movePoints[i].rotation;
            testDir[i] = Vector3.zero;
            testPos[i] = Vector3.zero;
        }

        GetUnitOfAnt();
        GetMaxDistanceAndStepHeight();
        SetAllGrounded(true);
        GetGroup1And2();


        //for body orientation and position
        SetBodyToGround();
        Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit);
        normalDistance = hit.distance;
    }




    void UpdateAntBodyPositionAndOrientation()
    {
        
        currentLegCenter = GetLegsCenter();
        Vector3 nextBodyPosition = currentLegCenter + transform.up * normalDistance;
        transform.position = Vector3.Lerp(transform.position, nextBodyPosition, 1/30f);
        


        //Calculate the average body rotation 
        bodyUp = Vector3.zero;
        for (int i = 0; i < nbOfLegs; i++)
        {
            bodyUp += nextNormals[i];
        }
        if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit, unit))
        {
            bodyUp += hit.normal;
        }
        Vector3 v40 = IKTargets[0].position - IKTargets[4].position;
        Vector3 v13 = IKTargets[3].position - IKTargets[1].position;
        Vector3 v51 = IKTargets[1].position - IKTargets[5].position;
        Vector3 v24 = IKTargets[4].position - IKTargets[2].position;
        Vector3 v50 = IKTargets[0].position - IKTargets[5].position;
        Vector3 v23 = IKTargets[3].position - IKTargets[2].position;
        Vector3 normal1 = Vector3.Cross(v40, v13);
        Vector3 normal2 = Vector3.Cross(v51, v24);
        Vector3 normal3 = Vector3.Cross(v50, v23);

        bodyUp += normal1;
      //  bodyUp += normal2;
        bodyUp += normal3;

        bodyUp.Normalize();

        bodyRight = Vector3.Cross(bodyUp, transform.forward);
        bodyForward = Vector3.Cross(bodyRight, bodyUp);

        bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, bodyRotation, 0.5f);
    }
    IEnumerator MoveLeg(int index)
    {
        FindNextTargetPointAndNormal(index);
        Vector3 targetPoint = nextPoints[index];

        movePoints[index].position = nextPoints[index];

        isGrounded[index] = false;

        for (int i = 1; i <= smoothness; ++i)
        {
            IKTargets[index].position = Vector3.Lerp(IKTargets[index].position, targetPoint, i / (float)(smoothness));
            IKTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness) * Mathf.PI) * stepHeight;
            
            yield return new WaitForFixedUpdate();
        }
        IKTargets[index].rotation = movePoints[index].rotation;
        isGrounded[index] = true;
    }
   
    void FindNextTargetPointAndNormal(int index)
    {
        RaycastHit hit;
        Vector3 forwardPoint = rayCastPoints[index].GetChild(0).position;
        Vector3 downPoint = rayCastPoints[index].GetChild(1).position;
        Vector3 dir = (downPoint - forwardPoint).normalized;
        float distance = (rayCastPoints[index].position - rayCastPoints[index].GetChild(0).position).magnitude;

        if (Physics.Raycast(rayCastPoints[index].position, rayCastPoints[index].forward, out hit, distance*1.5f))
        {
            nextPoints[index] = hit.point;
            nextNormals[index] = hit.normal;
            return;
        }
        if (Physics.Raycast(forwardPoint, dir, out hit))
        {
            nextPoints[index] = hit.point;
            nextNormals[index] = hit.normal;
            return;
        }


    }
    void SetBodyToGround()
    {
        Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hit);
        if (Mathf.Abs(hit.distance - unit) >= unit * 0.02f)
        {
            float differ = Mathf.Abs(hit.distance - unit);
            if (hit.distance - unit > 0)
            {
                transform.position += -transform.up * differ;
            }
            else if (hit.distance - unit < 0)
            {
                transform.position += transform.up * differ;

            }
        }

        
        for(int i= 0; i < nbOfLegs; i++)
        {
            IKTargets[i].position = movePoints[i].position;
        }
    }

    Vector3 GetLegsCenter()
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < nbOfLegs; i++)
        {
            result += IKTargets[i].position;
        }
        result /= nbOfLegs;
        return result;
    }

    void MoveLegs()
    {     
        
        if (CheckDistanceAll(group1) && AllGrouded(group2))
            MakeAStepAll(group1);

        if (CheckDistanceAll(group2) && AllGrouded(group1))
            MakeAStepAll(group2);
        
        /*
        if (CheckDistanceAll(group1))
            MakeAStepAll(group1);

        if (CheckDistanceAll(group2))
            MakeAStepAll(group2);
        */

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
    void MakeAStep(int index)
    {
        StopCoroutine(MoveLeg(index));//this prevent it to start another coroutine
        StartCoroutine(MoveLeg(index));
    }
    void GetMaxDistanceAndStepHeight()
    {
        maxDistance = unit * 0.8f;
        stepHeight = maxDistance * 0.5f;
    }
    void GetUnitOfAnt()
    {
        unit = Vector3.Distance(transform.position, transform.GetChild(0).position);
    }
    bool CheckDistance(int index)
    {
        return (IKTargets[index].position - movePoints[index].position).sqrMagnitude >= maxDistance * maxDistance;
    }

    public float GetUnit()
    {
        return unit;
    }
    public void SetSmoothness(int value)
    {
        smoothness = value;
    }
    void OnDrawGizmos()
    {
   

       
        for (int i = 0; i < nbOfLegs; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(movePoints[i].position, maxDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rayCastPoints[i].GetChild(0).position, (rayCastPoints[i].GetChild(1).position - rayCastPoints[i].GetChild(0).position).normalized * maxDistance*2f);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(rayCastPoints[i].position, (rayCastPoints[i].up * -1).normalized * unit * 2f);
 
        }
    }
}
