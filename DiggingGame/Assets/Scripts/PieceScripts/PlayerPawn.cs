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
    private List<GameObject> _shownPieces = new List<GameObject>();
    private List<GameObject> _boardPieces = new List<GameObject>();
    private AdjacencyChecker _ac;

    /// <summary>
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
        FindBoardPieces();
        _ac = FindObjectOfType<AdjacencyChecker>();
    }

    /// <summary>
    /// Temporary movement functionality.
    /// </summary>
    private void Update()
    {
        //Find every piece adjacent to the pawn's piece.
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Add it to the "_shownPieces" array and mark it as movable. 
            foreach(GameObject piece in _ac.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                piece.GetComponent<PieceController>().ShowHideMovable(true);
                _shownPieces.Add(piece);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Find every piece adjacent to the pawn's piece.
            foreach (GameObject piece in _ac.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                bool dontHighlight = false;
                //Check if that adjacent piece has a building.
                if (piece.GetComponent<PieceController>().HasBuilding)
                {
                    dontHighlight = true;
                }

                //Check if any currently adjacent pieces are adjacent to a building.
                foreach (GameObject pieceSquared in _ac.GenerateAdjacentPieceList(piece))
                {
                    if (pieceSquared.GetComponent<PieceController>().HasBuilding)
                    {
                        dontHighlight = true;
                    }
                }

                //If not, add it to the "_shownPieces" array and mark it as buildable.
                if(!dontHighlight)
                {
                    piece.GetComponent<PieceController>().ShowHideBuildable(true);
                    _shownPieces.Add(piece);
                }
            }
        }

        TemporaryMovement();
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
    private void TemporaryMovement()
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

        return closestPiece;
    }

    /// <summary>
    /// Sets currently adjacent tiles as no longer adjacent.
    /// </summary>
    private void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _shownPieces.Count; i++)
        {
            if(_shownPieces[i] != null)
            {
                _shownPieces[i].GetComponent<PieceController>().ShowHideMovable(false);
            }
        }

        _shownPieces.Clear();
    }
}
