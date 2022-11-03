/*****************************************************************************
// File Name :         GalleryMenu.cs
// Author :            Caelie Joyner
// Creation Date :     November 1st, 2022
//
// Brief Description : Script for moving between the pages of the gallery.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalleryMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> galleryPages = new List<GameObject>();
    [SerializeField] private GameObject _backButton, _forwardButton;
    private int currentPage = 0;

    private void Start()
    {
        currentPage = 0;
        SetActivePage(currentPage);
    }

    public void SetActivePage(int page)
    {
        foreach (GameObject gPage in galleryPages)
        {
            gPage.SetActive(false);
        }

        if (page == 0)
        {
            _backButton.SetActive(false);
            _forwardButton.SetActive(true);
        }
        else if (page == galleryPages.Count - 1)
        {
            _forwardButton.SetActive(false);
            _backButton.SetActive(true);
        }
        else
        {
            _forwardButton.SetActive(true);
            _backButton.SetActive(true);
        }

        galleryPages[page].SetActive(true);
    }

    public void ChangePage(int flipAmount)
    {
        currentPage += flipAmount;
        SetActivePage(currentPage);
    }

    public void BackToMenu()
    {
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
    }
}
