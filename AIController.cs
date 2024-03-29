﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform path;
    private List<Transform> nodes;
    private int nodeNumber = 0;

    private float steeringAngle = 30f;
    public float motorTorque = 25000f;
    private float carSpeed = 0f;
    private float brakeIntensity = 150f;

    public List<WheelCollider> wheels;//the first 2 being the front wheels and the last 2 the back ones[LF, LR, BL, BR]
    public List<GameObject> wheelMeshes;
    public List<GameObject> steeringWheels;

    public GameObject brakeLight;
    public GameObject reverseLight;

    public bool brakesApplied = false;
    public bool reverseApply = false;

    public Rigidbody rigidB;

    //for gearbox
    public bool movingVehicle = false;
    
    private float[] GearRatio = new float[] {3.437f, 3.626f, 2.188f, 1.541f, 1.213f, 1f, 0.76f }; //[R, 1, 2, 3, 4, 5, 6]
    private float finalDriveRatio = 4.1f;// this is multiplied with each gear ratio
    private int currentGear = 1;
    public float engineRPM = 0f;

    //engine characteristics
    public float maxEngineRPM = 7000F;
    private float MinEngineRPM = 950.0f;
    private float shiftUpRPM = 5500.0f;
    private float downShiftRPM = 3000.0f;
    private int index = 0;

    float[] engineEfficiency = {0f, 0.2f, 0.22f, 0.24f, 0.26f, 0.3f, 0.4f, 0.42f, 0.44f, 0.46f, 0.48f, 0.5f, 0.55f, 0.58f, 0.6f, 0.62f, 0.64f, 0.66f, 0.68f, 0.72f, 0.76f, 0.8f, 0.85f, 0.87f, 0.93f, 0.96f, 1.0f, 1.0f, 0.85f};
    float engineEfficiencyStep = 250.0f;

    private float newTorque = 0.0f;

    [Header("Sensors")]
    public float sensorLen = 30f;
    public Vector3 frontSensorPos = new Vector3(0f, 0.55f, 0f);
    public float frontSideSensorPos = 0.75f;
    public float frontSensorAngled = 30f;
    private bool collision = false;//if anything is hit, this becomes true

    public float objectSteeringAngle = 0f;

    public void ShiftUp()
    {
        if ((currentGear > 0 && currentGear < GearRatio.Length - 1 && engineRPM > shiftUpRPM) || (currentGear == 0 && engineRPM < 1050f))
        {
            currentGear++;
        }
    }

    public void ShiftDown()
    {
        if ((currentGear > 1) || (currentGear == 1 && engineRPM < 1050f))
        {
            currentGear--;
        }
    }

    void Start()
    {
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }

        rigidB.centerOfMass = new Vector3(0.0f, -0.2f, 0.0f);
    }

    void FixedUpdate()
    {
        engineRPM = MinEngineRPM + 2 * (30f * rigidB.velocity.magnitude * GearRatio[currentGear] * finalDriveRatio) / (3.6f * Mathf.PI * 0.3f);

        //automatic shifting 
        if (currentGear == 1 && reverseApply)
        {
            ShiftDown();//reverse
        }
        else if (currentGear == 0 && movingVehicle)
        {
            ShiftUp();//to change from reverse to drive
        }
        else if (movingVehicle && engineRPM > shiftUpRPM)
        {
            ShiftUp();//shift up
        }
        else if (engineRPM < downShiftRPM && currentGear > 1)
        {
            ShiftDown();//down shift
        }

        index = (int)(engineRPM / engineEfficiencyStep);
        if (index >= engineEfficiency.Length)
        {
            index = engineEfficiency.Length - 1;
        }
        if (index < 0)
        {
            index = 0;
        }

        newTorque = motorTorque * GearRatio[currentGear] * finalDriveRatio * engineEfficiency[index];

        GetWayPointDistance();
        Sensors();
        CheckWheelSlip();
        Move();
        ApplySteer();
        WheelRotation();
        CarBrake();
        LerpedSteerAngles();
        Reverse();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStart = transform.position;
        sensorStart += transform.forward * frontSensorPos.z;//to place the sensor at the front of the car
        sensorStart += transform.up * frontSensorPos.y;//to place the sensor a bit higher on the x axis
        float collisionMultiplicator = 0f;
        collision = false;

        //front right sensor
        sensorStart += transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
        {
            if (!hit.collider.CompareTag("Terrain")) 
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;
                collisionMultiplicator -= 1f;
            }
        }

        //front right angled sensor
        else if (Physics.Raycast(sensorStart, Quaternion.AngleAxis(frontSensorAngled, transform.up) * transform.forward, out hit, sensorLen))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;
                collisionMultiplicator -= 0.5f;
            }
        }

        //front left sensor
        sensorStart -= transform.right * 2 * frontSideSensorPos;
        if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;
                collisionMultiplicator += 1f;
            }
        }

        //front left angled sensor
        else if (Physics.Raycast(sensorStart, Quaternion.AngleAxis(-frontSensorAngled, transform.up) * transform.forward, out hit, sensorLen))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;
                collisionMultiplicator += 0.5f;
            }
        }

        //front center sensor
        if (collisionMultiplicator == 0)
        {
            if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
            {

                if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("LapCheckpoint"))
                {
                    Debug.DrawLine(sensorStart, hit.point);
                    collision = true;

                    if (hit.normal.x < 0)
                    {
                        collisionMultiplicator = -1f;
                    }
                    else
                    {
                        collisionMultiplicator = 1f;
                    }
                }
            }
        }
        
        if (carSpeed < 30f && (collisionMultiplicator == 1f || collisionMultiplicator == -1f))
        {
            if(collisionMultiplicator == 1f)
            {
                wheels[2].motorTorque = -0.7f * motorTorque;
                wheels[3].motorTorque = -0.5f * motorTorque;
                reverseApply = true;
            } else if (collisionMultiplicator == -1f)
            {
                wheels[2].motorTorque = -0.5f * motorTorque;
                wheels[3].motorTorque = -0.7f * motorTorque;
                reverseApply = true;
            }
        }
        else
        {
            wheels[2].motorTorque = motorTorque;
            wheels[3].motorTorque = motorTorque;
            reverseApply = false;
        }

        //check if there's an obstacle to avoid
        if (collision)
        {
            objectSteeringAngle = steeringAngle * collisionMultiplicator;
        }
    }

    float wheelAngle = 0f;
    private void ApplySteer()
    {
        if (collision) return;
        Vector3 relVector = transform.InverseTransformPoint(nodes[nodeNumber].position);
        float horizontalSteerVal = relVector.x / relVector.magnitude;
        wheelAngle = steeringAngle * horizontalSteerVal;
        objectSteeringAngle = wheelAngle;
    }

    private void CheckWheelSlip()
    {
        foreach(WheelCollider wheel in wheels)
        {
            WheelHit hit = new WheelHit();

            //determine if the wheel is grounded
            bool grounded = wheel.GetGroundHit(out hit);
            brakesApplied = hit.sidewaysSlip < -0.2f ? true : false;
        }
    }

    private void Move()
    {
        float maxSpeed = 240f;
        movingVehicle = true;

        carSpeed = 3.6f * rigidB.velocity.magnitude;

        if(carSpeed < maxSpeed && !brakesApplied)
         {
             for (int i = 2; i < 3; i++)
             {
                 wheels[i].motorTorque = newTorque;
             }
         }
         else
         {
             for (int i = 2; i < 3; i++)
             {
                 wheels[i].motorTorque = 0;
             }
         }
    }

    private void GetWayPointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[nodeNumber].position) < 0.5f)
        {
            if(nodeNumber == nodes.Count - 1)
            {
                nodeNumber = 0;
            } else nodeNumber++;
        }
    }

    public void WheelRotation()
    {
        foreach (GameObject wheel in wheelMeshes)
        {
            wheel.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).x >= 0 ? -1 : 1) / (2 * Mathf.PI * 0.32f), 0f, 0f);
        }
    }

    private void CarBrake()
    {
        if (brakesApplied)
        {
            brakeLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            foreach(WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = brakeIntensity;
            }
        }
        else
        {
            brakeLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = 0f;
            }
        }
    }

    private void LerpedSteerAngles()
    {
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = objectSteeringAngle;
        }
    }

    private void Reverse()
    {
        if (reverseApply)
        {
            reverseLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
        }
        else
        {
            reverseLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}
