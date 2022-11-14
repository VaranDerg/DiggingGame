/*****************************************************************************
// File Name :         OnlinePieceController.cs
// Author :            Andrea Swihart-DeCoster & Rudy W.
// Creation Date :     October 3rd, 2022
//
// Brief Description : This document controls the players pieces.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEditor;
using UnityEngine.Apple;

public class OnlinePieceController : MonoBehaviourPun
{
    //Edited: Rudy W. Organized with headers, added certain variables for functionality.

    [Header("Piece References")]
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _stoneSprite;
    [SerializeField] private Sprite _bedrockSprite;
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private Sprite _flowerSprite;
    [SerializeField] private GameObject _molePawn;
    [SerializeField] private GameObject _meerkatPawn;
    [SerializeField] private SpriteRenderer _borderSr;

    [Header("OnlineBuilding References")]
    [SerializeField] private Transform _buildingSlot;
    [SerializeField] private GameObject _mFactory, _mBurrow, _mMine, _meeFactory, _meeBurrow, _meeMine;

    [Header("Tile Values/Information")]
    [HideInInspector] public bool IsMovable = false, IsDiggable = false, IsPlaceable = false, IsBuildable = false;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasP1Building, HasP2Building;
    [HideInInspector] public bool HasPawn;
    [HideInInspector]
    public GameObject CurrentPawn;
    [HideInInspector] public bool PieceIsSelected = true;
    private OnlineBoardManager _bm;
    private OnlineActionManager _am;
    private OnlineCardManager _cm;
    private OnlineCanvasManager _gcm;
    private OnlinePersistentCardManager _pcm;
    private OnlineCardEffects _ce;
    [HideInInspector] public bool HasGold;    //true if the piece reveals gold when flipped
    [HideInInspector] public bool CheckedByPawn;

    [Header("Card Activation Stuff")]
    [HideInInspector] public bool FromActivatedCard = false;
    [HideInInspector] public bool IsEarthquakeable;
    [HideInInspector] public bool UsingWalkway;
    [HideInInspector] public bool IsFlippable;
    [HideInInspector] public bool DiscerningEye;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem _grassPS;
    [SerializeField] private ParticleSystem _dirtPS;
    [SerializeField] private ParticleSystem _stonePS;
    [SerializeField] private ParticleSystem _goldPS;

    [Header("Lights")]
    [SerializeField] private GameObject _goldLight;

    [Header("Other")]
    private bool _pawnIsMoving;

    [Header("Animations")]
    [SerializeField] private Animator _borderAnims;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _waitingColor;
    [SerializeField] private GameObject _goldGlitter;

    // ASD
    [Header("Photon")]
    [SerializeField] private int _currentPawnID;     // Photon network ID of the currently selected pawn
    [SerializeField] private int _pieceID;       // Photon network ID of a piece
    [SerializeField] GameObject destinationPiece;

    private void Awake()
    {
        _bm = FindObjectOfType<OnlineBoardManager>();
        _am = FindObjectOfType<OnlineActionManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _ce = FindObjectOfType<OnlineCardEffects>();
        _pcm = FindObjectOfType<OnlinePersistentCardManager>();
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
        _borderSr.color = _defaultColor;
        _borderAnims.Play("PieceBorderIdle");
        _pieceID = photonView.ViewID;
    }

    /// <summary>
    /// For pawn movement.
    /// 
    /// Edited - Andrea SD: modified for online use
    /// </summary>
    private void FixedUpdate()
    {
        if (_pawnIsMoving)
        {
            GameObject pawn = GameObject.Find(PhotonView.Find(_currentPawnID).gameObject.name);     //ASD
            pawn.transform.position = Vector2.Lerp(pawn.transform.position, gameObject.transform.position, _am.PawnMoveSpeed * Time.deltaTime);
        }
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
        if (IsPlaceable)
        {
            PiecePlacement();
        }

        if (IsDiggable)
        {
            if (FromActivatedCard)
            {
                ActivatedPieceRemoval();
            }
            else
            {
                StartCoroutine(PieceRemoval());
            }
        }

        if (UsingWalkway)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(UseWalkway());
            }
        }

        if (IsMovable && CurrentPawn != null)
        {
            StartCoroutine(PawnMovement());
        }

        if (!HasP1Building && !HasP2Building && IsBuildable)
        {
            StartBuildingPlacement();
        }

        if (IsEarthquakeable)
        {
            UseEarthquake();
        }

        if (IsFlippable)
        {
            FlipPiece();
        }
    }

    /// <summary>
    /// Method controlling piece removal.
    /// Edited: Andrea SD - modified conditionals to no longer account for gold
    /// </summary>
    private IEnumerator PieceRemoval()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start of Shovel Code
            if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Shovel") && ObjState == GameState.Two && !_am.ShovelUsed)
            {
                CallPieceState(3);
                CallRemovalAnim(2);
                _am.ShovelUsed = true;
                if (CurrentPawn != null)
                {
                    CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
                }

                StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Dig", 1);

                _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
            }
            //End of Shovel Code

            else
            {
                _borderSr.color = _waitingColor;
                _borderAnims.Play("PieceBorderWaiting");
                PieceIsSelected = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
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

                StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Dig", 1);

                switch (ObjState)
                {
                    case GameState.One:
                        CallPieceState(2);
                        CallRemovalAnim(1);
                        _am.CollectTile(_am.CurrentPlayer, "Grass", true);
                        break;
                    case GameState.Two:
                        CallPieceState(3);
                        CallRemovalAnim(2);
                        _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
                        break;
                    case GameState.Three:
                        if (HasGold)
                        {
                            CallRemovalAnim(4);
                            _am.CollectTile(_am.CurrentPlayer, "Gold", true);
                        }
                        else
                        {
                            CallPieceState(4);
                            CallRemovalAnim(3);
                            _am.CollectTile(_am.CurrentPlayer, "Stone", true);
                        }

                        break;
                }

                if (CurrentPawn != null)
                {
                    CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
                }
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Uses the card walkway.
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    private IEnumerator UseWalkway()
    {
        // Andrea SD
        CallMovePawn(_currentPawnID, _pieceID);

        while (_pawnIsMoving)
        {
            yield return null;
        }

        UsingWalkway = false;
        SetPieceState(3);
        _grassPS.Play();
        _dirtPS.Play();
        _am.CollectTile(_am.CurrentPlayer, "Grass", false);
        _am.CollectTile(_am.CurrentPlayer, "Dirt", true);
    }

    /// <summary>
    /// Uses the card Earthquake.
    /// </summary>
    private void UseEarthquake()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _ce.EarthquakePieceSelected = true;

            foreach (GameObject piece in _bm.GenerateAdjacentPieceList(gameObject))
            {
                if (piece.GetComponentInChildren<OnlineBuilding>())
                {
                    piece.GetComponentInChildren<OnlineBuilding>().PrepBuilidingDamaging(true);
                    _ce.AllowedDamages++;
                }
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                piece.GetComponent<OnlinePieceController>().ShowHideEarthquake(false);
            }
        }
    }

    /// <summary>
    /// Flips a stone piece to see if it has gold or not.
    /// </summary>
    private void FlipPiece()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _ce.RemainingFlips--;

            if (HasGold)
            {
                SetPieceState(5);
                _goldPS.Play();
                if (DiscerningEye)
                {
                    _am.CallUpdateScore(_am.CurrentPlayer, 1);
                }
            }
            else
            {
                _stonePS.Play();
            }

            ShowHideFlippable(false);
        }
    }

    /// <summary>
    /// Method for digging tiles through effects instead of cards. 
    /// </summary>
    private void ActivatedPieceRemoval()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (ObjState == GameState.Four)
            {
                return;
            }

            ShowHideDiggable(false);
            FindObjectOfType<OnlineCardEffects>().DugPieces++;

            switch (ObjState)
            {
                case GameState.One:
                    SetPieceState(2);
                    _grassPS.Play();
                    _am.CollectTile(_am.CurrentPlayer, "Grass", false);
                    break;
                case GameState.Six:
                    SetPieceState(2);
                    _grassPS.Play();
                    _am.CollectTile(_am.CurrentPlayer, "Grass", false);
                    break;
                case GameState.Two:
                    SetPieceState(3);
                    _dirtPS.Play();
                    _am.CollectTile(_am.CurrentPlayer, "Dirt", false);
                    break;
                case GameState.Three:
                    SetPieceState(4);
                    if (HasGold)
                    {
                        _goldPS.Play();
                        _am.CollectTile(_am.CurrentPlayer, "Gold", false);
                    }
                    else
                    {
                        _stonePS.Play();
                        _am.CollectTile(_am.CurrentPlayer, "Stone", false);
                    }
                    break;
                case GameState.Five:
                    SetPieceState(4);
                    _goldPS.Play();
                    _am.CollectTile(_am.CurrentPlayer, "Gold", true);
                    FindObjectOfType<SFXManager>().Play("DigGold");
                    break;
            }

            FromActivatedCard = false;
        }
    }

    /// <summary>
    /// Allows the placement of pieces back onto the board.
    /// </summary>
    private void PiecePlacement()
    {
        if (!IsPlaceable)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
            _gcm.Back();

            switch (ObjState)
            {
                case GameState.Two:
                    CallPieceState(1);
                    CallRemovalAnim(1);
                    _am.PlaceTile("Grass");
                    break;
                case GameState.Three:
                    CallPieceState(2);
                    CallRemovalAnim(2);
                    _am.PlaceTile("Dirt");
                    break;
                case GameState.Four:
                    CallPieceState(3);
                    CallRemovalAnim(3);
                    _am.PlaceTile("Stone");
                    break;
            }
        }
    }

    /// <summary>
    /// Marks piece as having a pawn and moves the pawn. Also unmarks the previous piece.
    /// </summary>
    /// <param name="pawn">CurrentPawn, usually.</param>
    /// <param name="destinationPiece">This piece, usually.</param>
    /// <returns></returns>
    public IEnumerator MovePawnTo(int pawnID, int pieceID, bool goBack)
    {
        GameObject pawn = GameObject.Find(PhotonView.Find(pawnID).gameObject.name);
        destinationPiece = gameObject;

        pawn.GetComponent<OnlinePlayerPawn>().ClosestPieceToPawn().GetComponent<OnlinePieceController>().HasPawn = false;
        _pawnIsMoving = true;
        pawn.GetComponent<Animator>().Play(pawn.GetComponent<OnlinePlayerPawn>().MoveAnimName);

        //Start anim?
        yield return new WaitForSeconds(_am.PawnMoveAnimTime);
        //End anim?

        pawn.transform.position = destinationPiece.transform.position;
        pawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
        HasPawn = true;
        _pawnIsMoving = false;

        if (goBack)
        {
            // Only is called if in the right turn phase AND it's your turn
            if (photonView.IsMine)
            {
                _gcm.ToThenPhase();
            }
            else if (_am.CurrentTurnPhase == 2 || _am.CurrentTurnPhase == 3)
            {
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Controls pawn movement.
    /// Edited: Andrea SD - modified conditionals to no longer account for gold
    /// </summary>
    private IEnumerator PawnMovement()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //For the game's initial free move. The player has to spend cards unless this is true.
            if (_am.CurrentTurnPhase != 1 && _am.CurrentTurnPhase != 3)
            {
                _borderSr.color = _waitingColor;
                _borderAnims.Play("PieceBorderWaiting");
                PieceIsSelected = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
                }

                //Start of Morning Jog
                if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Morning Jog") && !_am.MorningJogUsed)
                {
                    if (ObjState == GameState.One || ObjState == GameState.Six)
                    {
                        _am.MorningJogUsed = true;
                    }
                    else
                    {
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
                    }
                }
                //End of Morning Jog

                else
                {
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
                }
            }
            CallPawnID(CurrentPawn.GetComponent<OnlinePlayerPawn>().PawnID);
            CallMovePawn(_currentPawnID, _pieceID);    //Andrea SD
        }
    }

    /// <summary>
    /// Method controlling OnlineBuilding placement and removal.
    /// </summary>
    private void StartBuildingPlacement()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            string pieceSuit = "";
            if (ObjState == GameState.One || ObjState == GameState.Six)
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
            if (CurrentPawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild == "Factory")
            {
                buildingIndex = 0;
            }
            else if (CurrentPawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild == "Burrow")
            {
                buildingIndex = 1;
            }
            else if (CurrentPawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild == "Mine")
            {
                if (pieceSuit == "Grass")
                {
                    buildingIndex = 2;
                }
                else if (pieceSuit == "Dirt")
                {
                    buildingIndex = 3;
                }
                else if (pieceSuit == "Stone")
                {
                    buildingIndex = 4;
                }
            }

            bool areThereRemainingBuildings = _am.EnoughBuildingsRemaining(_am.CurrentPlayer, CurrentPawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild);
            if (areThereRemainingBuildings)
            {
                StartCoroutine(BuildingCardSelection(CurrentPawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild, buildingIndex, pieceSuit));
            }
            else
            {
                _gcm.Back();
                _gcm.UpdateCurrentActionText("You've built all of those buildings!");
            }
        }
    }


    public IEnumerator BuildingCardSelection(string buildingName, int buildingIndex, string suitOfPiece)
    {
        _borderSr.color = _waitingColor;
        _borderAnims.Play("PieceBorderWaiting");
        PieceIsSelected = true;
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
        }

        //Master Builder Start
        int bCostReduction = 0;
        if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Master Builder"))
        {
            bCostReduction += _ce.BuildingReduction;
        }
        //End Master Builder

        if (suitOfPiece == "Gold")
        {
            bCostReduction -= _am.BuildingPriceGoldRaise;
        }

        if (_am.CurrentPlayer == 1)
        {
            if (buildingIndex == 0 || buildingIndex == 1)
            {
                _cm.PrepareCardSelection(_am.P1CurrentBuildingPrices[buildingIndex] - bCostReduction, suitOfPiece, false);
            }
            else
            {
                _cm.PrepareCardSelection(_am.P1CurrentBuildingPrices[2] - bCostReduction, suitOfPiece, false);
            }
        }
        else
        {
            if (buildingIndex == 0 || buildingIndex == 1)
            {
                _cm.PrepareCardSelection(_am.P2CurrentBuildingPrices[buildingIndex] - bCostReduction, suitOfPiece, false);
            }
            else
            {
                _cm.PrepareCardSelection(_am.P2CurrentBuildingPrices[2] - bCostReduction, suitOfPiece, false);
            }
        }

        while (!_cm.CheckCardSelection())
        {
            yield return null;
        }
        _cm.PrepareCardSelection(0, "", true);

        if (_am.CurrentPlayer == 1)
        {
            if (buildingIndex == 0 || buildingIndex == 1)
            {
                _am.P1CurrentBuildingPrices[buildingIndex]++;
                _am.P1RemainingBuildings[buildingIndex]--;
                _am.P1BuiltBuildings[buildingIndex]++;
            }
            else
            {
                _am.P1CurrentBuildingPrices[2]++;
                _am.P1RemainingBuildings[2]--;
                _am.P1BuiltBuildings[buildingIndex]++;
            }
        }
        else
        {
            if (buildingIndex == 0 || buildingIndex == 1)
            {
                _am.P2CurrentBuildingPrices[buildingIndex]++;
                _am.P2RemainingBuildings[buildingIndex]--;
                _am.P2BuiltBuildings[buildingIndex]++;
            }
            else
            {
                _am.P2CurrentBuildingPrices[2]++;
                _am.P2RemainingBuildings[2]--;
                _am.P2BuiltBuildings[buildingIndex]++;
            }
        }

        InstantitateBuildingAndPawn(buildingName, buildingIndex, suitOfPiece);

        CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
        _gcm.Back();
        _gcm.UpdateCurrentActionText("Built " + buildingName + ".");
    }

    /// <summary>
    /// Places a building. Returns false and removes it if it's adjacent to another building. Also will spawn another Pawn if 3rd building is placed.
    /// 
    /// Edited: Andrea SD - modified for online useS
    /// </summary>
    /// <param name="building">"Factory" "Burrow" or "Mine"</param>
    private bool InstantitateBuildingAndPawn(string buildingName, int buildingArrayNum, string pieceSuit)
    {
        GameObject building = null;
        if (_am.CurrentPlayer == 1)
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

        foreach (GameObject piece in _bm.GenerateAdjacentPieceList(gameObject))
        {
            if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
            {
                canPlaceOnTile = false;
            }
        }

        if (canPlaceOnTile)
        {
            bool spawnPawn = false;

            // Instantiates building from the Resources folder on the network
            // for all clients
            GameObject thisBuilding = PhotonNetwork.Instantiate(building.name,
                _buildingSlot.position, Quaternion.identity);   //Andrea SD

            StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Building", 1);

            if (buildingArrayNum == 0)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Factory";
            }
            else if (buildingArrayNum == 1)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Burrow";
            }
            else if (buildingArrayNum == 2)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Grass Mine";
            }
            else if (buildingArrayNum == 3)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Dirt Mine";
            }
            else if (buildingArrayNum == 4)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Stone Mine";
            }
            else if (buildingArrayNum == 5)
            {
                thisBuilding.GetComponent<OnlineBuilding>().BuildingType = "Gold Mine";
            }

            thisBuilding.GetComponent<OnlineBuilding>().SuitOfPiece = pieceSuit;
            thisBuilding.GetComponent<OnlineBuilding>().PlayerOwning = _am.CurrentPlayer;

            //Planned Profit Code Start
            if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Planned Profit"))
            {
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Grass");
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Dirt");
                _am.CollectPiecesFromSupply(_ce.PiecesToCollect, "Stone");
                _pcm.DiscardPersistentCard(_am.CurrentPlayer, "Planned Profit");
            }
            //Planned Profit Code End

            if (_am.CurrentPlayer == 1)
            {
                _am.CallUpdateScore(1, 1);  // Andrea SD

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
                _am.CallUpdateScore(2, 1);  // Andrea SD

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
                if (_am.CurrentPlayer == 1)
                {
                    Vector3 _buildingPlacement = _buildingSlot.transform.position;   // Andrea SD
                    GameObject newPawn = PhotonNetwork.Instantiate("MolePawnWorkEdition", _buildingPlacement, Quaternion.identity);   // Andrea SD
                    newPawn.GetComponent<OnlinePlayerPawn>().SetPawnToPlayer(_am.CurrentPlayer);
                    newPawn.transform.SetParent(null);
                    CallSetHasPawn(true);
                }
                else
                {
                    Vector3 _buildingPlacement = _buildingSlot.transform.position;   // Andrea SD
                    GameObject newPawn = PhotonNetwork.Instantiate("MeerkatPawn", _buildingPlacement, Quaternion.identity);   // Andrea SD
                    newPawn.GetComponent<OnlinePlayerPawn>().CallSetPawnPlayer(_am.CurrentPlayer);      // Andrea SD
                    newPawn.transform.SetParent(null);
                    CallSetHasPawn(true);
                }

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
    /// Changes the sprite of a piece
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="newSprite"></param>
    public void ChangeSprite(String newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(newSprite);
    }

    /// <summary>
    /// Updates tiles for player movement.
    /// </summary>
    public void ShowHideMovable(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsMovable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            IsMovable = false;
            PieceIsSelected = false;
            CheckedByPawn = false;
        }
    }

    /// <summary>
    /// Updates tiles for buildability.
    /// </summary>
    public void ShowHideBuildable(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsBuildable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            IsBuildable = false;
            PieceIsSelected = false;
            CheckedByPawn = false;
        }
    }

    /// <summary>
    /// Updates tiles for piece removal.
    /// </summary>
    public void ShowHideDiggable(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsDiggable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            PieceIsSelected = false;
            IsDiggable = false;
            CheckedByPawn = false;
        }
    }

    /// <summary>
    /// Updates tiles for piece placement.
    /// </summary>
    public void ShowHidePlaceable(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsPlaceable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            PieceIsSelected = false;
            IsPlaceable = false;
            CheckedByPawn = false;
        }
    }

    /// <summary>
    /// Updates tiles for earthquake.
    /// </summary>
    /// <param name="show">Show or Hide</param>
    public void ShowHideEarthquake(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsEarthquakeable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            PieceIsSelected = false;
            IsEarthquakeable = false;
            CheckedByPawn = false;
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
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            IsFlippable = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            PieceIsSelected = false;
            IsFlippable = false;
            CheckedByPawn = false;
            DiscerningEye = false;
        }
    }

    /// <summary>
    /// Updates tiles for walkway.
    /// </summary>
    /// <param name="show">Show or Hide</param>
    public void ShowHideWalkway(bool show)
    {
        if (show)
        {
            _borderSr.color = _selectedColor;
            _borderAnims.Play("PieceBorderWaiting");
            UsingWalkway = true;
            CheckedByPawn = true;
        }
        else
        {
            _borderSr.color = _defaultColor;
            _borderAnims.Play("PieceBorderIdle");
            UsingWalkway = false;
            PieceIsSelected = false;
            CheckedByPawn = false;
        }
    }

    /// <summary>
    /// Assigns the gold value to true;
    /// </summary>
    public void GiveGold()
    {
        HasGold = true;
    }

    #region RPC Functions

    /// <summary>
    /// Calls the RPC that plays the piece's animation when it's removed
    /// </summary>
    /// <param name="piece"> 1 = grass, 2 = dirt, 3 = stone, 4 = gold </param>
    public void CallRemovalAnim(int piece)
    {
        photonView.RPC("PieceRemovalAnim", RpcTarget.All, piece);
    }

    /// <summary>
    /// Plays the piece's animation when it's removed
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="piece"> 1 = grass, 2 = dirt, 3 = stone, 4 = gold </param>
    [PunRPC]
    public void PieceRemovalAnim(int piece)
    {
        switch (piece)
        {
            case 1:
                _grassPS.Play();
                break;
            case 2:
                _dirtPS.Play();
                break;
            case 3:
                _stonePS.Play();
                break;
            case 4:
                _goldPS.Play();
                break;
        }
    }

    /// <summary>
    /// Calls the MovePawn RPC which moves a pawn on the other players screen.
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="objectID"> Network ID of the moving pawn </param>
    /// <param name="destinationID"> Network ID of the pawn destination 
    /// </param>
    public void CallMovePawn(int objectID, int destinationID)
    {
        photonView.RPC("MovePawn", RpcTarget.All, objectID, destinationID);
    }

    /// <summary>
    /// Moves a pawn on the other players screen.
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="objectID"> Network ID of the moving pawn </param>
    /// <param name="destinationID"> Network ID of the pawn destination 
    /// </param>
    [PunRPC]
    public void MovePawn(int objectID, int destinationID)
    {
        StartCoroutine(MovePawnTo(objectID, destinationID, true));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newID"></param>
    public void CallPawnID(int newID)
    {
        photonView.RPC("SetCurrentPawnID", RpcTarget.All, newID);
    }

    /// <summary>
    /// Updates the current pawn id to newID
    /// </summary>
    /// <param name="newID"> Network id of the current pawn </param>
    [PunRPC]
    public void SetCurrentPawnID(int newID)
    {
        _currentPawnID = newID;
    }

    /// <summary>
    /// Calls the RPC that switches the state of the piece
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="state"></param>
    public void CallPieceState(int state)
    {
        photonView.RPC("SetPieceState", RpcTarget.All, state);
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// Edited: 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    [PunRPC]
    public void SetPieceState(int state)
    {
        //Debug.Log("Switching " + gameObject.name + "'s State to " + state + ".");

        switch (state)
        {
            // Each case calls each client connected to the server and
            // changes the game object sprite to a sprite from the resources
            case 1:
                ChangeSprite("GrassPiece");
                ObjState = GameState.One;
                break;
            case 2:
                ChangeSprite("DirtPiece");
                ObjState = GameState.Two;
                break;
            case 3:
                ChangeSprite("StonePiece");
                ObjState = GameState.Three;
                break;
            case 4:
                ChangeSprite("BedrockPiece");
                ObjState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
    }

    /// <summary>
    /// Calls the RPC that sets HawPawn to pawnCheck
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pawnCheck"></param>
    public void CallSetHasPawn(bool pawnCheck)
    {
        photonView.RPC("SetHasPawn", RpcTarget.All, pawnCheck);
    }

    /// <summary>
    /// Sets HasPawn to pawnCheck
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pawnCheck"> true of piece has a pawn </param>
    [PunRPC]
    public void SetHasPawn(bool pawnCheck)
    {
        HasPawn = pawnCheck;
    }

/*    /// <summary>
    /// Calls the RPC that places a building on all clients.
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="type"> the building being placed </param>
    /// <param name="xPos"> x pos of building </param>
    /// <param name="yPos"> y pos of building </param>
    public void CallBuilding(String type, float xPos, float yPos)
    {
        photonView.RPC("PlaceBuilding", RpcTarget.All, type, xPos, yPos);
    }

    /// <summary>
    /// Places the building on all other clients
    /// Author: Andrea SD
    /// </summary>
    /// <param name="type"> the building to be placed </param>
    /// <param name="xPos"> x pos of building </param>
    /// <param name="yPos"> y position of building </param>
    [PunRPC]
    private void PlaceBuilding(String type, float xPos, float yPos)
    {
        PhotonNetwork.Instantiate(type, new Vector3(xPos, yPos, 0f), Quaternion.identity);
    }*/

    #endregion
}
