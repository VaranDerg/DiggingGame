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
    [SerializeField] GameObject creditsPage;

    /// <summary>
    /// Enabled correct parts on startup.
    /// Rudy W.
    /// </summary>
    private void Start()
    {
        selectMode.SetActive(false);
        mainPage.SetActive(true);
        optionsPage.SetActive(false);
        creditsPage.SetActive(false);
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
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
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
        FindObjectOfType<SceneLoader>().LoadScene("PnPWeather");    // Added - Andrea SD
    }

    /// <summary>
    /// Loads online game
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void PlayOnline()
    {
        FindObjectOfType<SceneLoader>().LoadScene("Loading");
    }

    /// <summary>
    /// Loads card collection
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void Collection()
    {
        FindObjectOfType<SceneLoader>().LoadScene("GalleryScene");
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
    /// Loads options page
    /// 
    /// Author: Rudy W
    /// </summary>
    public void Credits()
    {
        creditsPage.SetActive(true);
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
        creditsPage.SetActive(false);
    }

    /// <summary>
    /// Loads rules
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void HowToPlay()
    {
        FindObjectOfType<SceneLoader>().LoadScene("HowToPlay");
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
