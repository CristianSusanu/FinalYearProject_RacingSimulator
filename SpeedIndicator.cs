﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedIndicator : MonoBehaviour
{
    public Rigidbody car;
    public WheelCollider tractionWheel1;
    public WheelCollider tractionWheel2;
    public CarControl carControl;

    //for speed needle
    private float maxSpeed = 240f;//maximum speed of the vehicle
    private float speed = 0.0f;
    private float minSpeedArrowAngle = 0.0f;
    private float maxSpeedArrowAngle = -216.4f;

    //for rpm count needle
    private float maxRPM = 9000f;
    private float RPM = 0.0f;
    private float minRPMArrowAngle = -17f;
    private float maxRPMArrowAngle = -174;

    //for fuel needle
    private float maxFuelArrowAngle = -92f;
    private float minFuelArrowAngle = 0.0f;
    private float fuelCapacity = 50.0f;
    private float fuelAvailable = 25.0f;

    //for oil pressure
    private float maxOilArrowAngle = -88f;

    public GameObject steeringWheel;

    public GameObject speedNeedle;
    public GameObject RPMNeedle;
    public GameObject fuelNeedle;
    public GameObject oilNeedle;
    public GameObject tempNeedle;

    public Animator driverAnimation;
    private float animatorTurnAngle;

    private void Update()
    {
        speed = car.velocity.magnitude * 3.6f;
        RPM = carControl.engineRPM;

        if (speedNeedle != null)
        {
            speedNeedle.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
        }

        if (RPMNeedle != null)
        {
            RPMNeedle.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minRPMArrowAngle + 18f, maxRPMArrowAngle, RPM / maxRPM));
        }
        
        if(steeringWheel != null)
        {
            steeringWheel.transform.localEulerAngles = new Vector3(0f, 0f, -carControl.inputManager.steering * 80); //steering wheel rotation

            animatorTurnAngle = Mathf.Lerp(animatorTurnAngle, -carControl.inputManager.steering, 30f * Time.deltaTime);
            driverAnimation.SetFloat("turnAngle", animatorTurnAngle);
        }

        if (fuelNeedle != null && oilNeedle != null && tempNeedle != null)
        {
            fuelNeedle.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minFuelArrowAngle, maxFuelArrowAngle, fuelAvailable / fuelCapacity));
            oilNeedle.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minFuelArrowAngle, maxOilArrowAngle, 0.5f));
            tempNeedle.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minFuelArrowAngle, maxOilArrowAngle, 0.5f));
        }
    }
}
