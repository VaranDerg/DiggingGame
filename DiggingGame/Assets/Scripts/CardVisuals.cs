/*****************************************************************************
// File Name :         CardVisuals.cs
// Author :            Rudy Wolfer
// Creation Date :     September 21st, 2022
//
// Brief Description : Script that manages a card's appearance.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardVisuals : MonoBehaviour
{
    [Header("Card Scriptable Object")]
    [SerializeField] private Card _thisCard;

    [Header("General References")]
    [SerializeField] private Color _grassColor;
    [SerializeField] private Color _dirtColor;
    [SerializeField] private Color _stoneColor;
    [SerializeField] private Color _goldColor;
    [SerializeField] private Sprite _grassIcon, _dirtIcon, _stoneIcon, _goldIcon;

    [Header("Specific References")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _grassCost, _dirtCost, _stoneCost;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _suitIconImage;
    [SerializeField] private Image _suitImageColor;
    [SerializeField] private TextMeshProUGUI _suitName;

    private void PrepareCardSuit()
    {
        if(_thisCard.GrassSuit)
        {
            _suitIconImage.sprite = _grassIcon;
            _suitImageColor.color = _grassColor;
            _suitName.text = "Grass";
        }
        else if(_thisCard.DirtSuit)
        {
            _suitIconImage.sprite = _dirtIcon;
            _suitImageColor.color = _dirtColor;
            _suitName.text = "Dirt";
        }
        else if(_thisCard.StoneSuit)
        {
            _suitIconImage.sprite = _stoneIcon;
            _suitImageColor.color = _stoneColor;
            _suitName.text = "Stone";
        }
        else if(_thisCard.GoldSuit)
        {
            _suitIconImage.sprite = _goldIcon;
            _suitImageColor.color = _goldColor;
            _suitName.text = "Gold";
        }
        else
        {
            Debug.LogWarning("No suit selected for card " + gameObject.transform.parent.name + ". Please select a suit in the inspector.");
        }
    }

    private void PrepareCardValues()
    {
        _nameText.text = _thisCard.CardName;
        _descriptionText.text = _thisCard.CardDescription;
        _grassCost.text = _thisCard.GrassCost.ToString();
        _dirtCost.text = _thisCard.DirtCost.ToString();
        _stoneCost.text = _thisCard.StoneCost.ToString();
        _cardImage.sprite = _thisCard.CardArt;

    }

    private void Start()
    {
        PrepareCardSuit();
        PrepareCardValues();
    }
}
