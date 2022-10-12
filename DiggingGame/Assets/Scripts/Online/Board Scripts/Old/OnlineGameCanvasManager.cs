/*****************************************************************************
// File Name :         GameCanvasManager.cs
// Author :            Rudy Wolfer
// Creation Date :     October 3rd, 2022
//
// Brief Description : A script that updates the canvas during play.
*****************************************************************************//*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlineGameCanvasManager : MonoBehaviour
{
    //P1's Refs
    [Header("Text & Object References, Player 1")]
    [SerializeField] private GameObject _p1Everything;
    //Array for collected text objects. Grass, Dirt, Stone, and Gold counts.
    [SerializeField] private TextMeshProUGUI[] _p1CollectedPieces = new TextMeshProUGUI[4];
    //Array for refined text objects. Grass, Dirt, Stone, and Gold counts.
    [SerializeField] private TextMeshProUGUI[] _p1RefinedPieces = new TextMeshProUGUI[4];
    //Array for built buildings. Factories, Burrows, Mines.
    [SerializeField] private TextMeshProUGUI[] _p1BuiltBuildings = new TextMeshProUGUI[3];
    //Array for remaining buildings. Factories, Burrows, Mines.
    [SerializeField] private TextMeshProUGUI[] _p1RemainingBuildings = new TextMeshProUGUI[3];
    //Array for card info. Card Text, Gold Card Text, "Activate Cards" Text
    [SerializeField] private TextMeshProUGUI[] _p1CardInfo = new TextMeshProUGUI[3];
    //Array for Primary Canvas zones. "First" Zone, "Then" Zone, and "Finally" Zone.
    [SerializeField] private GameObject[] _p1PrimaryZoneObjRefs = new GameObject[3];
    //Array for Secondary Canvas zones. "Then" Actions, "Finally" Actions, Dig Zone, Build Zone, Build Mine Zone, Activate Card Zone, Move Zone.
    [SerializeField] private GameObject[] _p1SecondaryZoneObjRefs = new GameObject[7];

    //P2's Refs
    [Header("Text & Object References, Player 2")]
    [SerializeField] private GameObject _p2Everything;
    [SerializeField] private TextMeshProUGUI[] _p2CollectedPieces = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] _p2RefinedPieces = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] _p2BuiltBuildings = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] _p2RemainingBuildings = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] _p2CardInfo = new TextMeshProUGUI[3];
    [SerializeField] private GameObject[] _p2PrimaryZoneObjRefs = new GameObject[3];
    [SerializeField] private GameObject[] _p2SecondaryZoneObjRefs = new GameObject[7];

    //Universal Refs
    [Header("Constant Text & Object References")]
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private TextMeshProUGUI _playerText;
    [SerializeField] private GameObject _startTurnButton;
    //Array for Supply Text Refs
    [SerializeField] private TextMeshProUGUI[] _supplyPieces = new TextMeshProUGUI[4];

    //Other Misc Refs
    [Header("Other References")]
    private OnlineActionManager _am;
    private OnlineBoardManager _bm;

    //Code Things
    [Header("Other")]
    private List<GameObject> _canvasObjects = new List<GameObject>();

    private void Awake()
    {
        AssignToList();
        _am = FindObjectOfType<OnlineActionManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
    }

    /// <summary>
    /// Adds most Canvas Objects to a list.
    /// </summary>
    private void AssignToList()
    {
        _canvasObjects.Add(_startTurnButton);

        foreach(GameObject obj in _p1PrimaryZoneObjRefs)
        {
            _canvasObjects.Add(obj);
        }
        foreach(GameObject obj in _p1SecondaryZoneObjRefs)
        {
            _canvasObjects.Add(obj);
        }
        foreach(GameObject obj in _p2PrimaryZoneObjRefs)
        {
            _canvasObjects.Add(obj);
        }
        foreach(GameObject obj in _p2SecondaryZoneObjRefs)
        {
            _canvasObjects.Add(obj);
        }
    }

    /// <summary>
    /// Runs necessary starting functions and assigns some variables. 
    /// </summary>
    private void Start()
    {
        DisableObjects();
        _startTurnButton.SetActive(true);
        UpdateAllText();
    }

    /// <summary>
    /// Updates every text element in the scene.
    /// </summary>
    public void UpdateAllText()
    {
        //P1 Text
        _p1CollectedPieces[0].text = "x" + _am.P1CollectedPile[0].ToString();
        _p1CollectedPieces[1].text = "x" + _am.P1CollectedPile[1].ToString();
        _p1CollectedPieces[2].text = "x" + _am.P1CollectedPile[2].ToString();
        _p1CollectedPieces[3].text = "x" + _am.P1CollectedPile[3].ToString();
        _p1RefinedPieces[0].text = "x" + _am.P1RefinedPile[0].ToString();
        _p1RefinedPieces[1].text = "x" + _am.P1RefinedPile[1].ToString();
        _p1RefinedPieces[2].text = "x" + _am.P1RefinedPile[2].ToString();
        _p1RefinedPieces[3].text = "x" + _am.P1RefinedPile[3].ToString();
        _p1BuiltBuildings[0].text = "x" + _am.P1BuiltBuildings[0].ToString();
        _p1BuiltBuildings[1].text = "x" + _am.P1BuiltBuildings[1].ToString();
        _p1BuiltBuildings[2].text = "x" + (_am.P1BuiltBuildings[2] + _am.P1BuiltBuildings[3] + _am.P1BuiltBuildings[4]).ToString();
        _p1RemainingBuildings[0].text = _am.P1RemainingBuildings[0].ToString() + " Left";
        _p1RemainingBuildings[1].text = _am.P1RemainingBuildings[1].ToString() + " Left";
        _p1RemainingBuildings[2].text = _am.P1RemainingBuildings[2].ToString() + " Left";
        _p1CardInfo[0].text = _am.P1Cards.ToString() + " Cards";
        _p1CardInfo[1].text = _am.P1GoldCards.ToString() + " Gold Cards";
        _p1CardInfo[2].text = "Activate up to " + (_am.CardActivations + _am.P1BuiltBuildings[1]).ToString() + " Cards.";

        //P2Text
        _p2CollectedPieces[0].text = "x" + _am.P2CollectedPile[0].ToString();
        _p2CollectedPieces[1].text = "x" + _am.P2CollectedPile[1].ToString();
        _p2CollectedPieces[2].text = "x" + _am.P2CollectedPile[2].ToString();
        _p2CollectedPieces[3].text = "x" + _am.P2CollectedPile[3].ToString();
        _p2RefinedPieces[0].text = "x" + _am.P2RefinedPile[0].ToString();
        _p2RefinedPieces[1].text = "x" + _am.P2RefinedPile[1].ToString();
        _p2RefinedPieces[2].text = "x" + _am.P2RefinedPile[2].ToString();
        _p2RefinedPieces[3].text = "x" + _am.P2RefinedPile[3].ToString();
        _p2BuiltBuildings[0].text = "x" + _am.P2BuiltBuildings[0].ToString();
        _p2BuiltBuildings[1].text = "x" + _am.P2BuiltBuildings[1].ToString();
        _p2BuiltBuildings[2].text = "x" + (_am.P2BuiltBuildings[2] + _am.P2BuiltBuildings[3] + _am.P2BuiltBuildings[4]).ToString();
        _p2RemainingBuildings[0].text = _am.P2RemainingBuildings[0].ToString() + " Left";
        _p2RemainingBuildings[1].text = _am.P2RemainingBuildings[1].ToString() + " Left";
        _p2RemainingBuildings[2].text = _am.P2RemainingBuildings[2].ToString() + " Left";
        _p2CardInfo[0].text = _am.P2Cards.ToString() + " Cards";
        _p2CardInfo[1].text = _am.P2GoldCards.ToString() + " Gold Cards";
        _p2CardInfo[2].text = "Activate up to " + (_am.CardActivations + _am.P2BuiltBuildings[1]).ToString() + " Cards.";

        //Universal Text
        _turnText.text = "Start Turn " + _am.CurrentTurn.ToString();
        _roundText.text = "Round: " + _am.CurrentRound.ToString();
        _playerText.text = "Player " + _am.CurrentPlayer.ToString();
        _supplyPieces[0].text = "x" + _am.SupplyPile[0].ToString();
        _supplyPieces[1].text = "x" + _am.SupplyPile[1].ToString();
        _supplyPieces[2].text = "x" + _am.SupplyPile[2].ToString();
        _supplyPieces[3].text = "x" + _am.SupplyPile[3].ToString();
    }

    /// <summary>
    /// Disables every object in the scene.
    /// </summary>
    private void DisableObjects()
    {
        foreach(GameObject thing in _canvasObjects)
        {
            thing.SetActive(false);
        }
    }

    /// <summary>
    /// Begins a player's turn.
    /// </summary>
    public void StartTurn()
    {
        DisableObjects();

        if(_am.CurrentPlayer == 1)
        {
            _p1Everything.SetActive(true);
            _p1PrimaryZoneObjRefs[0].SetActive(true);
        }
        else if(_am.CurrentPlayer == 2)
        {
            _p2Everything.SetActive(true);
            _p2PrimaryZoneObjRefs[0].SetActive(true);
        }
        _am.RefineTiles(_am.CurrentPlayer);
        _am.ActivateMines(_am.CurrentPlayer);
        _am.StartMove(_am.CurrentPlayer);
        _am.CurrentTurnPhase++;

        UpdateAllText();
    }

    /// <summary>
    /// Moves to the "Then" phase
    /// </summary>
    public void ToThenPhase()
    {
        DisableObjects();
        _bm.DisablePawnBoardInteractions();

        if(_am.CurrentPlayer == 1)
        {
            _p1PrimaryZoneObjRefs[1].SetActive(true);
            _p1SecondaryZoneObjRefs[0].SetActive(true);
        }
        else if(_am.CurrentPlayer == 2)
        {
            _p2PrimaryZoneObjRefs[1].SetActive(true);
            _p2SecondaryZoneObjRefs[0].SetActive(true);
        }
        _am.CurrentTurnPhase++;

        UpdateAllText();
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    public void Move()
    {
        DisableObjects();

        if(!_am.SpendCards(_am.CurrentPlayer))
        {
            return;
        }

        _bm.DisablePawnBoardInteractions();
        _am.StartMove(_am.CurrentPlayer);

        if (_am.CurrentPlayer == 1)
        {
            _p1PrimaryZoneObjRefs[1].SetActive(true);
            _p1SecondaryZoneObjRefs[6].SetActive(true);
        }
        else if (_am.CurrentPlayer == 2)
        {
            _p2PrimaryZoneObjRefs[1].SetActive(true);
            _p2SecondaryZoneObjRefs[6].SetActive(true);
        }

        UpdateAllText();
    }

    /// <summary>
    /// (OLD) Opens the Dig Tile menu
    /// </summary>
    //public void Dig()
    //{
    //    _bm.DisableActiveBoardPieces();
    //    DisableObjects();

    //    if(_am.CurrentPlayer == 1)
    //    {
    //        _p1PrimaryZoneObjRefs[1].SetActive(true);
    //        _p1SecondaryZoneObjRefs[2].SetActive(true);
    //    }
    //    else if(_am.CurrentPlayer == 2)
    //    {
    //        _p2PrimaryZoneObjRefs[1].SetActive(true);
    //        _p2SecondaryZoneObjRefs[2].SetActive(true);
    //    }

    //    UpdateAllText();
    //}

    /// <summary>
    /// Prepares pawn for tile digging.
    /// </summary>
    public void DigTile()
    {
        if(_am.SpendCards(_am.CurrentPlayer))
        {
            _bm.DisablePawnBoardInteractions();
            _am.StartDig(_am.CurrentPlayer);
        }

        UpdateAllText();
    }

    /// <summary>
    /// Opens the Build Menu
    /// </summary>
    public void Build()
    {
        DisableObjects();
        _bm.DisablePawnBoardInteractions();

        if (_am.CurrentPlayer == 1)
        {
            _p1PrimaryZoneObjRefs[1].SetActive(true);
            _p1SecondaryZoneObjRefs[3].SetActive(true);
        }
        else if(_am.CurrentPlayer == 2)
        {
            _p2PrimaryZoneObjRefs[1].SetActive(true);
            _p2SecondaryZoneObjRefs[3].SetActive(true);
        }

        UpdateAllText();
    }

    /// <summary>
    /// Retrieves Gold
    /// </summary>
    public void Retrieve()
    {
        if(_am.UseGold(_am.CurrentPlayer))
        {
            Debug.Log("Player " + _am.CurrentPlayer + " Refined Gold!");
        }

        UpdateAllText();
    }

    /// <summary>
    /// Builds a building.
    /// </summary>
    /// <param name="buildingName">"Factory" "Burrow" or "Mine"</param>
    public void BuildNewBuilding(string buildingName)
    {
        _bm.DisablePawnBoardInteractions();
        _am.StartBuild(_am.CurrentPlayer, buildingName);

        UpdateAllText();
    }

    /// <summary>
    /// (OLD) Opens the Mine Menu.
    /// </summary>
    //public void OpenMineMenu()
    //{
    //    DisableObjects();

    //    if(_am.CurrentPlayer == 1)
    //    {
    //        _p1PrimaryZoneObjRefs[1].SetActive(true);
    //        _p1SecondaryZoneObjRefs[4].SetActive(true);
    //    }
    //    else if(_am.CurrentPlayer == 2)
    //    {
    //        _p2PrimaryZoneObjRefs[1].SetActive(true);
    //        _p2SecondaryZoneObjRefs[4].SetActive(true);
    //    }

    //    UpdateAllText();
    //}

    /// <summary>
    /// Moves to the "Finally" phase
    /// </summary>
    public void ToFinallyPhase()
    {
        _am.CurrentTurnPhase++;
        DisableObjects();
        _bm.DisablePawnBoardInteractions();

        if (_am.CurrentPlayer == 1)
        {
            _p1PrimaryZoneObjRefs[2].SetActive(true);
            _p1SecondaryZoneObjRefs[1].SetActive(true);
        }
        else if(_am.CurrentPlayer == 2)
        {
            _p2PrimaryZoneObjRefs[2].SetActive(true);
            _p2SecondaryZoneObjRefs[1].SetActive(true);
        }

        UpdateAllText();
    }

    /// <summary>
    /// Opens the Card Activation menu
    /// </summary>
    public void ActivateMenu()
    {
        DisableObjects();

        if(_am.CurrentPlayer == 1)
        {
            _p1PrimaryZoneObjRefs[2].SetActive(true);
            _p1SecondaryZoneObjRefs[5].SetActive(true);
        }
        else if(_am.CurrentPlayer == 2)
        {
            _p2PrimaryZoneObjRefs[2].SetActive(true);
            _p2SecondaryZoneObjRefs[5].SetActive(true);
        }

        UpdateAllText();
    }

    /// <summary>
    /// Activates a Card
    /// </summary>
    public void ActivateCard()
    {
        Debug.Log("Wow, Player " + _am.CurrentPlayer + ", you activated a card!");
    }

    /// <summary>
    /// Goes Back in a Menu
    /// </summary>
    public void Back()
    {
        DisableObjects();
        _bm.DisablePawnBoardInteractions();

        if (_am.CurrentPlayer == 1)
        {
            if (_am.CurrentTurnPhase == 2)
            {
                _p1PrimaryZoneObjRefs[1].SetActive(true);
                _p1SecondaryZoneObjRefs[0].SetActive(true);
            }
            else if (_am.CurrentTurnPhase == 3)
            {
                _p1PrimaryZoneObjRefs[2].SetActive(true);
                _p1SecondaryZoneObjRefs[1].SetActive(true);
            }
        }
        else if(_am.CurrentPlayer == 2)
        {
            if (_am.CurrentTurnPhase == 2)
            {
                _p2PrimaryZoneObjRefs[1].SetActive(true);
                _p2SecondaryZoneObjRefs[0].SetActive(true);
            }
            else if (_am.CurrentTurnPhase == 3)
            {
                _p2PrimaryZoneObjRefs[2].SetActive(true);
                _p2SecondaryZoneObjRefs[1].SetActive(true);
            }
        }

        UpdateAllText();
    }

    /// <summary>
    /// Ends the current player's turn
    /// </summary>
    public void EndTurn()
    {
        DisableObjects();

        if(_am.CurrentPlayer == 1)
        {
            _am.DrawCards(_am.CurrentPlayer, _am.CardDraw + _am.P1BuiltBuildings[0]);
            _am.DiscardCards(_am.CurrentPlayer);
            _am.CurrentTurnPhase = 0;
            _startTurnButton.SetActive(true);
            _p1Everything.SetActive(false);
            _am.CurrentTurn++;
            _am.CurrentPlayer = 2;
        }
        else if(_am.CurrentPlayer == 2)
        {
            _am.DrawCards(_am.CurrentPlayer, _am.CardDraw + _am.P2BuiltBuildings[0]);
            _am.DiscardCards(_am.CurrentPlayer);
            _am.CurrentTurnPhase = 0;
            _startTurnButton.SetActive(true);
            _p2Everything.SetActive(false);
            _am.CurrentTurn++;
            _am.CurrentRound++;
            _am.CurrentPlayer = 1;
        }

        UpdateAllText();
    }
}
*/