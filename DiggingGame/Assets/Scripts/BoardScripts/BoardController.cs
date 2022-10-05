/*****************************************************************************
// File Name :         BoardController.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     October 3rd, 2022
//
// Brief Description : This document controls the players interactions with the
                       game board.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class BoardController : MonoBehaviourPun
{

    private const byte SPRITE_CHANGE_EVENT = 0;

    [SerializeField] Sprite _grassSprite;
    [SerializeField] Sprite _dirtSprite;
    [SerializeField] Sprite _stoneSprite;
    [SerializeField] Sprite _bedRockSprite;

    private GameState _objState;
    private Action<EventData> networkingClient_EventRecieved;

    PhotonView view;

    /// <summary>
    /// Represents one of three states: One - Grass, Two - Dirt, Three - Stone,
    /// Four - Gone
    /// Author: Andrea SD
    /// </summary>
    enum GameState
    {
        One,
        Two,
        Three,
        Four
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Object will always listen for events
        PhotonNetwork.NetworkingClient.EventReceived += networkingClient_EventRecieved;
        SetObjectState(1);
        view = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Update is called before the first frame update
    /// </summary>
    void Update()
    {
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == SPRITE_CHANGE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            int newNum = (int)datas[0];
            gameObject.GetComponent<SpriteRenderer>().sprite = _dirtSprite;
        }
    }

    /// <summary>
    /// This method controls what happens when the player left clicks on the
    /// board piece.
    /// Author: Andrea SD
    /// </summary>
    private void OnMouseDown()
    {
        // Once clicked, the piece will change states to the piece below it and
        // the sprite is changed to reflect that.
        // Example: grass -> dirt, dirt -> stone, stone -> (disappears)
        if (view.IsMine)
        {
            switch (_objState)
            {

                case GameState.One:
                    ChangeSprite(_dirtSprite);
                    //photonView.RPC("ChangeSprite", RpcTarget.All, _dirtSprite);
                    SetObjectState(2);
                    Debug.Log(_objState);
                    break;
                case GameState.Two:
                    ChangeSprite(_stoneSprite);
                    SetObjectState(3);
                    Debug.Log(_objState);
                    break;
                case GameState.Three:
                    ChangeSprite(_bedRockSprite);
                    SetObjectState(4);
                    Debug.Log(_objState);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    public void SetObjectState(int state) 
    {
        switch(state)
        {
            case 1:
                _objState = GameState.One;
                break;
            case 2: 
                _objState = GameState.Two;
                break;
            case 3:
                _objState = GameState.Three;
                break;
            case 4:
                _objState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
        
    }

    //[PunRPC]
    public void ChangeSprite(Sprite newSprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        //Debug.Log("Worked");

        // cant specify obj type in raiseEvents
        //ensures objID matches
        //object[] datas = new object[] { base.photonView.ViewID, newSprite };
       // object[] datas = new object[] { 1 };
       // PhotonNetwork.RaiseEvent(SPRITE_CHANGE_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
    }
}
