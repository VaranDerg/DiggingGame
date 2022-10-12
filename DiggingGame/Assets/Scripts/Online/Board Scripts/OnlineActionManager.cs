/*****************************************************************************
// File Name :         ActionManager.cs
// Author :            Rudy Wolfer, Andrea SD
// Creation Date :     October 3rd, 2022
//
// Brief Description : A Script to hold all play information and functions 
                       that control and store a player's actions.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineActionManager : MonoBehaviour
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
    [HideInInspector] public int[] P1BuiltBuildings = new int[5];
    [HideInInspector] public int[] P2BuiltBuildings = new int[5];
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

    [Header("Building Values")]
    public int BaseBuildingPrice;
    public int TotalBuildings;

    [Header("Script References")]
    private OnlineBoardManager _bm;
    private CardManager _cm;
    private OnlineCanvasManager _gcm;

    /// <summary>
    /// Calls PrepareStartingValues and assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _bm = FindObjectOfType<OnlineBoardManager>();
        _cm = FindObjectOfType<CardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        PrepareStartingValues();
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
                _cm.DrawCard("Universal");
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
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<PlayerPawn>().IsMoving = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _bm.BoardColliderSwitch(false);
    }

    /// <summary>
    /// Prepares pawns to build.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StartBuild(int player, string building)
    {
        _bm.DisablePawnBoardInteractions();
        foreach (MonoBehaviour script in FindObjectsOfType<MonoBehaviour>())
        {
            script.StopAllCoroutines();
        }
        _cm.DeselectSelectedCards();

        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<PlayerPawn>().IsBuilding = true;
                pawn.GetComponent<PlayerPawn>().BuildingToBuild = building;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _bm.BoardColliderSwitch(false);
    }

    /// <summary>
    /// Prepares pawn to dig.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StartDig(int player)
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<PlayerPawn>().IsDigging = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _bm.BoardColliderSwitch(false);
    }

    /// <summary>
    /// Stops pawns from doing stuff.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StopPawnActions(int player)
    {
        foreach(GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if(pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == player)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsMoving = false;
                pawn.GetComponent<OnlinePlayerPawn>().IsBuilding = false;
                pawn.GetComponent<OnlinePlayerPawn>().IsDigging = false;
                pawn.GetComponent<Animator>().Play("TempPawnDefault");
            }
        }

        _bm.BoardColliderSwitch(true);
    }

    /// <summary>
    /// Collects a tile and adds it to one of the "collected" variables. 
    /// </summary>
    /// <param name="type">"Grass" "Dirt" "Stone" or "Gold"</param>
    public void CollectTile(int player, string type)
    {
        if (player == 1)
        {
            switch (type)
            {
                case "Grass":
                    P1CollectedPile[0]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Dirt":
                    P1CollectedPile[1]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Stone":
                    P1CollectedPile[2]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Gold":
                    P1CollectedPile[3]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
            }
        }
        else if (player == 2)
        {
            switch (type)
            {
                case "Grass":
                    P2CollectedPile[0]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Dirt":
                    P2CollectedPile[1]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Stone":
                    P2CollectedPile[2]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
                case "Gold":
                    P2CollectedPile[3]++;
                    _gcm.UpdateTextBothPlayers();
                    _gcm.Back();
                    break;
            }
        }
    }

    /// <summary>
    /// Moves tiles from the Collected pile to the Refined pile. 
    /// </summary>
    public void RefineTiles(int player)
    {
        if (player == 1)
        {
            P1RefinedPile[0] += P1CollectedPile[0];
            P1RefinedPile[1] += P1CollectedPile[1];
            P1RefinedPile[2] += P1CollectedPile[2];
            P1RefinedPile[3] += P1CollectedPile[3];

            P1CollectedPile[0] = 0;
            P1CollectedPile[1] = 0;
            P1CollectedPile[2] = 0;
            P1CollectedPile[3] = 0;
        }
        else if (player == 2)
        {
            P2RefinedPile[0] += P2CollectedPile[0];
            P2RefinedPile[1] += P2CollectedPile[1];
            P2RefinedPile[2] += P2CollectedPile[2];
            P2RefinedPile[3] += P2CollectedPile[3];

            P2CollectedPile[0] = 0;
            P2CollectedPile[1] = 0;
            P2CollectedPile[2] = 0;
            P2CollectedPile[3] = 0;
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

                _cm.DrawCard("Gold");
                P1RefinedPile[3]--;
                SupplyPile[3]++;
            }
        }
        else if (player == 2)
        {
            if (P2RefinedPile[3] == 0 || P2Cards == 0)
            {
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

                _cm.DrawCard("Gold");
                P1RefinedPile[3]--;
                SupplyPile[3]++;
                _gcm.UpdateCurrentActionText("Gold retrieved!");
            }
        }
    }


    /// <summary>
    /// (WIP) Place a tile back onto the board. 
    /// </summary>
    /// <param name="type">"Grass" "Dirt" or "Stone"</param>
    public void PlaceTile(int player, string type)
    {
        Debug.Log("Player " + player + " placed a " + type + " tile!");

        switch (type)
        {
            case "Grass":
                SupplyPile[0]--;
                break;
            case "Dirt":
                SupplyPile[1]--;
                break;
            case "Stone":
                SupplyPile[2]--;
                break;
        }
    }

    /// <summary>
    /// Checks if there's buildings remaining to build a specified building.
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    public bool EnoughBuildingsRemaining(int player, string type)
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

    /// <summary>
    /// Activates mines and adds tiles. 
    /// </summary>
    public void ActivateMines(int player)
    {
        if (player == 1)
        {
            P1CollectedPile[0] += P1BuiltBuildings[2];
            P1CollectedPile[1] += P1BuiltBuildings[3];
            P1CollectedPile[2] += P1BuiltBuildings[4];
        }
        else if (player == 2)
        {
            P2CollectedPile[0] += P2BuiltBuildings[2];
            P2CollectedPile[1] += P2BuiltBuildings[3];
            P2CollectedPile[2] += P2BuiltBuildings[4];
        }
    }

    public void EndTurn(int player)
    {
        if (player == 1)
        {
            if (P1Score >= 15)
            {
                _gcm.UpdateCurrentActionText("Player 1 wins, as they've reached 15 points!");
                return;
            }

            CurrentTurnPhase = 0;
            CurrentPlayer = 2;
            _gcm.StartTurnButton.SetActive(true);
            _gcm.UpdateCurrentActionText("Player " + CurrentPlayer + ", start your turn.");
        }
        else
        {
            if (P2Score >= 15)
            {
                _gcm.UpdateCurrentActionText("Player 2 wins, as they've reached 15 points!");
                return;
            }

            CurrentTurnPhase = 0;
            CurrentPlayer = 1;
            CurrentRound++;
            _gcm.StartTurnButton.SetActive(true);
            _gcm.UpdateCurrentActionText("Player " + CurrentPlayer + ", start your turn.");
        }
    }
}