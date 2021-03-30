using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour 
{
    public float throttle;
    public float steering;

    public bool shiftUp;
    public bool shiftDown;

    public bool li;//switch lights on/off
    public bool brake;//apply car brake
    public bool reverse;//car reverse
    public bool transmission;//switch between automatic and manual transmission
    public bool carReset;//reset the car in case it rolls over
    public bool tractionControlToggle;//to turn the traction control on or off

    // Update is called once per frame
    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steering = Input.GetAxis("Horizontal");

        shiftUp = Input.GetKey(KeyCode.LeftShift);
        shiftDown = Input.GetKey(KeyCode.LeftControl);

        li = Input.GetKeyDown(KeyCode.L);
        brake = Input.GetKey(KeyCode.Space);
        reverse = Input.GetKey(KeyCode.DownArrow);
        transmission = Input.GetKeyDown(KeyCode.X);
        carReset = Input.GetKeyDown(KeyCode.R);

        tractionControlToggle = Input.GetKeyDown(KeyCode.T);
        //tractionControlToggle = Input.GetKey(KeyCode.T);
    }
}
