using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LightningManager))]
public class CarControl : MonoBehaviour
{
    public InputManager inputManager;
    public LightningManager lightMan;

    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;

    private float wheelBaseLength = 2.4f;
    private float rearTrackSize = 1.35f;
    private float turnRadius = 0f;
    private float maxWheelTurn = 20f;

    public List<WheelCollider> wheels;

    public float[] GearRatio = new float[] {3.484f, 3.587f, 2.022f, 1.384f, 1f, 0.861f}; //[R, 1, 2, 3, 4, 5] 3.484f pt R T50 5 speed engine
    public float finalDriveRatio = 4.3f;// this is multiplied with each gear ratio
    public int currentGear = 1;
    public float engineRPM = 0.0f;

    public GameManager gameManager;
    public SoundController soundController;

    //engine charateristics
    public float MaxEngineRPM = 9000.0f;
    private float MinEngineRPM = 950.0f;
    private float shiftUpRPM = 6000.0f;
    private float downShiftRPM = 4500.0f;
    private int index = 0;

    //Engine max torque NM and RPM
    public float maxTorque = 149;//149NM at 5200RPM
    public float torqueRPM = 5200;

    //Engine max power in Watts and RPM
    public float maxPower = 96000;//96kw at 6600RPM
    public float powerRPM = 6600;

    float[] engineEfficiency = { 0f, 0.2f, 0.22f, 0.24f, 0.26f, 0.3f, 0.4f, 0.42f, 0.44f, 0.46f, 0.48f, 0.5f, 0.6f, 0.62f, 0.64f, 0.66f, 0.68f, 0.75f, 0.8f, 0.83f, 0.86f, 0.89f, 0.92f, 0.95f, 0.98f, 1.0f, 1.0f, 1.0f, 0.98f, 0.96f, 0.92f, 0.88f, 0.84f, 0.8f, 0.76f, 0.72f, 0.68f};
    float engineEfficiencyStep = 250.0f;

    public bool autoTransmission = false;
    public bool tractionControlEngage = true;

    public static float carSpeed = 0.0f;
    private float reverseGearMaxSpeed = 25f;
    private float firstGearMaxSpeed = 85f;
    private float secondGearMaxSpeed = 155f;
    private float thirdGearMaxSpeed = 195f;
    private float fourthGearMaxSpeed = 235f;
    private float fifthGearMaxSpeed = 270f;

    public List<GameObject> steeringWheels;
    public List<GameObject> wheelMeshes;//contain all the wheel objects
    public List<GameObject> brakeLight;
    public List<GameObject> reverseLight;
    public List<GameObject> signalLights;
    
    private float brakeIntensity = 13000f;

    private float downForce = 50.0f;

    public Rigidbody rigidB;

    float gearShiftDelay = 0.0f;
    private float torque = 65000f;
    private float newTorque = 0.0f;

    //car shop entries
    public int carPrice;
    public string carName;
    public int horsePower;
    public int topSpeed;
    public float acceleration0100;

    public void ShiftUp()
    {
        float now = Time.timeSinceLevelLoad;

        //check whether we have waited enough before shifting gear
        if (now < gearShiftDelay) return;

        if ((currentGear > 0 && currentGear < GearRatio.Length - 1 && engineRPM > shiftUpRPM) || (currentGear == 0 && engineRPM < 1050f))
        {
            currentGear++;
            gameManager.gearChange();
            soundController.PlayGearShiftUp();
            soundController.PlayTurboWhistle();

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
            soundController.PlayGearShiftDown();

            //delay next shift with 0.1s
            gearShiftDelay = now + 0.1f;
        }
    }

