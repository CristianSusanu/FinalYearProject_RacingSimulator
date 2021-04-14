using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfRaceButtons : MonoBehaviour
{
    public void BackToMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TryAgain(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void NextRace(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
