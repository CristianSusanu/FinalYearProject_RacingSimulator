using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRace : MonoBehaviour
{
    public GameObject car;
    public GameObject finishingCamera;
    public GameObject completeTrigger;
    public GameObject miniMapTrack;
    //public GameObject levelAudio;
    public GameObject lapManager;
    public AudioSource levelAudio;

    public GameObject textPos;
    public GameObject wonPanel;
    public GameObject lostPanel;

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
            //levelAudio.SetActive(false);
            levelAudio.enabled = false;
            lapManager.SetActive(false);

            if(textPos.GetComponent<Text>().text.Equals("1"))
            {
                wonPanel.SetActive(true);
            }
            else
            {
                lostPanel.SetActive(true);
            }
        }
    }
}
