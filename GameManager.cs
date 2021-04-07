using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject rpmNeedle;
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

    private float speed = 0.0f;

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
