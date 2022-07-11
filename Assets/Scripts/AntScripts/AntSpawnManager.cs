using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawnManager : MonoBehaviour
{
    public GameObject antPrefab;
    public float timeBetweenSpawn;
    public float howManyAntsPerSpawn;
    float nextTimeSpawn;
    GameObject temp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextTimeSpawn)
        {
            nextTimeSpawn = Time.time + timeBetweenSpawn;
            for (int i = 0; i < howManyAntsPerSpawn; i++)
            {
                temp = Instantiate(antPrefab, transform.position, Quaternion.identity);
                temp.transform.parent = transform;
            }
        }
    }
}
