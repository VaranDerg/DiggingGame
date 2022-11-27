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
    [SerializeField] private CanvasGroup _discardPileViewCGroup;
    [SerializeField] private GameObject _discardedCardButtonPrefab;
    [SerializeField] private GameObject _maximizedDiscardedCard;
    [SerializeField] private GameObject _backButton;

    [Header("Values")]
    [SerializeField] private Color _grassCardColor;
    [SerializeField] private Color _dirtCardColor;
    [SerializeField] private Color _stoneCardColor;
    [SerializeField] private Color _goldCardColor;
    private bool _viewShowing = false;
    private List<GameObject> _cardButtons = new List<GameObject>();

    [Header("Partner Scripts")]
    private CardManager _cm;

    /// <summary>
    /// Assigns CM
    /// </summary>
    private void Awake()
    {
        _cm = FindObjectOfType<CardManager>();
    }

    /// <summary>
    /// Makes the DPile View Invisible
    /// </summary>
    private void Start()
    {
        HideDiscardView();
    }

    /// <summary>
    /// Displays discarded cards based on the DPile.
    /// </summary>
    private void DisplayDiscardedCards()
    {
        foreach(GameObject cardButton in _cardButtons)
        {
            Destroy(cardButton);
        }

        GameObject newCard;

        for (int i = 0; i < _cm.DPile.Count; i++)
        {
            newCard = Instantiate(_discardedCardButtonPrefab, transform);
            newCard.GetComponent<DiscardedCardButton>().DiscardedCard = _cm.DPile[i].GetComponentInChildren<CardVisuals>().ThisCard;

            if (_cm.DPile[i].GetComponentInChildren<CardVisuals>().ThisCard.GrassSuit)
            {
                newCard.GetComponent<DiscardedCardButton>().PrepareButton(_grassCardColor);
            }
            else if (_cm.DPile[i].GetComponentInChildren<CardVisuals>().ThisCard.DirtSuit)
            {
                newCard.GetComponent<DiscardedCardButton>().PrepareButton(_dirtCardColor);
            }
            else if (_cm.DPile[i].GetComponentInChildren<CardVisuals>().ThisCard.StoneSuit)
            {
                newCard.GetComponent<DiscardedCardButton>().PrepareButton(_stoneCardColor);
            }
            else if(_cm.DPile[i].GetComponentInChildren<CardVisuals>().ThisCard.GoldSuit)
            {
                newCard.GetComponent<DiscardedCardButton>().PrepareButton(_goldCardColor);
            }

            _cardButtons.Add(newCard);
        }
    }

    /// <summary>
    /// Displays the actual card.
    /// </summary>
    /// <param name="cardToShow">Card Scriptable Obj, from button</param>
    public void DisplaySpecificCard(Card cardToShow)
    {
        ToggleDiscardView();

        _maximizedDiscardedCard.SetActive(true);
        _backButton.SetActive(true);

        _maximizedDiscardedCard.GetComponent<CardVisuals>().ThisCard = cardToShow;
        _maximizedDiscardedCard.GetComponent<CardVisuals>().PrepareCardSuit();
        _maximizedDiscardedCard.GetComponent<CardVisuals>().PrepareCardValues();
    }

    /// <summary>
    /// Toggles the discard view.
    /// </summary>
    public void ToggleDiscardView()
    {
        if(_viewShowing)
        {
            HideDiscardView();
        }
        else
        {
            ShowDiscardView();
        }
    }

    /// <summary>
    /// Shows the discard view.
    /// </summary>
    public void ShowDiscardView()
    {
        _viewShowing = true;

        _maximizedDiscardedCard.SetActive(false);
        _backButton.SetActive(false);
        DisplayDiscardedCards();

        _discardPileViewCGroup.blocksRaycasts = true;
        _discardPileViewCGroup.interactable = true;
        _discardPileViewCGroup.alpha = 1;
    }

    /// <summary>
    /// Hides the discard view.
    /// </summary>
    public void HideDiscardView()
    {
        _maximizedDiscardedCard.SetActive(false);
        _backButton.SetActive(false);

        _viewShowing = false;
        _discardPileViewCGroup.interactable = false;
        _discardPileViewCGroup.blocksRaycasts = false;
        _discardPileViewCGroup.alpha = 0;
    }
}
