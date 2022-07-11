using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    private Spring spring;
    private Vector3 currentGrapplePos;


    public LineRenderer lineRender;
    public GrapplingGun grapplingGun;

    //default value that works fine
    public int quality = 500;
    public float damper = 12f;
    public float strength = 800f;
    public float velocity = 20f;
    public float waveCount = 3f;
    public float waveHeight = 3f;
    public AnimationCurve affectCurve;



    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }





    private void LateUpdate()
    {
        DrawRope();
 
    }




    void DrawRope()
    {
        if (!grapplingGun.IsGrappling())
        {
            currentGrapplePos = grapplingGun.fireTip.position;
            spring.Reset();
            if (lineRender.positionCount > 0)
                lineRender.positionCount = 0;
            return;
        }

        if (lineRender.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lineRender.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = grapplingGun.GetGrapplePoint();
        var fireTipPos = grapplingGun.fireTip.position;
        var up = Quaternion.LookRotation(grapplePoint - fireTipPos).normalized * Vector3.up;



        currentGrapplePos = Vector3.Lerp(currentGrapplePos, grapplePoint, Time.deltaTime * 12f);


       


        for (int i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var right = Quaternion.LookRotation((grapplePoint - fireTipPos).normalized) * Vector3.right;
            /* //Rope goes up-down motion
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
            */


            //Rope goes circle motion
             var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                                      affectCurve.Evaluate(delta) +
                                      right * waveHeight * Mathf.Cos(delta * waveCount * Mathf.PI) * spring.Value *
                                      affectCurve.Evaluate(delta);
            
           








            lineRender.SetPosition(i, Vector3.Lerp(fireTipPos, currentGrapplePos, delta) + offset);


        }


    }


    


}
