/*****************************************************************************
// File Name :         WaitingLobby.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     November 29th, 2022
//
// Brief Description : This document controls the waiting lobby.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.UI;

public class WaitingLobby : MonoBehaviourPun
{
    // Start game button and info text for p2
    [SerializeField] GameObject _sceneButton;
    [SerializeField] GameObject _waitingText;

    private bool _hasTwoPlayers = false;    // if lobby has 2 players

    [SerializeField] string _joinScene;     // Scene to be loaded

    /// <summary>
    /// Checking for two players in the room
    /// </summary>
    private void Update()
    {
        if (!_hasTwoPlayers)
        {
            CheckForPlayers();
        }
    }

    /// <summary>
    /// If the room has 2 players, the MasterClient(P1) will be able to start
    /// the game and p2 will see that they are waiting for P1 to start the games
    /// </summary>
    public void CheckForPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            _hasTwoPlayers = true;
            if (PhotonNetwork.IsMasterClient)
            {
                _sceneButton.SetActive(true);
                CallText();
            }
        }
    }

    /// <summary>
    /// Calls the RPC that enables the waiting text for P2
    /// </summary>
    public void CallText()
    {
        photonView.RPC("EnableText", RpcTarget.OthersBuffered);
    }

    /// <summary>
    /// Enables the waiting text for P2
    /// </summary>
    [PunRPC]
    public void EnableText()
    { 
        _waitingText.SetActive(true);
    }

    /// <summary>
    /// Calls the RPC that enables the starts the game
    /// </summary>
    public void CallStartGame()
    {
        // photonView.RPC("StartGame", RpcTarget.All);
       // PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(_joinScene);
    }

    /// <summary>
    /// Loads the online scene
    /// </summary>
    [PunRPC]
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(_joinScene);
    }
}
