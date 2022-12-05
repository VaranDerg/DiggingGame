/*****************************************************************************
// File Name :         DiscardedCardButton.cs
// Author :            Rudy W
// Creation Date :     November 26th, 2022
//
// Brief Description : Very basic script for a discarded card button. Stores
                       a scriptable card object for display purposes.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardedCardButton : MonoBehaviour
{
    [Header("Card Stuff")]
    [HideInInspector] public Card DiscardedCard;
    [SerializeField] private Image _thisCardArt;
    private ViewDiscardPile _vdp;

    /// <summary>
    /// Assigns VDP
    /// </summary>
    private void Start()
    {
        _vdp = FindObjectOfType<ViewDiscardPile>();
        GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<SFXManager>().PlayButtonSound(); });
    }

    /// <summary>
    /// Prepares the button visually.
    /// </summary>
    /// <param name="buttonColor">Passed in thru vdp</param>
    public void PrepareButton(Color buttonColor)
    {
        _thisCardArt.sprite = DiscardedCard.CardArt;
        GetComponent<Image>().color = buttonColor;
    }

    /// <summary>
    /// Calls VDP's Display method
    /// </summary>
    public void OpenCardView()
    {
        _vdp.DisplaySpecificCard(DiscardedCard);
    }
}
