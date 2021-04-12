using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTime : MonoBehaviour
{
    public int minCounter;
    public int secCounter;
    public float miliSecCounter;

    public GameObject minImage;
    public GameObject secImage;
    public GameObject miliSecImage;

    void Start()
    {
        minCounter = PlayerPrefs.GetInt("SavedMinutes");
        secCounter = PlayerPrefs.GetInt("SavedSeconds");
        miliSecCounter = PlayerPrefs.GetFloat("SavedMilliseconds");

        minImage.GetComponent<Text>().text = "" + minCounter + ":";
        secImage.GetComponent<Text>().text = "" + secCounter + ".";
        miliSecImage.GetComponent<Text>().text = "" + LapTimeManager.miliSecCounter;
    }
}
