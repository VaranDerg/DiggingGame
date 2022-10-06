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
    [SerializeField] private Collider2D[] _playerNsewColliders = new Collider2D[4];

    [Header("Other")]
    private BoardController[] _tilesAdjacent = new BoardController[4];

    /// <summary>
    /// Finds adjacent tiles based on the Nsew colliders. Marks them as adjacent.
    /// </summary>
    /// <param name="collision">Touching parameter</param>
    private void CheckAdjacentTiles(Collision2D collision)
    {
        //Check if a collider is touching anything. 
        //Check if that collider is touching something with the tag "BoardPiece."
        //If it isn't, move to the nice collider in the array. 
        //If it is, update the tile in the _tilesAdjacent array to being adjacent. 

        for(int i = 0; i < _playerNsewColliders.Length; i++)
        {
            
        }
    }

    private void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _tilesAdjacent.Length; i++)
        {
            _tilesAdjacent[i].IsAdjacent = false;
        }
    }
}
