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
    [SerializeField] private Sprite _flowerSprite;
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
    [HideInInspector] public bool PieceIsSelected = true;
    private BoardManager _bm;
    private ActionManager _am;
    private CardManager _cm;
    private GameCanvasManagerNew _gcm;
    private PersistentCardManager _pcm;
    private CardEffects _ce;
    [HideInInspector] public bool HasGold;    //true if the piece reveals gold when flipped

    [Header("Card Activation Stuff")]
    [HideInInspector] public bool FromActivatedCard = false;
    [HideInInspector] public bool MovingForFree = false, JustMovedForFree = false;
    [HideInInspector] public bool IsEarthquakeable;
    [HideInInspector] public bool WalkwayDig;
    [HideInInspector] public bool IsFlippable;
    [HideInInspector] public bool DiscerningEye;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _cm = FindObjectOfType<CardManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _ce = FindObjectOfType<CardEffects>();
        _pcm = FindObjectOfType<PersistentCardManager>();
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
        Five,
        Six
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
        if(IsPlaceable)
        {
            PiecePlacement();
        }

        if (IsDiggable)
        {
            if(FromActivatedCard)
            {
                ActivatedPieceRemoval();
            }
            else
            {
                StartCoroutine(PieceRemoval());
            }
        }

        if (IsMovable && CurrentPawn != null)
        {
            StartCoroutine(PawnMovement());
        }

        if(!HasP1Building && !HasP2Building && IsBuildable)
        {
            StartBuildingPlacement();
        }

        if(IsEarthquakeable)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                FindObjectOfType<CardEffects>().SelectedPiece = this;
            }
        }

        if (IsFlippable)
        {
            FlipPiece();
        }
    }

    /// <summary>
    /// Method controlling piece removal.
    /// </summary>
    private IEnumerator PieceRemoval()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start of Shovel Code
            bool hasShovel = _pcm.CheckForPersistentCard("Shovel", false);
            if(hasShovel)
            {
                if(ObjState == GameState.Two && !_am.ShovelUsed)
                {
                    SetPieceState(3);
                    _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
                    _am.ShovelUsed = true;
                }
            }
            else if(!WalkwayDig)
            {
                _sr.color = _waitingColor;
                PieceIsSelected = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<PlayerPawn>().HideNonSelectedTiles();
                }

                if (ObjState == GameState.One || ObjState == GameState.Six)
                {
                    _cm.PrepareCardSelection(1, "Grass", false);
                }
                else if (ObjState == GameState.Two)
                {
                    _cm.PrepareCardSelection(1, "Dirt", false);
                }
                else if (ObjState == GameState.Three || ObjState == GameState.Five)
                {
                    _cm.PrepareCardSelection(1, "Stone", false);
                }
                else if (ObjState == GameState.Four)
                {
                    _cm.PrepareCardSelection(1, "Any", false);
                }

                while (!_cm.CheckCardSelection())
                {
                    yield return null;
                }
                _cm.PrepareCardSelection(0, "", true);

                CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
                _gcm.Back();

                switch (ObjState)
                {
                    case GameState.One:
                        SetPieceState(2);
                        _am.CollectTile(_am.CurrentPlayer, "Grass", true);
                        break;
                    case GameState.Six:
                        SetPieceState(2);
                        _am.CollectTile(_am.CurrentPlayer, "Grass", true);
                        break;
                    case GameState.Two:
                        SetPieceState(3);
                        _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
                        break;
                    case GameState.Three:
                        SetPieceState(4);

                        if(HasGold)
                        {
                            _am.CollectTile(_am.CurrentPlayer, "Gold", true);
                        }
                        else
                        {
                            _am.CollectTile(_am.CurrentPlayer, "Stone", true);
                        }

                        break;
                }
            }
            else
            {
                _am.CollectTile(_am.CurrentPlayer, "Grass", false);
                _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
                SetPieceState(3);

                CurrentPawn.GetComponent<PlayerPawn>().ClosestPieceToPawn().GetComponent<PieceController>().HasPawn = false;
                CurrentPawn.transform.position = gameObject.transform.position;
                HasPawn = true;
                CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
                WalkwayDig = false;
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Flips a stone piece to see if it has gold or not.
    /// </summary>
    private void FlipPiece()
    {
        _ce.RemainingFlips--;

        if(HasGold)
        {
            SetPieceState(5);
            if(DiscerningEye)
            {
                _am.ScorePoints(1);
            }
        }

        ShowHideFlippable(false);
    }

    /// <summary>
    /// Method for digging tiles through effects instead of cards. 
    /// </summary>
    private void ActivatedPieceRemoval()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (ObjState == GameState.Four)
            {
                return;
            }

            switch (ObjState)
            {
                case GameState.One:
                    SetPieceState(2);
                    _am.CollectTile(_am.CurrentPlayer, "Grass", false);
                    FindObjectOfType<CardEffects>().DugPieces++;
                    break;
                case GameState.Six:
                    SetPieceState(2);
                    _am.CollectTile(_am.CurrentPlayer, "Grass", false);
                    FindObjectOfType<CardEffects>().DugPieces++;
                    break;
                case GameState.Two:
                    SetPieceState(3);
                    _am.CollectTile(_am.CurrentPlayer, "Dirt", false);
                    FindObjectOfType<CardEffects>().DugPieces++;
                    break;
                case GameState.Three:
                    SetPieceState(4);
                    if (HasGold)
                    {
                        _am.CollectTile(_am.CurrentPlayer, "Gold", false);
                    }
                    else
                    {
                        _am.CollectTile(_am.CurrentPlayer, "Stone", false);
                    }
                    FindObjectOfType<CardEffects>().DugPieces++;
                    break;
            }

            FromActivatedCard = false;
        }    
    }

    /// <summary>
    /// (WIP) Allows the placement of pieces back onto the board.
    /// </summary>
    private void PiecePlacement()
    {
        if(!IsPlaceable)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
            FindObjectOfType<CardEffects>().PlacedPieces++;

            switch (ObjState)
            {
                case GameState.Two:
                    SetPieceState(6);
                    _am.PlaceTile(_am.CurrentPlayer, "Grass");
                    _am.SupplyPile[0]--;
                    break;
                case GameState.Three:
                    SetPieceState(2);
                    _am.PlaceTile(_am.CurrentPlayer, "Dirt");
                    _am.SupplyPile[1]--;
                    break;
                case GameState.Four:
                    SetPieceState(3);
                    _am.PlaceTile(_am.CurrentPlayer, "Stone");
                    _am.SupplyPile[2]--;
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
            //For the game's initial free move. The player has to spend cards unless this is true.
            if(_am.CurrentTurnPhase != 1 && _am.CurrentTurnPhase != 3)
            {
                _sr.color = _waitingColor;
                PieceIsSelected = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<PlayerPawn>().HideNonSelectedTiles();
                }

                if(!MovingForFree)
                {
                    if (ObjState == GameState.One)
                    {
                        _cm.PrepareCardSelection(1, "Grass", false);
                    }
                    else if (ObjState == GameState.Two)
                    {
                        _cm.PrepareCardSelection(1, "Dirt", false);
                    }
                    else if (ObjState == GameState.Three || ObjState == GameState.Five)
                    {
                        _cm.PrepareCardSelection(1, "Stone", false);
                    }
                    else if (ObjState == GameState.Four)
                    {
                        _cm.PrepareCardSelection(1, "Any", false);
                    }

                    while (!_cm.CheckCardSelection())
                    {
                        yield return null;
                    }
                    _cm. PrepareCardSelection(0, "", true);
                }
            }

            //Marks piece as having a pawn and moves the pawn. Also unmarks the previous piece.
            CurrentPawn.GetComponent<PlayerPawn>().ClosestPieceToPawn().GetComponent<PieceController>().HasPawn = false;
            CurrentPawn.transform.position = gameObject.transform.position;
            HasPawn = true;
            CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();

            //Start of Morning Jog code
            bool hasMorningJog = _pcm.CheckForPersistentCard("Morning Jog", false);
            if(hasMorningJog)
            {
                if (CurrentPawn.GetComponent<PlayerPawn>().ClosestPieceToPawn().GetComponent<PieceController>().ObjState == GameState.One || CurrentPawn.GetComponent<PlayerPawn>().ClosestPieceToPawn().GetComponent<PieceController>().ObjState == GameState.Six)
                {
                    foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("PlayerPawn"))
                    {
                        if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
                        {
                            pawn.GetComponent<PlayerPawn>().MorningJogMove = true;
                        }
                    }

                    if (MovingForFree && !JustMovedForFree)
                    {
                        _am.StartMove(_am.CurrentPlayer);
                        _gcm.UpdateCurrentActionText("Take a Move with Morning Jog.");
                        JustMovedForFree = true;
                        yield break;
                    }
                    else if (MovingForFree && JustMovedForFree)
                    {
                        JustMovedForFree = false;
                        MovingForFree = false;
                    }
                }
            }
            //End of Morning Jog code

            if (_am.CurrentTurnPhase == 1)
            {
                _gcm.ToThenPhase();
            }
            else if(_am.CurrentTurnPhase == 2 || _am.CurrentTurnPhase == 3)
            {
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Method controlling Building placement and removal.
    /// </summary>
    private void StartBuildingPlacement()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
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

            int buildingIndex = 0;
            if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Factory")
            {
                buildingIndex = 0;
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Burrow")
            {
                buildingIndex = 1;
            }
            else if(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild == "Mine")
            {
                if(pieceSuit == "Grass")
                {
                    buildingIndex = 2;
                }
                else if(pieceSuit == "Dirt")
                {
                    buildingIndex = 3;
                }
                else if(pieceSuit == "Stone")
                {
                    buildingIndex = 4;
                }
            }

            bool areThereRemainingBuildings = _am.EnoughBuildingsRemaining(_am.CurrentPlayer, CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild);
            if(areThereRemainingBuildings)
            {
                StartCoroutine(BuildingCardSelection(CurrentPawn.GetComponent<PlayerPawn>().BuildingToBuild, buildingIndex, pieceSuit));
            }
        }
    }

    private IEnumerator BuildingCardSelection(string buildingName, int buildingIndex, string suitOfPiece)
    {
        _sr.color = _waitingColor;
        PieceIsSelected = true;
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            pawn.GetComponent<PlayerPawn>().HideNonSelectedTiles();
        }

        //Master Builder Code
        bool hasMasterBuilder = _pcm.CheckForPersistentCard("Master Builder", false);
        if (hasMasterBuilder)
        {
            _cm.PrepareCardSelection(_ce.NewBuildingCost, suitOfPiece, false);
        }
        else
        {
            if (_am.CurrentPlayer == 1)
            {
                if(buildingIndex == 0 || buildingIndex == 1)
                {
                    _cm.PrepareCardSelection(_am.P1CurrentBuildingPrices[buildingIndex], suitOfPiece, false);
                }
                else
                {
                    _cm.PrepareCardSelection(_am.P1CurrentBuildingPrices[2], suitOfPiece, false);
                }
            }
            else
            {
                if (buildingIndex == 0 || buildingIndex == 1)
                {
                    _cm.PrepareCardSelection(_am.P2CurrentBuildingPrices[buildingIndex], suitOfPiece, false);
                }
                else
                {
                    _cm.PrepareCardSelection(_am.P2CurrentBuildingPrices[2], suitOfPiece, false);
                }
            }
        }
        //End Master Builder Code

        while (!_cm.CheckCardSelection())
        {
            yield return null;
        }
        _cm.PrepareCardSelection(0, "", true);

        if (_am.CurrentPlayer == 1)
        {
            _am.P1CurrentBuildingPrices[buildingIndex]++;
            _am.P1RemainingBuildings[buildingIndex]--;
            _am.P1BuiltBuildings[buildingIndex]++;
        }
        else
        {
            _am.P2CurrentBuildingPrices[buildingIndex]++;
            _am.P2RemainingBuildings[buildingIndex]--;
            _am.P2BuiltBuildings[buildingIndex]++;
        }

        //Master Builder Code
        if(hasMasterBuilder)
        {
            //Discards Master Builder.
            _pcm.CheckForPersistentCard("Master Builder", true);
        }
        //End Master Builder

        InstantitateBuildingAndPawn(buildingName, buildingIndex, suitOfPiece);

        CurrentPawn.GetComponent<PlayerPawn>().UnassignAdjacentTiles();
        _gcm.Back();
        _gcm.UpdateCurrentActionText("Built " + buildingName + ".");
    }

    /// <summary>
    /// Places a building. Returns false and removes it if it's adjacent to another building. Also will spawn another Pawn if 3rd building is placed.
    /// </summary>
    /// <param name="building">"Factory" "Burrow" or "Mine"</param>
    private bool InstantitateBuildingAndPawn(string buildingName, int buildingArrayNum, string pieceSuit)
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
            GameObject thisBuilding = Instantiate(building, _buildingSlot);
            if(buildingArrayNum == 1)
            {
                thisBuilding.GetComponent<Building>().BuildingType = "Factory";
            }
            else if(buildingArrayNum == 2)
            {
                thisBuilding.GetComponent<Building>().BuildingType = "Burrow";
            }
            else if(buildingArrayNum == 3)
            {
                thisBuilding.GetComponent<Building>().BuildingType = "GMine";
            }
            else if(buildingArrayNum == 4)
            {
                thisBuilding.GetComponent<Building>().BuildingType = "DMine";
            }
            else if(buildingArrayNum == 5)
            {
                thisBuilding.GetComponent<Building>().BuildingType = "SMine";
            }
            thisBuilding.GetComponent<Building>().SuitOfPiece = pieceSuit;
            thisBuilding.GetComponent<Building>().PlayerOwning = _am.CurrentPlayer;

            //Planned Profit Code Start
            bool hasPlannedProfit = _pcm.CheckForPersistentCard("Planned Profit", true);

            if(hasPlannedProfit)
            {
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Grass");
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Dirt");
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Stone");
            }
            //Planned Profit Code End

            if (_am.CurrentPlayer == 1)
            {
                _am.ScorePoints(1);

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
            }
            else
            {
                _am.ScorePoints(1);

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
            }

            if (spawnPawn)
            {
                GameObject newPawn = Instantiate(_playerPawn, _buildingSlot);
                newPawn.GetComponent<PlayerPawn>().SetPawnToPlayer(_am.CurrentPlayer);
                newPawn.transform.SetParent(null);
            }

            if (_am.CurrentPlayer == 1)
            {
                HasP1Building = true;
            }
            else
            {
                HasP2Building = true;
            }

            return true;
        }
        else
        {
            _gcm.UpdateCurrentActionText("Cannot place " + building.name + " adjacent to another building.");
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
            case 6:
                gameObject.GetComponent<SpriteRenderer>().sprite = _flowerSprite;
                ObjState = GameState.Six;
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
            PieceIsSelected = false;
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
            PieceIsSelected = false;
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
            PieceIsSelected = false;
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
            PieceIsSelected = false;
            IsPlaceable = false;
        }
    }

    /// <summary>
    /// Updates tiles for earthquake.
    /// </summary>
    /// <param name="show">Show or Hide</param>
    public void ShowHideEarthquake(bool show)
    {
        if(show)
        {
            _sr.color = _selectedColor;
            IsEarthquakeable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            PieceIsSelected = false;
            IsEarthquakeable = false;
        }
    }

    /// <summary>
    /// Updates tiles for flipping.
    /// </summary>
    /// <param name="show">Show or Hide</param>
    public void ShowHideFlippable(bool show)
    {
        if (show)
        {
            _sr.color = _selectedColor;
            IsFlippable = true;
        }
        else
        {
            _sr.color = _defaultColor;
            PieceIsSelected = false;
            IsFlippable = false;
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