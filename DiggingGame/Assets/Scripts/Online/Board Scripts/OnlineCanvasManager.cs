/*****************************************************************************
// File Name :         GameCanvasManagerNew.cs
// Author :            Rudy Wolfer, Andrea SD
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
using Photon.Pun;
using Photon.Realtime;

public class OnlineCanvasManager : MonoBehaviourPun
{
    //Edit: Andrea SD - Added online functionality

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
    private OnlineActionManager _am;
    private OnlineBoardManager _bm;
    private OnlineCardManager _cm;
    private OnlineCardEffects _ce;
    private OnlinePersistentCardManager _pcm;

    [Header("Animations")]
    [SerializeField] private Animator _oppInfoAnims;
    [SerializeField] private Animator _buildingInfoAnims;
    [SerializeField] private float _infoAnimWaitTime;
    private bool _midOppShowHideAnim;
    private bool _midBuildShowHideAnim;

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
        _allObjects.Add(_backButton);
        _allObjects.Add(_endPhaseButton);
    }

    /// <summary>
    /// Assigns other partner scripts and calls AddObjectsToList.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<OnlineActionManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _ce = FindObjectOfType<OnlineCardEffects>();
        _pcm = FindObjectOfType<OnlinePersistentCardManager>();
        AddObjectsToList();

        // Author: Andrea SD
        if(PhotonNetwork.IsMasterClient)
        {
            _factory.sprite = _moleFactory;
            _burrow.sprite = _moleBurrow;
            _mine.sprite = _moleMine;
        }
        else
        {
            _factory.sprite = _meerkatFactory;
            _burrow.sprite = _meerkatBurrow;
            _mine.sprite = _meerkatMine;
        }
    }

    /// <summary>
    /// Sets the game into a basic opening state upon startup.
    /// 
    /// Edit: Andrea SD - Modified for online
    /// </summary>
    public void Start()
    {
        DisableListObjects();

        StartTurnButton.SetActive(true);
        _opponentInfoZone.SetActive(false);

        UpdateTextBothPlayers();
        
        // Author: Andrea SD
        if(PhotonNetwork.IsMasterClient)
        {
            UpdateCurrentActionText("Press Start Turn to begin!");
        }
        else
        {
            UpdateCurrentActionText("Player 1 is going first!");
        }

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
    /// 
    /// Edited: Andrea SD - modified for online use
    /// </summary>
    /// <param name="curPlayer">1 or 2</param>
    private void UpdateCurrentPlayerText()
    {
        //Master Builder Start
        int bCostReduction = 0;
        if (_pcm.CheckForPersistentCard(_am.CurrentPlayer, "Master Builder"))
        {
            bCostReduction += _ce.BuildingReduction;
        }
        //End Master Builder

        if (PhotonNetwork.IsMasterClient)   /*curPlayer == 1*/ //Edited: Andrea SD
        {
            _currentPlayerScore.text = "Score: " + _am.P1Score + "/" + _am.WinningScore;
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
        else    // Player 2
        {
            _currentPlayerScore.text = "Score: " + _am.P2Score + "/" + _am.WinningScore;
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
        _activePlayerText.text = "Player " + _am.CurrentPlayer;
        _activeRoundText.text = "Round " + _am.CurrentRound;
        _supplyPieces[0].text = "x" + _am.SupplyPile[0];
        _supplyPieces[1].text = "x" + _am.SupplyPile[1];
        _supplyPieces[2].text = "x" + _am.SupplyPile[2];
        _supplyPieces[3].text = "x" + _am.SupplyPile[3];
    }

    /// <summary>
    /// Changes the opponent text on player two's screen with player one's vals
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void ChangeOpponentText()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _opponentScoreText.text = "Score: " + _am.P2Score;
            _opponentCardText.text = "Cards: " + _am.P2Cards;
            _opponentGoldCardText.text = "Gold Cards: " + _am.P2GoldCards;

            _opponentPieces[0].text = "Grass: " + (_am.P2CollectedPile[0] + _am.P2RefinedPile[0]);
            _opponentPieces[1].text = "Dirt: " + (_am.P2CollectedPile[1] + _am.P2RefinedPile[1]);
            _opponentPieces[2].text = "Stone: " + (_am.P2CollectedPile[2] + _am.P2RefinedPile[2]);
            _opponentPieces[3].text = "Gold: " + (_am.P2CollectedPile[3] + _am.P2RefinedPile[3]);
        }
        else
        {
            _opponentScoreText.text = "Score: " + _am.P1Score;
            _opponentCardText.text = "Cards: " + _am.P1Cards;
            _opponentGoldCardText.text = "Gold Cards: " + _am.P1GoldCards;

            _opponentPieces[0].text = "Grass: " + (_am.P1CollectedPile[0] + _am.P1RefinedPile[0]);
            _opponentPieces[1].text = "Dirt: " + (_am.P1CollectedPile[1] + _am.P1RefinedPile[1]);
            _opponentPieces[2].text = "Stone: " + (_am.P1CollectedPile[2] + _am.P1RefinedPile[2]);
            _opponentPieces[3].text = "Gold: " + (_am.P1CollectedPile[3] + _am.P1RefinedPile[3]);
        }
    }

    /// <summary>
    /// Updates the Building Info text.
    /// </summary>
    private void UpdateBuildingText()
    {
        if (_am.CurrentPlayer == 1)
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
            _factoryInfoText.text = "Factories" + Environment.NewLine + "Card Draw: " + (_am.CardDraw + _am.P1BuiltBuildings[0]) + Environment.NewLine + Environment.NewLine + _am.P1BuiltBuildings[0] + " Built" + Environment.NewLine + _am.P1RemainingBuildings[0] + " Left" + Environment.NewLine + "Cost " + (_am.P1CurrentBuildingPrices[0] - bCostReduction);
            _burrowInfoText.text = "Burrows" + Environment.NewLine + "Activations: " + (_am.CardActivations + _am.P1BuiltBuildings[1]) + Environment.NewLine + Environment.NewLine + _am.P1BuiltBuildings[1] + " Built" + Environment.NewLine + _am.P1RemainingBuildings[1] + " Left" + Environment.NewLine + "Cost " + (_am.P1CurrentBuildingPrices[1] - bCostReduction);
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
    /// Calls the local RPC to update opponent text across the network
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="updatedText"></param>
    public void UpdateOpponentActionText(string updatedText)
    {
        photonView.RPC("UpdateOnlineActionText", RpcTarget.Others, updatedText);
    }

    /// <summary>
    /// Updates the Current Action text for the non-active player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="updatedText"></param>
    [PunRPC]
    public void UpdateOnlineActionText(string updatedText)
    {
        _currentActionText.text = updatedText;
    }

    /// <summary>
    /// Calls every "UpdateText"-type function except the Current Action Text one.
    /// 
    /// Edited: Andrea SD - online use
    /// Updates the opponents text across the network
    /// </summary>
    public void UpdateTextBothPlayers()
    {
        UpdateCurrentPlayerText();
        photonView.RPC("UpdateActiveTextOnline", RpcTarget.All);

        /*if (_am.CurrentPlayer == 1)
        {
            photonView.RPC("ChangeOpponentTextTwo", RpcTarget.Others);     //Andrea SD
        }
        else
        {
            photonView.RPC("ChangeOpponentTextOne", RpcTarget.Others);     //Andrea SD
        }*/

        photonView.RPC("ChangeOpponentText", RpcTarget.All);     //Andrea SD
    }

    /// <summary>
    /// Updates always active text for each player
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC] 
    public void UpdateActiveTextOnline()
    {
        UpdateAlwaysActiveText();
    }

    /// <summary>
    /// Wrapper for starting the below Coroutine through a button.
    /// </summary>
    public void OpponentInfoToggleWrapper()
    {
        if (_midOppShowHideAnim)
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
        if (_opponentViewShowing)
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
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void StartTurn()
    {
        DisableListObjects();

        _firstZone.SetActive(true);
        _am.DrawStartingCards();
        _am.RefineTiles(_am.CurrentPlayer);
        _am.ActivateMines(_am.CurrentPlayer);
        _am.StartMove(_am.CurrentPlayer);
        CallChangePhase(1);
        StartCoroutine(_cm.ShowCards(_am.CurrentPlayer));

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select a Pawn to move, then a Piece to move onto.");
        UpdateOpponentActionText(_am.CurrentPlayer + " is taking their first move!");       // Andrea SD
    }

    /// <summary>
    /// Moves the turn into the "Then..." phase.
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void ToThenPhase()
    {
        UpdateOpponentActionText(_am.CurrentPlayer + " is contemplating their next action...");     // Andrea SD
        DisableListObjects();
        _bm.DisableAllBoardInteractions();
        CallChangePhase(1);     // ASD

        _thenZone.SetActive(true);
        _thenActions.SetActive(true);
        _endPhaseButton.SetActive(true);

        UpdateTextBothPlayers();
        UpdateCurrentActionText("Select an Action.");
    }

    /// <summary>
    /// Prepares a Move action.
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void Move()
    {
        UpdateOpponentActionText("Player " + _am.CurrentPlayer + " is moving...");      // Andrea SD
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
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void Dig()
    {
        UpdateOpponentActionText("Player " + _am.CurrentPlayer + " is digging...");     // Andrea SD
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
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void OpenBuildMenu()
    {
        UpdateOpponentActionText("Player " + _am.CurrentPlayer + " is building...");
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

        StartCoroutine(_am.UseGold(_am.CurrentPlayer));

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
    /// 
    /// Edited: Andrea SD - online use
    /// </summary>
    public void ToFinallyPhase()
    {
        UpdateOpponentActionText("Player " + _am.CurrentPlayer + " is thinking of playing a card...");  //Andrea SD
        DisableListObjects();
        _bm.DisableAllBoardInteractions();
        CallChangePhase(1);

        _finallyZone.SetActive(true);

        UpdateTextBothPlayers();
        if (_am.CurrentPlayer == 1)
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
        UpdateOpponentActionText("Player " + _am.CurrentPlayer + " is thinking of their next action...");
        DisableListObjects();
        _bm.DisableAllBoardInteractions();
        _bm.SetActiveCollider("Board");

        foreach (OnlinePieceController script in FindObjectsOfType<OnlinePieceController>())
        {
            script.StopAllCoroutines();
        }
        foreach (OnlineActionManager script in FindObjectsOfType<OnlineActionManager>())
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
                UpdateCurrentActionText("Activate up to " + _cm.AllowedActivations + " Card(s).");
            }
        }

        UpdateTextBothPlayers();
    }

    /// <summary>
    /// Ends the player's turn.
    /// 
    /// Edit: Andrea SD: Modified for online play
    /// </summary>
    public void EndTurn()
    {
        _am.CallStartButton();

        UpdateOpponentActionText("Start your turn.");
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

    #region RPC Functions

    /// <summary>
    /// Calls the RPC that modifies the current turn phase
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"></param>
    public void CallChangePhase(int amount)
    {
        photonView.RPC("ChangeTurnPhase", RpcTarget.All, amount);
    }

    /// <summary>
    /// Modifies the current turn phase
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> how the turn phase is modified </param>
    [PunRPC]
    public void ChangeTurnPhase(int amount)
    {
        _am.CurrentTurnPhase += amount;
    }

    /// <summary>
    /// Shows the opponent info
    /// </summary>
    [PunRPC]
    public void ShowOpponentInfo()
    {
        OpponentInfoToggleWrapper();
    }

    #endregion
}