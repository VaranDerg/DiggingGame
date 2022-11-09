/*****************************************************************************
// File Name :         MainMenuManager.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     October 11th, 2022
//
// Brief Description : A script that manages the buttons in the MainMenu
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Loads the online game
    /// Loads the online game
    /// </summary>
    public void LoadOnline()
    {
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// Loads the local game
    /// </summary>
    public void LoadLocal()
    {
        SceneManager.LoadScene("ActionsTestScene");
    }
}
