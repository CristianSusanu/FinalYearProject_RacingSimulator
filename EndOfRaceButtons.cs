﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfRaceButtons : MonoBehaviour
{

    public GameObject gamePaused;
    //public SoundController carSound;

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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePaused.SetActive(false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<SoundController>().enabled = true;
    }
}
