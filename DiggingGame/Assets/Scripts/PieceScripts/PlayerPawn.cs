/*****************************************************************************
// File Name :         PlayerPawn.cs
// Author :            Rudy Wolfer
// Creation Date :     October 6th, 2022
//
// Brief Description : Script that controls Players' Pawn pieces.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [Header("References/Values")]
    //1 or 2
    [SerializeField] private int _player;

    [Header("Other")]
    //The (up to) 4 Board Pieces surrounding a player. NSEW.
    private List<GameObject> _adjacentPieces = new List<GameObject>();
    private List<GameObject> _boardPieces = new List<GameObject>();

    /// <summary>
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
        FindBoardPieces();
    }

    /// <summary>
    /// Temporary movement functionality.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            CheckAdjacentTiles();
        }

        BasicMovement();
    }

    /// <summary>
    /// Adds every board piece to a list.
    /// </summary>
    private void FindBoardPieces()
    {
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            _boardPieces.Add(piece);
        }
    }

    /// <summary>
    /// Temporary method for example movement.
    /// </summary>
    private void BasicMovement()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 1);
            UnassignAdjacentTiles();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 1);
            UnassignAdjacentTiles();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = new Vector2(transform.position.x + 1, transform.position.y);
            UnassignAdjacentTiles();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = new Vector2(transform.position.x - 1, transform.position.y);
            UnassignAdjacentTiles();
        }
    }

    /// <summary>
    /// Finds adjacent tiles & the tile the player is on.
    /// 1. Finds the distance between the current tile and closest other tile. 
    /// 2. Finds each tile matching that distance.
    /// 3. Adds every tile to _adjacentPieces.
    /// 4. Calls "AdjacentToPlayer" for each Piece.
    /// </summary>
    private void CheckAdjacentTiles()
    {
        GameObject closestPiece = null;
        float curShortestDist = Mathf.Infinity;
        Vector3 curPiecePos = ClosestPieceToPawn().transform.position;

        foreach(GameObject piece in _boardPieces)
        {
            float pieceToPieceDist = Vector3.Distance(piece.transform.position, curPiecePos);
            if (pieceToPieceDist < curShortestDist)
            {
                closestPiece = piece;

                if(closestPiece.transform.position != curPiecePos)
                {
                    curShortestDist = pieceToPieceDist;
                }
            }
        }

        int i = 0;
        foreach(GameObject piece in _boardPieces)
        {
            if(Vector3.Distance(piece.transform.position, curPiecePos) == curShortestDist)
            {
                i++;
                _adjacentPieces.Add(piece);
            }
        }
        Debug.Log("Found " + i + " pieces " + curShortestDist + " units from " + closestPiece.name + ".");

        foreach(GameObject piece in _adjacentPieces)
        {
            piece.GetComponent<PieceController>().AdjacentToPlayer();
        }
    }

    /// <summary>
    /// Finds the closest piece to the pawn.
    /// </summary>
    /// <returns>GameObject "Piece" that's closest to the current Pawn.</returns>
    private GameObject ClosestPieceToPawn()
    {
        GameObject closestPiece = null;
        float curShortestDist = Mathf.Infinity;
        Vector3 pawnPosition = transform.position;
        foreach(GameObject piece in _boardPieces)
        {
            float pawnToPieceDist = Vector3.Distance(piece.transform.position, pawnPosition);
            if(pawnToPieceDist < curShortestDist)
            {
                closestPiece = piece;
                curShortestDist = pawnToPieceDist;
            }
        }

        if(closestPiece == null)
        {
            Debug.LogWarning("No closest piece to pawn found? Are pieces in the scene and marked with the BoardPiece tag?");
            return null;
        }
        else
        {
            Debug.Log("Found closest piece: " + closestPiece.name);
            return closestPiece;
        }
    }

    /// <summary>
    /// Sets currently adjacent tiles as no longer adjacent.
    /// </summary>
    private void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _adjacentPieces.Count; i++)
        {
            if(_adjacentPieces[i] != null)
            {
                _adjacentPieces[i].GetComponent<PieceController>().NoLongerAdjacent();
            }
        }

        _adjacentPieces.Clear();
    }
}
