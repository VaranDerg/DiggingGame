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
    [Header("References")]
    [SerializeField] private List<GameObject> _galleryPages = new List<GameObject>();
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private Color _grassColor, _dirtColor, _stoneColor, _goldColor;
    private Color _currentColor;
    [SerializeField] private GameObject _backButton, _forwardButton;
    [SerializeField] private GameObject _grassParticles, _dirtParticles, _stoneParticles, _goldParticles;

    [Header("Values")]
    private int _currentPage = 0;
    [SerializeField] private float _bgChangeSpeed;

    /// <summary>
    /// Author: Caelie
    /// </summary>
    private void Start()
    {
        _currentColor = _grassColor;
        _background.color = _currentColor;
        _currentPage = 0;
        SetActivePage(_currentPage);
    }

    /// <summary>
    /// Calls UpdateBGColor
    /// Author: Rudy
    /// </summary>
    private void Update()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// Lerps the BG Color
    /// Author: Rudy
    /// </summary>
    private void UpdateVisuals()
    {
        if (_currentPage == 0)
        {
            _currentColor = _grassColor;
            _grassParticles.SetActive(true);
            _dirtParticles.SetActive(false);
            _stoneParticles.SetActive(false);
            _goldParticles.SetActive(false);
        }
        else if (_currentPage == 1)
        {
            _currentColor = _dirtColor;
            _grassParticles.SetActive(false);
            _dirtParticles.SetActive(true);
            _stoneParticles.SetActive(false);
            _goldParticles.SetActive(false);
        }
        else if (_currentPage == 2)
        {
            _currentColor = _stoneColor;
            _grassParticles.SetActive(false);
            _dirtParticles.SetActive(false);
            _stoneParticles.SetActive(true);
            _goldParticles.SetActive(false);
        }
        else if (_currentPage == 3)
        {
            _currentColor = _goldColor;
            _grassParticles.SetActive(false);
            _dirtParticles.SetActive(false);
            _stoneParticles.SetActive(false);
            _goldParticles.SetActive(true);
        }

        _background.color = Color.Lerp(_background.color, _currentColor, _bgChangeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Author: Caelie
    /// </summary>
    /// <param name="page"></param>
    public void SetActivePage(int page)
    {
        foreach (GameObject gPage in _galleryPages)
        {
            gPage.SetActive(false);
        }

        if (page == 0)
        {
            _backButton.SetActive(false);
            _forwardButton.SetActive(true);
        }
        else if (page == _galleryPages.Count - 1)
        {
            _forwardButton.SetActive(false);
            _backButton.SetActive(true);
        }
        else
        {
            _forwardButton.SetActive(true);
            _backButton.SetActive(true);
        }

        _galleryPages[page].SetActive(true);
    }

    /// <summary>
    /// Author: Rudy
    /// </summary>
    public void GalleryPageShowHide(bool show)
    {
        _galleryPages[_currentPage].SetActive(show);
    }

    /// <summary>
    /// Author: Caelie
    /// </summary>
    public void ChangePage(int flipAmount)
    {
        _currentPage += flipAmount;
        SetActivePage(_currentPage);
    }

    /// <summary>
    /// Author: Caelie
    /// </summary>
    public void BackToMenu()
    {
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
    }
}