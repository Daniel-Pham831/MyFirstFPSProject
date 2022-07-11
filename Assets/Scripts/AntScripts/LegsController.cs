using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsController : MonoBehaviour
{
    
    public float moveSpeed = 10f;
    public Transform LegIKs;
    public Transform pointA;
    public Transform pointB;

    private List<RigLegMovementScript> legScripts;
    private float distance;

    
    private void Start()
    {
        GetListOfScriptFromTransform(LegIKs);
        distance = Vector3.Distance(pointA.position, pointB.position)*1.3f;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        SetDistanceForLegs(distance);

        StartCoroutine(MakeLegsMove());

        /*
        RigLegMovementScript furthestLeg = findLegWithFurthestDistance();
        if (furthestLeg == null) return;
        StartCoroutine(furthestLeg.MoveLeg());*/
        
    }

    IEnumerator MakeLegsMove()
    {
        while (true)
        {
            yield return new WaitForSeconds((1/moveSpeed)*5f);
            RigLegMovementScript furthestLeg = FindLegWithFurthestDistance();
            if (furthestLeg == null) break;


            StartCoroutine(furthestLeg.MoveLeg());
            if (!furthestLeg.isMove)
            {
                StopAllCoroutines();
                //StopCoroutine(MakeLegsMove());
            }


            yield return new WaitForSeconds(Time.deltaTime*Random.Range(.1f,1f));
        }
    }


    RigLegMovementScript FindLegWithFurthestDistance()
    {
        RigLegMovementScript temp = null;
        bool flag = false;
        for (int i = 0; i < legScripts.Count; i++)
        {
            if(legScripts[i].GetCurrentDistance()>= distance)
            {
                if (!flag)
                {
                    temp = legScripts[i];
                    flag = false;
                }
                else
                {
                    if (temp.GetCurrentDistance() < legScripts[i].GetCurrentDistance())
                        temp = legScripts[i];
                }
            }
        }
        return temp;
    }

    void GetListOfScriptFromTransform(Transform tf)
    {
        legScripts = new List<RigLegMovementScript>();
        for (int i = 0; i < tf.childCount; i++)
        {
            if (tf.GetChild(i) == null) continue;
            legScripts.Add(tf.GetChild(i).GetComponent<RigLegMovementScript>());
        }
        
    }

    /*
    IEnumerator MakeLegsMove1()
    {
        while (true)
        {
            if(legScripts[0].isReadyToMove && 
                legScripts[2].isReadyToMove &&
                legScripts[4].isReadyToMove)
            {
                legScripts[0].moveAble = true;
                legScripts[2].moveAble = true;
                legScripts[4].moveAble = true;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                legScripts[0].moveAble = false;
                legScripts[2].moveAble = false;
                legScripts[4].moveAble = false;
            }

         //   yield return new WaitForSeconds(0.3f);

            if (legScripts[1].isReadyToMove &&
               legScripts[3].isReadyToMove &&
               legScripts[5].isReadyToMove)
            {
                legScripts[1].moveAble = true;
                legScripts[3].moveAble = true;
                legScripts[5].moveAble = true;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                legScripts[1].moveAble = false;
                legScripts[3].moveAble = false;
                legScripts[5].moveAble = false;
            }

         //   yield return new WaitForSeconds(0.3f);

            yield return null;
        }
    }

    IEnumerator MakeLegsMove()
    {
        while (true)
        {
            for (int i = 0; i < legScripts.Count; i++)
            {
                if (legScripts[i].isReadyToMove && !legScripts[(i + 1) % legScripts.Count].isReadyToMove)
                    legScripts[i].moveAble = true;
                else
                    legScripts[i].moveAble = false;

                yield return null;
            }
            yield return null;
        }
    }*/

    /*
    IEnumerator SetStateForLegs()
    {
        for (int i = 0; i < legScripts.Count; i++)
        {
            if (i % 2 == 0)
            {
                SetReadyToMove(legScripts[i]);
            }
            else
            {
                SetReadyToMove(legScripts[i]);
            }
            yield return null;
        }
    }*/
    /*
    void SetReadyToMove(RigLegMovementScript script)
    {
        if (script.isReadyToMove)
            script.moveAble = true;
        else
            script.moveAble = false;
    }*/

    void SetDistanceForLegs(float _distance)
    {
        for (int i = 0; i < legScripts.Count; i++)
        {
            legScripts[i].SetMaxDistance(_distance);
        }
    }

    
}
