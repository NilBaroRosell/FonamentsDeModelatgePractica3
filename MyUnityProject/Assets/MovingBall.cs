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
    private Vector3 magnusDirection;
    private Vector3 effectDirection;
    public Vector3 impactPoint;
    private Vector3 finalForce;

    private Vector3 initPos;

    private GameObject positionsBlue;
    private GameObject arrowGreen;
    private GameObject arrowRed;

    public Transform arrow;

    private Vector3 arrowInitialForward;

    float mass = 0.5f;

    private bool showInfo = false;

    public bool ajustShot = true;

    private float startTime;

    private bool stopped;

    public bool goal;

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

        if (!goal && (transform.position.z < -71 && transform.position.z < -71.5f) && (transform.position.x < -109 && transform.position.x > -149) && transform.position.y < 24)
        {
            Debug.Log("GOOOAL!!");
            goal = true;
        }

        CalculateNewRotation();
    }

    public void SetShotForce(float _force, Vector3 _direction)
    {
        force = _force;
        direction = _direction.normalized;
        if (showInfo)
        {
            arrowGreen.SetActive(true);
            arrowGreen.transform.forward = magnusDirection;
            arrowRed.SetActive(true);
            arrowRed.transform.forward = effectDirection;
        }
        ajustShot = false;
    }

    public void SetShotDirection()
    {
        impact = false;
        positionsBlue.SetActive(true);
        arrowGreen.SetActive(false);
        arrowRed.SetActive(false);
    }

    public void Restart()
    {
        gameObject.transform.position = initPos;
        direction = Vector3.zero;
        shoot = false;
        positionsBlue.SetActive(false);
        arrowGreen.SetActive(false);
        arrowRed.SetActive(false);
        ajustShot = true;
        arrow.forward = arrowInitialForward;
        startTime = -1;
        stopped = false;
        _myOctopus.NotifyEndShoot();
        goal = false;
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
