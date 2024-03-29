﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject rpmNeedle;
    public GameObject interiorGauges;
    public GameObject tacho;
    public GameObject tractionCtrlIcon;
    public GameObject tractionCtrlInteriorIcon;

    public CarControl carControl;

    private float minRPMPosition = -533f;
    private float maxRPMPosition = -726f;

    private float RPM = 0.0f;

    public Text speedText;
    public Text speedTextInterior;
    public Text RPMIndicator;
    public Text RPMIndicatorInterior;
    public Text gearIndicatorText;
    public Text gearIndicatorInteriorText;
    public Text transmissionIndicatorText;
    public Text transmissionIndicatorInteriorText;

    public GameObject raceStartCounter;
    public GameObject lapTimer;
    public AudioSource getReady;
    public AudioSource go;

    public CarSelectorList carList;
    public GameObject startPos;

    private float speed = 0.0f;
    public GameObject gamePaused;
    private bool menuPause = false;

    private void Awake()
    {
        Instantiate(carList.cars[PlayerPrefs.GetInt("pointer")], startPos.transform.position, startPos.transform.rotation);
        carControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CarControl>();

    }

    private void Start()
    {
        StartCoroutine(StartCounter());
    }

    //Race Start CountDown
    IEnumerator StartCounter()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().enabled = false;

        if (SceneManager.GetActiveScene().name == "Circuit6-RedBullRing")
        {
            GameObject.Find("GT86(1)").GetComponent<AIController>().enabled = false;
        }
        else
        {
            GameObject.Find("GT86(1)").GetComponent<AIController>().enabled = false;
            GameObject.Find("GT86(2)").GetComponent<AIController>().enabled = false;
            GameObject.Find("GT86(3)").GetComponent<AIController>().enabled = false;
        }

        GameObject.Find("LapTimeManager").GetComponent<LapTimeManager>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        raceStartCounter.GetComponent<Text>().text = "3";
        getReady.Play();
        raceStartCounter.SetActive(true);
        yield return new WaitForSeconds(1);
        raceStartCounter.SetActive(false);

        raceStartCounter.GetComponent<Text>().text = "2";
        getReady.Play();
        raceStartCounter.SetActive(true);
        yield return new WaitForSeconds(1);
        raceStartCounter.SetActive(false);

        raceStartCounter.GetComponent<Text>().text = "1";
        getReady.Play();
        raceStartCounter.SetActive(true);
        yield return new WaitForSeconds(1);
        raceStartCounter.SetActive(false);

        go.Play();
        lapTimer.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().enabled = true;

        if (SceneManager.GetActiveScene().name == "Circuit6-RedBullRing")
        {
            GameObject.Find("GT86(1)").GetComponent<AIController>().enabled = true;
        }
        else
        {
            GameObject.Find("GT86(1)").GetComponent<AIController>().enabled = true;
            GameObject.Find("GT86(2)").GetComponent<AIController>().enabled = true;
            GameObject.Find("GT86(3)").GetComponent<AIController>().enabled = true;
        }

        GameObject.Find("LapTimeManager").GetComponent<LapTimeManager>().enabled = true;
    }
    
    private void Update()
    {
        if (InputManager.gamePause)
        {
            if (menuPause)
            {
                Time.timeScale = 1;
                gamePaused.SetActive(false);
                GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = true;
                menuPause = false;
            }
            else
            {
                Time.timeScale = 0;
                gamePaused.SetActive(true);
                menuPause = true;
                GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        // Speed display in kmh 
        speed = carControl.rigidB.velocity.magnitude * 3.6f;
        speedText.text = speedTextInterior.text = ((int)speed).ToString() + "Km/h";

        //RPM indicator
        RPM = carControl.engineRPM;
        RPMIndicator.text = RPMIndicatorInterior.text = ((int)RPM).ToString() + "RPM";
        rpmNeedle.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minRPMPosition + 20f, maxRPMPosition, carControl.engineRPM / carControl.MaxEngineRPM));

        //Transmission indicator
        if (carControl.autoTransmission)
        {
            transmissionIndicatorText.text = "D";
            transmissionIndicatorInteriorText.text = "D";
        }
        else
        {
            transmissionIndicatorText.text = "M";
            transmissionIndicatorInteriorText.text = "M";
        }
        
        if (carControl.tractionControlEngage)
        {
            tractionCtrlIcon.SetActive(false);
            tractionCtrlInteriorIcon.SetActive(false);
        }
        else
        {
            tractionCtrlIcon.SetActive(true);
            tractionCtrlInteriorIcon.SetActive(true);
        }      
    }

    public void gearChange()
    {
        if(carControl.currentGear == 0)
        {
            gearIndicatorText.text = gearIndicatorInteriorText.text = "R";
        }
        else
        {
            gearIndicatorText.text = gearIndicatorInteriorText.text = carControl.currentGear.ToString();
        }
    }
}
