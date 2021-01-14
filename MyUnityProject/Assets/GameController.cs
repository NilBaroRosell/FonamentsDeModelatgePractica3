using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private MovingBall ball;

    [SerializeField]
    private GameObject scorpion;

    [SerializeField]
    private MovingTarget movingTarget;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    private float spaceBarForce = 0;

    private bool increase = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Restart"))
        {
            ball.Restart();
            scorpion.GetComponent<IK_Scorpion>().Restart();
            movingTarget.Restart();
        }

        //get the Input from Horizontal axis
        movingTarget.horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        movingTarget.verticalInput = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Shot"))
        {
            spaceBarForce = 0;
            increase = true;
        }
        else if (Input.GetButton("Shot"))
        {
            if(increase)
            {
                spaceBarForce += 1;
                if(spaceBarForce > 100)
                {
                    spaceBarForce = 100;
                    increase = false;
                }
            }
            else
            {
                spaceBarForce -= 1;
                if (spaceBarForce < 0)
                {
                    spaceBarForce = 0;
                    increase = true;
                }
            }

            slider.value = spaceBarForce / 100;
            text.text = "FORCE: " + (int)spaceBarForce;
        }
        else if (Input.GetButtonUp("Shot"))
        {
            ball.SetShotForce(spaceBarForce/3);
        }

        ball.shoot = !scorpion.GetComponent<IK_Scorpion>().animPlaying;

        if(ball.impact) ball.SetShotDirection(movingTarget.transform.position - ball.transform.position);
    }
}
