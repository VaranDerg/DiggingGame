/*****************************************************************************
// File Name :         CreateAndJoinRooms.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 29th, 2022
//
// Brief Description : This document contains the functionality for joining
                       and creating rooms, and what happens when a player has
                       succesfully joined
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    // Where the player enters the name of the room they want to create or join
    [SerializeField] TMP_InputField _createInput, _joinInput;
    
    // Scene players will load into when a room is joined
    [SerializeField] string _joinScene;

    // Text that displays if name is invalid
    [SerializeField] TextMeshProUGUI _invalidName;

    /// <summary>
    /// Sets the invalid text to invis at the start
    /// </summary>
    public void Start()
    {
        _invalidName.gameObject.SetActive(false);
    }

    /// <summary>
    /// Creates a new room with the name of whatever was in the input field.
    /// When players create a room, they also automatically join it.
    /// Author: Andrea SD
    /// </summary>
    public void CreateRoom()
    {
        // Room name must be between 3 and 15 characters
        if (_createInput.text.Length >= 3 && _createInput.text.Length <= 15)
        {
            _invalidName.gameObject.SetActive(false);
            PhotonNetwork.CreateRoom(_createInput.text);
        }
        else
        {
            _invalidName.gameObject.SetActive(true);
        }
    }



    /// <summary>
    /// Joins room using the text in the input field
    /// Author: Andrea SD
    /// </summary>
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_joinInput.text);
    }

    /// <summary>
    /// Automatically called when a room is joined.
    /// Loads scene associated with the room
    /// 
    /// Author: Andrea SD
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(_joinScene);
    }
}
