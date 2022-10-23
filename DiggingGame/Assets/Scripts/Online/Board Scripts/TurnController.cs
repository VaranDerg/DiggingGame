/*****************************************************************************
// File Name :         TurnController.cs
// Author :            Andrea Swihart-DeCoster.
// Creation Date :     October 17th, 2022
//
// Brief Description : This document controls the turn cycle across clients.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class TurnController : MonoBehaviourPun
{
    //stores which players turn it is. Master Client is P1, other client is P2
    [HideInInspector]public static int CurrentTurn;

    /// <summary>
    /// Awake is called before the application starts.
    /// Author: Andrea SD
    /// </summary>
    private void Awake()
    {
        // P1 always goes first.
        CurrentTurn = 1;
    }

    [PunRPC]
    /// <summary>
    /// Changes the turn to the next player.
    /// </summary>
    /// <param name="nextPlayer"> Player who's turn it's changing to </param>
    public void ChangeTurn(int nextPlayer)
    {
        CurrentTurn = nextPlayer;
    }

    /// <summary>
    /// Enables the scene for player 1 (Master client) to use
    /// </summary>
    [PunRPC]
    public void StartPlayer1()
    {
        
    }

    /// <summary>
    /// Enables the scene for player 2 (non-master client) to use
    /// </summary>
    [PunRPC]
    public void StartPlayer2()
    {
       
    }

    /// <summary>
    /// Disables the scene for player 1 (Master client) in prep for p2 turn
    /// </summary>
    [PunRPC]
    public void EndPlayer1()
    {

    }

    /// <summary>
    /// Disables the scene for player 2 (non-master client) in prep for p1 turn
    /// </summary>
    [PunRPC]
    public void EndPlayer2()
    {

    }
}
