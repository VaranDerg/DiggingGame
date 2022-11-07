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
    public static StatManager s_Instance;
    private void Awake()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
        }

        DontDestroyOnLoad(s_Instance);
    }

    [Header("Statistics")]
    public static int s_RoundAmount; //The amount of rounds the game has gone through.
    public static int s_P1FinalScore, s_P2FinalScore; //Final scores.
    public static int s_P1BuildingTotal, s_P2BuildingTotal; //Amount of Built Buildings.
    public static int s_P1ActivationTotal, s_P2ActivationTotal; //Amount of Card Activations
    public static int s_P1DestroyTotal, s_P2DestroyTotal; //Amount of Destroyed Buildings
    public static int s_P1StealTotal, s_P2StealTotal; //Amount of Stolen Pieces
    public static int s_P1PlaceTotal, s_P2PlaceTotal; //Amount of Placed Pieces
    public static int s_P1RetrieveTotal, s_P2RetrieveTotal; //Amount of Gold Retrieved
    public static int s_P1DigTotal, s_P2DigTotal; //Amount of Dug Pieces
    public static int s_P1CardSpendTotal, s_P2CardSpendTotal; //Amount of Discarded Cards

    /// <summary>
    /// Sets every Stat to 0.
    /// </summary>
    public void ResetStatistics()
    {
        s_RoundAmount = 0;
        s_P1FinalScore = 0;
        s_P2FinalScore = 0;
        s_P1BuildingTotal = 0;
        s_P2BuildingTotal = 0;
        s_P1ActivationTotal = 0;
        s_P2ActivationTotal = 0;
        s_P1DestroyTotal = 0;
        s_P2DestroyTotal = 0;
        s_P1StealTotal = 0;
        s_P2StealTotal = 0;
        s_P1PlaceTotal = 0;
        s_P2PlaceTotal = 0;
        s_P1RetrieveTotal = 0;
        s_P2RetrieveTotal = 0;
        s_P1DigTotal = 0;
        s_P2DigTotal = 0;
        s_P1CardSpendTotal = 0;
        s_P2CardSpendTotal = 0;
    }

    /// <summary>
    /// Increases a given player's given statistic. 
    /// </summary>
    /// <param name="player">1 or 2</param>
    /// <param name="amount">Amount to Increase by</param>
    /// <param name="statName">Round, Score, Building, Activation, Destroy, Steal, Place, Retrieve, Dig, Card</param>
    public void IncreaseStatistic(int player, string statName, int amount)
    {
        if(statName == "Round")
        {
            s_RoundAmount += amount;
            return;
        }

        if(player == 1)
        {
            switch(statName)
            {
                case "Score":
                    s_P1FinalScore += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1FinalScore - amount) + " to " + s_P1FinalScore + ".");
                    break;
                case "Building":
                    s_P1BuildingTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1BuildingTotal - amount) + " to " + s_P1BuildingTotal + ".");
                    break;
                case "Activation":
                    s_P1ActivationTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1ActivationTotal - amount) + " to " + s_P1ActivationTotal + ".");
                    break;
                case "Destroy":
                    s_P1DestroyTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1DestroyTotal - amount) + " to " + s_P1DestroyTotal + ".");
                    break;
                case "Steal":
                    s_P1StealTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1StealTotal - amount) + " to " + s_P1StealTotal + ".");
                    break;
                case "Place":
                    s_P1PlaceTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1PlaceTotal - amount) + " to " + s_P1PlaceTotal + ".");
                    break;
                case "Retrieve":
                    s_P1RetrieveTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1RetrieveTotal - amount) + " to " + s_P1RetrieveTotal + ".");
                    break;
                case "Dig":
                    s_P1DigTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1DigTotal - amount) + " to " + s_P1DigTotal + ".");
                    break;
                case "Card":
                    s_P1CardSpendTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P1CardSpendTotal - amount) + " to " + s_P1CardSpendTotal + ".");
                    break;
            }
        }
        else
        {
            switch (statName)
            {
                case "Score":
                    s_P2FinalScore += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2FinalScore - amount) + " to " + s_P2FinalScore + ".");
                    break;
                case "Building":
                    s_P2BuildingTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2BuildingTotal - amount) + " to " + s_P2BuildingTotal + ".");
                    break;
                case "Activation":
                    s_P2ActivationTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2ActivationTotal - amount) + " to " + s_P2ActivationTotal + ".");
                    break;
                case "Destroy":
                    s_P2DestroyTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2DestroyTotal - amount) + " to " + s_P2DestroyTotal + ".");
                    break;
                case "Steal":
                    s_P2StealTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2StealTotal - amount) + " to " + s_P2StealTotal + ".");
                    break;
                case "Place":
                    s_P2PlaceTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2PlaceTotal - amount) + " to " + s_P2PlaceTotal + ".");
                    break;
                case "Retrieve":
                    s_P2RetrieveTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2RetrieveTotal - amount) + " to " + s_P2RetrieveTotal + ".");
                    break;
                case "Dig":
                    s_P2DigTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2DigTotal - amount) + " to " + s_P2DigTotal + ".");
                    break;
                case "Card":
                    s_P2CardSpendTotal += amount;
                    Debug.Log("Player " + player + "'s " + statName + " gone from " + (s_P2CardSpendTotal - amount) + " to " + s_P2CardSpendTotal + ".");
                    break;
            }
        }
    }
}
