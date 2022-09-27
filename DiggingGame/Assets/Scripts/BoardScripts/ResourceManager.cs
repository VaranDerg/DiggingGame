/*****************************************************************************
// File Name :         ResourceManager.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 26th, 2022
//
// Brief Description : This document controls each players amount of resources.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    // amount of collected resources
    private int _collectedGrass, _collectedDirt, _collectedStone;


    /// <summary>
    /// Changes amount of grass the player has
    /// </summary>
    /// <param name="amount"> The amount the grass changes by </param>
    public void SetGrass(int amount)
    {
        _collectedGrass += amount;
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();
        _gm.UpdateText(_gm.p1GrassText, "Grass: " + _collectedGrass.ToString());
    }

    /// <summary>
    /// Changes amount of dirt the player has
    /// </summary>
    /// <param name="amount"> The amount that dirt changes by </param>
    public void SetDirt(int amount)
    {
        _collectedDirt += amount;
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();
        _gm.UpdateText(_gm.p1DirtText, "Dirt: " + _collectedDirt.ToString());
    }

    /// <summary>
    /// Changes amount of stone the player has
    /// </summary>
    /// <param name="amount"></param>
    public void SetStone(int amount)
    {
        _collectedStone += amount;
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();
        _gm.UpdateText(_gm.p1StoneText, "Stone: " + _collectedStone.ToString());
    }

    /// <summary>
    /// How much grass the player has
    /// </summary>
    /// <returns> amount of grass the player currently has </returns>
    public int GetGrass()
    {
        return _collectedGrass;
    }

    /// <summary>
    /// How much dirt the player has
    /// </summary>
    /// <returns> amount of dirt the player currently has </returns>
    public int GetDirt()
    {
        return _collectedDirt;
    }

    /// <summary>
    /// How much stone the player has
    /// </summary>
    /// <returns> amount of stone the player currently has </returns>
    public int GetStone()
    {
        return _collectedStone;
    }
}
