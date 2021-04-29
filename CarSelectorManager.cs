using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectorManager : MonoBehaviour
{
    public CarSelectorList carList;
    //public GameObject car;
    public GameObject carStand;
    private float standRotationSpeed = 15f;
    public int vehiclePointer = 0;

    public Text currency;
    public Text carName;
    public Text carPrice;
    public Text horsePower;
    public Text topSpeed;
    public Text acceleration;
    public Text weight;

    public GameObject buyCarButton;
    public GameObject playButton;

    private void Awake()
    {
        vehiclePointer = PlayerPrefs.GetInt("pointer");//get the pointer       
        
        GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        individualCar.transform.parent = carStand.transform;

        GetCarDetails();
    }

    private void FixedUpdate()
    {
        carStand.transform.Rotate(Vector3.up * standRotationSpeed * Time.deltaTime);
        Debug.Log(PlayerPrefs.GetInt("currency"));
    }
    
    public void NextButton()
    {
        if (vehiclePointer < carList.cars.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));//destroy the current car from the scene
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject individualCar = Instantiate(carList.cars[vehiclePointer], Vector3.zero, carStand.transform.rotation) as GameObject;
            individualCar.transform.parent = carStand.transform;
            GetCarDetails();
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
            GetCarDetails();
        }
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("Circuit1-Indianopolis");
    }

    public void BackToScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BuyCar()
    {
        //check if the player has enough money
        if(PlayerPrefs.GetInt("currency") >= carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carPrice)
        {
            PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") - carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carPrice);

            PlayerPrefs.SetString(carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carName.ToString(),
                        carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carName.ToString());

            GetCarDetails();
        }
        else
        {
            carPrice.text = "Insufficient Funds";
        }
        
    }

    public void GetCarDetails()
    {
        //check if the player owns the car
        if (carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carName.ToString() ==
            PlayerPrefs.GetString(carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carName.ToString()))
        {
            carName.text = "Owned";
            carPrice.text = "";
            playButton.SetActive(true);
            buyCarButton.SetActive(false);
            currency.text = "Budget: $" + PlayerPrefs.GetInt("currency").ToString("");

            horsePower.text = "Power: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().horsePower.ToString() + " hp";
            topSpeed.text = "TopSpeed: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().topSpeed.ToString() + " km/h";
            acceleration.text = "Acceleration 0-100 km/h: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().acceleration0100.ToString() + "s";
            weight.text = "Weight: " + ((int)carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<Rigidbody>().mass).ToString() + "kg";

            return;
        }

        currency.text = "Budget: $" + PlayerPrefs.GetInt("currency").ToString("");

        carName.text = carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carName.ToString();
        carPrice.text = "$" + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().carPrice.ToString();

        horsePower.text = "Power: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().horsePower.ToString() + " hp";
        topSpeed.text = "TopSpeed: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().topSpeed.ToString() + " km/h";
        acceleration.text = "Acceleration 0-100 km/h: " + carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<CarControl>().acceleration0100.ToString() + "s";
        weight.text = "Weight: " + ((int)carList.cars[PlayerPrefs.GetInt("pointer")].GetComponent<Rigidbody>().mass).ToString() + "kg";

        playButton.SetActive(false);
        buyCarButton.SetActive(true);
    }
}
