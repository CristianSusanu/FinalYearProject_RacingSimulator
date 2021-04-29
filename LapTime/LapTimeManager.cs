using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTimeManager : MonoBehaviour
{
    public static int minCounter;//static to refer to the vriables in another script
    public static int secCounter;
    public static float miliSecCounter;
    public static string milisecDisplay;

    public GameObject minImage;
    public GameObject secImage;
    public GameObject miliSecImage;

    public static float rawTime;

    private void Update()
    {
        miliSecCounter +=  Time.deltaTime * 10;
        rawTime += Time.deltaTime;
        milisecDisplay = miliSecCounter.ToString("F0");
        miliSecImage.GetComponent<Text>().text = "" + milisecDisplay;

        if (miliSecCounter >= 9)//a second has passed
        {
            miliSecCounter = 0;
            secCounter++;
        }

        if(secCounter <= 9)
        {
            secImage.GetComponent<Text>().text = "0" + secCounter + ".";
        }
        else
        {
            secImage.GetComponent<Text>().text = "" + secCounter + ".";
        }

        if(secCounter == 60)
        {
            secCounter = 0;
            minCounter++;
        }

        if(minCounter <= 9)
        {
            minImage.GetComponent<Text>().text = "0" + minCounter + ":";
        }
        else
        {
            minImage.GetComponent<Text>().text = "" + minCounter + ":";
        }
    }
}
