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

    // Text that displays if name is invalid / joining or creating a room fails
    [SerializeField] TextMeshProUGUI _invalidName;
    [SerializeField] TextMeshProUGUI _createRoomFailed;
    [SerializeField] TextMeshProUGUI _joinRoomFailed;
    [SerializeField] TextMeshProUGUI _connecting;

    // Create and join buttons
    [SerializeField] Button _createButton;
    [SerializeField] Button _joinButton;

    /// <summary>
    /// Sets the invalid text to invis at the start
    /// </summary>
    public void Start()
    {
        DisableAllInfoTxt();
    }

    /// <summary>
    /// Creates a new room with the name of whatever was in the input field.
    /// When players create a room, they also automatically join it.
    /// Author: Andrea SD
    /// </summary>
    public void CreateRoom()
    {
        DisableAllInfoTxt();
        // Room name must be between 3 and 15 characters
        if (_createInput.text.Length >= 3 && _createInput.text.Length <= 15)
        {
            _invalidName.gameObject.SetActive(false);
            _connecting.gameObject.SetActive(true);
            SetInteractability(false);
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
        DisableAllInfoTxt();
        _connecting.gameObject.SetActive(true);
        SetInteractability(false);
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

    /// <summary>
    /// Calls if creating a room fails
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="returnCode"> Operation ReturnCode from the server. </param>
    /// <param name="message"> Debug message for the error. </param>
    public override void OnCreateRoomFailed(short returnCode,string message)
    {
        _connecting.gameObject.SetActive(false);
        SetInteractability(true);
        _createRoomFailed.gameObject.SetActive(true);
    }

    /// <summary>
    /// Calls if joining a room fails
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="returnCode"> Operation ReturnCode from the server. </param>
    /// <param name="message"> Debug message for the error. </param>
    public override void OnJoinRoomFailed(short returnCode,string message)
    {
        _connecting.gameObject.SetActive(false);
        SetInteractability(true);
        _joinRoomFailed.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Sets the interactability of the buttons and fields
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="state"> T if interactable </param>
    private void SetInteractability(bool state)
    {
        _createInput.interactable = state;
        _joinInput.interactable = state;
        _createButton.interactable = state;
        _joinButton.interactable = state;     
    }

    /// <summary>
    /// Disables all info text
    /// 
    /// Author: Andrea SD
    /// </summary>
    private void DisableAllInfoTxt()
    {
        _invalidName.gameObject.SetActive(false);
        _createRoomFailed.gameObject.SetActive(false);
        _joinRoomFailed.gameObject.SetActive(false);
        _connecting.gameObject.SetActive(false);
    }
}
