using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [HideInInspector]
    public float force;

    [HideInInspector]
    public bool shoot = false;

    [HideInInspector]
    public bool impact = false;

    [HideInInspector]
    public Vector3 direction;

    private Vector3 velocity;

    public Vector3 magnusForce;
    [HideInInspector]
    public Vector3 magnusDirection;
    [HideInInspector]
    public Vector3 effectDirection;
    [HideInInspector]
    public Vector3 impactPoint;
    private Vector3 finalForce;

    private Vector3 initPos;

    private GameObject positionsBlue;
    [HideInInspector]
    public GameObject arrowGreen;
    [HideInInspector]
    public GameObject arrowRed;

    public Transform arrow;

    private Vector3 arrowInitialForward;

    float mass = 0.5f;
    [HideInInspector]
    public bool showInfo = false;
    [HideInInspector]
    public bool adjustShot = true;

    private float startTime;

    private bool stopped;
    [HideInInspector]
    public bool goal;
    public bool miss;
    [HideInInspector]
    public bool showTrajectory;

    [SerializeField]
    private Text rotationVelocityText;

    private void Awake()
    {
        initPos = gameObject.transform.position;
        positionsBlue = gameObject.transform.GetChild(0).gameObject;
        arrowGreen = gameObject.transform.GetChild(1).gameObject;
        arrowGreen.SetActive(false);
        arrowRed = gameObject.transform.GetChild(2).gameObject;
        arrowRed.SetActive(false);
        showInfo = false;
        arrowInitialForward = arrow.forward;
        startTime = - 1;
        stopped = false;
        goal = false;
        miss = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) showInfo = !showInfo;

        if (direction != Vector3.zero)
        {
            if(shoot && !stopped)
            {
                velocity = magnusDirection * force;
                transform.position = GetNewPos();
            }
        }

        if (!goal && transform.position.z < -71 && (transform.position.x < -108 && transform.position.x > -150) && transform.position.y < 24)
        {
            Debug.Log("GOOOAL!!");
            goal = true;
        }
        else if(!goal && !miss && transform.position.z < -71)
        {
            Debug.Log("MIIISS!!");
            miss = true;
        }

        CalculateNewRotation();
    }

    public void SetShot()
    {
        direction = arrow.forward;
        adjustShot = false;
    }

    public void SetShotDirection()
    {
        impact = false;
        positionsBlue.SetActive(true);
        arrowGreen.SetActive(false);
        arrowRed.SetActive(false);        
        showTrajectory = false;        
    }

    public void Restart()
    {
        gameObject.transform.position = initPos;
        direction = Vector3.zero;
        shoot = false;
        positionsBlue.SetActive(false);
        arrowGreen.SetActive(false);
        arrowRed.SetActive(false);
        adjustShot = true;
        arrow.forward = arrowInitialForward;
        startTime = -1;
        stopped = false;
        _myOctopus.NotifyEndShoot();
        goal = false;
        miss = false;
    }

    private Vector3 GetNewPos()
    {
        if(startTime == -1)
        {
            startTime = Time.time;
        }

        float deltaTime = Time.time - startTime;
        
        float x = initPos.x + velocity.x * deltaTime + 0.5f * finalForce.x * deltaTime * deltaTime;
        float y = initPos.y + velocity.y * deltaTime + 0.5f * finalForce.y * deltaTime * deltaTime;
        float z = initPos.z + velocity.z * deltaTime + 0.5f * finalForce.z * deltaTime * deltaTime;

        return new Vector3(x, y, z);
    }
    private Vector3 GetNewRenderPos(float t)
    {
        float x = initPos.x + velocity.x * t + 0.5f * finalForce.x * t * t;
        float y = initPos.y + velocity.y * t + 0.5f * finalForce.y * t * t;
        float z = initPos.z + velocity.z * t + 0.5f * finalForce.z * t * t;

        return new Vector3(x, y, z);
    }
    private Vector3 GetNewRenderOriginalPos(float t)
    {
        Vector3 newVel = arrow.forward * force * 3;

        float x = initPos.x + newVel.x * t;
        float y = initPos.y + newVel.y * t;
        float z = initPos.z + newVel.z * t;

        return new Vector3(x, y, z);
    }

    private void CalculateNewRotation()
    {
        magnusDirection = magnusForce * force;

        Vector3 newVel = arrow.forward * force;

        float magnusOffset = Vector3.SignedAngle(-transform.forward, magnusDirection, transform.up);

        magnusDirection = newVel.normalized * force;
        Quaternion rotation = Quaternion.Euler(0, magnusOffset, 0);
        magnusDirection = rotation * magnusDirection;

        Debug.DrawRay(transform.position, magnusDirection, Color.red);
        Debug.DrawRay(transform.position, newVel, Color.magenta);

        Vector3 rotVector = Vector3.Project(newVel, magnusDirection);
        Vector3 projectPoint = transform.position + rotVector;
        effectDirection = transform.position + newVel - projectPoint;
        Debug.DrawRay(transform.GetChild(3).transform.position, effectDirection, Color.black);

        Vector3 torque = Vector3.Cross(impactPoint, effectDirection);
        Vector3 acceleration = effectDirection / mass;
        Vector3 velocityAux = 0.1f * acceleration;
        Debug.DrawRay(transform.position, torque, Color.blue);
        Debug.DrawRay(transform.position, velocityAux, Color.white);

        float rotationVelocity = velocityAux.magnitude / impactPoint.magnitude;

        if (torque.y < 0)
            rotationVelocity *= -1;

        rotationVelocity *= 2;

        rotationVelocityText.text = rotationVelocity.ToString("F1") + " deg/sec";

        finalForce = -Vector3.right * rotationVelocity;
    }

    void OnDrawGizmosSelected()
    {
        if (showTrajectory)
        {
            CalculateNewRotation();
            velocity = magnusDirection * force;

            float t = 0;
            for (int i = 0; i < 30; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(GetNewRenderPos(t), 0.25f);
                Gizmos.color = Color.gray;
                Gizmos.DrawSphere(GetNewRenderOriginalPos(t), 0.25f);
                t += 0.1f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Octopus")
        {
            stopped = true;
        }
        else
        {
            impact = true;
        }
    }
}
