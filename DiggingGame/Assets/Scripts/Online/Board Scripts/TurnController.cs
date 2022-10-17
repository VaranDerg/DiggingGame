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

public class TurnController : MonoBehaviourPun
{
    //stores which players turn it is. Master Client is P1, other client is P2
    public int CurrentTurn;

    /// <summary>
    /// Awake is called before the application starts.
    /// Author: Andrea SD
    /// </summary>
    private void Awake()
    {
        // P1 always goes first.
        CurrentTurn = 1;
    }

    public void ChangeTurn(int nextPlayer)
    {
        CurrentTurn = nextPlayer;
    }
}
