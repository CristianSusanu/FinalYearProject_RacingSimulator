using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRace : MonoBehaviour
{
    public GameObject car;
    public GameObject finishingCamera;
    public GameObject completeTrigger;
    //public GameObject view;
    //public GameObject levelMusic;

    private void OnTriggerEnter()
    {
        //car.SetActive(false);
        CarControl.carSpeed = 0f;
        completeTrigger.SetActive(false);
        //car.GetComponent<CarControl>().enabled = false;
        //car.GetComponent<AIController>().enabled = false;
        //car.SetActive(true);
        finishingCamera.SetActive(true);
        //levelMusic.SetActive(false);
        //view.SetActive(false);
    }
}
