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

    //public List<WheelCollider> throttleWheels;

    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider BackLeftWheel;
    public WheelCollider BackRightWheel;

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

    float[] engineEfficiency = {0.5f, 0.52f, 0.54f, 0.56f, 0.58f, 0.6f, 0.62f, 0.64f, 0.66f, 0.68f, 0.7f, 0.72f, 0.74f, 0.76f, 0.78f, 0.8f, 0.82f, 0.84f, 0.86f, 0.88f, 0.9f, 0.92f, 0.94f, 0.96f, 0.98f, 1.0f, 1.0f, 1.0f, 0.96f, 0.92f, 0.88f, 0.84f, 0.8f, 0.76f, 0.72f, 0.68f};
    float engineEfficiencyStep = 250.0f;

    public Vector3 engineOrientation = Vector3.right;//to control the car body movement with the increase in RPM

    public bool autoTransmission = false;
    public bool tractionControlEngage = true;

    public static float carSpeed = 0.0f;
    //private float carMaxSpeed = 240f;
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

    //public float strengthCoeffiecient = 30000f;
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

    // Start is called before the first frame update
    void Start()
    {
        rigidB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        //inputManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();

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

        //calculate torque
        torqueCalculation();

        brakeAndAccelerate();
        Steering();

        //Debug.Log("Traction Control: " + tractionControlEngage);

        /*  //front wheel steering
          FrontLeftWheel.GetComponent<WheelCollider>().steerAngle = maxWheelTurn * inputManager.steering;
          FrontLeftWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);

          FrontRightWheel.GetComponent<WheelCollider>().steerAngle = maxWheelTurn * inputManager.steering;
          FrontRightWheel.transform.localEulerAngles = new Vector3(0f, inputManager.steering * maxWheelTurn, 0f);*/
        
        foreach (GameObject wheel in wheelMeshes)
        {
            wheel.transform.Rotate(rigidB.velocity.magnitude * (transform.InverseTransformDirection(rigidB.velocity).x >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.3f), 0f, 0f); //ternary operator: 1 > 0? "?":"!"
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
    private float turnRadius = 0f;
    private float maxWheelTurn = 20f;

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

    //use anti brake lock system to prevent the wheels from locking
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
    //private float tractionControlMultiplier = 2.0f;
    //private float tractionSlip;//driftfactor de la el
    
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
            /*
            float tractionFactor = 0.7f * Time.deltaTime;//time taken to switch off the traction control

            if (!tractionControlEngage)
            {
                sidewaysFriction = wheels[0].sidewaysFriction;
                forwardFriction = wheels[0].forwardFriction;

                float velocity = 0f;
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue, tractionSlip * tractionControlMultiplier, ref velocity, tractionFactor);

                for(int i = 0; i < 4; i++)
                {
                    wheels[i].sidewaysFriction = sidewaysFriction;
                    wheels[i].forwardFriction = forwardFriction;
                }

                forwardFriction.stiffness = 1.8f;
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 0.8f;
                sidewaysFriction.stiffness = 4f;

                //extra grip for front tires
                for (int i = 0; i < 2; i++)
                {
                    wheels[i].sidewaysFriction = sidewaysFriction;
                    wheels[i].forwardFriction = forwardFriction;
                }
                rigidB.AddForce(transform.forward * (rigidB.velocity.magnitude * 3.6f / 400) * 10000);
            }
            //in case handbrake is being held
            else if(tractionControlEngage)
            {
                sidewaysFriction = wheels[0].sidewaysFriction;
                forwardFriction = wheels[0].forwardFriction;
                forwardFriction.extremumSlip = forwardFriction.extremumValue = forwardFriction.asymptoteSlip = forwardFriction.asymptoteValue = 2.0f;

                //forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                //    ((rigidB.velocity.magnitude * 3.6f * tractionControlMultiplier) / 300) +1;

                for (int i = 0; i < 4; i++)
                {
                    wheels[i].sidewaysFriction = sidewaysFriction;
                    wheels[i].forwardFriction = forwardFriction;
                }
            }

            //to check the amount of slip for controlling the drift
            for(int i = 2; i < 4; i++)
            {
                WheelHit hit;

                wheels[i].GetGroundHit(out hit);

                if(hit.sidewaysSlip < 0)
                {
                    tractionSlip = 1 - inputManager.steering * Mathf.Abs(hit.sidewaysSlip);
                }
                if (hit.sidewaysSlip > 0)
                {
                    tractionSlip = 1 + inputManager.steering * Mathf.Abs(hit.sidewaysSlip);
                }
            }


            /*
            if (inputManager.tractionControlToggle)
            {
                sidewaysFriction = FrontLeftWheel.sidewaysFriction;
                forwardFriction = FrontLeftWheel.forwardFriction;

                float velocity = 0.0f;

                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue, tractionSlip * tractionControlMultiplier, ref velocity, tractionFactor);
                FrontLeftWheel.sidewaysFriction = sidewaysFriction;
                FrontLeftWheel.forwardFriction = forwardFriction;
                FrontRightWheel.sidewaysFriction = sidewaysFriction;
                FrontRightWheel.forwardFriction = forwardFriction;

                BackLeftWheel.sidewaysFriction = sidewaysFriction;
                BackLeftWheel.forwardFriction = forwardFriction;
                BackRightWheel.sidewaysFriction = sidewaysFriction;
                BackRightWheel.forwardFriction = forwardFriction;

                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;

                //adding more grip to front wheels
                FrontLeftWheel.sidewaysFriction = sidewaysFriction;
                FrontLeftWheel.forwardFriction = forwardFriction;
                FrontRightWheel.sidewaysFriction = sidewaysFriction;
                FrontRightWheel.forwardFriction = forwardFriction;
                rigidB.AddForce(transform.forward * (rigidB.velocity.magnitude * 3.6f / 400) * 10000);
            }

            //check the slip amount
            WheelHit hitL;
            WheelHit hitR;
            BackLeftWheel.GetGroundHit(out hitL);
            BackRightWheel.GetGroundHit(out hitR);

            if (hitL.sidewaysSlip < 0)
            {
                tractionSlip = (1 + (-inputManager.steering) * Mathf.Abs(hitL.sidewaysSlip));
            }
            if(hitL.sidewaysSlip > 0)
            {
                tractionSlip = (1 + (inputManager.steering) * Mathf.Abs(hitL.sidewaysSlip));
            }

            if (hitR.sidewaysSlip < 0)
            {
                tractionSlip = (1 + (-inputManager.steering) * Mathf.Abs(hitR.sidewaysSlip));
            }
            if (hitR.sidewaysSlip > 0)
            {
                tractionSlip = (1 + (inputManager.steering) * Mathf.Abs(hitR.sidewaysSlip));
            }*/
        }
}