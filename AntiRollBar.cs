using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public WheelCollider WheelL;
    public WheelCollider WheelR;
    private Rigidbody carRigidBody;

    public float AntiRoll = 5000.0f;

    void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        WheelHit hit = new WheelHit();
        float travelL = 1.5f;
        float travelR = 1.5f;

        bool groundedL = WheelL.GetGroundHit(out hit);

        if (groundedL)
        {
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y
                    - WheelL.radius) / WheelL.suspensionDistance;
        }

        bool groundedR = WheelR.GetGroundHit(out hit);

        if (groundedR)
        {
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y
                    - WheelR.radius) / WheelR.suspensionDistance;
        }

        var antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            carRigidBody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                WheelL.transform.position);
        if (groundedR)
            carRigidBody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                WheelR.transform.position);
    }

}