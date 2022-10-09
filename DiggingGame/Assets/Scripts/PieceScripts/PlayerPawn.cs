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
    [Range(1, 2)] public int PawnPlayer;

    [Header("Other")]
    //The (up to) 4 Board Pieces surrounding a player. NSEW.
    private List<GameObject> _shownPieces = new List<GameObject>();
    private List<GameObject> _boardPieces = new List<GameObject>();
    private BoardManager _bm;
    private ActionManager _am;
    private GameCanvasManager _gcm;

    [Header("Pawn Status for Other Scripts")]
    [HideInInspector] public bool IsMoving = false, IsBuilding = false, IsDigging = false;
    [HideInInspector] public string BuildingToBuild = "";

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
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
        FindBoardPieces();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _gcm = FindObjectOfType<GameCanvasManager>();
    }

    /// <summary>
    /// Update, but for the mouse!
    /// </summary>
    private void OnMouseOver()
    {
        if (IsMoving)
        {
            PreparePawnMovement();
        }

        if(IsBuilding)
        {
            PreparePawnBuilding();
        }

        if(IsDigging)
        {
            PreparePawnDigging();
        }
    }

    /// <summary>
    /// Preps pawn for moving.
    /// </summary>
    private void PreparePawnMovement()
    {
        //Find every piece adjacent to the pawn's piece.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Add it to the "_shownPieces" array and mark it as movable. 
            foreach (GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                //If the piece has a pawn, don't mark it.
                if(piece.GetComponent<PieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideMovable(true);
                _shownPieces.Add(piece);
            }

            if(_shownPieces.Count > 0)
            {
                foreach(GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }

            _bm.BoardColliderSwitch(true);
        }
    }

    /// <summary>
    /// Preps pawn for digging.
    /// </summary>
    private void PreparePawnDigging()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach(GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                if(piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasBuilding)
                {
                    continue;
                }

                if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideDiggable(true);
                piece.GetComponent<PieceController>().IsDiggable = true;
                _shownPieces.Add(piece);
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }
            else
            {
                Debug.Log("No valid digging locations at this pawn.");
                _bm.DisablePawnBoardInteractions();
                _gcm.Back();
            }

            _bm.BoardColliderSwitch(true);
        }
    }

    /// <summary>
    /// Preps pawn for building.
    /// </summary>
    private void PreparePawnBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Find every piece adjacent to the pawn's piece.
            foreach (GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                bool dontHighlight = false;
                //Check if that adjacent piece has a building.
                if (piece.GetComponent<PieceController>().HasBuilding)
                {
                    dontHighlight = true;
                }

                if (piece.GetComponent<PieceController>().HasPawn)
                {
                    dontHighlight = true;
                }

                if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
                {
                    dontHighlight = true;
                }

                //Check if any currently adjacent pieces are adjacent to a building or if they're Bedrock or Gold.
                foreach (GameObject pieceSquared in _bm.GenerateAdjacentPieceList(piece))
                {
                    if (pieceSquared.GetComponent<PieceController>().HasBuilding)
                    {
                        dontHighlight = true;
                    }

                    if (pieceSquared.GetComponent<PieceController>().ObjState == PieceController.GameState.Four || pieceSquared.GetComponent<PieceController>().ObjState == PieceController.GameState.Five)
                    {
                        dontHighlight = true;
                    }
                }

                //If not, add it to the "_shownPieces" array and mark it as buildable.
                if (!dontHighlight)
                {
                    piece.GetComponent<PieceController>().ShowHideBuildable(true);
                    _shownPieces.Add(piece);
                }
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }
            else
            {
                Debug.Log("No valid building locations at this pawn.");
                _bm.DisablePawnBoardInteractions();
                _am.StopPawnActions(_am.CurrentPlayer);
                _gcm.Back();
            }

            _bm.BoardColliderSwitch(true);
        }
    }

    /// <summary>
    /// Finds the closest piece to the pawn.
    /// </summary>
    /// <returns>GameObject "Piece" that's closest to the current Pawn.</returns>
    public GameObject ClosestPieceToPawn()
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
    public void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _shownPieces.Count; i++)
        {
            if(_shownPieces[i] != null)
            {
                _shownPieces[i].GetComponent<PieceController>().ShowHideMovable(false);
            }
        }

        IsMoving = false;
        IsBuilding = false;
        IsDigging = false;
        BuildingToBuild = "";
        _shownPieces.Clear();
    }
}
