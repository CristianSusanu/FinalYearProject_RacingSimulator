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

    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;

    public float[] GearRatio = new float[] {3.484f, 3.587f, 2.022f, 1.384f, 1f, 0.861f}; //[R, 1, 2, 3, 4, 5] 3.484f pt R T50 5 speed engine
    public float finalDriveRatio = 4.3f;// this is multiplied with each gear ratio
    public int currentGear = 1;
    public float engineRPM = 0.0f;

    public GameManager gameManager;

    //engine charateristics
    public float MaxEngineRPM = 9000.0f;
    private float MinEngineRPM = 950.0f;
    private float shiftUpRPM = 5500.0f;
    private float downShiftRPM = 3000.0f;
    private int index = 0;

    //Engine max torque NM and RPM
    public float maxTorque = 149;//149NM at 5200RPM
    public float torqueRPM = 5200;

    //Engine max power in Watts and RPM
    public float maxPower = 96000;//96kw at 6600RPM
    public float powerRPM = 6600;

    float[] engineEfficiency = {0.5f, 0.52f, 0.54f, 0.56f, 0.58f, 0.6f, 0.62f, 0.64f, 0.66f, 0.68f, 0.7f, 0.72f, 0.74f, 0.76f, 0.78f, 0.8f, 0.82f, 0.84f, 0.86f, 0.88f, 0.9f, 0.92f, 0.94f, 0.96f, 0.98f, 1.0f, 1.0f, 1.0f, 0.96f, 0.92f, 0.88f, 0.84f, 0.8f, 0.76f, 0.72f, 0.68f};
    float engineEfficiencyStep = 250.0f;

    public bool autoTransmission = false;

    public float carSpeed = 0.0f;
    private float carMaxSpeed = 240f;
    private float reverseGearMaxSpeed = 20f;
    private float firstGearMaxSpeed = 85f;
    private float secondGearMaxSpeed = 135f;
    private float thirdGearMaxSpeed = 155f;
    private float fourthGearMaxSpeed = 170f;
    private float fifthGearMaxSpeed = 240f;

    public List<GameObject> steeringWheels;
    public List<GameObject> meshes;//contain all the wheel objects
    public List<GameObject> brakeLight;
    public List<GameObject> reverseLight;
    public List<GameObject> signalLights;

    //public float strengthCoeffiecient = 30000f;
    private float brakeIntensity = 10000f;

    private float downForce = 50.0f;

    public Rigidbody rigidB;

    float gearShiftDelay = 0.0f;
    private float torque = 15000f;
    private float newTorque = 0.0f;

    public void ShiftUp()
    {
        float now = Time.timeSinceLevelLoad;

        //check whether we have waited enough before shifting gear
        if (now < gearShiftDelay) return;

        if ((currentGear > 0 && currentGear < GearRatio.Length - 1 && engineRPM > shiftUpRPM) || (currentGear == 0 && engineRPM < 1050f))
        {
            currentGear++;
            gameManager.gearChange();

            //delay next shift with 1s
            gearShiftDelay = now + 1.0f;
        }
    }

    public void ShiftDown()
    {
        float now = Time.timeSinceLevelLoad;

        //check whether we have waited enough before shifting gear
        if (now < gearShiftDelay) return;

        if ((currentGear > 1) || (currentGear == 1 && engineRPM < 1050f))
        {
            currentGear--;
            gameManager.gearChange();

            //delay next shift with 0.1s
            gearShiftDelay = now + 0.1f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        rigidB.centerOfMass = new Vector3(0.0f, 0.25f, 0.0f);
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
        if (inputManager.transmission && autoTransmission)
        {
            autoTransmission = false;
        }
        else if (inputManager.transmission && !autoTransmission)
        {
            autoTransmission = true;
        }

        //for resetting the car in case it rolled over
        if (inputManager.carReset)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }
    }

    void FixedUpdate()
    {
        engineRPM = MinEngineRPM + 2 * (30f * rigidB.velocity.magnitude * GearRatio[currentGear] * finalDriveRatio) / (3.6f * Mathf.PI * 0.3f);
        carSpeed = rigidB.velocity.magnitude * 3.6f;

        if (autoTransmission)
        {
            //automatic shifting 
            if (autoTransmission && currentGear == 1 && inputManager.throttle < 0.0f)
            {
                ShiftDown();//reverse
            }
            else if (autoTransmission && currentGear == 0 && inputManager.throttle > 0.0f)
            {
                ShiftUp();//to change from reverse to drive
            }
            else if (autoTransmission && inputManager.throttle > 0.0f && engineRPM > shiftUpRPM)
            {
                ShiftUp();//shift up
            }
            else if (autoTransmission && engineRPM < downShiftRPM && currentGear > 1)
            {
                ShiftDown();//down shift
            }
        }
        else
        {
            //manual shifting
            if (inputManager.shiftUp)
            {
                ShiftUp();
            }

            if (inputManager.shiftDown)
            {
                ShiftDown();
            }
        }
       
        index = (int) (engineRPM / engineEfficiencyStep);
        if (index >= engineEfficiency.Length)
        {
            index = engineEfficiency.Length - 1;
        }
        if (index < 0)
        {
            index = 0;
        }

        //calculate torque
        torqueCalculation();

        //brakeAndAccelerate();
        brakeAndAccelerate();
        Steering();

        /*
        //braking part
        if (inputManager.brake)
        {
            BackLeftWheel.motorTorque = 0.0f;
            BackLeftWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames

            BackRightWheel.motorTorque = 0.0f;
            BackRightWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames

            FrontLeftWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames
            FrontRightWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames
        }
        else
        {
            BackLeftWheel.motorTorque = newTorque * Time.deltaTime * inputManager.throttle;
            BackLeftWheel.brakeTorque = 0.0f;

            BackRightWheel.motorTorque = newTorque * Time.deltaTime * inputManager.throttle;
            BackRightWheel.brakeTorque = 0.0f;

            FrontLeftWheel.brakeTorque = 0.0f;
            FrontRightWheel.brakeTorque = 0.0f;
        }*/

        Debug.Log("Current Gear:" + currentGear);
        Debug.Log("index: " + index);

        /*  //front wheel steering
          FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = maxWheelTurn * inputManager.steering;
          FrontLeftWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);

          FrontRightWheel.GetComponent<WheelCollider>().steerAngle = maxWheelTurn * inputManager.steering;
          FrontRightWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);*/

        foreach (GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).z >= 0 ? -1 : 1) / (2 * Mathf.PI * 0.3f), 0f, 0f); //ternary operator: 1 > 0? "?":"!"
        }

        //rigidB.AddForceAtPosition(-transform.up * rigidB.velocity.sqrMagnitude, transform.position);
        rigidB.AddForce(-transform.up * downForce * rigidB.velocity.magnitude);
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

    private float wheelBaseLength = 2.4f;
    private float rearTrackSize = 1.35f;
    private float turnRadius = 10f;
    private float maxWheelTurn = 30f;

    private void Steering()
    {
        if(inputManager.steering > 0)
        {
            FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + (rearTrackSize / 2))) * inputManager.steering ;
            FrontRightWheel.GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius - (rearTrackSize / 2))) * inputManager.steering;
        } else if(inputManager.steering < 0)
        {
            FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius - (rearTrackSize / 2))) * inputManager.steering;
            FrontRightWheel.GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + (rearTrackSize / 2))) * inputManager.steering;
        } else
        {
            FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = 0;
            FrontRightWheel.GetComponent<WheelCollider>().steerAngle = 0;
        }

        //Wheel movement left to right
        FrontLeftWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);
        FrontRightWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);
    }

    private void brakeAndAccelerate()
    {
        //braking part
        if (inputManager.brake)
        {
            BackLeftWheel.motorTorque = 0.0f;
            BackLeftWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames

            BackRightWheel.motorTorque = 0.0f;
            BackRightWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames

            FrontLeftWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames
            FrontRightWheel.brakeTorque = brakeIntensity;//multiply by time to account for faster computer frames
        }
        else
        {
            BackLeftWheel.motorTorque = newTorque * Time.deltaTime * inputManager.throttle;
            BackLeftWheel.brakeTorque = 0.0f;

            BackRightWheel.motorTorque = newTorque * Time.deltaTime * inputManager.throttle;
            BackRightWheel.brakeTorque = 0.0f;

            FrontLeftWheel.brakeTorque = 0.0f;
            FrontRightWheel.brakeTorque = 0.0f;
        }
    }

    private void torqueCalculation()
    {
        //calculate new torque value
        if (engineRPM < MaxEngineRPM && (currentGear == 0 && carSpeed < reverseGearMaxSpeed))
        {
            newTorque = torque * GearRatio[currentGear] * finalDriveRatio * engineEfficiency[index];
        }
        else if (engineRPM < MaxEngineRPM && ((currentGear == 1 && carSpeed < firstGearMaxSpeed) || (currentGear == 2 && carSpeed < secondGearMaxSpeed) ||
           (currentGear == 3 && carSpeed < thirdGearMaxSpeed) || (currentGear == 4 && carSpeed < fourthGearMaxSpeed) || (currentGear == 5 && carSpeed < fifthGearMaxSpeed)))
        {
            newTorque = torque * GearRatio[currentGear] * finalDriveRatio * engineEfficiency[index];
        }
        else
        {
            newTorque = 0.0f;
        }
    }
}