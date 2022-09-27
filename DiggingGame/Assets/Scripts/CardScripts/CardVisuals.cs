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
    [SerializeField] private TextMeshProUGUI _specialCardText;
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _suitIconImage;
    [SerializeField] private Image _suitImageColor;
    [SerializeField] private TextMeshProUGUI _suitName;

    /// <summary>
    /// Prepares the suit of the card. Does this automatically so all you have to do in the inspector is check a box.
    /// </summary>
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

    /// <summary>
    /// Prepares other defined values in the card's scriptable object.
    /// </summary>
    private void PrepareCardValues()
    {
        _nameText.text = _thisCard.CardName;
        _descriptionText.text = _thisCard.CardDescription;

        //Set a card's cost to 10 or higher to make its cost "X"
        if(_thisCard.GrassCost >= 10)
        {
            _grassCost.text = "X";
        }
        else
        {
            _grassCost.text = _thisCard.GrassCost.ToString();
        }

        if (_thisCard.DirtCost >= 10)
        {
            _grassCost.text = "X";
        }
        else
        {
            _dirtCost.text = _thisCard.DirtCost.ToString();
        }

        if (_thisCard.StoneCost >= 10)
        {
            _grassCost.text = "X";
        }
        else
        {
            _stoneCost.text = _thisCard.StoneCost.ToString();
        }

        _cardImage.sprite = _thisCard.CardArt;

        if(_thisCard.persistent)
        {
            _specialCardText.text = "P";
        }
        else if(_thisCard.disaster)
        {
            _specialCardText.text = "D";
        }
        else
        {
            _specialCardText.text = "";
        }
    }

    /// <summary>
    /// Runs both "Prepare" methods. 
    /// </summary>
    private void Start()
    {
        PrepareCardSuit();
        PrepareCardValues();
    }
}
