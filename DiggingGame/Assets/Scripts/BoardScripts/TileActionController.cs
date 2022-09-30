/*****************************************************************************
// File Name :         TileActionController.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     September 20th, 2022
//
// Brief Description : This document controls what happens when a player clicks 
                       on a tile.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class TileActionController : MonoBehaviour
{
    ResourceManager rManager = new ResourceManager();
    private GameplayManager _gm;

    private Grid _grid;     //grid that holds the tilemaps

    // map the player can interact with
    [SerializeField] private Tilemap _interactiveMap;

    [SerializeField] private Tile _grassTile;
    [SerializeField] private Tile _dirtTile;
    [SerializeField] private Tile _stoneTile;
    [SerializeField] private Tile _buildingTile;
    private Tile _newTile;

    // grass, dirt, and stone tile maps respectively
    [SerializeField] private Tilemap _grassMap;     
    [SerializeField] private Tilemap _dirtMap;
    [SerializeField] private Tilemap _stoneMap;

    PhotonView view;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _grid = gameObject.GetComponent<Grid>();
        _gm = FindObjectOfType<GameplayManager>();
        view = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        Vector3Int mousePos = GetMousePosition();
        //_interactiveMap = _grassMap;

        // Changes the interactable map depending on which tiles are null
        // (previously clicked) in each tile map ensuring the player is 
        // interacting with the currently visible tile they clicked on
        if (Input.GetMouseButtonDown(0))
        {
            if (_grassMap.GetTile(mousePos) != null)
            {
                _interactiveMap = _grassMap;
                rManager.SetGrass(1);
            }
            else if (_grassMap.GetTile(mousePos) == null )
            {
                if (_dirtMap.GetTile(mousePos) != null)
                {
                    _interactiveMap = _dirtMap;
                    rManager.SetDirt(1);
                }
                else if (_stoneMap.GetTile(mousePos) != null)
                {
                    _interactiveMap = _stoneMap;
                    rManager.SetStone(1);
                }
            }
            _interactiveMap.SetTile(mousePos, null);
        }

        //placing tiles
        if (Input.GetMouseButtonDown(1))
        {
            if (_stoneMap.GetTile(mousePos) == null && rManager.GetStone() > 0)
            {
                _interactiveMap = _stoneMap;
                rManager.SetStone(-1);
                _newTile = _stoneTile;
            }
            else if (_dirtMap.GetTile(mousePos) == null && rManager.GetDirt() > 0)
            {
                _interactiveMap = _dirtMap;
                rManager.SetDirt(-1);
                _newTile = _dirtTile;
            }
            else if (_grassMap.GetTile(mousePos) == null && rManager.GetGrass() > 0)
            {
                _interactiveMap = _grassMap;
                rManager.SetGrass(-1);
                _newTile = _grassTile;

            }
            
            _interactiveMap.SetTile(mousePos, _newTile);
        }

        //place building
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (_grassMap.GetTile(mousePos) != null)
            {
                _interactiveMap = _grassMap;
            }
            else if (_dirtMap.GetTile(mousePos) != null)
            {
                _interactiveMap = _dirtMap;
            }
            else if (_stoneMap.GetTile(mousePos) != null)
            {
                _interactiveMap = _stoneMap;
            }
            
            _interactiveMap.SetTile(mousePos, _buildingTile);
        }


    }

    /// <summary>
    /// Converts the mouse position on screen to  aworld point then to a cell 
    /// position
    /// </summary>
    /// <returns> cell position at the world point </returns>
    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = 
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(mouseWorldPos);
    }
}