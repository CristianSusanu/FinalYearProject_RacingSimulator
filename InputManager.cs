using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour 
{
    public float throttle;
    public float steering;
    public bool li;
    public bool brake;
    public bool reverse;

    // Update is called once per frame

    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steering = Input.GetAxis("Horizontal");

        li = Input.GetKeyDown(KeyCode.L);
        brake = Input.GetKey(KeyCode.Space);
        reverse = Input.GetKey(KeyCode.DownArrow);
    }
}
