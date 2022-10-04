/*****************************************************************************
// File Name :         BoardController.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     October 3rd, 2022
//
// Brief Description : This document controls the players interactions with the
                       game board.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;

public class BoardController : MonoBehaviour
{
    [SerializeField] Sprite _grassSprite;
    [SerializeField] Sprite _dirtSprite;
    [SerializeField] Sprite _stoneSprite;
    [SerializeField] Sprite _bedRockSprite;

    private GameState _objState;

    /// <summary>
    /// Represents one of three states: One - Grass, Two - Dirt, Three - Stone,
    /// Four - Gone
    /// Author: Andrea SD
    /// </summary>
    enum GameState
    {
        One,
        Two,
        Three,
        Four
    }

    // Start is called before the first frame update
    void Start()
    {
        SetObjectState(1);
    }

    /// <summary>
    /// Update is called before the first frame update
    /// </summary>
    void Update()
    {
    }

    /// <summary>
    /// This method controls what happens when the player left clicks on the
    /// board piece.
    /// Author: Andrea SD
    /// </summary>
    private void OnMouseDown()
    {
        // Once clicked, the piece will change states to the piece below it and
        // the sprite is changed to reflect that.
        // Example: grass -> dirt, dirt -> stone, stone -> (disappears)
        switch (_objState)
        {
            case GameState.One:
                gameObject.GetComponent<SpriteRenderer>().sprite = _dirtSprite;
                SetObjectState(2);
                Debug.Log(_objState);
                break;
            case GameState.Two:
                gameObject.GetComponent<SpriteRenderer>().sprite = _stoneSprite;
                SetObjectState(3);
                Debug.Log(_objState);
                break;
            case GameState.Three:
                gameObject.GetComponent<SpriteRenderer>().sprite = 
                    _bedRockSprite;
                SetObjectState(4);
                Debug.Log(_objState);
                break;
        }   
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    public void SetObjectState(int state) 
    {
        switch(state)
        {
            case 1:
                _objState = GameState.One;
                break;
            case 2: 
                _objState = GameState.Two;
                break;
            case 3:
                _objState = GameState.Three;
                break;
            case 4:
                _objState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
        
    }
}
