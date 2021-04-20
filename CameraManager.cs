using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform car;
    public GameObject rearViewCamera;
    public GameObject rearViewCameraImage;
    public GameObject rearViewCentralCamera;
    public GameObject rearViewCentralImage;
    public GameObject rearViewCameraOutside;
    public GameObject rearViewCameraOutsideImage;
    public GameObject rearViewCameraOutsideBorder;

    public float cameraDistance = 3.5f;
    public float cameraHeight = 1.8f;
    public float cameraDamping = 2.0f;

    public float lookAtHeight = 0.0f;

    public Rigidbody parentRigidbody;

    public float rotationSnapTime = 0.3F;

    public float distanceSnapTime = 0.2f;
    public float distanceMultiplier = 0.2f;

    private Vector3 lookAtVector;

    private float usedDistance;

    float wantedRotationAngle;
    float wantedHeight;

    float currentRotationAngle;
    float currentHeight;

    Quaternion currentRotation;
    Vector3 wantedPosition;

    private float yVelocity = 0.0F;
    private float zVelocity = 0.0F;

    //first person view
    public float driverViewHeight = 0.985f;
    public float distance2 = -0.06f;
    public float length2 = -0.3f;

    //bonet view
    public float bonetViewHeight = 1.3f;
    public float bonetHeight = 0.4f;

    private int cameraMode = 0;

    public GameManager gameMan;
    public AudioSource levelAudio;

    void Start()
    {
        lookAtVector = new Vector3(0, lookAtHeight, 0);

        rearViewCameraOutside.SetActive(false);
        rearViewCameraOutsideImage.SetActive(false);
        rearViewCameraOutsideBorder.SetActive(false);

        car = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        parentRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        //diplay central mirror only when V is pressed
        if (Input.GetKeyDown(KeyCode.V))
        {
            DisplayCentralOutsideMirror(true);
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            DisplayCentralOutsideMirror(false);
        }

        //display the central mirror when car is reversing
        if (Input.GetKeyDown(KeyCode.DownArrow) && parentRigidbody.velocity.magnitude * 3.6f < 25f)
        {
            DisplayCentralOutsideMirror(true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            DisplayCentralOutsideMirror(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = (cameraMode + 1) % 3; //where 3 is the number of cameras in use
        }

        switch (cameraMode)
        {
            //fps view
            case 1:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(length2, driverViewHeight, distance2));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(distance2, driverViewHeight, length2)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 75f;

                displaySupplementaryTacho(false);
                gameMan.tractionCtrlIcon.SetActive(false);
                displaySupplementaryInteriorInfo(true);
                DisplayMirrorImage(true);
                DisplayCentralOutsideMirror(false);
                levelAudio.volume = 0.65f;

                break;
            //bonet view
            case 2:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0, bonetViewHeight, bonetHeight));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, bonetViewHeight, bonetHeight)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 65f;

                displaySupplementaryTacho(true);
                displaySupplementaryInteriorInfo(false);
                gameMan.tractionCtrlInteriorIcon.SetActive(false);
                DisplayMirrorImage(false);
                levelAudio.volume = 0.55f;

                break;
            //behind the car view
            default:
                wantedHeight = car.position.y + cameraHeight;
                currentHeight = transform.position.y;

                wantedRotationAngle = car.eulerAngles.y;
                currentRotationAngle = transform.eulerAngles.y;

                currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);

                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, cameraDamping * Time.deltaTime);

                wantedPosition = car.position;
                wantedPosition.y = currentHeight;

                usedDistance = Mathf.SmoothDampAngle(usedDistance, cameraDistance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);

                wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);

                transform.position = wantedPosition;

                transform.LookAt(car.position + lookAtVector);

                Camera.main.fieldOfView = 60f;

                displaySupplementaryTacho(true);
                displaySupplementaryInteriorInfo(false);
                gameMan.tractionCtrlInteriorIcon.SetActive(false);
                DisplayMirrorImage(false);
                levelAudio.volume = 0.25f;

                break;
        }
    }

    //helper function to hide tacho when using in-car view
    void displaySupplementaryTacho(bool temp)
    {
        gameMan.gearIndicatorText.enabled = temp;
        gameMan.transmissionIndicatorText.enabled = temp;
        gameMan.RPMIndicator.enabled = temp;
        gameMan.speedText.enabled = temp;
        gameMan.rpmNeedle.SetActive(temp);
        gameMan.tacho.SetActive(temp);
    }

    void displaySupplementaryInteriorInfo(bool temp)
    {
        gameMan.gearIndicatorInteriorText.enabled = temp;
        gameMan.transmissionIndicatorInteriorText.enabled = temp;
        gameMan.RPMIndicatorInterior.enabled = temp;
        gameMan.speedTextInterior.enabled = temp;
        gameMan.interiorGauges.SetActive(temp);
    }

    void DisplayMirrorImage(bool temp)
    {
        //left mirror
        rearViewCamera.SetActive(temp);
        rearViewCameraImage.SetActive(temp);

        //interior central mirror
        rearViewCentralCamera.SetActive(temp);
        rearViewCentralImage.SetActive(temp);
    }

    //diplay function for central outside mirror
    void DisplayCentralOutsideMirror(bool temp)
    {
        rearViewCameraOutside.SetActive(temp);
        rearViewCameraOutsideImage.SetActive(temp);
        rearViewCameraOutsideBorder.SetActive(temp);
    }
}
    /*
    public GameObject car;

    //third person view
    public float cameraDistance = 4f;
    public float cameraHeight = 1.7f;
    public float dampening = 5f;

    //first person view
    public float driverViewHeight = 1.01f;
    public float distance2 = -0.28f;
    public float length2 = -0.33f;

    //bonet view
    public float bonetViewHeight = 1.3f;
    public float bonetHeight = 0.4f;

    private int cameraMode = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = (cameraMode + 1) % 3; //where 2 is the number of cameras in use
        }

        switch (cameraMode)
        {
            //fps view
            case 1:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(length2, driverViewHeight, distance2));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(distance2, driverViewHeight, length2)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 55f;
                break;
            //bonet view
            case 2:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0, bonetViewHeight, bonetHeight));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, bonetViewHeight, bonetHeight)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 65f;
                break;
            //behind the car view
            default:
                //distance is negative because we are behind the car on the z axis
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0f, cameraHeight, -cameraDistance));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, cameraHeight, -cameraDistance)), dampening * Time.deltaTime);
                transform.LookAt(car.transform);
                break;
        }
    }*/

