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
    [SerializeField] public Card ThisCard;

    [Header("General References")]
    [SerializeField] private Sprite _grassBar;
    [SerializeField] private Sprite _dirtBar;
    [SerializeField] private Sprite _stoneBar;
    [SerializeField] private Sprite _goldBar;
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
        if(ThisCard.GrassSuit)
        {
            _suitIconImage.sprite = _grassIcon;
            _suitImageColor.sprite = _grassBar;
            _suitName.text = "Grass";
        }
        else if(ThisCard.DirtSuit)
        {
            _suitIconImage.sprite = _dirtIcon;
            _suitImageColor.sprite = _dirtBar;
            _suitName.text = "Dirt";
        }
        else if(ThisCard.StoneSuit)
        {
            _suitIconImage.sprite = _stoneIcon;
            _suitImageColor.sprite = _stoneBar;
            _suitName.text = "Stone";
        }
        else if(ThisCard.GoldSuit)
        {
            _suitIconImage.sprite = _goldIcon;
            _suitImageColor.sprite = _goldBar;
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
        _nameText.text = ThisCard.CardName;
        _descriptionText.text = ThisCard.CardDescription;

        //Set a card's cost to 10 or higher to make its cost "X"
        if(ThisCard.GrassCost >= 10)
        {
            _grassCost.text = "X";
        }
        else
        {
            _grassCost.text = ThisCard.GrassCost.ToString();
        }

        if (ThisCard.DirtCost >= 10)
        {
            _dirtCost.text = "X";
        }
        else
        {
            _dirtCost.text = ThisCard.DirtCost.ToString();
        }

        if (ThisCard.StoneCost >= 10)
        {
            _stoneCost.text = "X";
        }
        else
        {
            _stoneCost.text = ThisCard.StoneCost.ToString();
        }

        _cardImage.sprite = ThisCard.CardArt;

        if(ThisCard.persistent)
        {
            _specialCardText.text = "P";
        }
        else if(ThisCard.disaster)
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
