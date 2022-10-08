/*****************************************************************************
// File Name :         BoardController.cs
// Author :            Andrea Swihart-DeCoster & Rudy W.
// Creation Date :     October 3rd, 2022
//
// Brief Description : This document controls the players interactions with the
                       game board.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PieceController : MonoBehaviour
{
    //Edited: Rudy W. Organized with headers, added certain variables for functionality.

    [Header("Piece References")]
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _stoneSprite;
    [SerializeField] private Sprite _bedRockSprite;
    private SpriteRenderer _sr;

    [Header("Building References")]
    [SerializeField] private Transform _buildingSlot;
    [SerializeField] private GameObject _factory, _burrow, _mine;

    [Header("Tile Values/Information")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _movableColor, _diggableColor, _placeableColor, buildableColor;
    [HideInInspector] public bool IsMovable = false, IsDiggable = false, IsPlaceable = false, IsBuildable = false;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasBuilding;
    [HideInInspector] public bool HasPawn;
    private AdjacencyChecker _ac;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _ac = FindObjectOfType<AdjacencyChecker>();
    }

    /// <summary>
    /// Represents one of three states: One - Grass, Two - Dirt, Three - Stone,
    /// Four - Bedrock
    /// Author: Andrea SD
    /// </summary>
    public enum GameState
    {
        One,
        Two,
        Three,
        Four
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetObjectState(1);
        _sr.color = _defaultColor;
    }

    /// <summary>
    /// This method controls what happens when the interacts with the board.
    /// Author: Andrea SD
    /// Edited: Rudy W. Moved Debug statements into SetObjectState along with sprite change lines, as states may change through 
    ///         separate effects in the future. Additionally, moved into OnMouseOver for further usability; allows for replacing 
    ///         pieces with right click temporarily.
    /// </summary>
    private void OnMouseOver()
    {
        BuildingPlacementAndRemoval();
        PiecePlacementAndRemoval();
    }

    /// <summary>
    /// Method controlling Piece placement and removal.
    /// </summary>
    private void PiecePlacementAndRemoval()
    {
        // Once clicked, the piece will change states to the piece below it and
        // the sprite is changed to reflect that.
        // Example: grass -> dirt, dirt -> stone, stone -> bedrock. reverse for right click

        //The board cannot be adjusted if a tile is not marked as interactable.If commented, it's being tested.
        if (!IsDiggable)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (ObjState)
            {
                case GameState.One:
                    SetObjectState(2);
                    break;
                case GameState.Two:
                    SetObjectState(3);
                    break;
                case GameState.Three:
                    SetObjectState(4);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (ObjState)
            {
                case GameState.Two:
                    SetObjectState(1);
                    break;
                case GameState.Three:
                    SetObjectState(2);
                    break;
                case GameState.Four:
                    SetObjectState(3);
                    break;
            }
        }
    }

    /// <summary>
    /// Method controlling Building placement and removal.
    /// </summary>
    private void BuildingPlacementAndRemoval()
    {
        if(!IsBuildable)
        {
            return;
        }

        if(HasBuilding)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            BuildBuilding(_factory);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BuildBuilding(_burrow);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            BuildBuilding(_mine);
        }
    }

    /// <summary>
    /// Places a building. Returns false and removes it if it's adjacent to another building.
    /// </summary>
    /// <param name="building"></param>
    private bool BuildBuilding(GameObject building)
    {
        bool canPlaceOnTile = true;

        foreach(GameObject piece in _ac.GenerateAdjacentPieceList(gameObject))
        {
            if(piece.GetComponent<PieceController>().HasBuilding)
            {
                canPlaceOnTile = false;
            }
        }

        if(canPlaceOnTile)
        {
            Instantiate(building, _buildingSlot);
            HasBuilding = true;
            Debug.Log("Placed " + building.name + ".");
            return true;
        }
        else
        {
            Debug.Log("Cannot place " + building.name + " adjacent to another building.");
            return false;
        }
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// Edited: 
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    public void SetObjectState(int state) 
    {
        //Debug.Log("Switching " + gameObject.name + "'s State to " + state + ".");

        switch (state)
        {
            case 1:
                gameObject.GetComponent<SpriteRenderer>().sprite = _grassSprite;
                ObjState = GameState.One;
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().sprite = _dirtSprite;
                ObjState = GameState.Two;
                break;
            case 3:
                gameObject.GetComponent<SpriteRenderer>().sprite = _stoneSprite;
                ObjState = GameState.Three;
                break;
            case 4:
                gameObject.GetComponent<SpriteRenderer>().sprite = _bedRockSprite;
                ObjState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
    }

    /// <summary>
    /// Updates tiles for player movement.
    /// </summary>
    public void ShowHideMovable(bool show)
    {
        if(show)
        {
            _sr.color = _movableColor;
            IsMovable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsMovable = false;
        }
    }

    /// <summary>
    /// Updates tiles for buildability.
    /// </summary>
    public void ShowHideBuildable(bool show)
    {
        if (show)
        {
            _sr.color = buildableColor;
            IsBuildable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsBuildable = false;
        }
    }

    /// <summary>
    /// Updates tiles for piece removal.
    /// </summary>
    public void ShowHideDiggable(bool show)
    {
        if (show)
        {
            _sr.color = _diggableColor;
            IsDiggable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsDiggable = false;
        }
    }

    /// <summary>
    /// Updates tiles for piece placement.
    /// </summary>
    public void ShowHidePlaceable(bool show)
    {
        if (show)
        {
            _sr.color = _placeableColor;
            IsPlaceable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsPlaceable = false;
        }
    }

    /// <summary>
    /// Changes a Tile's occupant. A tile can hold 1 Pawn and 1 Building.
    /// </summary>
    /// <param name="isPlayer">True = Pawn; False = Building</param>
    /// <param name="isEntering">True = Being Placed; False = Being Removed</param>
    public void ChangeOccupation(bool isPlayer, bool isEntering)
    {
        if(isEntering)
        {
            if (isPlayer)
            {
                HasPawn = true;
            }
            else if (!isPlayer)
            {
                HasBuilding = true;
            }
        }
        else
        {
            if (isPlayer)
            {
                HasPawn = false;
            }
            else if (!isPlayer)
            {
                HasBuilding = false;
            }
        }
    }
}
