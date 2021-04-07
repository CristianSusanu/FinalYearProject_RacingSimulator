using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform path;
    private List<Transform> nodes;
    private int nodeNumber = 0;

    private float steeringAngle = 30f;
    public float turningSpeed = 5f;
    public float motorTorque = 25000f;
    private float carSpeed = 0f;
    private float brakeIntensity = 150f;

    //public WheelCollider FrontLeftWheel;
    //public WheelCollider FrontRightWheel;
    //public WheelCollider BackLeftWheel;
    //public WheelCollider BackRightWheel;
    public List<WheelCollider> wheels;//the first 2 being the front wheels and the last 2 the back ones[LF, LR, BL, BR]
    public List<GameObject> wheelMeshes;
    public List<GameObject> steeringWheels;

    public GameObject brakeLight;

    public bool brakesApplied = false;

    public Rigidbody rigidB;

    [Header("Sensors")]
    public float sensorLen = 30f;
    public Vector3 frontSensorPos = new Vector3(0f, 0.55f, 0f);
    public float frontSideSensorPos = 0.75f;
    public float frontSensorAngled = 30f;
    private bool collision = false;//if anything is hit, this becomes true
    private float objectSteeringAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();//to make sure the list is empty at the beginning

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);//if it's not our own transform, it adds it to the nodes array
            }
        }

        rigidB.centerOfMass = new Vector3(0.0f, -0.2f, 0.0f);
    }

    void FixedUpdate()
    {
        Sensors();
        Move();
        GetWayPointDistance();
        ApplySteer();
        WheelRotation();
        CarBrake();
        LerpedSteerAngles();
    }

    private void Sensors()
    {
        RaycastHit hit;
        //Vector3 sensorStart = transform.position + frontSensorPos;
        Vector3 sensorStart = transform.position;
        sensorStart += transform.forward * frontSensorPos.z;//to place the sensor at the front of the car
        sensorStart += transform.up * frontSensorPos.y;//to place the sensor a bit higher on the x axis
        float collisionMultiplicator = 0f;//for each sensor, a certain amount is added to this (right is negative, left is positive)
        collision = false;

        //front right sensor
        //sensorStart.x += frontSideSensorPos;
        sensorStart += transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
        {
            //check if the terrain is hit, to not avoid it as it is not an obstacle
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;//to indicate something else than terrain was hit
                collisionMultiplicator -= 1f;//need to avoid detected object at the right, so wheels turn left (negative wheel angle)
            }
        }

        //front right angled sensor
        else if (Physics.Raycast(sensorStart, Quaternion.AngleAxis(frontSensorAngled, transform.up) * transform.forward, out hit, sensorLen))
        {
            //check if the terrain is hit, to not avoid it as it is not an obstacle
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;//to indicate something else than terrain was hit
                collisionMultiplicator -= 0.5f;//need to avoid detected object at the right, so wheels turn left (negative wheel angle)
            }
        }

        //front left sensor
        //sensorStart.x -= 2 * frontSideSensorPos;
        sensorStart -= transform.right * 2 * frontSideSensorPos;
        if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
        {
            //check if the terrain is hit, to not avoid it as it is not an obstacle
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;//to indicate something else than terrain was hit
                collisionMultiplicator += 1f;//need to avoid detected object at the right, so wheels turn left (negative wheel angle)
            }
        }

        //front left angled sensor
        else if (Physics.Raycast(sensorStart, Quaternion.AngleAxis(-frontSensorAngled, transform.up) * transform.forward, out hit, sensorLen))
        {
            //check if the terrain is hit, to not avoid it as it is not an obstacle
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStart, hit.point);

                collision = true;//to indicate something else than terrain was hit
                collisionMultiplicator += 0.5f;//need to avoid detected object at the right, so wheels turn left (negative wheel angle)
            }
        }

        //front center sensor
        if(collisionMultiplicator == 0)
        {
            if (Physics.Raycast(sensorStart, transform.forward, out hit, sensorLen))
            {
                //check if the terrain is hit, to not avoid it as it is not an obstacle
                if (!hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStart, hit.point);
                    collision = true;//to indicate something else than terrain was hit

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

        //check if there's an obstacle to avoid
        if (collision)
        {
            objectSteeringAngle = steeringAngle * collisionMultiplicator;
        }
    }

    float wheelAngle = 0f;
    private void ApplySteer()
    {
        if (collision) return;//in case you avoid something, don't need to run this
        Vector3 relVector = transform.InverseTransformPoint(nodes[nodeNumber].position);
        float horizontalSteerVal = relVector.x / relVector.magnitude;
        wheelAngle = steeringAngle * horizontalSteerVal;
        objectSteeringAngle = wheelAngle;
        /*
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = wheelAngle;//to turn the car
            wheel.transform.localEulerAngles = new Vector3(0f, wheelAngle, 0f);//to turn the wheels
        }
        */
    }

    private void Move()
    {
        float maxSpeed = 240f;

        carSpeed = 3.6f * rigidB.velocity.magnitude;
        
        if(carSpeed < maxSpeed && !brakesApplied)
        {
            for (int i = 2; i < 3; i++)
            {
                wheels[i].motorTorque = motorTorque;
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
            //wheel.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(wheel.GetComponent<WheelCollider>().steerAngle, objectSteeringAngle, Time.deltaTime * turningSpeed);//to turn the car
            wheel.GetComponent<WheelCollider>().steerAngle = objectSteeringAngle;
            //wheel.transform.localEulerAngles = new Vector3(0f, objectSteeringAngle, 0f);//to turn the wheels
            //wheel.transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(wheel.GetComponent<WheelCollider>().steerAngle, objectSteeringAngle, Time.deltaTime * turningSpeed), 0f);//to turn the wheels
        }
    }
}
