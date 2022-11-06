/*****************************************************************************
// File Name :         StatManager.cs
// Author :            Rudy Wolfer
// Creation Date :     November 6th, 2022
//
// Brief Description : A script to hold player stats for Results screen display
                       purposes.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    /// <summary>
    /// Nondestructable object.
    /// </summary>
    public static StatManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(instance);
    }

    [Header("Statistics")]
    public static int RoundAmount;
    public static int Player1FinalScore, Player2FinalScore;
    public static int Player1BuildingTotal, Player2BuildingTotal;
    public static int Player1ActivationTotal, Player2ActivationTotal;
    public static int Player1DestroyedTotal, Player2DestroyedTotal;
    public static int Player1StealTotal, Player2StealTotal;
    public static int Player1PlaceTotal, Player2PlaceTotal;
    public static int Player1RetrieveTotal, Player2RetrieveTotal;
    public static int Player1DigTotal, Player2DigTotal;
    public static int Player1CardSpendTotal, Player2CardSpendTotal;

    /// <summary>
    /// Sets every Stat to 0.
    /// </summary>
    public void ResetStatistics()
    {
        RoundAmount = 0;
        Player1FinalScore = 0;
        Player2FinalScore = 0;
        Player1BuildingTotal = 0;
        Player2BuildingTotal = 0;
        Player1ActivationTotal = 0;
        Player2ActivationTotal = 0;
        Player1DestroyedTotal = 0;
        Player2DestroyedTotal = 0;
        Player1StealTotal = 0;
        Player2StealTotal = 0;
        Player1PlaceTotal = 0;
        Player2PlaceTotal = 0;
        Player1RetrieveTotal = 0;
        Player2RetrieveTotal = 0;
        Player1DigTotal = 0;
        Player2DigTotal = 0;
        Player1CardSpendTotal = 0;
        Player2CardSpendTotal = 0;
    }
}
