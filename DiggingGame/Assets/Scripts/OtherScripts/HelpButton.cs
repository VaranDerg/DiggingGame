/*****************************************************************************
// File Name :         HelpButton.cs
// Author :            Rudy Wolfer
// Creation Date :     November 21st, 2022
//
// Brief Description : Script for each individual Help Button.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private string _helpName;
    [TextArea(3, 6)]
    [SerializeField] private string _helpDescription;

    [Header("References")]
    private HelpButtonController _hbc;

    /// <summary>
    /// Assigns hbc.
    /// </summary>
    private void Awake()
    {
        _hbc = FindObjectOfType<HelpButtonController>();
    }
    
    /// <summary>
    /// Shows the help menu with text.
    /// </summary>
    public void ShowHelpFromButton()
    {
        _hbc.ShowHelp(_helpName, _helpDescription);
    }
}
