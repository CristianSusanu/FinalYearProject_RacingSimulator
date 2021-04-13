using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTime : MonoBehaviour
{
    public int minCounter;
    public int secCounter;
    private double miliSecCounter;
    string miliSecDisplay;

    public GameObject minImage;
    public GameObject secImage;
    public GameObject miliSecImage;


    void Start()
    {
        minCounter = PlayerPrefs.GetInt("SavedMinutes");
        secCounter = PlayerPrefs.GetInt("SavedSeconds");
        miliSecCounter = PlayerPrefs.GetFloat("SavedMilliseconds");
        miliSecDisplay = miliSecCounter.ToString("F0");
        
        minImage.GetComponent<Text>().text = "" + minCounter + ":";
        secImage.GetComponent<Text>().text = "" + secCounter + ".";
        miliSecImage.GetComponent<Text>().text = "" + miliSecCounter;
    }
}
