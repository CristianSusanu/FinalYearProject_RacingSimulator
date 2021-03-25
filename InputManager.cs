using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour 
{
    public float throttle;
    public float steering;

    public bool shiftUp;
    public bool shiftDown;

    public bool li;
    public bool brake;
    public bool reverse;
    public bool transmission;

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
        transmission = Input.GetKey(KeyCode.X);
    }
}
