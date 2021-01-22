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

    [SerializeField]
    private Slider sliderMagnus;

    [SerializeField]
    private Text textMagnus;

    private float spaceBarForce = 0;

    private float effectForce = 0;

    private bool increase = true;

    [SerializeField]
    private Animator robot1, robot2, robot3, robot4;
    
    // Start is called before the first frame update
    void Start()
    {
        sliderMagnus.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(robot1.GetBool("Restart"))
        {
            robot1.SetBool("Restart", false);
        }
        if(robot2.GetBool("Restart"))
        {
            robot2.SetBool("Restart", false);
        }
        if(robot3.GetBool("Restart"))
        {
            robot3.SetBool("Restart", false);
        }
        if(robot4.GetBool("Restart"))
        {
            robot4.SetBool("Restart", false);
        }

        if (Input.GetButtonDown("Restart"))
        {
            ball.Restart();
            scorpion.GetComponent<IK_Scorpion>().Restart();
            movingTarget.Restart();
            slider.value = slider.minValue;
            spaceBarForce = 0;
            text.text = "FORCE: " + (int)spaceBarForce;
            sliderMagnus.value = 0.5f;
            effectForce = 0;
            textMagnus.text = "EFFECT FORCE: " + effectForce;
            robot1.SetBool("Restart", true);
            robot2.SetBool("Restart", true);
            robot3.SetBool("Restart", true);
            robot4.SetBool("Restart", true);
            robot1.SetBool("Goal", false);
            robot2.SetBool("Goal", false);
            robot3.SetBool("Goal", false);
            robot4.SetBool("Goal", false);
            robot1.SetBool("Miss", false);
            robot2.SetBool("Miss", false);
            robot3.SetBool("Miss", false);
            robot4.SetBool("Miss", false);
        }

        if (ball.adjustShot)
        {
            movingTarget.verticalInput = Input.GetAxis("Vertical");
            movingTarget.horizontalInput = Input.GetAxis("Horizontal");

            ball.arrow.forward = movingTarget.transform.position - ball.transform.position;

            if (Input.GetButtonDown("Shot"))
            {
                spaceBarForce = 0;
                increase = true;
                if (ball.showInfo)
                {
                    ball.arrowGreen.SetActive(true);
                    ball.arrowRed.SetActive(true);
                    ball.showTrajectory = true;
                }
            }
            else if (Input.GetButton("Shot"))
            {
                if (increase)
                {
                    spaceBarForce += 1;
                    if (spaceBarForce > 100)
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

                ball.force = slider.value * 5;
                ball.arrowGreen.transform.forward = ball.magnusDirection;
                ball.arrowRed.transform.forward = ball.effectDirection;
            }
            else if (Input.GetButtonUp("Shot"))
            {
                movingTarget.move = false;
                ball.SetShot();
            }

            if (Input.GetKey(KeyCode.X))
            {
                effectForce += 1;
                if (effectForce > 100) effectForce = 100;
                sliderMagnus.value = (effectForce + 100) / 200;
                textMagnus.text = "EFFECT FORCE: " + effectForce;
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                effectForce -= 1;
                if (effectForce < -100) effectForce = -100;
                sliderMagnus.value = (effectForce + 100) / 200;
                textMagnus.text = "EFFECT FORCE: " + effectForce;
            }

            if (movingTarget.move)
            {
                float value = (sliderMagnus.value - 0.5f) * 2;
                value = Mathf.Clamp(value, -0.8f, 0.8f);

                float angle = Mathf.Acos(value);
                float y = -Mathf.Sin(angle);
                ball.magnusForce = new Vector3((sliderMagnus.value - 0.5f) * 2, 0, y).normalized;
                ball.impactPoint = -ball.magnusForce * 0.25f;
                ball.transform.GetChild(3).transform.position = ball.transform.position + ball.impactPoint;
            }
        }
        else
        {
            movingTarget.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y, movingTarget.transform.position.z);
        }

        ball.shoot = !scorpion.GetComponent<IK_Scorpion>().animPlaying;

        if(ball.impact) ball.SetShotDirection();

        if(ball.goal)
        {
            robot1.SetBool("Goal", true);
            robot2.SetBool("Goal", true);
            robot3.SetBool("Goal", true);
            robot4.SetBool("Goal", true);
        }
        else if(ball.miss)
        {
            robot1.SetBool("Miss", true);
            robot2.SetBool("Miss", true);
            robot3.SetBool("Miss", true);
            robot4.SetBool("Miss", true);
        }
    }
}