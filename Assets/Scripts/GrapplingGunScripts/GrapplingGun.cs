using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
  
    private Vector3 grapplePoint;
    private Vector3 grapplePointNormal;
    public KeyCode hookButton;
    public LayerMask whatIsGrappleable;
    public float maxDistance = 150f;
    public Transform fireTip,cam, player;
    private SpringJoint joint;


    

    void Update()
    {

        if (Input.GetKeyDown(hookButton))
        {
            
            StartGrapple();
        }
        else if (Input.GetKeyUp(hookButton))
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        
       

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            grapplePointNormal = hit.normal;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;


            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.05f;



            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

           
        }
    }

    void StopGrapple()
    {    
        Destroy(joint);
    }
  
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public Vector3 GetGrapplePointNormal()
    {
        return grapplePointNormal;
    }


    public bool IsGrappling()
    {
        return (joint);
    }

  


}
