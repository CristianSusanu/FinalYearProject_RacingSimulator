using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndRace : MonoBehaviour
{
    public GameObject car;
    private GameObject finishingCamera;
    public GameObject completeTrigger;
    public GameObject miniMapTrack;
    //public GameObject levelAudio;
    public GameObject lapManager;
    public AudioSource levelAudio;

    public GameObject textPos;
    public GameObject wonPanel;
    public GameObject lostPanel;

    private void Awake()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        finishingCamera = car.transform.Find("RaceFinishCub").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            CarControl.carSpeed = 0f;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().enabled = false;
            //GameObject.FindGameObjectWithTag("Player").GetComponent<CarControl>().enabled = false;
            //GameObject.FindGameObjectWithTag("Player").GetComponent<SpeedIndicator>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = false;
            //GameObject.FindGameObjectWithTag("Player").GetComponent<AntiRollBar>().enabled = false;

            completeTrigger.SetActive(false);
            
            finishingCamera.SetActive(true);
            miniMapTrack.SetActive(false);
            //levelAudio.SetActive(false);
            levelAudio.enabled = false;
            lapManager.SetActive(false);

            if(textPos.GetComponent<Text>().text.Equals("1"))
            {
                wonPanel.SetActive(true);

                if(SceneManager.GetActiveScene().name == "Circuit1-Indianopolis")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 1000);
                }
                else if(SceneManager.GetActiveScene().name == "Circuit2-CastleCombe")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 2000);
                }
                else if (SceneManager.GetActiveScene().name == "Circuit3-LimeRock")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 3000);
                }
                else if (SceneManager.GetActiveScene().name == "Circuit4-LyddenHill")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 4000);
                }
                else if (SceneManager.GetActiveScene().name == "Circuit5-Tsukuba")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 5000);
                }
                else if (SceneManager.GetActiveScene().name == "Circuit6-RedBullRing")
                {
                    PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") + 10000);
                }

                Debug.Log(PlayerPrefs.GetInt("currency"));
            }
            else
            {
                lostPanel.SetActive(true);
            }
            //car.transform.Find("RaceFinishCub").gameObject.SetActive(true);
        }
    }
}
