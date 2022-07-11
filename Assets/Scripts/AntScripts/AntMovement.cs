using UnityEngine;
using System.Collections;

public class AntMovement : MonoBehaviour
{
    private float antUnit;
    private IkLegController ikScript;

    //for speed;
    [Range(2f, 5f)]
    public float maxSpeedPercent = 2f;
    private float speedMultiplier = 2f;
    private float currentSpeed;



    //for wandering and turnning
    [Range(0.1f, 1f)]
    public float wanderStrength = 0.1f;
    private float maxSpeed = 5;
    public float steerStrength = 10;
    [Range(0.01f, 5f)]
    public float turnRate = 1;

    public float collisionTurnStrength = 20;
    public LayerMask thingsThatAntsAvoid;

    private Rigidbody rb;


    float angle;
    Vector3 position;
    Vector3 velocity;
    Vector3 desiredDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Init();
        StartCoroutine(Wandering(Random.Range(0.01f,2f)));
    }

    void Init()
    {
        ikScript = GetComponent<IkLegController>();
        antUnit = ikScript.GetUnit();
        UpdateSpeed();
        
    }

    private void FixedUpdate()
    {
        UpdateSpeed();
        //transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
       // position = transform.position;
        
        /*
        if((transform.forward - desiredDirection).sqrMagnitude>=0.1f)
            transform.forward = Vector3.Lerp(transform.forward, desiredDirection*steerStrength, Time.fixedDeltaTime);
        else
        {
            Debug.Log("hello");
            FindANewDirection();
        }
        */
        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime; 
    }
    IEnumerator Wandering(float waitTime)
    {
        while (true)
        {
            if (Random.Range(0, 1f) >= 0.2f)
            {
                FindANewDirection();
                Debug.Log(angle);
                float counter = 0;
                float normalTurnRate = turnRate;
                if (angle < 0)
                    normalTurnRate = -turnRate;
                else
                    normalTurnRate = turnRate;
                while (true)
                {
                    if (counter < Mathf.Abs(angle))
                    {
                        transform.Rotate(0, normalTurnRate, 0, Space.Self);
                        //  counter += Mathf.Abs(angle / turnRate);
                        counter += turnRate;
                    }
                    else
                    {
                        break;
                    }
                    yield return null;
                }
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
    void FindANewDirection()
    {
        int random = Random.Range(1, 3);
        if (random == 1)
            angle = Random.Range(0f, 90f);
        else if (random == 2)
            angle = Random.Range(-90f, 0);
        else
            angle = 0;

    }

    void UpdateSpeed()
    {
        currentSpeed = antUnit * maxSpeedPercent * speedMultiplier;
        float value = maxSpeedPercent * speedMultiplier;
        int smoothness = UpdateSmoothness(value);
        ikScript.SetSmoothness(smoothness);
    }
    int UpdateSmoothness(float value)
    {
        if (value >= 2 && value < 4)
            return 7;
        else if (value >= 4 && value < 6)
            return 6;
        else if (value >= 6 && value < 8)
            return 5;
        else
            return 4;
    }

}



/*
 
 
 using UnityEngine;
using System.Collections;

public class AntMovement : MonoBehaviour
{
    private float antUnit;
    private IkLegController ikScript;

    //for speed;
    [Range(2f, 5f)]
    public float maxSpeedPercent = 2f;
    private float speedMultiplier = 2f;
    private float currentSpeed;



    //for wandering and turnning
    [Range(0.1f, 1f)]
    public float wanderStrength = 0.1f;
    private float maxSpeed = 5;
    public float steerStrength = 10;

    public float collisionTurnStrength = 20;
    public LayerMask thingsThatAntsAvoid;

    private Rigidbody rb;


    float angle;
    Vector3 position;
    Vector3 velocity;
    Vector3 desiredDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Init();
        StartCoroutine(Wandering(0.5f));
    }

    void Init()
    {
        ikScript = GetComponent<IkLegController>();
        antUnit = ikScript.GetUnit();
        UpdateSpeed();
        
    }

    private void FixedUpdate()
    {
       // FindANewDirection();

        UpdateSpeed();
        //transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
       // position = transform.position;
        
        
        if((transform.forward - desiredDirection).sqrMagnitude>=0.1f)
            transform.forward = Vector3.Lerp(transform.forward, desiredDirection*steerStrength, Time.fixedDeltaTime);
        else
        {
            Debug.Log("hello");
            FindANewDirection();
        }
        
transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
    }
    IEnumerator Wandering(float waitTime)
{
    while (true)
    {
        FindANewDirection();
        while (true)
        {

            if ((transform.forward - desiredDirection).magnitude >= 0.1f)
            {
                transform.forward = Vector3.Lerp(transform.forward, desiredDirection, Time.fixedDeltaTime * 100);
                Debug.Log("lerp");
            }
            else
            {
                Debug.Log("break");

                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
    }
}
void FindANewDirection()
{
    Vector3 randomUnitSphere = Random.insideUnitSphere;


    desiredDirection = Vector3.ProjectOnPlane(randomUnitSphere * wanderStrength, transform.up);
}

void UpdateSpeed()
{
    currentSpeed = antUnit * maxSpeedPercent * speedMultiplier;
    float value = maxSpeedPercent * speedMultiplier;
    int smoothness = UpdateSmoothness(value);
    ikScript.SetSmoothness(smoothness);
}
int UpdateSmoothness(float value)
{
    if (value >= 2 && value < 4)
        return 7;
    else if (value >= 4 && value < 6)
        return 6;
    else if (value >= 6 && value < 8)
        return 5;
    else
        return 4;
}
private void OnDrawGizmos()
{
    Gizmos.color = Color.black;
    Gizmos.DrawRay(transform.position, desiredDirection);
}

}




 
 
 
 
 
 
 
 */