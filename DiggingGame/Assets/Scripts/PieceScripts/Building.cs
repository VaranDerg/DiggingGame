/*****************************************************************************
// File Name :         Building.cs
// Author :            Rudy Wolfer
// Creation Date :     October 3rd, 2022
//
// Brief Description : General script for universal building parameters.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building : MonoBehaviour
{
    [HideInInspector] public bool Damaged;

    private List<GameObject> _boardPieces = new List<GameObject>();

    private void Start()
    {
        FindBoardPieces();
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

    public virtual void PlaceBuilding()
    {

    }

    public virtual void DamageBuiliding()
    {

    }
}
