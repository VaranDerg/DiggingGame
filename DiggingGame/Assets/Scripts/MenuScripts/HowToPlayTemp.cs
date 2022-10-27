/*****************************************************************************
// File Name :         HowToPlayTemp.cs
// Author :            Rudy Wolfer
// Creation Date :     October 27th, 2022
//
// Brief Description : Extremely complex and temporary How To Play script.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayTemp : MonoBehaviour
{
    /// <summary>
    /// Loads the Main Menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Links to the Rules Doc.
    /// </summary>
    public void OpenRules()
    {
        Application.OpenURL("https://docs.google.com/document/d/1xR1zlYT4rUaA1m1OP98pxGKvQqt7RB9VVsK4ikHUPjc/edit?usp=sharing");
    }
}
