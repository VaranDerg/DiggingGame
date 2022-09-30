using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreateBoard : MonoBehaviour
{
    //board to be spawned
    public GameObject boardPrefab;

    // x and y positions the board will spawn it
    public float xPos, yPos;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {    
        // position where the board will spawn
        Vector2 boardPos = new Vector2(xPos, yPos);

        // Instantiate with photon network so each player can see the same
        // version of the object
        PhotonNetwork.Instantiate(boardPrefab.name, boardPos, 
            Quaternion.identity);
    }
}
