using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform car;
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
    public float length2 = -0.33f;

    //bonet view
    public float bonetViewHeight = 1.3f;
    public float bonetHeight = 0.4f;

    private int cameraMode = 0;

    public GameManager gameMan;

    void Start()
    {
        lookAtVector = new Vector3(0, lookAtHeight, 0);
    }

    void LateUpdate()
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
                Camera.main.fieldOfView = 70f;

                displaySupplementaryTacho(false);

                break;
            //bonet view
            case 2:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0, bonetViewHeight, bonetHeight));
                //transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, bonetViewHeight, bonetHeight)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 65f;

                displaySupplementaryTacho(true);

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

                displaySupplementaryTacho(true);

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
}
