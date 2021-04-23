using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndOfRaceButtons : MonoBehaviour
{

    public GameObject gamePaused;
    public GameObject raceEndTrigger;
    public Text muteAudioText;
    public Text currency;
    private bool muted = false;

    public void BackToMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TryAgain(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        Time.timeScale = 1;

        //to reset the current lap time when game is reset
        LapTimeManager.minCounter = 0;
        LapTimeManager.secCounter = 0;
        LapTimeManager.miliSecCounter = 0;
    }

    public void NextRace(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
        gamePaused.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().enabled = false;
        currency.text = "Budget: $" + PlayerPrefs.GetInt("currency").ToString();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePaused.SetActive(false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>().enabled = true;
    }

    public void MuteAudio()
    {
        if (!muted)
        {
            AudioListener.volume = 0;
            muted = true;
            muteAudioText.text = "UnMute Audio";
        }
        else
        {
            AudioListener.volume = 1;
            muted = false;
            muteAudioText.text = "Mute Audio";
        }
    }
}
