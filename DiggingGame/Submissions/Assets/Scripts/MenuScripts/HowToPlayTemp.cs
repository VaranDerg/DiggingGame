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
    [Header("References")]
    [SerializeField] private List<GameObject> _howToPlayPages = new List<GameObject>();
    [SerializeField] private GameObject _backButton, _forwardButton;
    private int currentPage = 0;

    /// <summary>
    /// Sets the current active page to 0.
    /// </summary>
    private void Start()
    {
        currentPage = 0;
        SetActivePage(currentPage);
    }

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

    /// <summary>
    /// Sets a page in the list to active.
    /// </summary>
    /// <param name="page">0 -> List max size - 1.</param>
    public void SetActivePage(int page)
    {
        foreach(GameObject htpPage in _howToPlayPages)
        {
            htpPage.SetActive(false);
        }

        if(page == 0)
        {
            _backButton.SetActive(false);
            _forwardButton.SetActive(true);
        }
        else if(page == _howToPlayPages.Count - 1)
        {
            _forwardButton.SetActive(false);
            _backButton.SetActive(true);
        }
        else
        {
            _forwardButton.SetActive(true);
            _backButton.SetActive(true);
        }

        _howToPlayPages[page].SetActive(true);
    }

    /// <summary>
    /// Changes the How To Play page by the flip amount.
    /// </summary>
    /// <param name="flipAmount">1 or -1</param>
    public void ChangePage(int flipAmount)
    {
        currentPage += flipAmount;
        SetActivePage(currentPage);
    }
}
