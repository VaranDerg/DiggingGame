using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
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
    //Variables for the current phase of the turn (First, Then, Finally) and the current player (1, 2)
    [HideInInspector] public int CurrentTurnPhase = 0;
    [HideInInspector] public int CurrentPlayer = 0;
    //Variables for the current turn number and the current round.
    [HideInInspector] public int CurrentTurn = 0;
    [HideInInspector] public int CurrentRound = 0;
    //Array for each players' remaining buildings. Factory, Burrow, Mine.
    [HideInInspector] public int[] P1RemainingBuildings = new int[3];
    [HideInInspector] public int[] P2RemainingBuildings = new int[3];
    //Array for each players' current building prices. Factory, Burrow, Mine.
    [HideInInspector] public int[] P1CurrentBuildingPrices = new int[3];
    [HideInInspector] public int[] P2CurrentBuildingPrices = new int[3];

    [Header("Player Values")]
    public int StartingPlayer;
    public int CardActivations;
    public int StartingCards;
    public int HandLimit;
    public int CardDraw;

    [Header("Building Values")]
    public int BaseBuildingPrice;
    public int TotalBuildings;

    /// <summary>
    /// Sets the card amount.
    /// </summary>
    private void Start()
    {
        PrepareStartingValues();
    }

    /// <summary>
    /// Sets up every internal variable with the serialized ones.
    /// </summary>
    private void PrepareStartingValues()
    {
        CurrentPlayer = StartingPlayer;

        P1Cards = StartingCards;
        P2Cards = StartingCards;

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
                    break;
                case "Dirt":
                    P1CollectedPile[1]++;
                    break;
                case "Stone":
                    P1CollectedPile[2]++;
                    break;
                case "Gold":
                    P1CollectedPile[3]++;
                    break;
            }
        }
        else if (player == 2)
        {
            switch (type)
            {
                case "Grass":
                    P2CollectedPile[0]++;
                    break;
                case "Dirt":
                    P2CollectedPile[1]++;
                    break;
                case "Stone":
                    P2CollectedPile[2]++;
                    break;
                case "Gold":
                    P2CollectedPile[3]++;
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
    /// Uses tiles based on provided cost. 
    /// </summary>
    /// <param name="grassCost">How many grass tiles to use.</param>
    /// <param name="dirtCost">How many dirt tiles to use.</param>
    /// <param name="stoneCost">How many stone tiles to use.</param>
    public bool UseTiles(int player, int grassCost, int dirtCost, int stoneCost)
    {
        if (player == 1)
        {
            if (grassCost > P1RefinedPile[0] || dirtCost > P1RefinedPile[1] || stoneCost > P1RefinedPile[2])
            {
                Debug.Log("Not enough tiles to use this effect!");
                return false;
            }
            P1RefinedPile[0] -= grassCost;
            P1RefinedPile[1] -= dirtCost;
            P1RefinedPile[2] -= stoneCost;

            SupplyPile[0] += grassCost;
            SupplyPile[1] += dirtCost;
            SupplyPile[2] += stoneCost;
            return true;
        }
        else if (player == 2)
        {
            if (grassCost > P2RefinedPile[0] || dirtCost > P2RefinedPile[1] || stoneCost > P2RefinedPile[2])
            {
                Debug.Log("Not enough tiles to use this effect!");
                return false;
            }
            P2RefinedPile[0] -= grassCost;
            P2RefinedPile[1] -= dirtCost;
            P2RefinedPile[2] -= stoneCost;

            SupplyPile[0] += grassCost;
            SupplyPile[1] += dirtCost;
            SupplyPile[2] += stoneCost;
            return true;
        }
        else
        {
            Debug.LogWarning("Incorrect player var provided.");
            return false;
        }
    }

    /// <summary>
    /// Uses gold tiles.
    /// </summary>
    public bool UseGold(int player)
    {
        if (player == 1)
        {
            if (P1RefinedPile[3] == 0)
            {
                Debug.Log("No gold to use!");
                return false;
            }
            if (P1Cards == 0)
            {
                Debug.Log("No cards to Retrieve with!");
                return false;
            }
            P1Cards--;
            P1GoldCards--;
            P1RefinedPile[3]--;
            SupplyPile[3]++;
            Debug.Log("Spent 1 card and 1 gold for 1 gold card!");
            return true;
        }
        else if (player == 2)
        {
            if (P2RefinedPile[3] == 0)
            {
                Debug.Log("No gold to use!");
                return false;
            }
            if (P2Cards == 0)
            {
                Debug.Log("No cards to Retrieve with!");
                return false;
            }
            P2Cards--;
            P2GoldCards--;
            P2RefinedPile[3]--;
            SupplyPile[3]++;
            Debug.Log("Spent 1 card and 1 gold for 1 gold card!");
            return true;
        }
        else
        {
            Debug.LogWarning("Incorrect player parameter provided.");
            return false;
        }
    }

    /// <summary>
    /// Place a tile back onto the board. 
    /// </summary>
    /// <param name="type">"Grass" "Dirt" or "Stone"</param>
    public void PlaceTiles(int player, string type)
    {
        Debug.Log("Player " + player + " placed a " + type + " tile!");

        switch (type)
        {
            case "Grass":
                //Place a grass tile
                SupplyPile[0]--;
                break;
            case "Dirt":
                //Place a dirt tile
                SupplyPile[1]--;
                break;
            case "Stone":
                //Place a stone tile
                SupplyPile[2]--;
                break;
        }
    }

    /// <summary>
    /// Builds a building. 
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    /// <param name="mineType">If "Mine" must list a type. "Grass" "Dirt" or "Stone"</param>
    public void BuildBuilding(int player, string type, string mineType)
    {
        if (player == 1)
        {
            switch (type)
            {
                case "Factory":
                    if (P1RemainingBuildings[0] == 0)
                    {
                        Debug.Log("All Factories have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P1CurrentBuildingPrices[0], "Factory"))
                    {
                        P1CurrentBuildingPrices[0]++;
                        P1RemainingBuildings[0]--;
                        P1BuiltBuildings[0]++;
                    }
                    break;
                case "Burrow":
                    if (P1RemainingBuildings[1] == 0)
                    {
                        Debug.Log("All Burrows have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P1CurrentBuildingPrices[1], "Burrow"))
                    {
                        P1CurrentBuildingPrices[1]++;
                        P1RemainingBuildings[1]--;
                        P1BuiltBuildings[1]++;
                    }
                    break;
                case "Mine":
                    if (P1RemainingBuildings[2] == 0)
                    {
                        Debug.Log("All Mines have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P1CurrentBuildingPrices[2], "Mine"))
                    {
                        P1CurrentBuildingPrices[2]++;
                        P1RemainingBuildings[2]--;
                        if (mineType == "Grass")
                        {
                            P1BuiltBuildings[2]++;
                        }
                        else if (mineType == "Dirt")
                        {
                            P1BuiltBuildings[3]++;
                        }
                        else if (mineType == "Stone")
                        {
                            P1BuiltBuildings[4]++;
                        }
                    }
                    break;
            }
        }
        else if (player == 2)
        {
            switch (type)
            {
                case "Factory":
                    if (P2RemainingBuildings[0] == 0)
                    {
                        Debug.Log("All Factories have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P2CurrentBuildingPrices[0], "Factory"))
                    {
                        P2CurrentBuildingPrices[0]++;
                        P2RemainingBuildings[0]--;
                        P2BuiltBuildings[0]++;
                    }
                    break;
                case "Burrow":
                    if (P2RemainingBuildings[1] == 0)
                    {
                        Debug.Log("All Burrows have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P2CurrentBuildingPrices[1], "Burrow"))
                    {
                        P2CurrentBuildingPrices[1]++;
                        P2RemainingBuildings[1]--;
                        P2BuiltBuildings[1]++;
                    }
                    break;
                case "Mine":
                    if (P2RemainingBuildings[2] == 0)
                    {
                        Debug.Log("All Mines have been built!");
                        break;
                    }

                    if (AbleToBuild(CurrentPlayer, P2CurrentBuildingPrices[2], "Mine"))
                    {
                        P2CurrentBuildingPrices[2]++;
                        P2RemainingBuildings[2]--;
                        if (mineType == "Grass")
                        {
                            P2BuiltBuildings[2]++;
                        }
                        else if (mineType == "Dirt")
                        {
                            P2BuiltBuildings[3]++;
                        }
                        else if (mineType == "Stone")
                        {
                            P2BuiltBuildings[4]++;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if the player has enough "Cards" to build something. Subtracts cards accordingly.
    /// </summary>
    /// <param name="currentPrice">Price var of current building type.</param>
    /// <param name="type">Name of building.</param>
    /// <returns>True if cards are substantial. False if otherwise.</returns>
    private bool AbleToBuild(int player, int currentPrice, string type)
    {
        if(player == 1)
        {
            if (P1Cards + P1GoldCards >= currentPrice)
            {
                for (int i = currentPrice; i != 0; i--)
                {
                    if (P1Cards > 0)
                    {
                        P1Cards--;
                    }
                    else if (P1GoldCards > 0)
                    {
                        P1GoldCards--;
                    }
                }
                Debug.Log("Built " + type + " for " + currentPrice + " cards.");
                return true;
            }
            else
            {
                Debug.Log("Not enough cards to build " + type + "! (Need " + currentPrice + " have " + (P1Cards + P1GoldCards) + ".)");
                return false;
            }
        }
        else if(player == 2)
        {
            if (P2Cards + P2GoldCards >= currentPrice)
            {
                for (int i = currentPrice; i != 0; i--)
                {
                    if (P2Cards > 0)
                    {
                        P2Cards--;
                    }
                    else if (P2GoldCards > 0)
                    {
                        P2GoldCards--;
                    }
                }
                Debug.Log("Built " + type + " for " + currentPrice + " cards.");
                return true;
            }
            else
            {
                Debug.Log("Not enough cards to build " + type + "! (Need " + currentPrice + " have " + (P2Cards + P2GoldCards) + ".)");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("Incorrect parameter provided for player.");
            return false;
        }
    }

    /// <summary>
    /// Activates mines and adds tiles. 
    /// </summary>
    public void ActivateMines(int player)
    {
        if(player == 1)
        {
            P1CollectedPile[0] += P1BuiltBuildings[2];
            P1CollectedPile[1] += P1BuiltBuildings[3];
            P1CollectedPile[2] += P1BuiltBuildings[4];
        }
        else if(player == 2)
        {
            P2CollectedPile[0] += P2BuiltBuildings[2];
            P2CollectedPile[1] += P2BuiltBuildings[3];
            P2CollectedPile[2] += P2BuiltBuildings[4];
        }
    }

    /// <summary>
    /// Draws cards for a selected player.
    /// </summary>
    /// <param name="player">1 or 2. Which hand to send the cards to.</param>
    /// <param name="cardsToDraw">How many cards you should draw.</param>
    public void DrawCards(int player, int cardsToDraw)
    {
        if(player == 1)
        {
            P1Cards += cardsToDraw;
        }
        else if(player == 2)
        {
            P2Cards += cardsToDraw;
        }
        Debug.Log("Drew " + cardsToDraw + " cards!");
    }

    /// <summary>
    /// Discards cards down to the hand limit.
    /// </summary>
    public void DiscardCards(int player)
    {
        if(player == 1)
        {
            int discardedCards = 0;
            while (P1Cards + P1GoldCards > HandLimit)
            {
                discardedCards++;
                if (P1Cards > 0)
                {
                    P1Cards--;
                }
                else
                {
                    P1GoldCards--;
                }
            }
            Debug.Log("Player " + player + " discarded " + discardedCards + " cards!");
        }
        else if(player == 2)
        {
            int discardedCards = 0;
            while (P2Cards + P2GoldCards > HandLimit)
            {
                discardedCards++;
                if (P2Cards > 0)
                {
                    P2Cards--;
                }
                else
                {
                    P2GoldCards--;
                }
            }
            Debug.Log("Player " + player + " discarded " + discardedCards + " cards!");
        }
    }
}