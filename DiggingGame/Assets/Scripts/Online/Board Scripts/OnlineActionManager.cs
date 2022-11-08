/*****************************************************************************
// File Name :         ActionManager.cs
// Author :            Rudy Wolfer, Andrea SD
// Creation Date :     October 3rd, 2022
//
// Brief Description : A Script to hold all play information and functions 
                       that control and store a player's actions.
*****************************************************************************/

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OnlineActionManager : MonoBehaviourPun
{
    //Edit: Andrea SD, Multiplayer functionality

    //Arrays for collected pile. Grass, Dirt, Stone, Gold.
    [HideInInspector] public int[] P1CollectedPile = new int[4];
    [HideInInspector] public int[] P2CollectedPile = new int[4];
    //Arrays for refined pile. Grass, Dirt, Stone Gold.
    [HideInInspector] public int[] P1RefinedPile = new int[4];
    [HideInInspector] public int[] P2RefinedPile = new int[4];
    //Array for supply. Grass, Dirt, Stone, Gold.
    [HideInInspector] public int[] SupplyPile = new int[4];
    //Arrays for players' build buildings. Factory, Burrow, Grass Mine, Dirt Mine, Stone Mine. 
    [HideInInspector] public int[] P1BuiltBuildings = new int[6];
    [HideInInspector] public int[] P2BuiltBuildings = new int[6];
    //Variables for each players' cards. 
    [HideInInspector] public int P1Cards = 0, P2Cards = 0;
    [HideInInspector] public int P1GoldCards = 0, P2GoldCards = 0;
    //Variables for each players' score.
    [HideInInspector] public int P1Score = 0;
    [HideInInspector] public int P2Score = 0;
    //Variables for the current phase of the turn (First, Then, Finally) and the current player (1, 2)
    [HideInInspector] public int CurrentTurnPhase = 0;
    [HideInInspector] public int CurrentPlayer = 1;
    //Variables for the current turn number and the current round.
    [HideInInspector] public int CurrentTurn = 0;
    [HideInInspector] public int CurrentRound = 0;
    //Array for each players' remaining buildings. Factory, Burrow, Mine.
    [HideInInspector] public int[] P1RemainingBuildings = new int[3];
    [HideInInspector] public int[] P2RemainingBuildings = new int[3];
    //Array for each players' current building prices. Factory, Burrow, Mine.
    [HideInInspector] public int[] P1CurrentBuildingPrices = new int[3];
    [HideInInspector] public int[] P2CurrentBuildingPrices = new int[3];

    [Header("Game Values")]
    public int StartingPlayer;
    public int CardActivations;
    public int StartingCards;
    public int HandLimit;
    public int CardDraw;
    public int WinningScore;
    [SerializeField] private GameObject _menuButton;

    [Header("Building Values")]
    public int BaseBuildingPrice;
    public int BuildingPriceGoldRaise;
    public int TotalBuildings;

    [Header("Card Effects")]
    [HideInInspector] public bool ShovelUsed;
    [HideInInspector] public bool MorningJogUsed;

    [Header("Animations")]
    [SerializeField] private Animator _scoreTextAnimator;
    public float PawnMoveSpeed;
    public float PawnMoveAnimTime;

    [Header("Script References")]
    private OnlineBoardManager _bm;
    private OnlineCardManager _cm;
    private OnlineCanvasManager _gcm;
    private OnlinePersistentCardManager _pcm;

    /// <summary>
    /// Calls PrepareStartingValues and assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _menuButton.SetActive(false);
        _bm = FindObjectOfType<OnlineBoardManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _pcm = FindObjectOfType<OnlinePersistentCardManager>();
        PrepareStartingValues();
    }
    
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Board is disbled for player 2 at the start and enabled for player 1
        // Andrea SD
        DisableBoard();
        photonView.RPC("EnableBoard", RpcTarget.MasterClient);
    }

    /// <summary>
    /// Sets up every internal variable with the serialized ones.
    /// </summary>
    private void PrepareStartingValues()
    {
        CurrentPlayer = StartingPlayer;

        CurrentTurn = 1;
        CurrentRound = 1;

        P1CurrentBuildingPrices[0] = BaseBuildingPrice;
        P1CurrentBuildingPrices[1] = BaseBuildingPrice;
        P1CurrentBuildingPrices[2] = BaseBuildingPrice;
        P2CurrentBuildingPrices[0] = BaseBuildingPrice;
        P2CurrentBuildingPrices[1] = BaseBuildingPrice;
        P2CurrentBuildingPrices[2] = BaseBuildingPrice;

        P1RemainingBuildings[0] = TotalBuildings;
        P1RemainingBuildings[1] = TotalBuildings;
        P1RemainingBuildings[2] = TotalBuildings;
        P2RemainingBuildings[0] = TotalBuildings;
        P2RemainingBuildings[1] = TotalBuildings;
        P2RemainingBuildings[2] = TotalBuildings;
    }

    /// <summary>
    /// Draws cards up to the starting cards for each player if it's the first round.
    /// </summary>
    public void DrawStartingCards()
    {
        if (CurrentRound == 1)
        {
            for (int i = StartingCards; i > 0; i--)
            {
                StartCoroutine(_cm.DrawCard("Universal"));
            }
        }
    }

    /// <summary>
    /// Prepares pawns to move.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StartMove(int player)
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsMoving = true;
                pawn.GetComponent<Animator>().Play(pawn.GetComponent<PlayerPawn>().WaitingAnimName);
            }
        }

        _bm.SetActiveCollider("Pawn");
    }

    /// <summary>
    /// Prepares pawns to build.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StartBuild(int player, string building)
    {
        _bm.DisableAllBoardInteractions();
        foreach (OnlineActionManager script in FindObjectsOfType<OnlineActionManager>())
        {
            script.StopAllCoroutines();
        }
        foreach (OnlinePieceController script in FindObjectsOfType<OnlinePieceController>())
        {
            script.StopAllCoroutines();
        }
        _cm.DeselectSelectedCards();

        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsBuilding = true;
                pawn.GetComponent<OnlinePlayerPawn>().BuildingToBuild = building;
                pawn.GetComponent<Animator>().Play(pawn.GetComponent<OnlinePlayerPawn>().WaitingAnimName);
            }
        }

        _bm.SetActiveCollider("Pawn");
    }

    /// <summary>
    /// Prepares pawn to dig.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StartDig(int player)
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsDigging = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _bm.SetActiveCollider("Pawn");
    }

    /// <summary>
    /// Stops pawns from doing stuff.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StopPawnActions(int player)
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsMoving = false;
                pawn.GetComponent<OnlinePlayerPawn>().IsBuilding = false;
                pawn.GetComponent<OnlinePlayerPawn>().IsDigging = false;
                pawn.GetComponent<OnlinePlayerPawn>().IsPlacing = false;
                pawn.GetComponent<Animator>().Play(pawn.GetComponent<PlayerPawn>().IdleAnimName);
            }
        }

        _bm.SetActiveCollider("Board");
    }

    /// <summary>
    /// Collects a tile and adds it to one of the "collected" variables.
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="type">"Grass" "Dirt" "Stone" or "Gold"</param>
    public void CollectTile(int player, string type, bool goBack)
    {
        if (player == 1)
        {
            switch (type)
            {
                case "Grass":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 1, 0, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Dirt":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 1, 1, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Stone":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 1, 2, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Gold":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 1, 3, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
            }
        }
        else if (player == 2)
        {
            switch (type)
            {
                case "Grass":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 2, 0, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Dirt":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 2, 1, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Stone":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 2, 2, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
                case "Gold":
                    photonView.RPC("UpdateCollected", RpcTarget.All, 2, 3, 1);
                    _gcm.UpdateTextBothPlayers();
                    if (goBack)
                    {
                        _gcm.Back();
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Moves tiles from the Collected pile to the Refined pile. 
    /// 
    /// Edited: Andrea SD - Modified for online usage.
    /// </summary>
    public void RefineTiles(int player)
    {
        if (player == 1)
        {
            photonView.RPC("UpdateRefined", RpcTarget.All, 1, 0, P1CollectedPile[0]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 1, 1, P1CollectedPile[1]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 1, 2, P1CollectedPile[2]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 1, 3, P1CollectedPile[3]);

            photonView.RPC("EmptyCollection", RpcTarget.All, 1);
        }
        else if (player == 2)
        {
            photonView.RPC("UpdateRefined", RpcTarget.All, 2, 0, P2CollectedPile[0]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 2, 1, P2CollectedPile[1]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 2, 2, P2CollectedPile[2]);
            photonView.RPC("UpdateRefined", RpcTarget.All, 2, 3, P2CollectedPile[3]);

            photonView.RPC("EmptyCollection", RpcTarget.All, 2);
        }
    }

    /// <summary>
    /// Uses gold tiles to draw a gold card.
    /// </summary>
    public IEnumerator UseGold(int player)
    {
        if (player == 1)
        {
            if (P1RefinedPile[3] == 0 || P1Cards == 0)
            {
                _gcm.Back();
                _gcm.UpdateCurrentActionText("No Refined Gold or Cards to retrieve with!");
            }
            else
            {
                _cm.PrepareCardSelection(1, "Any", false);
                while (!_cm.CheckCardSelection())
                {
                    yield return null;
                }
                _cm.PrepareCardSelection(0, "", true);

                //Start of Geologist code.
                if (_pcm.CheckForPersistentCard(CurrentPlayer, "Geologist"))
                {
                    CallUpdateScore(CurrentPlayer, 1);
                }
                //End of Geologist code.

                StartCoroutine(_cm.DrawCard("Gold"));
                CallUpdatePieces(1, 1, 3, -1);  // Andrea SD
                SupplyPileRPC(3, -1);   // Andrea SD
                _gcm.UpdateTextBothPlayers();
                _gcm.Back();
                _gcm.UpdateCurrentActionText("Gold retrieved!");
            }
        }
        else if (player == 2)
        {
            if (P2RefinedPile[3] == 0 || P2Cards == 0)
            {
                _gcm.Back();
                _gcm.UpdateCurrentActionText("No Refined Gold or Cards to retrieve with!");
            }
            else
            {
                _cm.PrepareCardSelection(1, "Any", false);
                while (!_cm.CheckCardSelection())
                {
                    yield return null;
                }
                _cm.PrepareCardSelection(0, "", true);

                //Start of Geologist code.
                if (_pcm.CheckForPersistentCard(CurrentPlayer, "Geologist"))
                {
                    CallUpdateScore(CurrentPlayer, 1);
                }
                //End of Geologist code.

                StatManager.s_Instance.IncreaseStatistic(CurrentPlayer, "Retrieve", 1);

                StartCoroutine(_cm.DrawCard("Gold"));
                CallUpdatePieces(1, 2, 3, -1);  // Andrea SD
                SupplyPileRPC(3, -1);
                _gcm.Back();
                _gcm.UpdateCurrentActionText("Gold retrieved!");
                _gcm.UpdateTextBothPlayers();
            }
        }
    }


    /// <summary>
    /// Place a tile back onto the board. 
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <param name="type">"Grass" "Dirt" or "Stone"</param>
    public void PlaceTile(string type)
    {
        //Debug.Log("Player " + player + " placed a " + type + " tile!");

        switch (type)
        {
            case "Grass":
                SupplyPileRPC(0, 1);
                break;
            case "Dirt":
                SupplyPileRPC(1, 1);
                break;
            case "Stone":
                SupplyPileRPC(2, 1);
                break;
        }
    }

    /// <summary>
    /// Calls the RPC to modify the supply pile across all clients
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="amount"></param>
    public void SupplyPileRPC(int pieceType, int amount)
    {
        photonView.RPC("ModifySupplyPile", RpcTarget.All, pieceType, amount);
    }

    /// <summary>
    /// Removes pieces from the supply pile
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pieceType"> type of piece being removed </param>
    /// <param name="amount"> amount being removed </param>
    [PunRPC]
    public void ModifySupplyPile(int pieceType, int amount)
    {
        switch(pieceType)
        {
            case 0:
                SupplyPile[0] -= amount;
                break;
            case 1:
                SupplyPile[1] -= amount;
                break;
            case 2:
                SupplyPile[2] -= amount;
                break;
        }
    }

    /// <summary>
    /// Checks if there's buildings remaining to build a specified building.
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    public bool EnoughBuildingsRemaining(int player, string type)
    {
        {
            if (player == 1)
            {
                switch (type)
                {
                    case "Factory":
                        if (P1RemainingBuildings[0] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Factories have been built!");
                            return false;
                        }
                        return true;
                    case "Burrow":
                        if (P1RemainingBuildings[1] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Burrows have been built!");
                            return false;
                        }
                        return true;
                    case "Mine":
                        if (P1RemainingBuildings[2] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Mines have been built!");
                            return false;
                        }
                        return true;
                    default:
                        Debug.LogWarning("Incorrect building provided: " + type);
                        return false;
                }
            }
            else if (player == 2)
            {
                switch (type)
                {
                    case "Factory":
                        if (P2RemainingBuildings[0] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Factories have been built!");
                            return false;
                        }
                        return true;
                    case "Burrow":
                        if (P2RemainingBuildings[1] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Burrows have been built!");
                            return false;
                        }
                        return true;
                    case "Mine":
                        if (P2RemainingBuildings[2] == 0)
                        {
                            _gcm.UpdateCurrentActionText("All Mines have been built!");
                            return false;
                        }
                        return true;
                    default:
                        Debug.LogWarning("Incorrect building provided: " + type);
                        return false;
                }
            }
            else
            {
                Debug.LogWarning("Player " + player + " is not valid.");
                return false;
            }
        }
    }

    /// <summary>
    /// Collects pieces from the supply and adds them to the current player's collected pile.
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <param name="amount">Int, number of pieces to collect</param>
    /// <param name="suit">"Grass" "Dirt" or "Stone"</param>
    public void CollectPiecesFromSupply(int amount, string suit)
    {
        if (CurrentPlayer == 1)
        {
            if (suit == "Grass")
            {
                if (SupplyPile[0] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(0, -amount);
                    CallUpdatePieces(0, 1, 0, amount);
                }
            }
            else if (suit == "Dirt")
            {
                if (SupplyPile[1] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(1, -amount);                   
                    CallUpdatePieces(0, 1, 1, amount);
                }
            }
            else if (suit == "Stone")
            {
                if (SupplyPile[2] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(2, -amount);
                    CallUpdatePieces(0, 1, 2, amount);
                }
            }
        }
        else
        {
            if (suit == "Grass")
            {
                if (SupplyPile[0] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(0, -amount);
                    CallUpdatePieces(0, 2, 0, amount);
                }
            }
            else if (suit == "Dirt")
            {
                if (SupplyPile[1] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(1, -amount);
                    CallUpdatePieces(0, 2, 1, amount);
                }
            }
            else if (suit == "Stone")
            {
                if (SupplyPile[2] >= amount)
                {
                    // Andrea SD
                    SupplyPileRPC(2, -amount);
                    CallUpdatePieces(0, 2, 2, amount);
                }
            }
        }
    }

    /// <summary>
    /// Activates mines and adds tiles. 
    /// </summary>
    public void ActivateMines(int player)
    {
        if (player == 1)
        {
            for (int i = P1BuiltBuildings[2]
                ; i != 0; i--)
            {
                if (SupplyPile[0] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Grass");
            }
            for (int i = P1BuiltBuildings[3]; i != 0; i--)
            {
                if (SupplyPile[1] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Dirt");
            }
            for (int i = P1BuiltBuildings[4]; i != 0; i--)
            {
                if (SupplyPile[2] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Stone");
            }
        }
        else if (player == 2)
        {
            for (int i = P2BuiltBuildings[2]; i != 0; i--)
            {
                if (SupplyPile[0] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Grass");
            }
            for (int i = P2BuiltBuildings[3]; i != 0; i--)
            {
                if (SupplyPile[1] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Dirt");
            }
            for (int i = P2BuiltBuildings[4]; i != 0; i--)
            {
                if (SupplyPile[2] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Stone");
            }
            for (int i = P2BuiltBuildings[5]; i != 0; i--)
            {
                if (SupplyPile[3] == 0)
                {
                    continue;
                }

                CollectPiecesFromSupply(1, "Gold");
            }
        }
    }

    /// <summary>
    /// Ends the player's turn
    /// Updates round text values and progresses the game phase
    /// Edit: Andrea SD, modified for online play
    /// </summary>
    /// <param name="player"> Player who's turn is ending </param>
    public void EndTurn(int player)
    {
        if (player == 1)
        {
            if (P1Score >= WinningScore)
            {
                /* _gcm.UpdateCurrentActionText("Player 1 wins, as they've reached 15 points!");
                 _gcm.UpdateOpponentActionText("Player 1 wins, as they've reached 15 points!");    //Andrea SD
                 _menuButton.SetActive(true);
                 return;*/

                FindObjectOfType<SceneLoader>().LoadScene("ResultsScreen");

                //_gcm.UpdateCurrentActionText("Player 1 wins, as they've reached " + WinningScore + " points!");
                //_menuButton.SetActive(true);
                return;
            }
            photonView.RPC("ResetTurnPhase", RpcTarget.All);    // Andrea SD

            //_gcm.StartTurnButton.SetActive(true);
            _gcm.UpdateCurrentActionText("Player 2 is starting their turn.");
            photonView.RPC("ChangeTurn", RpcTarget.All, 2);     // Andrea SD
        }
        else
        {
            if (P2Score >= WinningScore)
            {
                /*_gcm.UpdateCurrentActionText("Player 2 wins, as they've reached 15 points!");
                _gcm.UpdateOpponentActionText("Player 2 wins, as they've reached 15 points!");    //Andrea SD
                _menuButton.SetActive(true);
                return;*/

                FindObjectOfType<SceneLoader>().LoadScene("ResultsScreen");

                //_gcm.UpdateCurrentActionText("Player 2 wins, as they've reached " + WinningScore + " points!");
                //_menuButton.SetActive(true);
                return;
            }

            CurrentTurnPhase = 0;
            photonView.RPC("IncrementRound", RpcTarget.All);    // Andrea SD

            //_gcm.UpdateCurrentActionText("Player " + CurrentPlayer + ", start your turn.");
            _gcm.UpdateCurrentActionText("Player 1 is starting their turn.");
            photonView.RPC("ChangeTurn", RpcTarget.All, 1);     // Andrea SD

            StatManager.s_Instance.IncreaseStatistic(CurrentPlayer, "Round", 1);
        }

        //Enables the start button for the other player then disables your own board 
        DisableBoard();
        photonView.RPC("EnableBoard", RpcTarget.Others);

        if (CurrentRound % 2 == 0)
        {
            FindObjectOfType<WeatherManager>().SetActiveWeather(WeatherState.Weather.Night);
        }
        else
        {
            FindObjectOfType<WeatherManager>().SetActiveWeather(WeatherState.Weather.Day);
        }

        //Refresh persistent cards.
        ShovelUsed = false;
        MorningJogUsed = false;
    }


    /// <summary>
    /// Loads the Menu
    /// </summary>
    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Resets the current turn phase
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void ResetTurnPhase()
    {
        CurrentTurnPhase = 0;
    }

    /// <summary>
    /// Increases the round number by 1 across all clients
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void IncrementRound()
    {
        CurrentRound++;
    }

    /// <summary>
    /// Enables & shows the start turn button
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void EnableStartButton()
    {
        _gcm.StartTurnButton.SetActive(true);
    }

    /// <summary>
    /// Disables & hides start turn button
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void DisableStartButton()
    {
        _gcm.StartTurnButton.SetActive(false);
    }

    /// <summary>
    /// Changes the turn to the next player for online play
    ///
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> Player who's turn it is changing to </param>
    [PunRPC]
    private void ChangeTurn(int player)
    {
        CurrentPlayer = player;
    }

    /// <summary>
    /// Disables all interactions by the player. This occurs when it is not
    /// their turn. Enables the board for the other player.
    ///
    /// Author: Andrea SD
    /// </summary>
    private void DisableBoard()
    { 
        _gcm.ShowOpponentInfo();
        DisableStartButton();

        //EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //eventSystem.enabled = false;

        photonView.RPC("EnableBoard", RpcTarget.Others);
    }

    /// <summary>
    /// Enables all interactions by the player. This occurs when their turn is
    /// resumed.
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    private void EnableBoard()
    {
        EnableStartButton();

        //EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //eventSystem.enabled = true;
    }

    /// <summary>
    /// Calls the PunRPC to update of the collected piles for a specific player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="function"> 0 = UpdateCollected, 1 = UpdateRefined </param>
    /// <param name="player"> 1 or 2</param>
    /// <param name="material"> 0 = Grass, 1 = Dirt, 2 = Stone, 3 = Gold 
    /// </param>
    /// <param name="amount"> How much the amount is changing </param>
    public void CallUpdatePieces(int function, int player, int material, 
        int amount)
    {
        switch(function)
        {
            case 0:
                photonView.RPC("UpdateCollected", RpcTarget.All, player, 
                    material, amount);
                break;
            case 1:
                photonView.RPC("UpdateRefined", RpcTarget.All, player,
                    material, amount);
                break;
        }
    }

    /// <summary>
    /// Updates a material in Collected Pieces
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> player who's pieces are updated </param>
    /// <param name="material"> 0 = Grass, 1 = Dirt, 2 = Stone, 3 = Gold 
    /// </param>
    /// <param name="amount"> how much the # pieces changes by </param>
    [PunRPC]
    private void UpdateCollected(int player, int material, int amount)
    {
        switch(player)
        {
            case 1:
                P1CollectedPile[material] += amount;
                break;
            case 2:
                P2CollectedPile[material] += amount;
                break;
        }
    }

    /// <summary>
    /// Updates a material in Refined Pieces
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> player who's pieces are updated </param>
    /// <param name="material"> 0 = Grass, 1 = Dirt, 2 = Stone, 3 = Gold 
    /// </param>
    /// <param name="amount"> how much the # pieces changes by </param>
    [PunRPC]
    private void UpdateRefined(int player, int material, int amount)
    {
        switch (player)
        {
            case 1:
                P1RefinedPile[material] += amount;
                break;
            case 2:
                P2RefinedPile[material] += amount;
                break;
        }
    }

    /// <summary>
    /// Sets all collected pieces value to 0
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> player who's collection is emptied </param>
    [PunRPC]
    private void EmptyCollection(int player)
    {
        switch(player)
        {
            case 1:
                // P1CollectedPile.Initialize();
                P1CollectedPile[0] = 0;
                P1CollectedPile[1] = 0;
                P1CollectedPile[2] = 0;
                P1CollectedPile[3] = 0;
                break;
            case 2:
               // P2CollectedPile.Initialize();
                P2CollectedPile[0] = 0;
                P2CollectedPile[1] = 0;
                P2CollectedPile[2] = 0;
                P2CollectedPile[3] = 0;
                break;
        }    
    }

    /// <summary>
    /// Calls the UpdateScore RPC
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> how much the score is changing by </param>
    public void CallUpdateScore(int player, int amount)
    {
        photonView.RPC("UpdateScore", RpcTarget.All, player, amount);
    }

    /// <summary>
    /// Updates the players scores across the network
    /// </summary>
    /// <param name="player"> player who's score updating (1 or 2) </param>
    /// <param name="amount"> amount the player's score is changing by
    /// </param>
    [PunRPC]
    public void UpdateScore(int player, int amount)
    {
        if (photonView.IsMine)
        {
            _scoreTextAnimator.Play("ScorePoint");
        }
        if (player == 1)
        {
            P1Score += amount;
        }
        else
        {
            P2Score += amount;
        }            
    }
}