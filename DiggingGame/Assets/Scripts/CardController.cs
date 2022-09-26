/*****************************************************************************
// File Name :         CardController.cs
// Author :            Rudy Wolfer
// Creation Date :     September 20th, 2022
//
// Brief Description : Script that allows a card to be "maximized" and played.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _cardAreaToMaximize;
    [SerializeField] private GameObject _parentObj;

    [Header("Values")]
    [SerializeField] private float _flipRotationDist;

    [Header("Other")]
    [HideInInspector] public int _handPosition;
    private GameManager _gm;
    private Transform _cardMaximizeZone;
    private GameObject _currentCard;
    [HideInInspector] public bool Played;

    /// <summary>
    /// Assigns the anchor point
    /// </summary>
    private void Start()
    {
        _gm = FindObjectOfType<GameManager>();

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
    /// Moves the card to the play area or discard pile
    /// </summary>
    private void OnMouseDown()
    {
        if(!Played)
        {
            //Put the card in the play area
            _parentObj.transform.position = new Vector2(Random.Range(_gm.PlayZoneTL.position.x, _gm.PlayZoneBR.position.x), Random.Range(_gm.PlayZoneBR.position.y, _gm.PlayZoneTL.position.y));
            Played = true;
            //Opens up that hand position again. 
            _gm.OpenHandPositions[_handPosition] = true;
        }
        else
        {
            MoveToDiscardPile();
        }
    }

    /// <summary>
    /// Moves the card to the discard pile
    /// </summary>
    private void MoveToDiscardPile()
    {
        _gm.Discard.Add(_parentObj.gameObject);
        UpdateMaximizedView(_cardAreaToMaximize);
        Debug.Log("Discarded " + _parentObj.name + ".");
        _parentObj.gameObject.SetActive(false);
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
