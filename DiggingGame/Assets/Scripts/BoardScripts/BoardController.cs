/*****************************************************************************
// File Name :         BoardController.cs
// Author :            Andrea Swihart-DeCoster & Rudy W.
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
    //Edited: Rudy W. Organized with headers, added certain variables for functionality.

    [Header("References")]
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _stoneSprite;
    [SerializeField] private Sprite _bedRockSprite;

    [Header("Tile Values/Information")]
    public bool IsInteractable = true;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasBuilding;
    [HideInInspector] public bool HasPawn;
    [HideInInspector] public bool IsAdjacent;

    /// <summary>
    /// Represents one of three states: One - Grass, Two - Dirt, Three - Stone,
    /// Four - Bedrock
    /// Author: Andrea SD
    /// </summary>
    public enum GameState
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
    /// This method controls what happens when the player left/right clicks on the
    /// board piece.
    /// Author: Andrea SD
    /// Edited: Rudy W. Moved Debug statements into SetObjectState along with sprite change lines, as states may change through 
    ///         separate effects in the future. Additionally, moved into OnMouseOver for further usability; allows for replacing 
    ///         pieces with right click temporarily.
    /// </summary>
    private void OnMouseOver()
    {
        // Once clicked, the piece will change states to the piece below it and
        // the sprite is changed to reflect that.
        // Example: grass -> dirt, dirt -> stone, stone -> bedrock. reverse for right click

        //The board cannot be adjusted if a tile is not marked as interactable. If commented, it's being tested.
        //if(!IsInteractable)
        //{
        //    return;
        //}

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (ObjState)
            {
                case GameState.One:
                    SetObjectState(2);
                    break;
                case GameState.Two:
                    SetObjectState(3);
                    break;
                case GameState.Three:
                    SetObjectState(4);
                    break;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (ObjState)
            {
                case GameState.Two:
                    SetObjectState(1);
                    break;
                case GameState.Three:
                    SetObjectState(2);
                    break;
                case GameState.Four:
                    SetObjectState(3);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// Edited: 
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    public void SetObjectState(int state) 
    {
        Debug.Log("Switching " + gameObject.name + "'s State to " + state + ".");

        switch (state)
        {
            case 1:
                gameObject.GetComponent<SpriteRenderer>().sprite = _grassSprite;
                ObjState = GameState.One;
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().sprite = _dirtSprite;
                ObjState = GameState.Two;
                break;
            case 3:
                gameObject.GetComponent<SpriteRenderer>().sprite = _stoneSprite;
                ObjState = GameState.Three;
                break;
            case 4:
                gameObject.GetComponent<SpriteRenderer>().sprite = _bedRockSprite;
                ObjState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
    }
}
