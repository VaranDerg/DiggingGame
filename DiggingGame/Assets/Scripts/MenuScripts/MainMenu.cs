/*****************************************************************************
// File Name :         MainMenu.cs
// Author :            Sean Forrester, Andrea Swihart-DeCoster
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
        SceneManager.LoadScene("LocalGame");    // Added - Andrea SD
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
        SceneManager.LoadScene("Collection");
    }

    /// <summary>
    /// Loads rules
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void Rules()
    {
        SceneManager.LoadScene("Rules");
    }

    /// <summary>
    /// Quits game
    /// 
    /// Author: Sean Forrester
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
