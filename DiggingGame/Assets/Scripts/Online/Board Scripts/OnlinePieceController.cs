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
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEditor;

public class OnlinePieceController : MonoBehaviourPun
{
    //Edited: Rudy W. Organized with headers, added certain variables for functionality.

    [Header("Piece References")]
/*    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _stoneSprite;
    [SerializeField] private Sprite _bedrockSprite;
    [SerializeField] private Sprite _goldSprite;*/
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
    private OnlineBoardManager _bm;
    private OnlineActionManager _am;
    private OnlineCardManager _cm;
    private OnlineCanvasManager _gcm;
    [HideInInspector] public bool HasGold;    //true if the piece reveals gold when flipped

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _am = FindObjectOfType<OnlineActionManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
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
        if (IsPlaceable)
        {
            PiecePlacement();
        }

        if (IsDiggable)
        {
            StartCoroutine(PieceRemoval());
        }

        if (IsMovable && CurrentPawn != null)
        {
            StartCoroutine(PawnMovement());
        }

        if (!HasP1Building && !HasP2Building && IsBuildable)
        {
            StartBuildingPlacement();
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
            PieceIsSelected = true;
            foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
            {
                pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
            }

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
            _cm.PrepareCardSelection(0, "", true);

            CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
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

                    if (HasGold)
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
                    photonView.RPC("SetPieceState", RpcTarget.All, 1);
                    _am.PlaceTile(_am.CurrentPlayer, "Grass");
                    break;
                case GameState.Three:
                    photonView.RPC("SetPieceState", RpcTarget.All, 2);
                    _am.PlaceTile(_am.CurrentPlayer, "Dirt");
                    break;
                case GameState.Four:
                    photonView.RPC("SetPieceState", RpcTarget.All, 3);
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //For the game's initial free move. The player has to spend cards unless this is true.
            if (_am.CurrentTurnPhase != 1)
            {
                _sr.color = _waitingColor;
                PieceIsSelected = true;
                foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
                {
                    pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
                }

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
                _cm.PrepareCardSelection(0, "", true);
            }

            //Marks piece as having a pawn and moves the pawn. Also unmarks the previous piece.
            CurrentPawn.GetComponent<OnlinePlayerPawn>().ClosestPieceToPawn().GetComponent<OnlinePieceController>().HasPawn = false;
            CurrentPawn.transform.position = gameObject.transform.position;
            HasPawn = true;
            CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();

            if (_am.CurrentTurnPhase == 1)
            {
                _gcm.ToThenPhase();
            }
            else if (_am.CurrentTurnPhase == 2)
            {
                _gcm.Back();
            }
        }
    }

    /// <summary>
    /// Moves a pawn on the other players screen.
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pawn"> pawn that will be moved </param>
    /// <param name="newPosition"> position where pawn will be moved to </param>
    [PunRPC]
    private void MovePawn(float newX, float newY)
    {
        if(_am.CurrentPlayer  == 1)
        {
            GameObject.Find("Player1Pawn").transform.position = new Vector2(newX, newY);
        }
        else
        {
            GameObject.Find("Player2Pawn").transform.position = new Vector2(newX, newY);
        }
        HasPawn = true;
    }


    /// <summary>
    /// Method controlling Building placement and removal.
    /// </summary>
    private void StartBuildingPlacement()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
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
        }
    }


    private IEnumerator BuildingCardSelection(string buildingName, int buildingIndex, string suitOfPiece)
    {
        _sr.color = _waitingColor;
        PieceIsSelected = true;
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            pawn.GetComponent<OnlinePlayerPawn>().HideNonSelectedTiles();
        }

        if (_am.CurrentPlayer == 1)
        {
            _cm.PrepareCardSelection(_am.P1CurrentBuildingPrices[buildingIndex], suitOfPiece, false);
        }
        else
        {
            _cm.PrepareCardSelection(_am.P2CurrentBuildingPrices[buildingIndex], suitOfPiece, false);
        }

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

        InstantitateBuildingAndPawn(buildingName);

        CurrentPawn.GetComponent<OnlinePlayerPawn>().UnassignAdjacentTiles();
        _gcm.Back();
        _gcm.UpdateCurrentActionText("Built " + buildingName + ".");
    }

    /// <summary>
    /// Places a building. Returns false and removes it if it's adjacent to another building. Also will spawn another Pawn if 3rd building is placed.
    /// </summary>
    /// <param name="building">"Factory" "Burrow" or "Mine"</param>
    private bool InstantitateBuildingAndPawn(string buildingName)
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
            Instantiate(building, _buildingSlot);

            if (_am.CurrentPlayer == 1)
            {
                _am.P1Score++;

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
                _am.P2Score++;

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
                newPawn.GetComponent<OnlinePlayerPawn>().SetPawnToPlayer(_am.CurrentPlayer);
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
            Debug.Log("Cannot place " + building.name + " adjacent to another building.");
            return false;
        }
    }

    public void ChangeSprite(String newSprite)
    {
        _sr.sprite = Resources.Load<Sprite>(newSprite);
    }

    /// <summary>
    /// Updates tiles for player movement.
    /// </summary>
    public void ShowHideMovable(bool show)
    {
        if (show)
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
    /// Assigns the gold value to true;
    /// </summary>
    public void GiveGold()
    {
        HasGold = true;
    }

    /// <summary>
    /// Places the building on all other clients
    /// Author: Andrea SD
    /// </summary>
    /// <param name="type"> the building to be placed </param>
    [PunRPC]
    private void PlaceBuilding(String type, float xPos, float yPos)
    {
        PhotonNetwork.Instantiate(type, new Vector3(xPos, yPos, 0f), Quaternion.identity);
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// Edited: 
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
            case 5:
                ChangeSprite("GoldPiece");
                ObjState = GameState.Five;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
    }
}
