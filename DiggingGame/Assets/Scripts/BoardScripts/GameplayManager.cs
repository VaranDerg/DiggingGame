/*****************************************************************************
// File Name :         GameplayManager.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 26th, 2022
//
// Brief Description : This document control the basic gameplay loop and 
                       mechanics.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public int CurrentPlayer;

    // TextMeshPro UIs
    [SerializeField] TextMeshProUGUI currentPlayerText;

    public TextMeshProUGUI p1DirtText;
    public TextMeshProUGUI p2DirtText;
    public TextMeshProUGUI p1GrassText;
    public TextMeshProUGUI p2GrassText;
    public TextMeshProUGUI p1StoneText;
    public TextMeshProUGUI p2StoneText;

    // Game board piece prefab
    [SerializeField] GameObject _grassPrefab;

    // X and Y positions the game spaces will spawn at
    [SerializeField] float xPos;
    [SerializeField] float yPos;

    // Game board parent object to each space piece
    [SerializeField] GameObject gameBoard;

    /// <summary>
    /// Start is called before the first frame update
    /// Author: Andrea SD
    /// </summary>
    void Start()
    {
        SpawnBoard();
        CurrentPlayer = 1;
        UpdateText(currentPlayerText, "Current Turn: " + CurrentPlayer);
    }

    /// <summary>
    /// Switches between the two players
    /// Author: Andrea SD
    /// </summary>
    public void ChangePlayer()
    {
        if(CurrentPlayer == 1)
        {
            CurrentPlayer = 2;
        }
        else
        {
            CurrentPlayer = 1;
        }
        
        UpdateText(currentPlayerText, "Current Turn: " + CurrentPlayer);

    }

    /// <summary>
    /// Updates a text UI element
    /// Author: Andrea SD
    /// </summary>
    /// <param name="textEdit"> Text UI element to be changed </param>
    /// <param name="newText"> what the text will be changed to </param>
    public void UpdateText(TextMeshProUGUI textEdit, string newText)
    {
        textEdit.text = newText;
    }

    /// <summary>
    /// Access the current player variable
    /// Author: Andrea SD
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPlayer()
    {
        return CurrentPlayer;
    }    

    /// <summary>
    /// Spawns the game board of objects vertically beginning at the bottom 
    /// left.
    /// Author: Andrea SD
    /// </summary>
    private void SpawnBoard()
    {      
        float newXPos = xPos;
        float newYPos = yPos;

        for (int i = 0; i < 6; i++)
        { 
            for(int j = 0; j < 6; j++)
            { 
                GameObject tempPiece = Instantiate(_grassPrefab);
                
                tempPiece.transform.position = new Vector3(newXPos, newYPos);
                // Places each new piece into a parent object to keep the
                // hierarchy organized.
                tempPiece.transform.SetParent(gameBoard.transform);
                newYPos++;
            }
            newYPos = yPos;
            newXPos++;
        }
    }
}
