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
    [Header("References")]
    [SerializeField] private Collider2D _playerMainCollider;
    //The 4 colliders surrounding a player. NSEW.
    [SerializeField] private Collider2D[] _playerNsewColliders = new Collider2D[4];

    [Header("Other")]
    //The (up to) 4 Board Pieces surrounding a player. NSEW.
    private BoardController[] _tilesAdjacent = new BoardController[4];
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

        if (Input.GetKeyDown(KeyCode.E))
        {
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
    /// Finds adjacent tiles based on the Nsew colliders. Marks them as adjacent.
    /// </summary>
    private void CheckAdjacentTiles()
    {
        //Check if a collider is touching anything. 
        //Check if that collider is touching something with the tag "BoardPiece."
        //If it isn't, move to the nice collider in the array. 
        //If it is, update the tile in the _tilesAdjacent array to being adjacent. 

        for(int i = 0; i < _playerNsewColliders.Length; i++)
        {
            foreach(GameObject piece in _boardPieces)
            {
                if(piece.gameObject.GetComponent<Collider2D>().IsTouching(_playerNsewColliders[i]))
                {
                    _tilesAdjacent[i] = piece.GetComponent<BoardController>();
                }
            }
        }
    }

    private void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _tilesAdjacent.Length; i++)
        {
            _tilesAdjacent[i].NoLongerAdjacent();
        }
    }
}
