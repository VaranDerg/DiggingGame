/*****************************************************************************
// File Name :         CardController.cs
// Author :            Rudy Wolfer
// Creation Date :     September 20th, 2022
//
// Brief Description : Script that allows a card to be "maximized" and flipped.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _cardAreaToMaximize;

    [Header("Values")]
    [SerializeField] private float _flipRotationDist;

    [Header("Other")]
    private Transform _cardMaximizeZone;
    private GameObject _currentCard;

    /// <summary>
    /// Assigns the anchor point
    /// </summary>
    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("MaximizeAnchor"))
        {
            _cardMaximizeZone = GameObject.FindGameObjectWithTag("MaximizeAnchor").transform;
        }
        else
        {
            Debug.LogWarning("No card anchor found, cannot maximize for larger view. Please assign the tag 'MaximizeAnchor' to a GameObject to proceed.");
        }
    }

    /// <summary>
    /// Maximizes the card the mouse is over
    /// </summary>
    private void OnMouseEnter()
    {
        UpdateMaximizedView(_cardAreaToMaximize);
    }

    /// <summary>
    /// Removes the card once the mouse leaves
    /// </summary>
    private void OnMouseExit()
    {
        UpdateMaximizedView(null);
    }

    /// <summary>
    /// Updates the card
    /// </summary>
    /// <param name="setMaximized">Card part to update. Generally the part without interactivity.</param>
    private void UpdateMaximizedView(GameObject setMaximized)
    {
        if(_currentCard != null)
        {
            Destroy(_currentCard.gameObject);
            return;
        }

        _currentCard = Instantiate(setMaximized, _cardMaximizeZone);
    }
}
