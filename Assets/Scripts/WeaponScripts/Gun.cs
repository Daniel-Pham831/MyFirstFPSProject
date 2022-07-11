using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    /*
     * every gun has its own values,and bullet type
     * whenever player press mouse(0) button 
     * => a bullet will spawn and travel toward the camera.main forward direction
     *

    */
    public ParticleSystem muzzle;
    private Animator animator;
    public float bulletPerSecond;  //bullet per second;
    private float nextFire = 0.0f;
    public int maxBulletsPerRound; //how many bullets per round ex: 30 bullets per round
    private int currentBulletsPerRound;
    public int maxRound; //how many round of bullets that the gun can carry
    private int currentRound;

    public Transform fireTip; //where the bullet spawn;
    public Transform gunStates;
    public readonly float smoothtimeToSwitchState = 15f;
    public Transform bulletsHolder; // where to parent the spawned bullet

    public Camera cam;
    public GameObject bulletPrefab;

    public float bulletSpeed;
    public float bulletDmg;

    
    private Vector3 fireDir;
    public bool isFireAble;

    private void Start()
    {
        currentBulletsPerRound = maxBulletsPerRound;
        currentRound = maxRound;
        animator = GetComponent<Animator>();
        animator.SetTrigger("Into-Idle");
    }
    private void Update()
    {
        if (!isFireAble) return;

        Shoot();
        ReloadGun();
        
    }

   
    


    void Shoot()
    {
        /*if (currentBulletsPerRound == 0)
        {
            Debug.Log("out of bullets");
            return;
        }*/
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
           
            animator.SetBool("IsFire", true);
            if(muzzle != null)
                muzzle.Play();



            nextFire = Time.time + 1f / bulletPerSecond;
            // currentBulletsPerRound--; //bullet

            GameObject tempBullet;

            tempBullet = Instantiate(bulletPrefab, fireTip.position, Quaternion.identity);

            RaycastHit hit;
            if (Physics.Raycast(fireTip.position, cam.transform.forward, out hit))
            {
                fireDir = (hit.point - fireTip.position).normalized;
            }
            else
            {
                fireDir = cam.transform.forward;
            }

            tempBullet.transform.parent = bulletsHolder.transform;
            Bullet bl = tempBullet.GetComponentInChildren<Bullet>();
            bl.SetBulletSpeed(bulletSpeed);
            bl.SetFireDirection(fireDir);
            bl.SetMove(true);
            

        }
        else
        {
            animator.SetBool("IsFire", false);
           

        }
    }

    void ReloadGun()
    {
        if (currentRound == 0)
        {
            Debug.Log("out of rounds");
        }

        if (Input.GetKeyDown(KeyCode.R) && currentRound > 0)
        {
            Debug.Log("reloaded");
            currentBulletsPerRound = maxBulletsPerRound;
            currentRound--;
        }
    }

   
}
