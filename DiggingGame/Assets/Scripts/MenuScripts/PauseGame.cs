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
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject _pauseScreen;

    private void Start()
    {
        _pauseScreen.SetActive(false);
    }

    public void Pause()
    {
        _pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        _pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Menu()
    {
        Time.timeScale = 1.0f;
        StatManager.s_Instance.ResetStatistics();
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        Application.Quit();
    }

    public void Rules()
    {
        Application.OpenURL("https://docs.google.com/document/d/1xR1zlYT4rUaA1m1OP98pxGKvQqt7RB9VVsK4ikHUPjc/edit?usp=sharing");
    }
}
