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
    [SerializeField] private float _raycastAdjuster;

    [Header("Other")]
    //The (up to) 4 Board Pieces surrounding a player. NSEW.
    private List<GameObject> _adjacentPieces = new List<GameObject>();
    private List<GameObject> _boardPieces = new List<GameObject>();

    private void Start()
    {
        FindBoardPieces();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            CheckAdjacentTiles();
        }

        BasicMovement();
    }

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

    private void FindBoardPieces()
    {
        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            _boardPieces.Add(piece);
        }
    }

    /// <summary>
    /// Finds adjacent tiles based on the Nsew points. Marks them as adjacent.
    /// </summary>
    private void CheckAdjacentTiles()
    {
        RaycastHit2D nPiece = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + _raycastAdjuster), Vector2.up, _raycastAdjuster);
        RaycastHit2D sPiece = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - _raycastAdjuster), Vector2.down, -_raycastAdjuster);
        RaycastHit2D ePiece = Physics2D.Raycast(new Vector2(transform.position.x + _raycastAdjuster, transform.position.y), Vector2.right, _raycastAdjuster);
        RaycastHit2D wPiece = Physics2D.Raycast(new Vector2(transform.position.x - _raycastAdjuster, transform.position.y), Vector2.left, -_raycastAdjuster);

        //if(nPiece.transform.gameObject != null)
        //{
        //    _adjacentPieces.Add(nPiece.transform.gameObject);
        //}
        //if (sPiece.transform.gameObject != null)
        //{
        //    _adjacentPieces.Add(sPiece.transform.gameObject);
        //}
        //if (ePiece.transform.gameObject != null)
        //{
        //    _adjacentPieces.Add(ePiece.transform.gameObject);
        //}
        //if(wPiece.transform.gameObject != null)
        //{
        //    _adjacentPieces.Add(wPiece.transform.gameObject);
        //}

        _adjacentPieces.Add(nPiece.transform.gameObject);
        _adjacentPieces.Add(sPiece.transform.gameObject);
        _adjacentPieces.Add(ePiece.transform.gameObject);
        _adjacentPieces.Add(wPiece.transform.gameObject);

        foreach (GameObject piece in _adjacentPieces)
        {
            if(piece == null)
            {
                return;
            }
            piece.GetComponent<PieceController>().AdjacentToPlayer();
        }
    }

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
