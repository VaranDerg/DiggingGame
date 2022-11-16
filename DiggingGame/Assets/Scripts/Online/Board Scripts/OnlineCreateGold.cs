/*****************************************************************************
// File Name :         BoardController.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     October 10th, 2022
//
// Brief Description : This document assigns 12 random board pieces the gold
                       value.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class OnlineCreateGold : MonoBehaviourPun
{
    //List of stone pieces without gold
    [SerializeField] List<GameObject> _stonePieces; 

    /// <summary>
    /// Awake occurs before the first frame update
    /// </summary>
    private void Awake()
    {
        // only called on the master client to make sure each client has the 
        // same board layout
        if(PhotonNetwork.IsMasterClient)
        {
            SetGold(12);
        }    
    }

    /// <summary>
    /// Assigns 12 random pieces gold on the game board.
    /// </summary>
    /// <param name="numGold"></param>
    private void SetGold(int numGold)
    {
        int _randomNum;

        while (numGold > 0)
        { 
            if(_stonePieces.Count >=  numGold)
            {
                // Once a piece has gold, it's removed from the list of options
                _randomNum = Random.Range(0, _stonePieces.Count);

                photonView.RPC("SetGoldOnline", RpcTarget.AllBuffered,
                    _randomNum);
            }
            numGold--;
        }
    }

    /// <summary>
    /// Assigns a piece to gold across each players client
    /// </summary>
    /// <param name="pieceNum"></param>
    [PunRPC]
    public void SetGoldOnline(int pieceNum)
    {
        GameObject tempPiece = _stonePieces.ElementAt<GameObject>(pieceNum);
        tempPiece.GetComponent<OnlinePieceController>().GiveGold();
        _stonePieces.Remove(tempPiece);
    }    
}
