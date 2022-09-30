/*****************************************************************************
// File Name :         ConnectToServer.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 29th, 2022
//
// Brief Description : This document controls the connecting and joining 
                       process for joining a Photon server.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacks gives access to the callback functions which
// automatically happens when specific events occur.
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //Connects to photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Callback function to check when successfully connected to server.
    /// The player then joins the lobby.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // Allows for joining and creating rooms later on
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Loads lobby scene once connected
    /// </summary>
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
