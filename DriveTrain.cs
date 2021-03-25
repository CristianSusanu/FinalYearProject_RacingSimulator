using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTrain : MonoBehaviour
{
    public float[] gearRatios = new float[] { 3.587f, 2.022f, 1.384f, 1f, 0.861f }; //[1, 2, 3, 4, 5] 3.484f pt R T50 5 speed engine
    private float finalDriveRatoin = 4.3f;// this is multiplied with each gear ratio

    //engine charateristics
    public float maxRPM = 8000.0f;
    public float minRPM = 1100.0f;

    //Engine max torque NM and RPM
    public float maxTorque = 149;//149NM at 5200RPM
    public float torqueRPM = 5200;

    //Engine max power in Watts and RPM
    public float maxPower = 96000;
    public float powerRPM = 6600;

    //engine inertia (how fast the engine spins up) in kg * m^2
    public float engineInertia = 0.3f;

    //engine friction coefficients - cause the engine to slow down,giving engine breaking
    //constant friction coeff.
    public float engineBaseFriction = 25f;
    //linear friction coeff (When engine spins faster)
    public float engineRPMFriction = 0.02f;

    // Engine orientation (typically either Vector3.forward or Vector3.right). 
    // This determines how the car body moves as the engine revs up.	
    public Vector3 engineOrientation = Vector3.forward;

    // Coefficient determining how muchg torque is transfered between the wheels when they move at 
    // different speeds, to simulate differential locking.
    public float differentialLockCoefficient = 0;

    // shift gears automatically?
    public bool automatic = true;

    // inputs
    // engine throttle
    public float throttle = 0;
    // engine throttle without traction control (used for automatic gear shifting)
    public float throttleInput = 0;

    // state
    public int gear = 1;
    public float rpm;
    public float slipRatio = 0.0f;
    float engineAngularVelo;

    float Sqr(float x) { return x * x; }

    // Calculate engine torque for current rpm and throttle values.
    float CalcEngineTorque()
    {
        float result;
        if (rpm < torqueRPM)
            result = maxTorque * (-Sqr(rpm / torqueRPM - 1) + 1);
        else
        {
            float maxPowerTorque = maxPower / (powerRPM * 2 * Mathf.PI / 60);
            float aproxFactor = (maxTorque - maxPowerTorque) / (2 * torqueRPM * powerRPM - Sqr(powerRPM) - Sqr(torqueRPM));
            float torque = aproxFactor * Sqr(rpm - torqueRPM) + maxTorque;
            result = torque > 0 ? torque : 0;
        }
        if (rpm > maxRPM)
        {
            result *= 1 - ((rpm - maxRPM) * 0.006f);
            if (result < 0)
                result = 0;
        }
        if (rpm < 0)
            result = 0;
        return result;
    }
    void FixedUpdate () 
	{
		
	}
}
