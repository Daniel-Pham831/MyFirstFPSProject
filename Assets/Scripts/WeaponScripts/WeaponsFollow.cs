using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsFollow : MonoBehaviour
{
    public Camera weaponCam;

    private void Update()
    {
        FollowCam();
    }

    void FollowCam()
    {
        transform.position = weaponCam.transform.position;
        transform.rotation = weaponCam.transform.rotation;
    }
}
