using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 initPos;

    private void Awake()
    {
        initPos = gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(direction != Vector3.zero)
        {
            if(shoot)
            {
                velocity = direction * force;
                transform.position = transform.position + velocity * Time.deltaTime;
            }
        }
        //transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        //float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        //float verticalInput = Input.GetAxis("Vertical");

        //update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);

    }

    public void SetShotForce(float _force)
    {
        force = _force;
    }

    public void SetShotDirection(Vector3 _direction)
    {
        direction = _direction.normalized;
        impact = false;
    }

    public void Restart()
    {
        gameObject.transform.position = initPos;
        direction = Vector3.zero;
        shoot = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Octopus")
        {
            direction = Vector3.zero - new Vector3(direction.x, -0.5f, direction.z);
        }
        else
        {
            impact = true;
            _myOctopus.NotifyShoot();
        }
    }
}
