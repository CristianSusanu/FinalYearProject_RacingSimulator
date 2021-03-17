using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
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
                //transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(lelngth2, driverViewHeight, distance2));
                transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(distance2, driverViewHeight, length2)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 55f;
                break;
            //bonet view
            case 2:
                //transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0, bonetViewHeight, bonetHeight));
                transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, bonetViewHeight, bonetHeight)), dampening * Time.deltaTime);
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 65f;
                break;
            //behind the car view
            default:
                //distance is negative because we are behind the car on the z axis
                //transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(0f, cameraHeight, -cameraDistance));
                transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, cameraHeight, -cameraDistance)), dampening * Time.deltaTime);
                transform.LookAt(car.transform);
                break;
        }
    }
}
