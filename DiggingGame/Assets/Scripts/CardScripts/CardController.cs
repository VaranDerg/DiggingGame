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
    [SerializeField] private Transform _mouseOverPos;
    [SerializeField] private Transform _defaultPos;
    [SerializeField] private GameObject _cardVisualToMaximize;

    [Header("Values")]
    [SerializeField] private float _cardSlideSpeed;

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
    [HideInInspector] public Vector3 NextPos;

    [Header("Selection Variables")]
    [HideInInspector] public bool CanBeSelected;
    [HideInInspector] public bool CanBeDiscarded;
    [HideInInspector] public bool Selected;

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
        HeldByPlayer = 0;
    }

    /// <summary>
    /// Adjusts the card's visual position.
    /// </summary>
    private void FixedUpdate()
    {
        if(Selected)
        {
            transform.position = _mouseOverPos.position;
            return;
        }

        if(transform.position != NextPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, NextPos, _cardSlideSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Effects that occur when the mouse hovers over a card.
    /// </summary>
    private void OnMouseEnter()
    {
        NextPos = _mouseOverPos.position;
    }

    /// <summary>
    /// Effects that occur when the mouse leaves a card.
    /// </summary>
    private void OnMouseExit()
    {
        NextPos = _defaultPos.position;
        if(_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
    }

    private void OnMouseOver()
    {
        MaximizeCard(_cardVisualToMaximize);

        if(CanBeDiscarded)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SpendCard();
            }
        }

        if(CanBeSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectCard();
            }
        }
    }

    private void SelectCard()
    {
        if(!Selected)
        {
            _cm.SelectedCards.Add(gameObject);
            Selected = true;
        }
        else
        {
            _cm.SelectedCards.Remove(gameObject);
            Selected = false;
        }
    }

    public void SpendCard()
    {
        _cm.DPile.Add(gameObject);

        if(HeldByPlayer == 1)
        {
            if (gameObject.CompareTag("Card"))
            {
                _am.P1Cards--;
            }
            else if (gameObject.CompareTag("GoldCard"))
            {
                _am.P1GoldCards--;
            }
            _cm.P1Hand.Remove(gameObject);
        }
        else if(HeldByPlayer == 2)
        {
            if (gameObject.CompareTag("Card"))
            {
                _am.P2Cards--;
            }
            else if (gameObject.CompareTag("GoldCard"))
            {
                _am.P2GoldCards--;
            }
            _cm.P2Hand.Remove(gameObject);
        }
        HeldByPlayer = 0;
        Selected = false;

        if (_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
        gameObject.SetActive(false);

        Debug.Log("Spent " + gameObject.transform.parent.name + ".");
    }

    /// <summary>
    /// Maximizes a card for easier view.
    /// </summary>
    /// <param name="thingToMaximize">Card zone to maximize</param>
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
