/*****************************************************************************
// File Name :         AdjacencyChecker.cs
// Author :            Rudy W.
// Creation Date :     October 7th, 2022
//
// Brief Description : Script exclusively existing to check adjacent Pieces.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyChecker : MonoBehaviour
{
    private List<GameObject> _boardPieces = new List<GameObject>();

    /// <summary>
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
        FindBoardPieces();
    }

    /// <summary>
    /// Adds every Board Piece to a List.
    /// </summary>
    private void FindBoardPieces()
    {
        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            _boardPieces.Add(piece);
        }
    }

    /// <summary>
    /// Finds adjacent tiles & the tile the player is on.
    /// 1. Finds the distance between the current tile and closest other tile. 
    /// 2. Finds each tile matching that distance.
    /// 3. Adds every tile to _adjacentPieces.
    /// 4. Calls "AdjacentToPlayer" for each Piece.
    /// </summary>
    public List<GameObject> GenerateAdjacentPieceList(GameObject centralPiece)
    {
        List<GameObject> _adjacentPieces = new List<GameObject>();
        GameObject closestPiece = null;
        float curShortestDist = Mathf.Infinity;
        Vector3 curPiecePos = centralPiece.transform.position;

        foreach (GameObject piece in _boardPieces)
        {
            float pieceToPieceDist = Vector3.Distance(piece.transform.position, curPiecePos);
            if (pieceToPieceDist < curShortestDist)
            {
                closestPiece = piece;

                if (closestPiece.transform.position != curPiecePos)
                {
                    curShortestDist = pieceToPieceDist;
                }
            }
        }

        int i = 0;
        foreach (GameObject piece in _boardPieces)
        {
            if (Vector3.Distance(piece.transform.position, curPiecePos) == curShortestDist)
            {
                i++;
                _adjacentPieces.Add(piece);
            }
        }
        //Debug.Log("Found " + i + " pieces " + curShortestDist + " units from " + closestPiece.name + ".");

        return _adjacentPieces;
    }
}
