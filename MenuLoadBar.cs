using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //to reference slider

public class MenuLoadBar : MonoBehaviour
{
    public GameObject loadScreen;
    public Slider loadSlider;
    public Text loadText;

    public void LevelLoading(int sceneNumber)
    {
        StartCoroutine(LoadSceneAsyncronously(sceneNumber));

        //to accomodate for when the time is 0 for winning screens
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator LoadSceneAsyncronously(int sceneNumber)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneNumber);//keeps the current scene running while it's loading the new scene into memory
        loadScreen.SetActive(true);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadSlider.value = progress;
            loadText.text = progress * 100f + "%";
            
            yield return null;//to wait until the next frame before continuing
        }
    }

    public void Exit()
    {
        Application.Quit();
        //Debug.Log("Game Closed");
    }

    public void ResetGameStats()
    {
        PlayerPrefs.DeleteAll();
    }
}
