/*****************************************************************************
// File Name :         MainMenu.cs
// Author :            Sean Forrester, Andrea Swihart-DeCoster, Rudy Wolfer
// Creation Date :     October 26th, 2022
//
// Brief Description : This document controls the main menu.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Edited: Andrea SD - added all comments, the header, modified PlayGame(), Added code for the other buttons

public class MainMenu : MonoBehaviour
{
    //Andrea SD
    [SerializeField] GameObject selectMode;     
    [SerializeField] GameObject mainPage;
    [SerializeField] GameObject optionsPage;

    /// <summary>
    /// Enabled correct parts on startup.
    /// Rudy W.
    /// </summary>
    private void Start()
    {
        selectMode.SetActive(false);
        mainPage.SetActive(true);
        optionsPage.SetActive(false);
    }

    /// <summary>
    /// Enables the menu to select which game mode to play
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void StartGame()
    {
        selectMode.SetActive(true);
        mainPage.SetActive(false);
    }

    /// <summary>
    /// Loads main menu
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Loads the pass and play.
    /// 
    /// Sean Forrester
    /// Edited: Andrea SD - renamed function to PlayLocal, general changes
    /// </summary>
    public void PlayLocal()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Edited: Andrea - removed
        SceneManager.LoadScene("PassAndPlayScene");    // Added - Andrea SD
    }

    /// <summary>
    /// Loads online game
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void PlayOnline()
    {
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// Loads card collection
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void Collection()
    {
        SceneManager.LoadScene("GalleryScene");
    }

    /// <summary>
    /// Loads options page
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void Options()
    {
        optionsPage.SetActive(true);
        mainPage.SetActive(false);
    }

    /// <summary>
    /// Returns to main menu page
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void Back()
    {
        optionsPage.SetActive(false);
        selectMode.SetActive(false);
        mainPage.SetActive(true);
    }

    /// <summary>
    /// Loads rules
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    /// <summary>
    /// Quits game
    /// 
    /// Author: Sean Forrester
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
