/*****************************************************************************
// File Name :         GameCanvasManagerNew.cs
// Author :            Rudy Wolfer
// Creation Date :     October 10th, 2022
//
// Brief Description : Better version of GameCanvasManager. A script that 
                       updates the canvas during play.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCanvasManagerNew : MonoBehaviour
{
    [Header("Text and Object References, Current Player View")]
    public GameObject StartTurnButton;
    [SerializeField] private GameObject _firstZone;
    [SerializeField] private GameObject _thenZone;
    [SerializeField] private GameObject _finallyZone;
    [SerializeField] private GameObject _thenActions;
    [SerializeField] private GameObject _thenBuildMenu;
    [SerializeField] private GameObject _thenMDRMenu;
    [SerializeField] private GameObject _endPhaseButton;
    [SerializeField] private TextMeshProUGUI _currentPlayerScore;
    [SerializeField] private TextMeshProUGUI[] _currentPlayerCollectedPieces = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] _currentPlayerRefinedPieces = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] _currentPlayerRemainingBuildings = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] _currentPlayerRemainingBuildingCost = new TextMeshProUGUI[3];

    [Header("Text and Object References, Always Active View")]
    [SerializeField] private TextMeshProUGUI _activePlayerText;
    [SerializeField] private TextMeshProUGUI _activeRoundText;
    [SerializeField] private TextMeshProUGUI _currentActionText;
    [SerializeField] private TextMeshProUGUI _showHideOpponentInfoText;
    [SerializeField] private TextMeshProUGUI[] _supplyPieces = new TextMeshProUGUI[4];

    [Header("Text and Object References, Opponent View")]
    [SerializeField] private GameObject _opponentInfoZone;
    [SerializeField] private TextMeshProUGUI _opponentScoreText;
    [SerializeField] private TextMeshProUGUI _opponentCardText;
    [SerializeField] private TextMeshProUGUI _opponentGoldCardText;
    [SerializeField] private TextMeshProUGUI[] _opponentPieces = new TextMeshProUGUI[4];

    [Header("Other References")]
    public bool _opponentViewShowing = false;
    [SerializeField] private Sprite _moleFactory, _moleBurrow, _moleMine, _meerkatFactory, _meerkatBurrow, _meerkatMine;
    [SerializeField] private Image _factory, _burrow, _mine;
    private ActionManager _am;
    private BoardManager _bm;
    private CardManager _cm;

    [Header("Other")]
    private List<GameObject> _allObjects = new List<GameObject>();

    /// <summary>
    /// Adds most Canvas Objects to a list. Does not add the Opponent Info Zone, since that can be left opened or closed.
    /// </summary>
    private void AddObjectsToList()
    {
        _allObjects.Add(StartTurnButton);
        _allObjects.Add(_firstZone);
        _allObjects.Add(_thenZone);
        _allObjects.Add(_finallyZone);
        _allObjects.Add(_thenActions);
        _allObjects.Add(_thenBuildMenu);
        _allObjects.Add(_thenMDRMenu);
        _allObjects.Add(_endPhaseButton);
    }

    /// <summary>
    /// Assigns other partner scripts and calls AddObjectsToList.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _cm = FindObjectOfType<CardManager>();
        AddObjectsToList();
    }

    /// <summary>
    /// Disables every object in AllObjects
    /// </summary>
    private void DisableListObjects()
    {
        foreach (GameObject obj in _allObjects)
        {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// Updates text for the current player.
    /// </summary>
    /// <param name="curPlayer">1 or 2</param>
    private void UpdateCurrentPlayerText(int curPlayer)
    {
        if (curPlayer == 1)
        {
            _currentPlayerScore.text = "Score: " + _am.P1Score;
            _currentPlayerCollectedPieces[0].text = "x" + _am.P1CollectedPile[0];
            _currentPlayerCollectedPieces[1].text = "x" + _am.P1CollectedPile[1];
            _currentPlayerCollectedPieces[2].text = "x" + _am.P1CollectedPile[2];
            _currentPlayerCollectedPieces[3].text = "x" + _am.P1CollectedPile[3];
            _currentPlayerRefinedPieces[0].text = "x" + _am.P1RefinedPile[0];
            _currentPlayerRefinedPieces[1].text = "x" + _am.P1RefinedPile[1];
            _currentPlayerRefinedPieces[2].text = "x" + _am.P1RefinedPile[2];
            _currentPlayerRefinedPieces[3].text = "x" + _am.P1RefinedPile[3];
            _currentPlayerRemainingBuildings[0].text = _am.P1RemainingBuildings[0] + " Left";
            _currentPlayerRemainingBuildings[1].text = _am.P1RemainingBuildings[1] + " Left";
            _currentPlayerRemainingBuildings[2].text = _am.P1RemainingBuildings[2] + " Left";
            _currentPlayerRemainingBuildingCost[0].text = "Cost " + _am.P1CurrentBuildingPrices[0];
            _currentPlayerRemainingBuildingCost[1].text = "Cost " + _am.P1CurrentBuildingPrices[1];
            _currentPlayerRemainingBuildingCost[2].text = "Cost " + _am.P1CurrentBuildingPrices[2];
        }
        else
        {
            _currentPlayerScore.text = "Score: " + _am.P2Score;
            _currentPlayerCollectedPieces[0].text = "x" + _am.P2CollectedPile[0];
            _currentPlayerCollectedPieces[1].text = "x" + _am.P2CollectedPile[1];
            _currentPlayerCollectedPieces[2].text = "x" + _am.P2CollectedPile[2];
            _currentPlayerCollectedPieces[3].text = "x" + _am.P2CollectedPile[3];
            _currentPlayerRefinedPieces[0].text = "x" + _am.P2RefinedPile[0];
            _currentPlayerRefinedPieces[1].text = "x" + _am.P2RefinedPile[1];
            _currentPlayerRefinedPieces[2].text = "x" + _am.P2RefinedPile[2];
            _currentPlayerRefinedPieces[3].text = "x" + _am.P2RefinedPile[3];
            _currentPlayerRemainingBuildings[0].text = _am.P2RemainingBuildings[0] + " Left";
            _currentPlayerRemainingBuildings[1].text = _am.P2RemainingBuildings[1] + " Left";
            _currentPlayerRemainingBuildings[2].text = _am.P2RemainingBuildings[2] + " Left";
            _currentPlayerRemainingBuildingCost[0].text = "Cost " + _am.P2CurrentBuildingPrices[0];
            _currentPlayerRemainingBuildingCost[1].text = "Cost " + _am.P2CurrentBuildingPrices[1];
            _currentPlayerRemainingBuildingCost[2].text = "Cost " + _am.P2CurrentBuildingPrices[2];
        }
    }

    /// <summary>
    /// Updates the Always Active Text.
    /// </summary>
    private void UpdateAlwaysActiveText()
    {
        _activePlayerText.text = "Player " + _am.CurrentPlayer;
        _activeRoundText.text = "Round " + _am.CurrentRound;
        _supplyPieces[0].text = "x" + _am.SupplyPile[0];
        _supplyPieces[1].text = "x" + _am.SupplyPile[1];
        _supplyPieces[2].text = "x" + _am.SupplyPile[2];
        _supplyPieces[3].text = "x" + _am.SupplyPile[3];
    }

    /// <summary>
    /// Updates the Opponent View text. 
    /// </summary>
    /// <param name="curOpponent">1 or 2</param>
    private void UpdateOpponentText(int curOpponent)
    {
        if (curOpponent == 1)
        {
            _opponentScoreText.text = "Score: " + _am.P1Score;
            _opponentCardText.text = "Cards: " + _am.P1Cards;
            _opponentGoldCardText.text = "Gold Cards: " + _am.P1GoldCards;
            _opponentPieces[0].text = "Grass: " + (_am.P1CollectedPile[0] + _am.P1RefinedPile[0]);
            _opponentPieces[1].text = "Dirt: " + (_am.P1CollectedPile[1] + _am.P1RefinedPile[1]);
            _opponentPieces[2].text = "Stone: " + (_am.P1CollectedPile[2] + _am.P1RefinedPile[2]);
            _opponentPieces[3].text = "Gold: " + (_am.P1CollectedPile[3] + _am.P1RefinedPile[3]);
        }
        else
        {
            _opponentScoreText.text = "Score: " + _am.P2Score;
            _opponentCardText.text = "Cards: " + _am.P2Cards;
            _opponentGoldCardText.text = "Gold Cards: " + _am.P2GoldCards;
            _opponentPieces[0].text = "Grass: " + (_am.P2CollectedPile[0] + _am.P2RefinedPile[0]);
            _opponentPieces[1].text = "Dirt: " + (_am.P2CollectedPile[1] + _am.P2RefinedPile[1]);
            _opponentPieces[2].text = "Stone: " + (_am.P2CollectedPile[2] + _am.P2RefinedPile[2]);
            _opponentPieces[3].text = "Gold: " + (_am.P2CollectedPile[3] + _am.P2RefinedPile[3]);
        }
    }

    /// <summary>
    /// Updates the Current Action text.
    /// </summary>
    /// <param name="updatedText">New text to show</param>
    public void UpdateCurrentActionText(string updatedText)
    {
        _currentActionText.text = updatedText;
    }

    /// <summary>
    /// Calls every "UpdateText"-type function except the Current Action Text one.
    /// </summary>
    public void UpdateTextBothPlayers()
    {
        UpdateCurrentPlayerText(_am.CurrentPlayer);
        UpdateAlwaysActiveText();
        if (_am.CurrentPlayer == 1)
        {
            UpdateOpponentText(2);
        }
        else
        {
            UpdateOpponentText(1);
        }
    }

    /// <summary>
    /// Sets the game into a basic opening state upon startup.
    /// </summary>
    private void Start()
    {
        DisableListObjects();

        StartTurnButton.SetActive(true);
        _opponentInfoZone.SetActive(false);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Press Start Turn to begin!");
    }

    public void OpponentInfoToggle()
    {
        if(_opponentViewShowing)
        {
            _opponentInfoZone.SetActive(false);
            _showHideOpponentInfoText.text = "Show Opponent Info";
            _opponentViewShowing = false;
        }
        else
        {
            _opponentInfoZone.SetActive(true);
            _showHideOpponentInfoText.text = "Hide Opponent Info";
            _opponentViewShowing = true;
        }
    }

    /// <summary>
    /// Moves the turn into the "First..." phase.
    /// </summary>
    public void StartTurn()
    {
        DisableListObjects();

        _firstZone.SetActive(true);
        _am.DrawStartingCards();
        _am.RefineTiles(_am.CurrentPlayer);
        _am.ActivateMines(_am.CurrentPlayer);
        _am.StartMove(_am.CurrentPlayer);
        _am.CurrentTurnPhase++;
        _cm.ShowCards(_am.CurrentPlayer);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to move, then a Piece to move onto.");
    }

    /// <summary>
    /// Moves the turn into the "Then..." phase.
    /// </summary>
    public void ToThenPhase()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();
        _am.CurrentTurnPhase++;

        _thenZone.SetActive(true);
        _thenActions.SetActive(true);
        _endPhaseButton.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select an Action.");
    }

    /// <summary>
    /// Prepares a Move action.
    /// </summary>
    public void Move()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();

        _am.StartMove(_am.CurrentPlayer);
        _thenZone.SetActive(true);
        _thenMDRMenu.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to Move, then a Piece to move onto.");
    }

    /// <summary>
    /// Prepares a Dig action.
    /// </summary>
    public void Dig()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();

        _am.StartDig(_am.CurrentPlayer);
        _thenZone.SetActive(true);
        _thenMDRMenu.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to Dig with, then a Piece to Dig.");
    }

    /// <summary>
    /// Opens the Build menu.
    /// </summary>
    public void OpenBuildMenu()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();

        _thenZone.SetActive(true);
        _thenBuildMenu.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Building to Build.");
    }

    /// <summary>
    /// Retrieves gold, if any.
    /// </summary>
    public void Retrieve()
    {
        StartCoroutine(_am.UseGold(_am.CurrentPlayer));

        UpdateTextBothPlayers();
    }

    /// <summary>
    /// Builds a Building.
    /// </summary>
    /// <param name="buildingName">"Factory" "Burrow" or "Mine</param>
    public void Build(string buildingName)
    {
        _bm.DisablePawnBoardInteractions();
        _am.StartBuild(_am.CurrentPlayer, buildingName);

        UpdateCurrentActionText("Select a Pawn, then Piece for your " + buildingName + ".");
        UpdateTextBothPlayers();
    }

    /// <summary>
    /// Moves the turn into the "Finally..." phase.
    /// </summary>
    public void ToFinallyPhase()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();
        _am.CurrentTurnPhase++;

        _finallyZone.SetActive(true);

        UpdateTextBothPlayers();
        if(_am.CurrentPlayer == 1)
        {
            UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P1BuiltBuildings[1]) + " Card(s).");
            _cm.PrepareCardActivating(_am.CurrentPlayer, _am.CardActivations + _am.P1BuiltBuildings[1]);
        }
        else
        {
            UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P2BuiltBuildings[1]) + " Card(s).");
            _cm.PrepareCardActivating(_am.CurrentPlayer, _am.CardActivations + _am.P2BuiltBuildings[1]);
        }
    }

    /// <summary>
    /// Moves the turn back a step based on the CurrentTurnPhase variable.
    /// </summary>
    public void Back()
    {
        DisableListObjects();
        _bm.DisablePawnBoardInteractions();
        //I HATE THIS PART OF CODE. If a workaround appears I wanna change it ASAP.
        foreach(MonoBehaviour script in FindObjectsOfType<MonoBehaviour>())
        {
            script.StopAllCoroutines();
        }
        _cm.DeselectSelectedCards();
        _cm.PrepareCardSelection(0, "", true);

        if (_am.CurrentTurnPhase == 2)
        {
            UpdateCurrentActionText("Select an Action.");
            _thenZone.SetActive(true);
            _thenActions.SetActive(true);
            _endPhaseButton.SetActive(true);
        }
        else if(_am.CurrentTurnPhase == 3)
        {
            _finallyZone.SetActive(true);
            if (_am.CurrentPlayer == 1)
            {
                UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P1BuiltBuildings[1]) + " Cards.");
            }
            else
            {
                UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P2BuiltBuildings[1]) + " Cards.");
            }
        }

        UpdateTextBothPlayers();
    }

    public void EndTurn()
    {
        DisableListObjects();
        _cm.StopCardActivating(_am.CurrentPlayer);

        if (_am.CurrentPlayer == 1)
        {
            _cm.DrawAlottedCards(_am.CardDraw + _am.P1BuiltBuildings[0]);
            StartCoroutine(_cm.CardDiscardProcess(_am.CurrentPlayer));
            _factory.sprite = _meerkatFactory;
            _burrow.sprite = _meerkatBurrow;
            _mine.sprite = _meerkatMine;
        }
        else
        {
            _cm.DrawAlottedCards(_am.CardDraw + _am.P2BuiltBuildings[0]);
            StartCoroutine(_cm.CardDiscardProcess(_am.CurrentPlayer));
            _factory.sprite = _moleFactory;
            _burrow.sprite = _moleBurrow;
            _mine.sprite = _moleMine;
        }

        UpdateTextBothPlayers();
    }
}