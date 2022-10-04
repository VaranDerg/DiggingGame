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
    private int _p2CollectedGrass, _p2CollectedDirt, _p2CollectedStone;


    /// <summary>
    /// Changes amount of grass the player has
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> The amount the grass changes by </param>
    public void SetGrass(int amount)
    {
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();

        if (_gm.CurrentPlayer == 1)
        {
            _collectedGrass += amount;
        }
        else if(_gm.CurrentPlayer == 2)
        {
            _p2CollectedGrass += amount;
        }

        if(_gm.CurrentPlayer == 1)
        {
            _gm.UpdateText(_gm.p1GrassText, "Grass: " + _collectedGrass.ToString());
        }
        else if(_gm.CurrentPlayer == 2)
        {
            _gm.UpdateText(_gm.p2GrassText, "Grass: " + _p2CollectedGrass.ToString());
        }
    }

    /// <summary>
    /// Changes amount of dirt the player has
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> The amount that dirt changes by </param>
    public void SetDirt(int amount)
    {
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();
        if (_gm.CurrentPlayer == 1)
        {
            _collectedDirt += amount;
        }
        else if (_gm.CurrentPlayer == 2)
        {
            _p2CollectedDirt += amount;
        }

        if (_gm.CurrentPlayer == 1)
        {
            _gm.UpdateText(_gm.p1DirtText, "Dirt: " + _collectedDirt.ToString());
        }
        else if (_gm.CurrentPlayer == 2)
        {
            _gm.UpdateText(_gm.p2DirtText, "Dirt: " + _p2CollectedDirt.ToString());
        }
    }

    /// <summary>
    /// Changes amount of stone the player has
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"></param>
    public void SetStone(int amount)
    {
        GameplayManager _gm = GameObject.FindObjectOfType<GameplayManager>();
        if (_gm.CurrentPlayer == 1)
        {
            _collectedStone += amount;
        }
        else if (_gm.CurrentPlayer == 2)
        {
            _p2CollectedStone += amount;
        }

        if (_gm.CurrentPlayer == 1)
        {
            _gm.UpdateText(_gm.p1StoneText, "Stone: " + _collectedStone.ToString());
        }
        else if (_gm.CurrentPlayer == 2)
        {
            _gm.UpdateText(_gm.p2StoneText, "Stone: " + _p2CollectedStone.ToString());
        }
    }

    /// <summary>
    /// How much grass the player has
    /// Author: Andrea SD
    /// </summary>
    /// <returns> amount of grass the player currently has </returns>
    public int GetGrass()
    {
        return _collectedGrass;
    }

    /// <summary>
    /// How much dirt the player has
    /// Author: Andrea SD
    /// </summary>
    /// <returns> amount of dirt the player currently has </returns>
    public int GetDirt()
    {
        return _collectedDirt;
    }

    /// <summary>
    /// How much stone the player has
    /// Author: Andrea SD
    /// </summary>
    /// <returns> amount of stone the player currently has </returns>
    public int GetStone()
    {
        return _collectedStone;
    }
}
