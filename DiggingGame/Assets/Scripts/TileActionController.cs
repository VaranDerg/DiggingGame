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
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileActionController : MonoBehaviour
{

    private Grid _grid;     //grid that holds the tilemaps

    // map the player can interact with
    [SerializeField] private Tilemap _interactiveMap;

    // grass, dirt, and stone tile maps respectively
    [SerializeField] private Tilemap _grassMap;     
    [SerializeField] private Tilemap _dirtMap;
    [SerializeField] private Tilemap _stoneMap;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _grid = gameObject.GetComponent<Grid>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        Vector3Int mousePos = GetMousePosition();
        _interactiveMap = _grassMap;

        // Changes the interactable map depending on which tiles are null
        // (previously clicked) in each tile map ensuring the player is 
        // interacting with the currently visible tile they clicked on
        if (Input.GetMouseButtonDown(0))
        {
            if (_grassMap.GetTile(mousePos) == null)
            {
                _interactiveMap = _dirtMap;
            }
            if (_dirtMap.GetTile(mousePos) == null)
            {
                _interactiveMap = _stoneMap;
            }       
            _interactiveMap.SetTile(mousePos, null);
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