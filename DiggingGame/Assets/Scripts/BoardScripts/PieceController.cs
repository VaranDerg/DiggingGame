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
    [SerializeField] private GameObject _playerPawn;
    private SpriteRenderer _sr;

    [Header("Building References")]
    [SerializeField] private Transform _buildingSlot;
    [SerializeField] private GameObject _mFactory, _mBurrow, _mMine, _meeFactory, _meeBurrow, _meeMine;

    [Header("Tile Values/Information")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _waitingColor;
    [HideInInspector] public bool IsMovable = false, IsDiggable = false, IsPlaceable = false, IsBuildable = false;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasP1Building, HasP2Building;
    [HideInInspector] public bool HasPawn;
    [HideInInspector] public GameObject CurrentPawn;
    [HideInInspector] public bool PieceIsWaiting = true;
    private BoardManager _bm;
    private ActionManager _am;
    private CardManager _cm;
    private GameCanvasManagerNew _gcm;

    public bool HasGold;    //true if the piece reveals gold when flipped

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _cm = FindObjectOfType<CardManager>();
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

        if (IsDiggable)
        {
            StartCoroutine(PieceRemoval());
        }

        if (IsMovable && CurrentPawn != null)
        {
            StartCoroutine(PawnMovement());
        }

        if(!HasP1Building && !HasP2Building && IsBuildable)
        {
            StartCoroutine(BuildingPlacement());
        }
    }

    /// <summary>
    /// Method controlling piece removal.
    /// </summary>
    private IEnumerator PieceRemoval()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _sr.color = _waitingColor;
            PieceIsWaiting = true;
            foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
            {
                pawn.GetComponent<PlayerPawn>().HideNonWaitingTiles();
            }

            if (ObjState == GameState.One)
            {
                _cm.PrepSelectionVariables(1, "Grass", false);
            }
            else if (ObjState == GameState.Two)
            {
                _cm.PrepSelectionVariables(1, "Dirt", false);
            }
            else if (ObjState == GameState.Three || ObjState == GameState.Five)
            {
                _cm.PrepSelectionVariables(1, "Stone", false);
            }
            else if (ObjState == GameState.Four)
            {
                _cm.PrepSelectionVariables(1, "Any", false);
            }
            bool isGood = false;
            while (!isGood)
            {
                isGood = _cm.CheckSelectedCards();
                yield return null;
            }
            _cm.PrepSelectionVariables(0, "", true);

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
    private IEnumerator PawnMovement()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(_am.CurrentTurnPhase != 1)
            {
                _sr.color = _waitingColor;
                PieceIsWaiting = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<PlayerPawn>().HideNonWaitingTiles();
                }

                if (ObjState == GameState.One)
                {
                    _cm.PrepSelectionVariables(1, "Grass", false);
                }
                else if (ObjState == GameState.Two)
                {
                    _cm.PrepSelectionVariables(1, "Dirt", false);
                }
                else if (ObjState == GameState.Three || ObjState == GameState.Five)
                {
                    _cm.PrepSelectionVariables(1, "Stone", false);
                }
                else if (ObjState == GameState.Four)
                {
                    _cm.PrepSelectionVariables(1, "Any", false);
                }

                bool isGood = false;
                while (!isGood)
                {
                    isGood = _cm.CheckSelectedCards();
                    yield return null;
                }
                _cm. PrepSelectionVariables(0, "", true);
            }

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
    private IEnumerator BuildingPlacement()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool canPlaceThere = false;

            string pieceSuit = "";
            if (ObjState == GameState.One)
            {
                pieceSuit = "Grass";
            }
            else if (ObjState == GameState.Two)
            {
                pieceSuit = "Dirt";
            }
            else if (ObjState == GameState.Three)
            {
                pieceSuit = "Stone";
            }
            else if (ObjState == GameState.Four || ObjState == GameState.Five)
            {
                Debug.LogWarning("Cannot place building on this piece, yet it was able to be selected?");
            }

            if (CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Factory")
            {
                canPlaceThere = _am.EnoughBuildingsRemaining(_am.CurrentPlayer, "Factory");

                if (canPlaceThere)
                {
                    _sr.color = _waitingColor;
                    PieceIsWaiting = true;
                    foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                    {
                        pawn.GetComponent<PlayerPawn>().HideNonWaitingTiles();
                    }

                    if(_am.CurrentPlayer == 1)
                    {
                        _cm.PrepSelectionVariables(_am.P1CurrentBuildingPrices[0], pieceSuit, false);
                    }
                    else
                    {
                        _cm.PrepSelectionVariables(_am.P2CurrentBuildingPrices[0], pieceSuit, false);
                    }
                    bool isGood = false;
                    while (!isGood)
                    {
                        isGood = _cm.CheckSelectedCards();
                        yield return null;
                    }
                    _cm.PrepSelectionVariables(0, "", true);

                    if(_am.CurrentPlayer == 1)
                    {
                        _am.P1CurrentBuildingPrices[0]++;
                        _am.P1RemainingBuildings[0]--;
                        _am.P1BuiltBuildings[0]++;
                    }
                    else
                    {
                        _am.P2CurrentBuildingPrices[0]++;
                        _am.P2RemainingBuildings[0]--;
                        _am.P2BuiltBuildings[0]++;
                    }
                    PlaceBuildingOnPiece("Factory");
                }
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Burrow")
            {
                canPlaceThere = _am.EnoughBuildingsRemaining(_am.CurrentPlayer, "Burrow");

                if(canPlaceThere)
                {
                    _sr.color = _waitingColor;

                    if (_am.CurrentPlayer == 1)
                    {
                        _cm.PrepSelectionVariables(_am.P1CurrentBuildingPrices[1], pieceSuit, false);
                    }
                    else
                    {
                        _cm.PrepSelectionVariables(_am.P2CurrentBuildingPrices[1], pieceSuit, false);
                    }
                    bool isGood = false;
                    while (!isGood)
                    {
                        isGood = _cm.CheckSelectedCards();
                        yield return null;
                    }
                    _cm.PrepSelectionVariables(0, "", true);

                    if(_am.CurrentPlayer == 1)
                    {
                        _am.P1CurrentBuildingPrices[1]++;
                        _am.P1RemainingBuildings[1]--;
                        _am.P1BuiltBuildings[1]++;
                    }
                    else
                    {
                        _am.P2CurrentBuildingPrices[1]++;
                        _am.P2RemainingBuildings[1]--;
                        _am.P2BuiltBuildings[1]++;
                    }
                    PlaceBuildingOnPiece("Burrow");
                }
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Mine")
            {
                canPlaceThere = _am.EnoughBuildingsRemaining(_am.CurrentPlayer, "Mine");

                if (canPlaceThere)
                {
                    _sr.color = _waitingColor;

                    if (_am.CurrentPlayer == 1)
                    {
                        _cm.PrepSelectionVariables(_am.P1CurrentBuildingPrices[2], pieceSuit, false);
                    }
                    else
                    {
                        _cm.PrepSelectionVariables(_am.P2CurrentBuildingPrices[2], pieceSuit, false);
                    }
                    bool isGood = false;
                    while (!isGood)
                    {
                        isGood = _cm.CheckSelectedCards();
                        yield return null;
                    }
                    _cm.PrepSelectionVariables(0, "", true);

                    if(_am.CurrentPlayer == 1)
                    {
                        _am.P1CurrentBuildingPrices[2]++;
                        _am.P1RemainingBuildings[2]--;
                        if (ObjState == GameState.One)
                        {
                            _am.P1BuiltBuildings[2]++;
                        }
                        else if (ObjState == GameState.Two)
                        {
                            _am.P1BuiltBuildings[3]++;
                        }
                        else if (ObjState == GameState.Three)
                        {
                            _am.P1BuiltBuildings[4]++;
                        }
                    }
                    else
                    {
                        _am.P2CurrentBuildingPrices[2]++;
                        _am.P2RemainingBuildings[2]--;
                        if (ObjState == GameState.One)
                        {
                            _am.P2BuiltBuildings[2]++;
                        }
                        else if (ObjState == GameState.Two)
                        {
                            _am.P2BuiltBuildings[3]++;
                        }
                        else if (ObjState == GameState.Three)
                        {
                            _am.P2BuiltBuildings[4]++;
                        }
                    }
                    PlaceBuildingOnPiece("Mine");
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
    /// Places a building. Returns false and removes it if it's adjacent to another building. Also will spawn more Pawns if 3rd building is placed.
    /// </summary>
    /// <param name="building"></param>
    public bool PlaceBuildingOnPiece(string buildingName)
    {
        GameObject building = null;
        if(_am.CurrentPlayer == 1)
        {
            if (buildingName == "Factory")
            {
                building = _mFactory;
            }
            else if (buildingName == "Burrow")
            {
                building = _mBurrow;
            }
            else if (buildingName == "Mine")
            {
                building = _mMine;
            }
        }
        else
        {
            if (buildingName == "Factory")
            {
                building = _meeFactory;
            }
            else if (buildingName == "Burrow")
            {
                building = _meeBurrow;
            }
            else if (buildingName == "Mine")
            {
                building = _meeMine;
            }
        }
        bool canPlaceOnTile = true;

        foreach(GameObject piece in _bm.GenerateAdjacentPieceList(gameObject))
        {
            if(piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
            {
                canPlaceOnTile = false;
            }
        }

        if(canPlaceOnTile)
        {
            bool spawnPawn = false;
            Instantiate(building, _buildingSlot);

            if (_am.CurrentPlayer == 1)
            {
                if (buildingName == "Factory")
                {
                    if (_am.P1RemainingBuildings[0] == 0)
                    {
                        spawnPawn = true;
                    }
                }
                else if (buildingName == "Burrow")
                {
                    if (_am.P1RemainingBuildings[1] == 0)
                    {
                        spawnPawn = true;
                    }
                }
                else if (buildingName == "Mine")
                {
                    if (_am.P1RemainingBuildings[2] == 0)
                    {
                        spawnPawn = true;
                    }
                }

                _am.P1Score++;
            }
            else
            {
                if (buildingName == "Factory")
                {
                    if (_am.P2RemainingBuildings[0] == 0)
                    {
                        spawnPawn = true;
                    }
                }
                else if (buildingName == "Burrow")
                {
                    if (_am.P2RemainingBuildings[1] == 0)
                    {
                        spawnPawn = true;
                    }
                }
                else if (buildingName == "Mine")
                {
                    if (_am.P2RemainingBuildings[2] == 0)
                    {
                        spawnPawn = true;
                    }
                }

                _am.P2Score++;
            }

            if (_am.CurrentPlayer == 1)
            {
                HasP1Building = true;
            }
            else
            {
                HasP2Building = true;
            }

            if (spawnPawn)
            {
                GameObject newPawn = Instantiate(_playerPawn, _buildingSlot);
                newPawn.GetComponent<PlayerPawn>().SetPawnToPlayer(_am.CurrentPlayer);
                newPawn.transform.SetParent(null);
            }

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
            PieceIsWaiting = false;
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
            PieceIsWaiting = false;
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
            PieceIsWaiting = false;
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
            PieceIsWaiting = false;
            IsPlaceable = false;
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