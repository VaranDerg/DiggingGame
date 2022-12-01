/*****************************************************************************
// File Name :         StatManager.cs
// Author :            Rudy Wolfer, Andrea Swihart-DeCoster
// Creation Date :     November 6th, 2022
//
// Brief Description : A script to hold player stats for Results screen display
                       purposes.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StatManager : MonoBehaviourPun
{
    /// <summary>
    /// Nondestructable object.
    /// </summary>
    public static StatManager s_Instance;
    private void Awake()
    {
        if (s_Instance == null)
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

    private bool _isOnline = false;

    /// <summary>
    /// Sets every Stat to 0.
    /// </summary>
    public void ResetStatistics()
    {
        if (_isOnline)
        {
            CallReset();
        }
        else
        {
            ResetValues();
        }
        _isOnline = false;
    }

    /// <summary>
    /// Increases a given player's given statistic. 
    /// </summary>
    /// <param name="player">1 or 2</param>
    /// <param name="amount">Amount to Increase by</param>
    /// <param name="statName">Round, Score, Building, Activation, Destroy, Steal, Place, Retrieve, Dig, Card</param>
    public void IncreaseStatistic(int player, string statName, int amount)
    {
        if (_isOnline)
        {
            CallModifyValue(player, statName, amount);
        }
        else
        {
            ModifyValue(player, statName, amount);
        }
    }

    /// <summary>
    /// Resets values of the statistics
    /// 
    /// Author: Andrea SD
    /// </summary>
    private void ResetValues()
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
    /// Modifies the value of a stat
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="statName"> stat being modified </param>
    /// <param name="amount"> amount stat is modified by </param>
    private void ModifyValue(int player, string statName, int amount)
    {
        if (statName == "Round")
        {
            s_RoundAmount += amount;
            return;
        }

        if (player == 1)
        {
            switch (statName)
            {
                case "Score":
                    s_P1FinalScore += amount;
                    break;
                case "Building":
                    s_P1BuildingTotal += amount;
                    break;
                case "Activation":
                    s_P1ActivationTotal += amount;
                    break;
                case "Destroy":
                    s_P1DestroyTotal += amount;
                    break;
                case "Steal":
                    s_P1StealTotal += amount;
                    break;
                case "Place":
                    s_P1PlaceTotal += amount;
                    break;
                case "Retrieve":
                    s_P1RetrieveTotal += amount;
                    break;
                case "Dig":
                    s_P1DigTotal += amount;
                    break;
                case "Card":
                    s_P1CardSpendTotal += amount;
                    break;
            }
        }
        else
        {
            switch (statName)
            {
                case "Score":
                    s_P2FinalScore += amount;
                    break;
                case "Building":
                    s_P2BuildingTotal += amount;
                    break;
                case "Activation":
                    s_P2ActivationTotal += amount;
                    break;
                case "Destroy":
                    s_P2DestroyTotal += amount;
                    break;
                case "Steal":
                    s_P2StealTotal += amount;
                    break;
                case "Place":
                    s_P2PlaceTotal += amount;
                    break;
                case "Retrieve":
                    s_P2RetrieveTotal += amount;
                    break;
                case "Dig":
                    s_P2DigTotal += amount;
                    break;
                case "Card":
                    s_P2CardSpendTotal += amount;
                    break;
            }
        }
    }


    #region RPC Functions
    
    /// <summary>
    /// Calls the RPC that sets the _isOnline variable
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="state"> T/F if online or not </param>
    public void CallIsOnline(bool state)
    {
        photonView.RPC("SetIsOnline", RpcTarget.All, state);
    }

    /// <summary>
    /// Sets isOnline to true
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="state"> T/F if online or not </param>
    [PunRPC]
    public void SetIsOnline(bool state)  {  _isOnline = state; }

    /// <summary>
    /// Calls the RPC that increases statistics
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="statName"> stat being modified </param>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the stat is being increased by </param>
    private void CallModifyValue(int player, string statName, int amount)
    {
        photonView.RPC("ModifyValueONL", RpcTarget.All, statName, player, amount);
    }

    /// <summary>
    /// Increases statistics
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="statName"> stat being modified </param>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the stat is being increased by </param>
    [PunRPC]
    public void ModifyValueONL(int player, string statName, int amount)
    {
        ModifyValue(player, statName, amount);
    }

    /// <summary>
    /// Calls the RPC that resets statistic values
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void CallReset()
    {
        photonView.RPC("ResetStatisticsONL", RpcTarget.All);
    }

    /// <summary>
    /// Resets statistic values
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void ResetStatisticsONL()
    {
        ResetValues();
    }
    #endregion
}
