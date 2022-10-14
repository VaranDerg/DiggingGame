/*****************************************************************************
// File Name :         CardManager.cs
// Author :            Rudy Wolfer, Andrea SD
// Creation Date :     October 10th, 2022
//
// Brief Description : Script managing card/mouse interactivity and Activation.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineCardController : MonoBehaviour
{
    //Edit: Andrea SD - Added online functionality

    [Header("References")]
    [SerializeField] private Transform _mouseOverPos;
    [SerializeField] private Transform _selectedPos;
    [SerializeField] private Transform _defaultPos;
    [SerializeField] private GameObject _cardVisualToMaximize;
    [SerializeField] private GameObject _cardBody;

    [Header("Values")]
    [SerializeField] private float _cardSlideSpeed;

    [Header("Other")]
    private OnlineCardManager _cm;
    private OnlineActionManager _am;
    private OnlineCanvasManager _gcm;
    private OnlineBoardManager _bm;
    [HideInInspector] public int HandPosition;
    [HideInInspector] public int HeldByPlayer;
    private bool _currentlyMaximized = false;
    private GameObject _maximizedCard;
    private Transform _maximizeAnchor;
    [HideInInspector] public Vector3 NextPos;

    [Header("Selection Variables")]
    [HideInInspector] public bool CanBeSelected;
    [HideInInspector] public bool CanBeDiscarded;
    [HideInInspector] public bool CanBeActivated;
    [HideInInspector] public bool Selected;

    /// <summary>
    /// Assigns partner scripts and the maximize anchor.
    /// </summary>
    private void Awake()
    {
        _maximizeAnchor = GameObject.FindGameObjectWithTag("MaximizeAnchor").GetComponent<Transform>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _am = FindObjectOfType<OnlineActionManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        HeldByPlayer = 0;
    }

    /// <summary>
    /// Adjusts the card's visual position.
    /// </summary>
    private void FixedUpdate()
    {
        if(Selected)
        {
            transform.position = _selectedPos.position;
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
                ToDiscard();
            }
        }

        if(CanBeSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectCard();
            }
        }

        if(CanBeActivated)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                ActivateCard();
            }
        }
    }

    private void SelectCard()
    {
        if(!Selected)
        {
            _cm.SelectedCards.Add(_cardBody);
            Selected = true;
        }
        else
        {
            _cm.SelectedCards.Remove(_cardBody);
            Selected = false;
        }
    }

    private void ActivateCard()
    {
        int grassCost = _cardBody.GetComponentInChildren<OnlineCardVisuals>().ThisCard.GrassCost;
        int dirtCost = _cardBody.GetComponentInChildren<OnlineCardVisuals>().ThisCard.DirtCost;
        int stoneCost = _cardBody.GetComponentInChildren<OnlineCardVisuals>().ThisCard.StoneCost;

        if(_cm.AllowedActivations == 0)
        {
            _gcm.UpdateCurrentActionText("You've Activated the max amount of Cards.");
            return;
        }

        if(_am.CurrentPlayer == 1)
        {
            if(_am.P1RefinedPile[0] >= grassCost && _am.P1RefinedPile[1] >= dirtCost && _am.P1RefinedPile[2] >= stoneCost)
            {
                _am.P1RefinedPile[0] -= grassCost;
                _am.P1RefinedPile[1] -= dirtCost;
                _am.P1RefinedPile[2] -= stoneCost;
                _am.SupplyPile[0] += grassCost;
                _am.SupplyPile[1] += dirtCost;
                _am.SupplyPile[2] += stoneCost;

                _gcm.UpdateCurrentActionText("Activated " + _cardBody.name + "!");
                Debug.Log("Activation code will go in this line in the future.");
                _am.P1Score++;
                _cm.AllowedActivations--;
                _gcm.UpdateTextBothPlayers();

                ToDiscard();
            }
            else
            {
                _gcm.UpdateCurrentActionText("Not enough Pieces to Activate this Card!");
            }
        }
        else
        {
            if (_am.P2RefinedPile[0] >= grassCost && _am.P2RefinedPile[1] >= dirtCost && _am.P2RefinedPile[2] >= stoneCost)
            {
                _am.P2RefinedPile[0] -= grassCost;
                _am.P2RefinedPile[1] -= dirtCost;
                _am.P2RefinedPile[2] -= stoneCost;
                _am.SupplyPile[0] += grassCost;
                _am.SupplyPile[1] += dirtCost;
                _am.SupplyPile[2] += stoneCost;

                _gcm.UpdateCurrentActionText("Activated " + _cardBody.name + "!");
                Debug.Log("Activation code will go in this line in the future.");
                _am.P2Score++;
                _cm.AllowedActivations--;
                _gcm.UpdateTextBothPlayers();

                ToDiscard();
            }
            else
            {
                _gcm.UpdateCurrentActionText("Not enough Pieces to Activate this card!");
            }
        }
    }

    public void ToDiscard()
    {
        if(HeldByPlayer == 1)
        {
            if (_cardBody.CompareTag("Card"))
            {
                _am.P1Cards--;
            }
            else if (_cardBody.CompareTag("GoldCard"))
            {
                _am.P1GoldCards--;
            }
            _cm.P1OpenHandPositions[HandPosition] = true;
            _cm.P1Hand.Remove(_cardBody);
        }
        else if(HeldByPlayer == 2)
        {
            if (_cardBody.CompareTag("Card"))
            {
                _am.P2Cards--;
            }
            else if (_cardBody.CompareTag("GoldCard"))
            {
                _am.P2GoldCards--;
            }
            _cm.P2OpenHandPositions[HandPosition] = true;
            _cm.P2Hand.Remove(_cardBody);
        }
        HeldByPlayer = 0;
        Selected = false;
        CanBeSelected = false;
        CanBeDiscarded = false;
        CanBeActivated = false;
        _cm.DPile.Add(_cardBody);
        _cm.UpdatePileText();

        if (_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
        _cardBody.SetActive(false);
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
