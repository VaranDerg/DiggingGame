/*****************************************************************************
// File Name :         ViewDiscardPile.cs
// Author :            Rudy W
// Creation Date :     November 26th, 2022
//
// Brief Description : Script that populates a grid with buttons based on
                       cards that are in the discard pile. These can be
                       pressed and will show that card in a maximized view.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewDiscardPile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _discardedCardButton;

    [Header("Values")]
    [SerializeField] private int _numberToSpawn;

    /// <summary>
    /// Runs the function (Temp)
    /// </summary>
    private void Start()
    {
        DisplayDiscardedCards();
    }

    /// <summary>
    /// Displays discarded cards based on the DPile.
    /// </summary>
    private void DisplayDiscardedCards()
    {
        GameObject newObj;
        for(int i = 0; i < _numberToSpawn; i++)
        {
            newObj = Instantiate(_discardedCardButton, transform);
            newObj.GetComponent<Image>().color = Random.ColorHSV();
        }
    }
}
