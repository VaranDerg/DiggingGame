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
    [SerializeField] private Sprite _bedrockSprite;
    [SerializeField] private Sprite _goldSprite;
    private SpriteRenderer _sr;

    [Header("Building References")]
    [SerializeField] private Transform _buildingSlot;
    [SerializeField] private GameObject _factory, _burrow, _mine;

    [Header("Tile Values/Information")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [HideInInspector] public bool IsMovable = false, IsDiggable = false, IsPlaceable = false, IsBuildable = false;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasBuilding;
    [HideInInspector] public bool HasPawn;
    [HideInInspector] public GameObject CurrentPawn;
    private BoardManager _bm;
    private ActionManager _am;
    private GameCanvasManagerNew _gcm;

    public bool HasGold;    //true if the piece reveals gold when flipped

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
    }

    /// <summary>
    /// Represents one of three states: One - Grass, Two - Dirt, Three - Stone,
    /// Four - Bedrock, Five - Gold
    /// Author: Andrea SD
    /// </summary>
    public enum GameState
    {
        One,
        Two,
        Three,
        Four,
        Five
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetPieceState(1);
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
        BuildingPlacement();
        PiecePlacement();
        PieceRemoval();
        PawnMovement();
    }

    /// <summary>
    /// Method controlling piece removal.
    /// </summary>
    private void PieceRemoval()
    {
        if (!IsDiggable)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Run "CheckSelectedCards" until adequate cards are selected.

            CurrentPawn.GetComponent<Animator>().Play("TempPawnDefault");
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
            _gcm.Back();

            switch (ObjState)
            {
                case GameState.One:
                    SetPieceState(2);
                    _am.CollectTile(_am.CurrentPlayer, "Grass");
                    break;
                case GameState.Two:
                    SetPieceState(3);
                    _am.CollectTile(_am.CurrentPlayer, "Dirt");
                    break;
                case GameState.Three:
                    SetPieceState(4);

                    if(HasGold)
                    {
                        _am.CollectTile(_am.CurrentPlayer, "Gold");
                    }
                    else
                    {
                        _am.CollectTile(_am.CurrentPlayer, "Stone");
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Allows the placement of pieces back onto the board.
    /// </summary>
    private void PiecePlacement()
    {
        if(!IsPlaceable)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CurrentPawn.GetComponent<Animator>().Play("TempPawnDefault");
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
            _gcm.Back();

            switch (ObjState)
            {
                case GameState.Two:
                    SetPieceState(1);
                    _am.PlaceTile(_am.CurrentPlayer, "Grass");
                    break;
                case GameState.Three:
                    SetPieceState(2);
                    _am.PlaceTile(_am.CurrentPlayer, "Dirt");
                    break;
                case GameState.Four:
                    SetPieceState(3);
                    _am.PlaceTile(_am.CurrentPlayer, "Stone");
                    break;
            }
        }
    }

    /// <summary>
    /// Controls pawn movement.
    /// </summary>
    private void PawnMovement()
    {
        if(!IsMovable || CurrentPawn == null)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Run "CheckSelectedCards" until adequate cards are selected."

            //Marks piece as having a pawn and moves the pawn. Also unmarks the previous piece.
            CurrentPawn.GetComponent<PlayerPawn>().ClosestPieceToPawn().GetComponent<PieceController>().HasPawn = false;
            CurrentPawn.transform.position = gameObject.transform.position;
            HasPawn = true;

            CurrentPawn.GetComponent<Animator>().Play("TempPawnDefault");
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();

            if(_am.CurrentTurnPhase == 1)
            {
                _gcm.ToThenPhase();
            }
            else if(_am.CurrentTurnPhase == 2)
            {
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Method controlling Building placement and removal.
    /// </summary>
    private void BuildingPlacement()
    {
        if(!IsBuildable)
        {
            return;
        }

        if(HasBuilding)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool canPlaceThere = false;
            if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Factory")
            {
                canPlaceThere = _am.EnoughBuildingsToBuild(_am.CurrentPlayer, "Factory", "");

                if(canPlaceThere)
                {
                    //Run "CheckSelectedCards" until adequate cards are selected."

                    PlaceBuildingOnPiece(_factory);
                }
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Burrow")
            {
                canPlaceThere = _am.EnoughBuildingsToBuild(_am.CurrentPlayer, "Burrow", "");

                if(canPlaceThere)
                {
                    //Run "CheckSelectedCards" until adequate cards are selected."

                    PlaceBuildingOnPiece(_burrow);
                }
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Mine")
            {
                if(ObjState == GameState.One)
                {
                    canPlaceThere = _am.EnoughBuildingsToBuild(_am.CurrentPlayer, "Mine", "Grass");
                }
                else if(ObjState == GameState.Two)
                {
                    canPlaceThere = _am.EnoughBuildingsToBuild(_am.CurrentPlayer, "Mine", "Dirt");
                }
                else if(ObjState == GameState.Three)
                {
                    canPlaceThere = _am.EnoughBuildingsToBuild(_am.CurrentPlayer, "Mine", "Stone");
                }

                if(canPlaceThere)
                {
                    //Run "CheckSelectedCards" until adequate cards are selected."

                    PlaceBuildingOnPiece(_mine);
                }
            }
            else
            {
                Debug.LogWarning("There is no building called " + CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild + "!");
            }

            CurrentPawn.GetComponent<Animator>().Play("TempPawnDefault");
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
            _gcm.Back();
        }
    }

    /// <summary>
    /// Places a building. Returns false and removes it if it's adjacent to another building.
    /// </summary>
    /// <param name="building"></param>
    public bool PlaceBuildingOnPiece(GameObject building)
    {
        bool canPlaceOnTile = true;

        foreach(GameObject piece in _bm.GenerateAdjacentPieceList(gameObject))
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
    public void SetPieceState(int state) 
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
                gameObject.GetComponent<SpriteRenderer>().sprite = _bedrockSprite;
                ObjState = GameState.Four;
                break;
            case 5:
                gameObject.GetComponent<SpriteRenderer>().sprite = _goldSprite;
                ObjState = GameState.Five;
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
            _sr.color = _selectedColor;
            IsMovable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsMovable = false;
            CurrentPawn = null;
        }
    }

    /// <summary>
    /// Updates tiles for buildability.
    /// </summary>
    public void ShowHideBuildable(bool show)
    {
        if (show)
        {
            _sr.color = _selectedColor;
            IsBuildable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            IsBuildable = false;
            CurrentPawn = null;
        }
    }

    /// <summary>
    /// Updates tiles for piece removal.
    /// </summary>
    public void ShowHideDiggable(bool show)
    {
        if (show)
        {
            _sr.color = _selectedColor;
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
            _sr.color = _selectedColor;
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

    /// <summary>
    /// Assigns the gold value to true.
    /// </summary>
    public void GiveGold()
    {
        HasGold = true;
    }
}
