using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoadBar : MonoBehaviour
{
    public GameObject loadScreen;
    public Slider loadSlider;
    public Text loadText;
    public Text currency;
    private bool muted = false;

    public void LevelLoading(int sceneNumber)
    {
        StartCoroutine(LoadSceneAsyncronously(sceneNumber));

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

    public void ResetGameStats()
    {
        PlayerPrefs.DeleteAll();
    }

    public void MuteAudio()
    {
        if (!muted)
        {
            AudioListener.volume = 0;
            muted = true;
        }
        else
        {
            AudioListener.volume = 1;
            muted = false;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void Update()
    {
        currency.text = "Budget: $" + PlayerPrefs.GetInt("currency").ToString();
    }
}
