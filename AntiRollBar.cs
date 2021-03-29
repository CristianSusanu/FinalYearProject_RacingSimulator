using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;
    private Rigidbody car;

    private float frontAxleAntiRoll = 300.0f;
    private float rearAxleAntiRoll = 450.0f;

    void Start()
    {
        car = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        WheelHit hit = new WheelHit();
        float travelFrontWheelLeft = 1.0f;
        float travelFrontWheelRight = 1.0f;

        //front axle
        //determine if the wheels are grounded
        bool groundedFrontLeft = FrontLeftWheel.GetGroundHit(out hit);
        bool groundedFrontRight = FrontRightWheel.GetGroundHit(out hit);

        //calculate suspension travel on left wheel
        if (groundedFrontLeft)
        {
            travelFrontWheelLeft = (-FrontLeftWheel.transform.InverseTransformPoint(hit.point).y - FrontLeftWheel.radius) / FrontLeftWheel.suspensionDistance;
        }

        //calculate suspension travel on right wheel
        if (groundedFrontRight)
        {
            travelFrontWheelRight = (-FrontRightWheel.transform.InverseTransformPoint(hit.point).y - FrontRightWheel.radius) / FrontRightWheel.suspensionDistance;
        }

        var FrontAxleAntiRollForce = (travelFrontWheelLeft - travelFrontWheelRight) * frontAxleAntiRoll;

        if (groundedFrontLeft)
            car.AddForceAtPosition(FrontLeftWheel.transform.up * -FrontAxleAntiRollForce, FrontLeftWheel.transform.position);
        if (groundedFrontRight)
            car.AddForceAtPosition(FrontRightWheel.transform.up * FrontAxleAntiRollForce, FrontRightWheel.transform.position);

        //rear axle
        float travelBackWheelLeft = 1.0f;
        float travelBackWheelRight = 1.0f;

        //determine if the wheels are grounded
        bool groundedBackLeft = BackLeftWheel.GetGroundHit(out hit);
        bool groundedBackRight = BackRightWheel.GetGroundHit(out hit);

        //calculate suspension travel on left wheel
        if (groundedBackLeft)
        {
            travelBackWheelLeft = (-BackLeftWheel.transform.InverseTransformPoint(hit.point).y - BackLeftWheel.radius) / BackLeftWheel.suspensionDistance;
        }

        //calculate suspension travel on right wheel
        if (groundedBackRight)
        {
            travelBackWheelRight = (-BackRightWheel.transform.InverseTransformPoint(hit.point).y - BackRightWheel.radius) / BackRightWheel.suspensionDistance;
        }

        var antiRollForce = (travelBackWheelLeft - travelBackWheelRight) * rearAxleAntiRoll;

        if (groundedBackLeft)
            car.AddForceAtPosition(BackLeftWheel.transform.up* -antiRollForce, BackLeftWheel.transform.position);
        if (groundedBackRight)
            car.AddForceAtPosition(BackRightWheel.transform.up* antiRollForce, BackRightWheel.transform.position);
    }

}