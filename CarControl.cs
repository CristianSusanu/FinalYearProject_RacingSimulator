using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LightningManager))]
public class CarControl : MonoBehaviour
{
    public InputManager inputManager;
    public LightningManager lightMan;

    //public List<WheelCollider> throttleWheels;

    //aici pun ce am schimbat
    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;
    
    public float[] GearRatio = new float[] {3.587f, 2.022f, 1.384f, 1f, 0.861f}; //[R, 1, 2, 3, 4, 5] 3.484f pt R
    //private int CurrentGear = 0;
    
    //public float EngineTorque = 999999f;
    public float MaxEngineRPM = 8000f;
    public float MinEngineRPM = 1100f;
    private float EngineRPM = 0f;
    //pana aici

    public List<GameObject> steeringWheels;
    public List<GameObject> meshes;//contain all the wheel objects
    public List<GameObject> brakeLight;
    public List<GameObject> reverseLight;
    public List<GameObject> signalLights;

    public float strengthCoeffiecient = 30000f;
    public float maxTurn = 20f;
    public float brakeIntensity;

    public Rigidbody rigidB;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
       //rigidB.centerOfMass = new Vector3(0f, 0f, -1.0f);
    }

    void Update()
    {
        if (inputManager.li)
        {
            lightMan.OnOffHeadLights();
        }
        
        foreach (GameObject t in brakeLight)
        {
            t.GetComponent<Renderer>().material.SetColor("_EmissionColor", inputManager.brake ? Color.red : Color.black);
        }

        foreach (GameObject t in reverseLight)
        {
            t.GetComponent<Renderer>().material.SetColor("_EmissionColor", inputManager.reverse ? Color.white : Color.black);
        }

        foreach (GameObject sLight in signalLights)
        {
            sLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }

        //de aici am schimbat
        //rigidB.drag = rigidB.velocity.magnitude / 250;//This is to limith the maximum speed of the car, adjusting the drag probably isn't the best way of doing it, but it's easy, and it doesn't interfere with the physics processing.  
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        
        // Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
        //EngineRPM = (BackLeftWheel.rpm + BackRightWheel.rpm) / 2 * GearRatio[CurrentGear];

        //braking part
        if (inputManager.brake)
        {
            BackLeftWheel.motorTorque = 0f;
            BackLeftWheel.brakeTorque = brakeIntensity * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear]; //multiply by time to account for faster computer frames

            BackRightWheel.motorTorque = 0f;
            BackRightWheel.brakeTorque = brakeIntensity * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear]; //multiply by time to account for faster computer frames

            FrontLeftWheel.motorTorque = 0f;
            FrontLeftWheel.brakeTorque = brakeIntensity * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear]; //multiply by time to account for faster computer frames

            FrontRightWheel.motorTorque = 0f;
            FrontRightWheel.brakeTorque = brakeIntensity * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear]; //multiply by time to account for faster computer frames

            //ShiftGear();
        }
        else
        {
            BackLeftWheel.motorTorque = strengthCoeffiecient * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear];
            BackLeftWheel.brakeTorque = 0f;

            BackRightWheel.motorTorque = strengthCoeffiecient * Time.deltaTime * inputManager.throttle;// / GearRatio[CurrentGear];
            BackRightWheel.brakeTorque = 0f;
            

            //ShiftGear();
        }

        //front wheel turn
        FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = maxTurn * inputManager.steering;
        FrontLeftWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxTurn, 0f);

        FrontRightWheel.GetComponent<WheelCollider>().steerAngle = maxTurn * inputManager.steering;
        FrontRightWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxTurn, 0f);

        foreach (GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.3f), 0f, 0f); //ternary operator: 1 > 0? "?":"!"
        }

        /*

        foreach (WheelCollider wheel in throttleWheels)
        {
            if (inputManager.brake)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = brakeIntensity * Time.deltaTime; //multiply by time to account for faster computer frames
            }
            else
            {
                wheel.motorTorque = strengthCoeffiecient * Time.deltaTime * inputManager.throttle;
                wheel.brakeTorque = 0f;
            }
        }
        
        foreach (GameObject wheel in steeringWheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * inputManager.steering;
            wheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxTurn, 0f);
        }

        foreach (GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).z >= 0 ? -1 : 1) / (2 * Mathf.PI * 0.3f), 0f, 0f); //ternary operator: 1 > 0? "?":"!"
        }*/

    }

    /*
    public void ShiftGear() // this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
                            // the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
    {
        int gear = CurrentGear;
        if (EngineRPM >= MaxEngineRPM)
        {
            for (int i = 0; i < GearRatio.Length; i++)
            {
                if (BackLeftWheel.rpm * GearRatio[i] < MaxEngineRPM)
                {
                    gear = i;
                    break;
                }
            }

            CurrentGear = gear;
        }

        if (EngineRPM <= MinEngineRPM)
        {
            gear = CurrentGear;

            for (int j = GearRatio.Length - 1; j >= 0; j--)
            {
                if (BackLeftWheel.rpm * GearRatio[j] > MinEngineRPM)
                {
                    gear = j;
                    break;
                }
            }
            CurrentGear = gear;
        }

    }*/
}
