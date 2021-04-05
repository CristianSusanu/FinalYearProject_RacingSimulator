using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform path;
    private List<Transform> nodes;
    private int nodeNumber = 0;

    private float steeringAngle = 30f;
    private float motorTorque = 25000f;
    private float carSpeed = 0f;

    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;

    public Rigidbody rigidB;

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

        //rigidB.centerOfMass = new Vector3(0.0f, 0.2f, 0.0f);
    }

    void FixedUpdate()
    {
        ApplySteer();
        Move();
        GetWayPointDistance();
    }

    private void ApplySteer()
    {
        Vector3 relVector = transform.InverseTransformPoint(nodes[nodeNumber].position);
        //relVector = relVector / relVector.magnitude; //relVector.magnitude gives the length
        float wheelAngle = steeringAngle* relVector.x / relVector.magnitude;//relVector.magnitude gives the length
        FrontLeftWheel.steerAngle = wheelAngle;
        FrontRightWheel.steerAngle = wheelAngle;
    }

    private void Move()
    {
        float maxSpeed = 240f;

        carSpeed = 3.6f * rigidB.velocity.magnitude;
        
        if(carSpeed < maxSpeed)
        {
            BackLeftWheel.motorTorque = motorTorque;
            BackRightWheel.motorTorque = motorTorque;
        }
        else
        {
            BackLeftWheel.motorTorque = 0f;
            BackRightWheel.motorTorque = 0f; ;
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
}
