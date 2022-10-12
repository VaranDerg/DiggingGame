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

public class CreateGold : MonoBehaviour
{
    [SerializeField] List<GameObject> _stonePieces; //List of stone pieces without gold

    /// <summary>
    /// Awake occurs before the first frame update
    /// </summary>
    private void Awake()
    {
        SetGold(12);
    }

    /// <summary>
    /// Assigns 12 random pieces gold on the game board.
    /// </summary>
    /// <param name="numGold"></param>
    private void SetGold(int numGold)
    {
        while(numGold > 0)
        { 
            if(_stonePieces.Count >= numGold)
            {
                GameObject tempPiece = _stonePieces.ElementAt<GameObject>(Random.Range(0, _stonePieces.Count));
                tempPiece.GetComponent<MultiPieceController>().GiveGold();  
                _stonePieces.Remove(tempPiece);
            }
            numGold--;
        }
    }
}
