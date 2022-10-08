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
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MultiPieceController : MonoBehaviourPun
{
    //Edited: Rudy W. Organized with headers, added certain variables for functionality.

    [Header("Piece References")]
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _dirtSprite;
    [SerializeField] private Sprite _stoneSprite;
    [SerializeField] private Sprite _bedRockSprite;
    private SpriteRenderer _sr;

    [Header("Building References")]
    [SerializeField] private Transform _buildingSlot;
    [SerializeField] private GameObject _factory, _burrow, _mine;


    [Header("Tile Values/Information")]
    [HideInInspector] public bool IsInteractable = false;
    [HideInInspector] public bool IsRemovable = false;
    [SerializeField] private Color _adjacentColor;
    [SerializeField] private Color _defaultColor;
    [HideInInspector] public GameState ObjState;
    [HideInInspector] public bool HasBuilding;
    [HideInInspector] public bool HasPawn;
    [HideInInspector] public bool IsAdjacent;


    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

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
    private void Start()
    {
        
        SetObjectState(1);
        _sr.color = _defaultColor;
    }

    /// <summary>
    /// This method controls what happens when the interacts with the board.
    /// Author: Andrea SD
    /// Edited: Rudy W. Moved Debug statements into SetObjectState along with sprite change lines, as states may change through 
    ///         separate effects in the future. Additionally, moved into OnMouseOver for further usability; allows for replacing 
    ///         pieces with right click temporarily.
    /// </summary>
    private void OnMouseOver()
    {
        BuildingPlacementAndRemoval();
        PiecePlacementAndRemoval();
    }

    /// <summary>
    /// Method controlling Piece placement and removal.
    /// </summary>
    private void PiecePlacementAndRemoval()
    {
        // Once clicked, the piece will change states to the piece below it and
        // the sprite is changed to reflect that.
        // Example: grass -> dirt, dirt -> stone, stone -> bedrock. reverse for right click

        //The board cannot be adjusted if a tile is not marked as interactable.If commented, it's being tested.
        if (!IsRemovable)
        {
            //return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (ObjState)
            {
                case GameState.One:
                    photonView.RPC("SetObjectState", RpcTarget.All, 2);
                    break;
                case GameState.Two:
                    photonView.RPC("SetObjectState", RpcTarget.All, 3);
                    break;
                case GameState.Three:
                    photonView.RPC("SetObjectState", RpcTarget.All, 4);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (ObjState)
            {
                case GameState.Two:
                    photonView.RPC("SetObjectState", RpcTarget.All, 1);
                    break;
                case GameState.Three:
                    photonView.RPC("SetObjectState", RpcTarget.All, 2);
                    break;
                case GameState.Four:
                    photonView.RPC("SetObjectState", RpcTarget.All, 3);
                    break;
            }
        }
    }

    [PunRPC]
    /// <summary>
    /// Method controlling Building placement and removal.
    /// </summary>
    private void BuildingPlacementAndRemoval()
    {
        if(!IsInteractable)
        {
            return;
        }

        if(HasBuilding)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            BuildBuilding(_factory);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BuildBuilding(_burrow);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            BuildBuilding(_mine);
        }
    }

    private void BuildBuilding(GameObject building)
    {
        GameObject newestBuilding = Instantiate(building, _buildingSlot);
        HasBuilding = true;
    }

    [PunRPC]
    /// <summary>
    /// Sets the state of the game object to one of the valid enum values
    /// Edited: 
    /// </summary>
    /// <param name="state"> determines which state the obj is set to </param>
    public void SetObjectState(int state) 
    {
        //Debug.Log("Switching " + gameObject.name + "'s State to " + state + ".");

        switch (state)
        {
                // Each case calls each client connected to the server and
                // changes the game object sprite to a sprite from the resources
            case 1:
                ChangeSprite("GrassPiece");
                ObjState = GameState.One;
                break;
            case 2:
                ChangeSprite("DirtPiece");
                ObjState = GameState.Two;
                break;
            case 3:
                ChangeSprite("StonePiece");
                ObjState = GameState.Three;
                break;
            case 4:
                ChangeSprite("BedrockPiece");
                ObjState = GameState.Four;
                break;
            default:
                throw new Exception("This board piece state does not exist.");
        }
    }

    [PunRPC]
    public void ChangeSprite(String newSprite)
    {
        _sr.sprite = Resources.Load<Sprite>(newSprite);
    }

        /// <summary>
        /// Updates tile to an adjacent state, allowing more interaction.
        /// </summary>
        public void AdjacentToPlayer()
    {
        _sr.color = _adjacentColor;
        IsAdjacent = true;
        IsInteractable = true;
    }

    /// <summary>
    /// Removes tile's adjacent state.
    /// </summary>
    public void NoLongerAdjacent()
    {
        _sr.color = _defaultColor;
        IsAdjacent = false;
        //IsInteractable = false;
    }

    /// <summary>
    /// Changes a Tile's occupant. A tile can hold 1 Pawn and 1 Building.
    /// </summary>
    /// <param name="isPlayer">True = Pawn; False = Building</param>
    /// <param name="isEntering">True = Being Placed; False = Being Removed</param>
    public void ChangeOccupation(bool isPlayer, bool isEntering)
    {
        if(isEntering)
        {
            if (isPlayer)
            {
                HasPawn = true;
            }
            else if (!isPlayer)
            {
                HasBuilding = true;
            }
        }
        else
        {
            if (isPlayer)
            {
                HasPawn = false;
            }
            else if (!isPlayer)
            {
                HasBuilding = false;
            }
        }
    }
}
