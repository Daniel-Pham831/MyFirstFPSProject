using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //get weapon from gunHolder into state.unarmed
    //store weapon back into gunholder when swapping new gun or deselect the current gun

    public Transform GunHolder;
    public Transform States;

    private GameObject currentGun;
    private int currentGunIndex;
    private int gunIndex;
    private Transform unarmedPos;

    void Start()
    {
        GunHolder.gameObject.SetActive(false);
        unarmedPos = States.GetChild(1);
        gunIndex = 0;
        currentGunIndex = 0;
        currentGun = null;
    }

    void Update()
    {
        //get user input
        GunInput();

        //get gun using user input
        GetWeapon();
  

    }

    void GunInput()
    {
        int tempGunIndex = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            tempGunIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            tempGunIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            tempGunIndex = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            tempGunIndex = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            tempGunIndex = 5;

        gunIndex = tempGunIndex;   

    }

    void GetWeapon()
    {
        if (gunIndex == 0) return; //no input from user -> return;


        if (currentGunIndex == gunIndex) //if user pressing the same index as the current gun , put it back to the gunholder
        {
            //  PutGunBackToGunHolder(currentGun, currentGunIndex);
            //   currentGun = null;
            DestroyGun(currentGun);


            currentGunIndex = 0;
            return;
        }



        if (currentGun != null) //if player already has a gun , put it back to gunHolder
        {
          //  PutGunBackToGunHolder(currentGun, currentGunIndex);
            DestroyGun(currentGun);
        }

        currentGunIndex = gunIndex;

        //after user press a button => gunIndex , use this to get the gun in Gunholder
        currentGun = InstantiateGunFromGunHolderWithIndex(gunIndex);
        if (currentGun == null) return; //there is no gun in the gun Index

        GetGunIntoUnarmedPosition(currentGun);

        ResetGunPosition(currentGun);
        
    }

/*
    void Aim()
    {
        //if there is no gun in player's hand then return
        if (currentGun == null) return;


        //set gun position according to aimState ,  1 == Default , 2 == ADS
        currentGun.transform.parent = States.GetChild(aimState);

        //move gun from Unarmed => Default => ADS  and vice versa
        currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, States.GetChild(aimState).position, Time.deltaTime * smoothtimeToSwitchState);
        currentGun.transform.rotation = Quaternion.Lerp(currentGun.transform.rotation, States.GetChild(aimState).rotation, Time.deltaTime * smoothtimeToSwitchState);
    }*/
    void DestroyGun(GameObject gunToDestroy)
    {
        Destroy(gunToDestroy);
        gunToDestroy = null;
    }

    GameObject InstantiateGunFromGunHolderWithIndex(int index)
    {
        Transform slotHolder = GunHolder.GetChild(index); //find the slot holder with index
 
        if (slotHolder.childCount == 0) return null; //if there is no gun in the slot , return null

        GameObject gunPrefab = slotHolder.GetChild(0).gameObject;
        GameObject result = Instantiate(gunPrefab, gunPrefab.transform.position, Quaternion.identity);

        return result;
    }

    void GetGunIntoUnarmedPosition(GameObject gunToGet)
    {
        if (gunToGet == null) return; //just to be sure that if there is no gun => can not put anything into unarmed position

        gunToGet.transform.parent = unarmedPos;
    }

    void ResetGunPosition(GameObject gunToReset)
    {
        if (gunToReset == null) return;
        gunToReset.transform.localPosition = Vector3.zero;
        gunToReset.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void PutGunBackToGunHolder(GameObject gunToPutBack, int slotIndex)
    {
        if (currentGun == null) return;

        gunToPutBack.transform.parent = GunHolder.GetChild(slotIndex);
    }




















   
    void GetGunIntoUnarmedPos(GameObject gunToGet)
    {
        if (gunToGet == null) return;

        gunToGet.transform.parent = States.GetChild(0);
    }

    void ResetGunPosition() {
        if (currentGun == null) return;
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    GameObject GetPrefabFromSlotHolder(Transform slotHolder)
    {
        if (slotHolder.childCount == 0) return null;
        return slotHolder.GetChild(0).gameObject;
    }
    


    /* void GetWeapon()
    {

        //if player press the same gun , put the gun back to gunholder
        if (currentGunIndex == gunIndex)
        {
            //PutGunBackToGunHolder(currentGun, gunIndex);
            return;
        }



        if (currentGun != null) //if player already has a gun , put it back to gunHolder
        {
            PutGunBackToGunHolder(currentGun, currentGunIndex);
        }


        //get the Gun from gunholder
        GameObject tempGun = GetPrefabFromSlotHolder(GunHolder.GetChild(gunIndex));
        GetGunIntoUnarmedPos(tempGun);

        //set currentGun and resetCurrentGun position
        currentGun = tempGun;
        currentGunIndex = gunIndex;
        ResetGunPosition();
    }
   */
}
