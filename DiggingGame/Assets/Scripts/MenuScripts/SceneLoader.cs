/*****************************************************************************
// File Name :         SceneLoader.cs
// Author :            Rudy Wolfer
// Creation Date :     November 3rd, 2022
//
// Brief Description : A script for Scene Transitions and Loading Time.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _anims;

    [Header("Values")]
    [SerializeField] private float _transitionTime;

    private static bool s_enteredGame = false;

    /// <summary>
    /// Turns on the Transition Manager.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Wrapper for SceneLoadingCoroutine.
    /// </summary>
    /// <param name="sceneName">Name of the Scene.</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(SceneLoadingCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine for loading Scenes.
    /// </summary>
    /// <param name="sceneName">Name of the Scene.</param>
    /// <returns>Wait Time</returns>
    private IEnumerator SceneLoadingCoroutine(string sceneName)
    {
        //Starts transition
        _anims.SetTrigger("Start");
        FindObjectOfType<SFXManager>().Play("LoadScene");

        yield return new WaitForSeconds(_transitionTime);

        if (sceneName == "MainMenu" && s_enteredGame)
        {
            BGMManager.s_Instance.Invoke("PlayMenuTheme", BGMManager.s_Instance.SongFadeTime);
            s_enteredGame = false;
        }
        else if(sceneName == "PnPWeather" || sceneName == "OnlineScene")
        {
            //Enable Gameplay Music
            BGMManager.s_Instance.SwapTrack();
            s_enteredGame = true;
        }

        SceneManager.LoadScene(sceneName);
    }
}
