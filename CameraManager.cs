using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject car;

    //third person view
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float dampening = 1f;

    //first person view
    public float driverViewHeight = 1.01f;
    public float distance2 = -0.28f;
    public float lelngth2 = -0.33f;

    private int cameraMode = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = (cameraMode + 1) % 2; //where 2 is the number of cameras in use
        }

        switch (cameraMode)
        {
            case 1:
                transform.position = car.transform.position + car.transform.TransformDirection(new Vector3(lelngth2, driverViewHeight, distance2));
                transform.rotation = car.transform.rotation;
                Camera.main.fieldOfView = 55f;
                break;
            default:
                //distance is negative because we are behind the car on the z axis
                transform.position = Vector3.Lerp(transform.position, car.transform.position + car.transform.TransformDirection(new Vector3(0f, cameraHeight, -cameraDistance)), dampening * Time.deltaTime);
                transform.LookAt(car.transform);
                //Camera.main.fieldOfView = 80f;
                break;
        }
    }
}