    void Start()
    {
        rigidB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        inputManager = GetComponent<InputManager>();
        soundController = GetComponent<SoundController>();
        rigidB.centerOfMass = new Vector3(0.0f, 0.2f, 0.0f);
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

        if(inputManager.tractionControlToggle && tractionControlEngage)
        {
            tractionControlEngage = false;
        }
        else if(inputManager.tractionControlToggle && !tractionControlEngage)
        {
            tractionControlEngage = true;
        }

        tractionControl();
    }

    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "CarSelectorScene") return;

        wheels.Add(FrontLeftWheel);
        wheels.Add(FrontRightWheel);
        wheels.Add(BackLeftWheel);
        wheels.Add(BackRightWheel);

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

        index = (int)(engineRPM / engineEfficiencyStep);
        if (index >= engineEfficiency.Length)
        {
            index = engineEfficiency.Length - 1;
        }
        if (index < 0)
        {
            index = 0;
        }

        torqueCalculation();

        brakeAndAccelerate();
        Steering();
        
        foreach (GameObject wheel in wheelMeshes)
        {
            wheel.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).x >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.3f), 0f, 0f); 
        }
        
        rigidB.AddForce(-transform.up * downForce * rigidB.velocity.magnitude);
    }

    private void Steering()
    {
        if(carSpeed <= 40f)
        {
            turnRadius = 5f;
        } else if(carSpeed > 40f && carSpeed <= 100f)
        {
            turnRadius = 12;
        } else if(carSpeed > 100f && carSpeed <= 150)
        {
            turnRadius = 15f;
        }
        else
        {
            turnRadius = 18f;
        }

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
            BackLeftWheel.brakeTorque = brakeIntensity;
            absApply(BackLeftWheel);

            BackRightWheel.motorTorque = 0.0f;
            BackRightWheel.brakeTorque = brakeIntensity;
            absApply(BackRightWheel);

            FrontLeftWheel.brakeTorque = brakeIntensity;
            absApply(FrontLeftWheel);

            FrontRightWheel.brakeTorque = brakeIntensity;
            absApply(FrontRightWheel);
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

    //use anti-lock brake system
    private void absApply(WheelCollider wheel)
    {
        if (wheel.brakeTorque >= 0.1f)
        {
            wheel.brakeTorque -= 0.1f * brakeIntensity;
        }
        else
        {
            wheel.brakeTorque = 0.0f;
        }
    }

    private WheelFrictionCurve sidewaysFriction;
    private WheelFrictionCurve forwardFriction;
    
    private void tractionControl()
    {
        float tractionFactor = 0.7f * Time.deltaTime;//time taken to switch off the traction control

        if (inputManager.tractionControlToggle && !tractionControlEngage)
        {
            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            //front wheel setup
            for(int i = 0; i < 2; i++)
            {
                forwardFriction.extremumSlip = forwardFriction.extremumValue = forwardFriction.asymptoteSlip = forwardFriction.asymptoteValue = 1.2f;
                sidewaysFriction.extremumSlip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteSlip = sidewaysFriction.asymptoteValue = 1.4f;
                sidewaysFriction.stiffness = 3.0f;

                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            //back wheels setup
            for (int i = 2; i < 4; i++)
            {
                forwardFriction.extremumSlip = forwardFriction.extremumValue = forwardFriction.asymptoteSlip = forwardFriction.asymptoteValue = 0.9f;
                sidewaysFriction.extremumSlip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteSlip = sidewaysFriction.asymptoteValue = 1.0f;
                sidewaysFriction.stiffness = 2.9f;

                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }
        }
        else if (inputManager.tractionControlToggle && tractionControlEngage)
        {
            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;
            
            foreach (WheelCollider wheel in wheels)
            {
                forwardFriction.extremumSlip = forwardFriction.extremumValue = forwardFriction.asymptoteSlip = forwardFriction.asymptoteValue = 1.0f;
                forwardFriction.stiffness = 2.5f;
                sidewaysFriction.extremumSlip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteSlip = sidewaysFriction.asymptoteValue = 2.0f;
                sidewaysFriction.stiffness = 8.0f;

                wheel.sidewaysFriction = sidewaysFriction;
                wheel.forwardFriction = forwardFriction;
            }
        }
    }
}