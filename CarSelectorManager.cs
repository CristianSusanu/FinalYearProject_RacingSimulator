using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectorManager : MonoBehaviour
{
    public CarSelectorList carList;
    //public GameObject car;
    public GameObject carStand;
    public float standRotationSpeed = 0f;
    public int vehiclePointer = 0;

    private void Awake()
    {
        vehiclePointer = PlayerPrefs.GetInt("pointer");//get the pointer

        //instantiate the vehicle stored in the pointer
        GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        individualCar.transform.parent = carStand.transform;
    }

    private void FixedUpdate()
    {
        /*
        car = GameObject.FindGameObjectWithTag("Car");
        car.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
        carStand.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
        */

        carStand.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
    }

    public void NextButton()
    {
        if (vehiclePointer < carList.cars.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Car"));
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            individualCar.transform.parent = carStand.transform;
        }
    }

    public void PreviousButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Car"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            individualCar.transform.parent = carStand.transform;
        }
    }
}
