using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelectorManager : MonoBehaviour
{
    //public CarSelectorList carList;
    public GameObject car;
    public GameObject carStand;
    private float standRotationSpeed = 15f;
    public int vehiclePointer = 0;
    /*
    private void Awake()
    {
        //PlayerPrefs.SetInt("pointer", 0);
        //PlayerPrefs.SetInt("pointer", vehiclePointer);
        vehiclePointer = PlayerPrefs.GetInt("pointer");//get the pointer

        //instantiate the vehicle stored in the pointer
        GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        individualCar.transform.parent = carStand.transform;
    }*/

    private void FixedUpdate()
    {
        
        car = GameObject.FindGameObjectWithTag("Car");
        car.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
        carStand.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
        

        //carStand.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
    }
    /*
    public void NextButton()
    {
        if (vehiclePointer < carList.cars.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));//destroy the current car from the scene
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, carStand.transform.rotation) as GameObject;
            individualCar.transform.parent = carStand.transform;
        }
    }

    public void PreviousButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, carStand.transform.rotation) as GameObject;
            individualCar.transform.parent = carStand.transform;
        }
    }
    */
    public void StartGame()
    {
        SceneManager.LoadScene("Circuit1-Indianopolis");
    }

    public void BackToScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
