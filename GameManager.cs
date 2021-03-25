using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject rpmNeedle;
    public CarControl carControl;

    private float minRPMPosition = -533f;
    private float maxRPMPosition = -726f;
    private float wantedRPMPosition = 0.0f;

    public float RPM = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RPM = carControl.engineRPM;
        rpmNeedle.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(minRPMPosition + 20f, maxRPMPosition, carControl.engineRPM / carControl.MaxEngineRPM));
        wantedRPMPosition = minRPMPosition - maxRPMPosition;
        float temp = carControl.engineRPM / carControl.MaxEngineRPM;
        //rpmNeedle.transform.eulerAngles = new Vector3(0f, 0f, minRPMPosition - wantedRPMPosition * RPM / carControl.MaxEngineRPM);
    }
}
