using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroToMenu : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ToIntroMenu());
    }

    //to wait for a few seconds before changing to menu
    IEnumerator ToIntroMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}
