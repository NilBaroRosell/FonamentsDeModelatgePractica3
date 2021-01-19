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
    
    // Start is called before the first frame update
    void Start()
    {
        sliderMagnus.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
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
        }

        if (ball.ajustShot)
        {
            movingTarget.verticalInput = Input.GetAxis("Vertical");
            movingTarget.horizontalInput = Input.GetAxis("Horizontal");

            ball.arrow.forward = movingTarget.transform.position - ball.transform.position;

            if (Input.GetButtonDown("Shot"))
            {
                spaceBarForce = 0;
                increase = true;
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
            }
            else if (Input.GetButtonUp("Shot"))
            {
                movingTarget.move = false;
                if(slider.value < 0.5f)
                {
                    ball.SetShotForce(0.5f * 5, movingTarget.transform.position - ball.transform.position);
                }
                else
                {
                    ball.SetShotForce(slider.value * 5, movingTarget.transform.position - ball.transform.position);
                }
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
    }
}
