using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSentivity = 100f;
    public Transform playerBody;
    public Transform mainCam;
    float xRot;
    float x, y;

    // Update is called once per frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        x = Input.GetAxis("Mouse X") * mouseSentivity * Time.deltaTime;
        y = Input.GetAxis("Mouse Y") * mouseSentivity * Time.deltaTime;

        xRot -= y;
        xRot = Mathf.Clamp(xRot, -90, 90);

        mainCam.localRotation = Quaternion.Euler(xRot, 0, 0);
        playerBody.Rotate(Vector3.up * x);

    }
 

}
