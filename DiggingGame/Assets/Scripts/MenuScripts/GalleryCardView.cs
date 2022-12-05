/*****************************************************************************
// File Name :         GalleryCardView.cs
// Author :            Rudy Wolfer
// Creation Date :     November 8th, 2022
//
// Brief Description : Partner script for GalleryMenu for card displays.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryCardView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Card> _cards = new List<Card>();
    [SerializeField] private GameObject _galleryCard;
    [SerializeField] private TextMeshProUGUI _cardNameText, _cardDescText, _moleQuoteText, _meerkatQuoteText;
    [SerializeField] private GameObject _cardViewPage;
    [SerializeField] private GameObject _otherGalleryThings;

    private void Start()
    {
        _galleryCard.SetActive(false);
        _cardViewPage.SetActive(false);
    }

    /// <summary>
    /// Displays a Card in the card gallery based on provided name.
    /// </summary>
    /// <param name="cardName">Name of card. Case sensitive.</param>
    public void DisplayCard(string cardName)
    {
        foreach(Card card in _cards)
        {
            if (card.CardName == cardName)
            {
                _galleryCard.GetComponent<CardVisuals>().ThisCard = card;
                _galleryCard.GetComponent<CardVisuals>().PrepareCardSuit();
                _galleryCard.GetComponent<CardVisuals>().PrepareCardValues();

                _cardNameText.text = card.CardName;
                _cardDescText.text = card.FlavorText;
                _moleQuoteText.text = card.MoleQuote;
                _meerkatQuoteText.text = card.MeerkatQuote;

                _galleryCard.SetActive(true);
                _cardViewPage.SetActive(true);
                _otherGalleryThings.SetActive(false);
                FindObjectOfType<GalleryMenu>().GalleryPageShowHide(false);
                return;
            }
        }
    }

    public void BackToGallery()
    {
        FindObjectOfType<GalleryMenu>().GalleryPageShowHide(true);
        _galleryCard.SetActive(false);
        _cardViewPage.SetActive(false);
        _otherGalleryThings.SetActive(true);
    }
}
