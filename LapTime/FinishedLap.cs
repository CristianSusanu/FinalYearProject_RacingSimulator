using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishedLap : MonoBehaviour
{
    public GameObject finishedLapTrigger;
    public GameObject halfLapTrigger;

    public GameObject min;
    public GameObject sec;
    public GameObject miliSec;

    public int minCount;
    public int secCount;
    public float miliSecCount;
    public string milisecDispl;

    //public GameObject LapTimeImage;

    private void OnTriggerEnter(Collider col)
    {
        //if(col.tag == "Player")
        //{
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

            miliSec.GetComponent<Text>().text = "" + LapTimeManager.miliSecCounter;

            PlayerPrefs.SetInt("SavedMinutes", LapTimeManager.minCounter);
            PlayerPrefs.SetInt("SavedSeconds", LapTimeManager.secCounter);
            PlayerPrefs.SetFloat("SavedMilliseconds", LapTimeManager.miliSecCounter);

            LapTimeManager.minCounter = 0;
            LapTimeManager.secCounter = 0;
            LapTimeManager.miliSecCounter = 0;

            halfLapTrigger.SetActive(true);
            finishedLapTrigger.SetActive(false);
        //}
    }

}
