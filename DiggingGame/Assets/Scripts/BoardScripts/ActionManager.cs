using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [HideInInspector] public int CollectedGrass, CollectedDirt, CollectedStone, CollectedGold;
    [HideInInspector] public int RefinedGrass, RefinedDirt, RefinedStone, RefinedGold;
    [HideInInspector] public int SupplyGrass, SupplyDirt, SupplyStone, SupplyGold;
    [HideInInspector] public int BuiltFactories, BuiltBurrows, GrassMines, DirtMines, StoneMines;
    [HideInInspector] public int Cards = 0;
    [HideInInspector] public int GoldCards = 0;
    [HideInInspector] public int CurrentTurnPhase = 0;
    [HideInInspector] public int CurrentFactoryPrice, CurrentBurrowPrice, CurrentMinePrice;

    [Header("Player Values")]
    public int CardActivations;
    public int StartingCards;
    public int HandLimit;
    public int CardDraw;

    [Header("Building Values")]
    public int BuildingPrice;
    public int RemainingFactories, RemainingBurrows, RemainingMines;

    /// <summary>
    /// Sets the card amount.
    /// </summary>
    private void Start()
    {
        Cards = StartingCards;
        CurrentFactoryPrice = BuildingPrice;
        CurrentBurrowPrice = BuildingPrice;
        CurrentMinePrice = BuildingPrice;
    }

    /// <summary>
    /// Collects a tile and adds it to one of the "collected" variables. 
    /// </summary>
    /// <param name="type">"Grass" "Dirt" "Stone" or "Gold"</param>
    public void CollectTile(string type)
    {
        switch(type)
        {
            case "Grass":
                CollectedGrass++;
                break;
            case "Dirt":
                CollectedDirt++;
                break;
            case "Stone":
                CollectedStone++;
                break;
            case "Gold":
                CollectedGold++;
                break;
        }
    }

    /// <summary>
    /// Moves tiles from the Collected pile to the Refined pile. 
    /// </summary>
    public void RefineTiles()
    {
        RefinedGrass += CollectedGrass;
        RefinedDirt += CollectedDirt;
        RefinedStone += CollectedStone;
        RefinedGold += CollectedGold;

        CollectedGrass = 0;
        CollectedDirt = 0;
        CollectedStone = 0;
        CollectedGold = 0;
    }

    /// <summary>
    /// Uses tiles based on provided cost. 
    /// </summary>
    /// <param name="grassCost">How many grass tiles to use.</param>
    /// <param name="dirtCost">How many dirt tiles to use.</param>
    /// <param name="stoneCost">How many stone tiles to use.</param>
    public bool UseTiles(int grassCost, int dirtCost, int stoneCost)
    {
        if(grassCost > RefinedGrass || dirtCost > RefinedDirt || stoneCost > RefinedStone)
        {
            Debug.Log("Not enough tiles to use this effect!");
            return false;
        }
        RefinedGrass -= grassCost;
        RefinedDirt -= dirtCost;
        RefinedStone -= stoneCost;

        SupplyGrass += grassCost;
        SupplyDirt += dirtCost;
        SupplyStone += stoneCost;
        return true;
    }

    /// <summary>
    /// Uses gold tiles.
    /// </summary>
    public bool UseGold()
    {
        if(RefinedGold == 0)
        {
            Debug.Log("No gold to use!");
            return false;
        }

        if(Cards == 0)
        {
            Debug.Log("No cards to Retrieve with!");
            return false;
        }

        Cards--;
        GoldCards++;
        RefinedGold--;
        SupplyGold++;
        Debug.Log("Spent 1 card and 1 gold for 1 gold card!");
        return true;
    }

    /// <summary>
    /// Place a tile back onto the board. 
    /// </summary>
    /// <param name="type">"Grass" "Dirt" or "Stone"</param>
    public void PlaceTiles(string type)
    {
        switch(type)
        {
            case "Grass":
                //Place a grass tile
                SupplyGrass--;
                break;
            case "Dirt":
                //Place a dirt tile
                SupplyDirt--;
                break;
            case "Stone":
                //Place a stone tile
                SupplyStone--;
                break;
        }
    }

    /// <summary>
    /// Builds a building. 
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    /// <param name="mineType">If "Mine" must list a type. "Grass" "Dirt" or "Stone"</param>
    public void BuildBuilding(string type, string mineType)
    {
        switch(type)
        {
            case "Factory":
                if(RemainingFactories == 0)
                {
                    Debug.Log("All Factories have been built!");
                    break;
                }

                if(AbleToBuild(CurrentFactoryPrice, "Factory"))
                {
                    CurrentFactoryPrice++;
                    RemainingFactories--;
                    BuiltFactories++;
                }
                break;
            case "Burrow":
                if (RemainingBurrows == 0)
                {
                    Debug.Log("All Burrows have been built!");
                    break;
                }

                if (AbleToBuild(CurrentBurrowPrice, "Burrow"))
                {
                    CurrentBurrowPrice++;
                    RemainingBurrows--;
                    BuiltBurrows++;
                }
                break;
            case "Mine":
                if (RemainingMines == 0)
                {
                    Debug.Log("All Mines have been built!");
                    break;
                }

                if (AbleToBuild(CurrentMinePrice, "Mine"))
                {
                    CurrentMinePrice++;
                    RemainingMines--;
                    if (mineType == "Grass")
                    {
                        GrassMines++;
                    }
                    else if (mineType == "Dirt")
                    {
                        DirtMines++;
                    }
                    else if (mineType == "Stone")
                    {
                        StoneMines++;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Checks if the player has enough "Cards" to build something. Subtracts cards accordingly.
    /// </summary>
    /// <param name="currentPrice">Price var of current building type.</param>
    /// <param name="type">Name of building.</param>
    /// <returns>True if cards are substantial. False if otherwise.</returns>
    private bool AbleToBuild(int currentPrice, string type)
    {
        if (Cards + GoldCards >= currentPrice)
        {
            for (int i = currentPrice; i != 0; i--)
            {
                if (Cards > 0)
                {
                    Cards--;
                }
                else if (GoldCards > 0)
                {
                    GoldCards--;
                }
            }
            Debug.Log("Built " + type +  " for " + currentPrice + " cards.");
            return true;
        }
        else
        {
            Debug.Log("Not enough cards to build " + type + "! (Need " + currentPrice + " have " + (Cards + GoldCards) + ".)");
            return false;
        }
    }

    /// <summary>
    /// Activates mines and adds tiles. 
    /// </summary>
    public void ActivateMines()
    {
        CollectedGrass += GrassMines;
        CollectedDirt += DirtMines;
        CollectedStone += StoneMines;
    }

    /// <summary>
    /// Draws cards for a selected player.
    /// </summary>
    /// <param name="player">1 or 2. Which hand to send the cards to.</param>
    /// <param name="cardsToDraw">How many cards you should draw.</param>
    public void DrawCards(int player, int cardsToDraw)
    {
        Cards += cardsToDraw;
        Debug.Log("Drew " + cardsToDraw + " cards!"); 
    }

    /// <summary>
    /// Discards cards down to the hand limit.
    /// </summary>
    public void DiscardCards()
    {
        int discardedCards = 0;
        while (Cards + GoldCards > HandLimit)
        {
            discardedCards++;
            if (Cards > 0)
            {
                Cards--;
            }
            else
            {
                GoldCards--;
            }
        }
        Debug.Log("Discarded " + discardedCards + " cards!");
    }
}
