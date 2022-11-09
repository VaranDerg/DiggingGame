/*****************************************************************************
// File Name :         Card.cs
// Author :            Rudy Wolfer
// Creation Date :     September 21st, 2022
//
// Brief Description : Scriptable Object that manages a card's values. 
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string CardName;
    [TextArea(2, 4)]
    public string CardDescription;
    public bool GrassSuit;
    public bool DirtSuit;
    public bool StoneSuit;
    public bool GoldSuit;

    public int GrassCost;
    public int DirtCost;
    public int StoneCost;

    public bool persistent;
    public bool disaster;

    [TextArea(2, 4)]
    public string MoleQuote;
    [TextArea(2, 4)]
    public string MeerkatQuote;

    public Sprite CardArt;

    public WeatherState.Weather ChangeWeatherTo;
}
