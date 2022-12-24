/*****************************************************************************
// File Name :         PauseGame.cs
// Author :            Rudy Wolfer
// Creation Date :     October 27th, 2022
//
// Brief Description : Simple script for Pause Menu functionality.
*****************************************************************************/

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject _pauseScreen;
    private bool _isPaused;

    /// <summary>
    /// Disables the pause screen, just in case.
    /// </summary>
    private void Start()
    {
        _pauseScreen.SetActive(false);
    }

    /// <summary>
    /// Esc functionality in case people like that.
    /// </summary>
    private void Update()
    {
        if (ViewDiscardPile.s_ViewOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        _pauseScreen.SetActive(true);
        Time.timeScale = 0;
        _isPaused = true;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void Resume()
    {
        _pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
        _isPaused = false;
    }

    /// <summary>
    /// Goes back to the menu.
    /// </summary>
    public void Menu()
    {
        Resume();
        _pauseScreen.SetActive(true);
        StatManager.s_Instance.ResetStatistics();
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Resume();
        Application.Quit();
    }

    /// <summary>
    /// Opens the rules.
    /// </summary>
    public void Rules()
    {
        Application.OpenURL("https://docs.google.com/document/d/1xR1zlYT4rUaA1m1OP98pxGKvQqt7RB9VVsK4ikHUPjc/edit?usp=sharing");
    }

    /// <summary>
    /// Opens the Bug Report form.
    /// </summary>
    public void BugReport()
    {
        Application.OpenURL("https://forms.gle/heCizWF7Qhwevs8o9");
    }
}
