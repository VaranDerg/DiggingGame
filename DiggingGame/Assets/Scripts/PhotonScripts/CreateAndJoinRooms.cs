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

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    // Where the player enters the name of the room they want to create or join
    [SerializeField] TMP_InputField createInput, joinInput;

    /// <summary>
    /// Creates a new room with the name of whatever was in the input field.
    /// When players create a room, they also automatically join it.
    /// Author: Andrea SD
    /// </summary>
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    /// <summary>
    /// Joins room using the text in the input field
    /// Author: Andrea SD
    /// </summary>
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    /// <summary>
    /// Automatically called when a room is joined.
    /// Loads scene associated with the room
    /// Author: Andrea SD
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("PrototypeScene");
    }
}
