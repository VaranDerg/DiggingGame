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
using System;
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
    [SerializeField] private GameObject _backButton;
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

    [Header("Text and Object References, Building View")]
    [SerializeField] private GameObject _buildingInfoZone;
    [SerializeField] private TextMeshProUGUI _showHideBuildingInfoText;
    [SerializeField] private TextMeshProUGUI _factoryInfoText, _burrowInfoText, _mineInfoText;
    [SerializeField] private Image _bothFactoryImage, _bothBurrowImage, _bothMineImage;

    [Header("Other References")]
    private bool _opponentViewShowing = false;
    private bool _buildingViewShowing = false;
    [SerializeField] private Sprite _moleFactory, _moleBurrow, _moleMine, _meerkatFactory, _meerkatBurrow, _meerkatMine;
    [SerializeField] private Image _factory, _burrow, _mine;
    private ActionManager _am;
    private BoardManager _bm;
    private CardManager _cm;
    private CardEffects _ce;
    private PersistentCardManager _pcm;

    [Header("Animations")]
    [SerializeField] private Animator _oppInfoAnims;
    [SerializeField] private Animator _buildingInfoAnims;
    [SerializeField] private float _infoAnimWaitTime;
    private bool _midOppShowHideAnim;
    private bool _midBuildShowHideAnim;

    [Header("Other")]
    private List<GameObject> _allObjects = new List<GameObject>();
    private Coroutine _curGoldCoroutine;

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
        _allObjects.Add(_backButton);
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
        _ce = FindObjectOfType<CardEffects>();
        _pcm = FindObjectOfType<PersistentCardManager>();
        AddObjectsToList();
    }

    /// <summary>
    /// Disables every object in AllObjects
    /// </summary>
    public void DisableListObjects()
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
        //Master Builder Start
        int bCostReduction = 0;
        if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Master Builder"))
        {
            bCostReduction += _ce.BuildingReduction;
        }
        //End Master Builder

        if (curPlayer == 1)
        {
            _currentPlayerScore.text = "Score: " + _am.P1Score + "/" + _am.WinningScore;
            _currentPlayerCollectedPieces[0].text = _am.P1CollectedPile[0].ToString();
            _currentPlayerCollectedPieces[1].text = _am.P1CollectedPile[1].ToString();
            _currentPlayerCollectedPieces[2].text = _am.P1CollectedPile[2].ToString();
            _currentPlayerCollectedPieces[3].text = _am.P1CollectedPile[3].ToString();
            _currentPlayerRefinedPieces[0].text = _am.P1RefinedPile[0].ToString();
            _currentPlayerRefinedPieces[1].text = _am.P1RefinedPile[1].ToString();
            _currentPlayerRefinedPieces[2].text = _am.P1RefinedPile[2].ToString();
            _currentPlayerRefinedPieces[3].text = _am.P1RefinedPile[3].ToString();
            _currentPlayerRemainingBuildings[0].text = _am.P1RemainingBuildings[0] + " Left";
            _currentPlayerRemainingBuildings[1].text = _am.P1RemainingBuildings[1] + " Left";
            _currentPlayerRemainingBuildings[2].text = _am.P1RemainingBuildings[2] + " Left";

            if (_am.P1RemainingBuildings[0] == 0)
            {
                _currentPlayerRemainingBuildingCost[0].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[0].text = "Cost " + (_am.P1CurrentBuildingPrices[0] - bCostReduction);
            }

            if (_am.P1RemainingBuildings[1] == 0)
            {
                _currentPlayerRemainingBuildingCost[1].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[1].text = "Cost " + (_am.P1CurrentBuildingPrices[1] - bCostReduction);
            }
                
            if (_am.P1RemainingBuildings[2] == 0)
            {
                _currentPlayerRemainingBuildingCost[2].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[2].text = "Cost " + (_am.P1CurrentBuildingPrices[2] - bCostReduction);
            }
        }
        else
        {
            _currentPlayerScore.text = "Score: " + _am.P2Score + "/" + _am.WinningScore;
            _currentPlayerCollectedPieces[0].text = _am.P2CollectedPile[0].ToString();
            _currentPlayerCollectedPieces[1].text = _am.P2CollectedPile[1].ToString();
            _currentPlayerCollectedPieces[2].text = _am.P2CollectedPile[2].ToString();
            _currentPlayerCollectedPieces[3].text = _am.P2CollectedPile[3].ToString();
            _currentPlayerRefinedPieces[0].text = _am.P2RefinedPile[0].ToString();
            _currentPlayerRefinedPieces[1].text = _am.P2RefinedPile[1].ToString();
            _currentPlayerRefinedPieces[2].text = _am.P2RefinedPile[2].ToString();
            _currentPlayerRefinedPieces[3].text = _am.P2RefinedPile[3].ToString();
            _currentPlayerRemainingBuildings[0].text = _am.P2RemainingBuildings[0] + " Left";
            _currentPlayerRemainingBuildings[1].text = _am.P2RemainingBuildings[1] + " Left";
            _currentPlayerRemainingBuildings[2].text = _am.P2RemainingBuildings[2] + " Left";

            if (_am.P2RemainingBuildings[0] == 0)
            {
                _currentPlayerRemainingBuildingCost[0].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[0].text = "Cost " + (_am.P2CurrentBuildingPrices[0] - bCostReduction);
            }

            if (_am.P2RemainingBuildings[1] == 0)
            {
                _currentPlayerRemainingBuildingCost[1].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[1].text = "Cost " + (_am.P2CurrentBuildingPrices[1] - bCostReduction);
            }

            if (_am.P2RemainingBuildings[2] == 0)
            {
                _currentPlayerRemainingBuildingCost[2].text = "Sold Out!";
            }
            else
            {
                _currentPlayerRemainingBuildingCost[2].text = "Cost " + (_am.P2CurrentBuildingPrices[2] - bCostReduction);
            }
        }
    }

    /// <summary>
    /// Updates the Always Active Text.
    /// </summary>
    private void UpdateAlwaysActiveText()
    {
        _activePlayerText.text = _am.CurrentPlayerName + "s";
        _activeRoundText.text = "Round " + _am.CurrentRound;
        _supplyPieces[0].text = _am.SupplyPile[0].ToString();
        _supplyPieces[1].text = _am.SupplyPile[1].ToString();
        _supplyPieces[2].text = _am.SupplyPile[2].ToString();
        _supplyPieces[3].text = _am.SupplyPile[3].ToString();
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
    /// Updates the Building Info text.
    /// </summary>
    private void UpdateBuildingText()
    {
        if(_am.CurrentPlayer == 1)
        {
            _bothFactoryImage.sprite = _moleFactory;
            _bothBurrowImage.sprite = _moleBurrow;
            _bothMineImage.sprite = _moleMine;
        }
        else
        {
            _bothFactoryImage.sprite = _meerkatFactory;
            _bothBurrowImage.sprite = _meerkatBurrow;
            _bothMineImage.sprite = _meerkatMine;
        }

        //Master Builder Start
        int bCostReduction = 0;
        if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Master Builder"))
        {
            bCostReduction += _ce.BuildingReduction;
        }
        //End Master Builder

        if (_am.CurrentPlayer == 1)
        {
            _factoryInfoText.text = "Factories" + Environment.NewLine + "Card Draw: "+ (_am.CardDraw + _am.P1BuiltBuildings[0])  + Environment.NewLine + Environment.NewLine + _am.P1BuiltBuildings[0] + " Built" + Environment.NewLine + _am.P1RemainingBuildings[0] + " Left" + Environment.NewLine + "Cost " + (_am.P1CurrentBuildingPrices[0] - bCostReduction);
            _burrowInfoText.text = "Burrows" + Environment.NewLine + "Activations: " + (_am.CardActivations + _am.P1BuiltBuildings[1])  + Environment.NewLine + Environment.NewLine + _am.P1BuiltBuildings[1] + " Built" + Environment.NewLine + _am.P1RemainingBuildings[1] + " Left" + Environment.NewLine + "Cost " + (_am.P1CurrentBuildingPrices[1] - bCostReduction);
            _mineInfoText.text = "Mines" + Environment.NewLine + "Supply Pieces" + Environment.NewLine + Environment.NewLine + (_am.P1BuiltBuildings[2] + _am.P1BuiltBuildings[3] + _am.P1BuiltBuildings[4] + _am.P1BuiltBuildings[5]) + " Built" + Environment.NewLine + _am.P1RemainingBuildings[2] + " Left" + Environment.NewLine + "Cost " + (_am.P1CurrentBuildingPrices[2] - bCostReduction);
        }
        else
        {
            _factoryInfoText.text = "Factories" + Environment.NewLine + "Card Draw: " + (_am.CardDraw + _am.P2BuiltBuildings[0]) + Environment.NewLine + Environment.NewLine + _am.P2BuiltBuildings[0] + " Built" + Environment.NewLine + _am.P2RemainingBuildings[0] + " Left" + Environment.NewLine + "Cost " + (_am.P2CurrentBuildingPrices[0] - bCostReduction);
            _burrowInfoText.text = "Burrows" + Environment.NewLine + "Activations: " + (_am.CardActivations + _am.P2BuiltBuildings[1]) + Environment.NewLine + Environment.NewLine + _am.P2BuiltBuildings[1] + " Built" + Environment.NewLine + _am.P2RemainingBuildings[1] + " Left" + Environment.NewLine + "Cost " + (_am.P2CurrentBuildingPrices[1] - bCostReduction);
            _mineInfoText.text = "Mines" + Environment.NewLine + "Supply Pieces" + Environment.NewLine + Environment.NewLine + (_am.P2BuiltBuildings[2] + _am.P2BuiltBuildings[3] + _am.P2BuiltBuildings[4] + _am.P2BuiltBuildings[5]) + " Built" + Environment.NewLine + _am.P2RemainingBuildings[2] + " Left" + Environment.NewLine + "Cost " + (_am.P2CurrentBuildingPrices[2] - bCostReduction);
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
        UpdateBuildingText();
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
        _buildingInfoZone.SetActive(false);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Press Start Turn to begin!");
    }

    /// <summary>
    /// Wrapper for starting the below Coroutine through a button.
    /// </summary>
    public void OpponentInfoToggleWrapper()
    {
        if(_midOppShowHideAnim)
        {
            return;
        }

        StartCoroutine(OpponentInfoToggle());
    }

    /// <summary>
    /// Shows or hides the Opponent Info.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpponentInfoToggle()
    {
        if(_opponentViewShowing)
        {
            _oppInfoAnims.Play("OppInfoHide");

            _midOppShowHideAnim = true;
            yield return new WaitForSeconds(_infoAnimWaitTime);
            _midOppShowHideAnim = false;

            _showHideOpponentInfoText.text = "Show Opponent Info";
            _opponentInfoZone.SetActive(false);
            _opponentViewShowing = false;
        }
        else
        {
            _opponentInfoZone.SetActive(true);
            _oppInfoAnims.Play("OppInfoShow");

            _midOppShowHideAnim = true;
            yield return new WaitForSeconds(_infoAnimWaitTime);
            _midOppShowHideAnim = false;

            _showHideOpponentInfoText.text = "Hide Opponent Info";
            _opponentViewShowing = true;
        }
    }

    /// <summary>
    /// Wrapper for starting the below Coroutine through a button.
    /// </summary>
    public void BuildingInfoToggleWrapper()
    {
        if (_midBuildShowHideAnim)
        {
            return;
        }

        StartCoroutine(BuildingInfoToggle());
    }

    /// <summary>
    /// Shows or hides the Building Info.
    /// </summary>
    /// <returns></returns>
    public IEnumerator BuildingInfoToggle()
    {
        if (_buildingViewShowing)
        {
            _buildingInfoAnims.Play("OppInfoHide");

            _midBuildShowHideAnim = true;
            yield return new WaitForSeconds(_infoAnimWaitTime);
            _midBuildShowHideAnim = false;

            _showHideBuildingInfoText.text = "Show Building Info";
            _buildingInfoZone.SetActive(false);
            _buildingViewShowing = false;
        }
        else
        {
            _buildingInfoZone.SetActive(true);
            _buildingInfoAnims.Play("OppInfoShow");

            _midBuildShowHideAnim = true;
            yield return new WaitForSeconds(_infoAnimWaitTime);
            _midBuildShowHideAnim = false;

            _showHideBuildingInfoText.text = "Hide Building Info";
            _buildingViewShowing = true;
        }
    }

    /// <summary>
    /// Moves the turn into the "First..." phase.
    /// </summary>
    public void StartTurn()
    {
        DisableListObjects();

        _firstZone.SetActive(true);
        _backButton.SetActive(true);
        _am.DrawStartingCards();
        _am.RefineTiles(_am.CurrentPlayer);
        _am.ActivateMines(_am.CurrentPlayer);
        _am.StartMove(_am.CurrentPlayer);
        _am.CurrentTurnPhase++;
        StartCoroutine(_cm.ShowCards(_am.CurrentPlayer));

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to move, then a Piece to move onto, or select Skip Move.");
    }

    /// <summary>
    /// Moves the turn into the "Then..." phase.
    /// </summary>
    public void ToThenPhase()
    {
        DisableListObjects();
        _bm.DisableAllBoardInteractions();
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
        _bm.DisableAllBoardInteractions();

        _am.StartMove(_am.CurrentPlayer);
        _thenZone.SetActive(true);
        _backButton.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to Move, then a Piece to move onto.");
    }

    /// <summary>
    /// Prepares a Dig action.
    /// </summary>
    public void Dig()
    {
        DisableListObjects();
        _bm.DisableAllBoardInteractions();

        _am.StartDig(_am.CurrentPlayer);
        _thenZone.SetActive(true);
        _backButton.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to Dig with, then a Piece to Dig.");
    }

    /// <summary>
    /// Opens the Build menu.
    /// </summary>
    public void OpenBuildMenu()
    {
        DisableListObjects();
        _bm.DisableAllBoardInteractions();

        _thenZone.SetActive(true);
        _thenBuildMenu.SetActive(true);
        _backButton.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Building to Build.");
    }

    /// <summary>
    /// Retrieves gold, if any.
    /// </summary>
    public void Retrieve()
    {
        DisableListObjects();
        _thenZone.SetActive(true);
        _backButton.SetActive(true);

        _curGoldCoroutine = StartCoroutine(_am.UseGold(_am.CurrentPlayer));

        UpdateTextBothPlayers();
    }

    /// <summary>
    /// Builds a Building.
    /// </summary>
    /// <param name="buildingName">"Factory" "Burrow" or "Mine</param>
    public void Build(string buildingName)
    {
        DisableListObjects();
        _bm.DisableAllBoardInteractions();

        _thenZone.SetActive(true);
        _backButton.SetActive(true);
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
        _bm.DisableAllBoardInteractions();
        _am.CurrentTurnPhase++;

        _finallyZone.SetActive(true);

        UpdateTextBothPlayers();
        if(_am.CurrentPlayer == 1)
        {
            UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P1BuiltBuildings[1]) + " Card(s).");
            _cm.PrepareCardActivating(_am.CurrentPlayer, _am.CardActivations + _am.P1BuiltBuildings[1], true);
        }
        else
        {
            UpdateCurrentActionText("Activate up to " + (_am.CardActivations + _am.P2BuiltBuildings[1]) + " Card(s).");
            _cm.PrepareCardActivating(_am.CurrentPlayer, _am.CardActivations + _am.P2BuiltBuildings[1], true);
        }
    }

    /// <summary>
    /// Moves the turn back a step based on the CurrentTurnPhase variable.
    /// </summary>
    public void Back()
    {
        DisableListObjects();
        _bm.DisableAllBoardInteractions();
        _bm.SetActiveCollider("Board");

        //For going back during the initial move. I have to invoke a move wrapper since compilation makes animations work incorrectly.
        if (_am.CurrentTurnPhase == 1)
        {
            _backButton.SetActive(true);
            _firstZone.SetActive(true);
            Invoke("PlayerMoveWrapper", 0.1f);
            UpdateCurrentActionText("Select a Pawn to move, then a Piece to move onto, or select Skip Move.");
            return;
        }

        //Hm... Weird retrieval bug. This fixed it, but I'm not happy about it.
        if (_curGoldCoroutine != null)
        {
            StopCoroutine(_curGoldCoroutine);
        }

        foreach (PieceController script in FindObjectsOfType<PieceController>())
        {
            script.StopAllCoroutines();
        }
        foreach (ActionManager script in FindObjectsOfType<ActionManager>())
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
            _cm.PrepareCardActivating(_am.CurrentPlayer, _am.CardActivations, false);
            if (_am.CurrentPlayer == 1)
            {
                UpdateCurrentActionText("Activate up to " + _cm.AllowedActivations + " Card(s).");
            }
            else
            {
                UpdateCurrentActionText("Activate up to " + _cm.AllowedActivations  + " Card(s).");
            }
        }

        UpdateTextBothPlayers();
    }

    /// <summary>
    /// A wrapper to call startmove with the above method.
    /// </summary>
    private void PlayerMoveWrapper()
    {
        _am.StartMove(_am.CurrentPlayer);
    }

    /// <summary>
    /// Ends the player's turn.
    /// </summary>
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