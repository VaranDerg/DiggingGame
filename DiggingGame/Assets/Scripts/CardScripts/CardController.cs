/*****************************************************************************
// File Name :         CardManager.cs
// Author :            Rudy Wolfer
// Creation Date :     October 10th, 2022
//
// Brief Description : Script managing card/mouse interactivity and Activation.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _anims;
    [SerializeField] private GameObject _cardParent;

    [Header("Other")]
    private CardManager _cm;
    private ActionManager _am;
    private GameCanvasManagerNew _gcm;
    private BoardManager _bm;
    [HideInInspector] public int HandPosition;
    [HideInInspector] public int HeldByPlayer;
    private bool _currentlyMaximized = false;
    private GameObject _maximizedCard;
    private Transform _maximizeAnchor;

    /// <summary>
    /// Assigns partner scripts and the maximize anchor.
    /// </summary>
    private void Awake()
    {
        _maximizeAnchor = GameObject.FindGameObjectWithTag("MaximizeAnchor").GetComponent<Transform>();
        _cm = FindObjectOfType<CardManager>();
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
    }

    /// <summary>
    /// Effects that occur when the mouse hovers over a card.
    /// </summary>
    private void OnMouseEnter()
    {
        _anims.Play("CardSlideUp");
    }

    /// <summary>
    /// Effects that occur when the mouse leaves a card.
    /// </summary>
    private void OnMouseExit()
    {
        _anims.Play("CardSlideDown");

        if(_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
    }

    private void OnMouseOver()
    {
        MaximizeCard(_cardParent);

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpendCard();
        }
    }

    private void SpendCard()
    {
        _cm.DPile.Add(_cardParent);
        if (_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
        _cardParent.SetActive(false);
        Debug.Log("Spent " + _cardParent.name + ".");
    }

    private void MaximizeCard(GameObject thingToMaximize)
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_currentlyMaximized)
            {
                return;
            }

            _maximizedCard = Instantiate(thingToMaximize, _maximizeAnchor);
            _maximizedCard.transform.position = _maximizeAnchor.transform.position;
            _currentlyMaximized = true;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(!_currentlyMaximized)
            {
                return;
            }

            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
    }
}
