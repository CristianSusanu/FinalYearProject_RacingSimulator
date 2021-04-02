using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject rpmNeedle;
    public GameObject tacho;
    public GameObject tractionCtrlIcon;

    public CarControl carControl;

    private float minRPMPosition = -533f;
    private float maxRPMPosition = -726f;

    private float RPM = 0.0f;

    public Text speedText;

    public Text RPMIndicator;
    public Text gearIndicatorText;
    public Text transmissionIndicatorText;

    private float speed = 0.0f;

    void FixedUpdate()
    {
        // Speed display in kmh
        speed = carControl.rigidB.velocity.magnitude * 3.6f;
        speedText.text = ((int)speed).ToString() + "Km/h";

        //RPM indicator
        RPM = carControl.engineRPM;
        RPMIndicator.text = ((int)RPM).ToString() + "RPM";
        rpmNeedle.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minRPMPosition + 20f, maxRPMPosition, carControl.engineRPM / carControl.MaxEngineRPM));

        //Transmission indicator
        if (carControl.autoTransmission)
        {
            transmissionIndicatorText.text = "A";
        }
        else
        {
            transmissionIndicatorText.text = "M";
        }

        if (carControl.tractionControlEngage)
        {
            tractionCtrlIcon.SetActive(false);
        }
        else
        {
            tractionCtrlIcon.SetActive(true);
        }
    }

    public void gearChange()
    {
        //gearIndicatorText.text = carControl.currentGear.ToString();
        if(carControl.currentGear == 0)
        {
            gearIndicatorText.text = "R";
        }
        else
        {
            gearIndicatorText.text = carControl.currentGear.ToString();
        }
    }
}
