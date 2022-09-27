/*****************************************************************************
// File Name :         GameplayManager.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 26th, 2022
//
// Brief Description : This document control the basic gameplay loop and 
                       mechanics.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private GameObject _currentPlayer;

    [SerializeField] GameObject _player1;
    [SerializeField] GameObject _player2;

    [SerializeField] TextMeshProUGUI currentPlayerText;

    public TextMeshProUGUI p1DirtText;
    public TextMeshProUGUI p2DirtText;
    public TextMeshProUGUI p1GrassText;
    public TextMeshProUGUI p2GrassText;
    public TextMeshProUGUI p1StoneText;
    public TextMeshProUGUI p2StoneText;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _currentPlayer = _player1;
        UpdateText(currentPlayerText, "Current Turn: " + _currentPlayer.name);
    }

    /// <summary>
    /// Switches between the two players
    /// </summary>
    public void ChangePlayer()
    {
        if(_currentPlayer.Equals(_player1))
        {
            _currentPlayer = _player2;
        }
        else
        {
            _currentPlayer = _player1;
        }
        
        UpdateText(currentPlayerText, "Current Turn: " + _currentPlayer.name);

    }

    /// <summary>
    /// Updates a text UI element
    /// </summary>
    /// <param name="textEdit"> Text UI element to be changed </param>
    /// <param name="newText"> what the text will be changed to </param>
    public void UpdateText(TextMeshProUGUI textEdit, string newText)
    {
        textEdit.text = newText;
    }
}
