/*****************************************************************************
// File Name :         PauseGame.cs
// Author :            Rudy Wolfer, Andrea Swihart-DeCoster
// Creation Date :     October 27th, 2022
//
// Brief Description : Simple script for Pause Menu functionality.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// Edited: Andrea SD - modified for online use

public class OnlinePause : MonoBehaviourPun
{
    [SerializeField] private GameObject _pauseScreen;

    private void Start()
    {
        // Andrea SD
        if(SceneManager.GetActiveScene().name.Equals("OnlineScene"))
        {
            _pauseScreen.SetActive(false);  
        }     
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
        SceneManager.LoadScene("MainMenu");
        PhotonNetwork.Disconnect();     // ASD
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
