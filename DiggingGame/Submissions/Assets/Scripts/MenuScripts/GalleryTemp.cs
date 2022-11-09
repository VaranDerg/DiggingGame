/*****************************************************************************
// File Name :         GalleryTemp.cs
// Author :            Rudy Wolfer
// Creation Date :     October 27th, 2022
//
// Brief Description : Extremely complex and temporary gallery script.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalleryTemp : MonoBehaviour
{
    /// <summary>
    /// Loads the Main Menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
