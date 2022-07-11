using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegControllerVer2 : MonoBehaviour
{


    public float moveSpeed;
    public Transform IKLegs;
    



    private float averageDistance;
    private List<IKLeg> ikLegScriptList;


    private void Awake()
    {
      
    }

    
    private void Start()
    {
        ikLegScriptList = new List<IKLeg>();
        GetIKLegScriptList();
        SetDistanceAndSpeedForLegs();
        
        for (int i = 0; i < 1; i++)
        {
            ikLegScriptList[(i + 0) % ikLegScriptList.Count].SetLegPos();
            ikLegScriptList[(i + 2) % ikLegScriptList.Count].SetLegPos();
            ikLegScriptList[(i + 4) % ikLegScriptList.Count].SetLegPos();
        }
        //  StartCoroutine(MoveAllLegs());
    }


    void Update()
    {
        SetDistanceAndSpeedForLegs();
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        /*
        for (int i = 0; i < 2; i++)
        {

            if (isMoveAble(i))
            {
                ikLegScriptList[(i + 0) % ikLegScriptList.Count].moveAble = true;
                ikLegScriptList[(i + 2) % ikLegScriptList.Count].moveAble = true;
                ikLegScriptList[(i + 4) % ikLegScriptList.Count].moveAble = true;
            }

        }
        */
    }

    IEnumerator MoveAllLegs()
    {
        for (int i = 0; i < 1; i++)
        {          
            ikLegScriptList[(i + 0) % ikLegScriptList.Count].StartMoveLeg(moveSpeed*10);
            ikLegScriptList[(i + 2) % ikLegScriptList.Count].StartMoveLeg(moveSpeed*10);
            ikLegScriptList[(i + 4) % ikLegScriptList.Count].StartMoveLeg(moveSpeed*10);
        }


        while (true)
        {
            //cach 1
            /*
            for(int i = 0; i < ikLegScriptList.Count; i++)
            {
                /*
                if (ikLegScriptList[i].CheckDistance() &&
                    !ikLegScriptList[(i + 1) % ikLegScriptList.Count].CheckDistance())
                   
                {
                    ikLegScriptList[i].StartMoveLeg(moveSpeed);

                }*/


            /*
            if (ikLegScriptList[i].CheckDistance() &&
                ikLegScriptList[(i + 2) % ikLegScriptList.Count].CheckDistance()&&
                ikLegScriptList[(i + 4) % ikLegScriptList.Count].CheckDistance())
            {
                ikLegScriptList[(i + 0) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                ikLegScriptList[(i + 2) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                ikLegScriptList[(i + 4) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
            }



            yield return new WaitForSeconds(0.1f);

            if (ikLegScriptList[i].CheckDistance())
            {
                ikLegScriptList[i].StartMoveLeg(moveSpeed);
            }
            yield return null;

        }

        */


            
            //cach 2
            for (int i = 0; i < 2; i++)
            {
                
                if (isMoveAble(i))
                {
                    ikLegScriptList[(i + 0) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                    ikLegScriptList[(i + 2) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                    ikLegScriptList[(i + 4) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                }

            }
            


            //cach 3
            /*
            for (int i = 0; i < ikLegScriptList.Count; i++)
            {

                if (ikLegScriptList[i].CheckDistance())
                {
                    ikLegScriptList[(i + 0) % ikLegScriptList.Count].StartMoveLeg(moveSpeed);
                   
                }

            }
            */



            yield return null;
        }
    }



    bool isMoveAble(int i)
    {  

        if(ikLegScriptList[(i + 0) % ikLegScriptList.Count].CheckDistance()
            && ikLegScriptList[(i + 2) % ikLegScriptList.Count].CheckDistance()
                    && ikLegScriptList[(i + 4) % ikLegScriptList.Count].CheckDistance())
        {
            if(!ikLegScriptList[(i + 1) % ikLegScriptList.Count].isMove
            && !ikLegScriptList[(i + 3) % ikLegScriptList.Count].isMove
                    && !ikLegScriptList[(i + 5) % ikLegScriptList.Count].isMove)
            {
                return true;
            }

        }
        return false;
    }


    bool isMoveAble1(int i)
    {

        if (ikLegScriptList[(i + 0) % ikLegScriptList.Count].CheckDistance())
        {
            if (!ikLegScriptList[(i + 1) % ikLegScriptList.Count].isMove)
            {
                return true;
            }

        }
        return false;
    }
 
    void GetIKLegScriptList()
    {
        for(int i = 0; i < IKLegs.childCount; i++)
        {
            if (IKLegs.GetChild(i) == null) continue;
            ikLegScriptList.Add(IKLegs.GetChild(i).GetComponent<IKLeg>());
        }
    }
    void GetAverageDistance()
    {
        float sum = 0;
        for (int i = 0; i < ikLegScriptList.Count; i++)
        {
            sum += Vector3.Distance(ikLegScriptList[i].midPoint.position, ikLegScriptList[i].transform.position);
        }
        averageDistance = sum / ikLegScriptList.Count;
        averageDistance *= 0.5f;
    }
    void SetDistanceAndSpeedForLegs()
    {
        GetAverageDistance();
        for (int i = 0; i < ikLegScriptList.Count; i++)
        {
            ikLegScriptList[i].SetDistance(averageDistance);
            ikLegScriptList[i].SetSpeed(moveSpeed);
        }
    }
}
