using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRace : MonoBehaviour
{
    public GameObject car;
    public GameObject finishingCamera;
    public GameObject completeTrigger;
    public GameObject miniMapTrack;
    public GameObject levelAudio;
    public GameObject lapManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            CarControl.carSpeed = 0f;
            car.GetComponent<InputManager>().enabled = false;
            car.GetComponent<CarControl>().enabled = false;
            car.GetComponent<SoundController>().enabled = false;
            car.GetComponent<SpeedIndicator>().enabled = false;
            car.GetComponent<AntiRollBar>().enabled = false;

            completeTrigger.SetActive(false);
            finishingCamera.SetActive(true);
            miniMapTrack.SetActive(false);
            levelAudio.SetActive(false);
            lapManager.SetActive(false);
        }
    }
}
