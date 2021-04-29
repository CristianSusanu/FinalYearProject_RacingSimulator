using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishedLap : MonoBehaviour
{
    public GameObject finishedLapTrigger;
    public GameObject halfLapTrigger;

    public GameObject min;
    public GameObject sec;
    public GameObject miliSec;
    public GameObject finish;

    //to count the number of laps
    public GameObject lapCount;
    public int numberOfCompletedLaps;

    public float rawTime;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Circuit1-Indianopolis")
        {
            if (numberOfCompletedLaps == 1)
            {
                finish.SetActive(true);
            }
        }else
        {
            if (numberOfCompletedLaps == 0)
            {
                finish.SetActive(true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            numberOfCompletedLaps++;
            rawTime = PlayerPrefs.GetFloat("RawTime");

            //check if the currently set time is better than the saved one
            if (LapTimeManager.rawTime <= rawTime)
            {
                if (LapTimeManager.secCounter <= 9)
                {
                    sec.GetComponent<Text>().text = "0" + LapTimeManager.secCounter + ".";
                }
                else
                {
                    sec.GetComponent<Text>().text = "" + LapTimeManager.secCounter + ".";
                }

                if (LapTimeManager.minCounter <= 9)
                {
                    min.GetComponent<Text>().text = "0" + LapTimeManager.minCounter + ".";
                }
                else
                {
                    min.GetComponent<Text>().text = "" + LapTimeManager.minCounter + ".";
                }

                miliSec.GetComponent<Text>().text = LapTimeManager.miliSecCounter.ToString("F0");
            }

            PlayerPrefs.SetInt("SavedMinutes", LapTimeManager.minCounter);
            PlayerPrefs.SetInt("SavedSeconds", LapTimeManager.secCounter);
            PlayerPrefs.SetFloat("SavedMilliseconds", LapTimeManager.miliSecCounter);
            PlayerPrefs.SetFloat("RawTime", LapTimeManager.rawTime);

            LapTimeManager.minCounter = 0;
            LapTimeManager.secCounter = 0;
            LapTimeManager.miliSecCounter = 0;
            LapTimeManager.rawTime = 0;

            lapCount.GetComponent<Text>().text = "" + numberOfCompletedLaps;

            halfLapTrigger.SetActive(true);
            finishedLapTrigger.SetActive(false);
        }
    }
}
