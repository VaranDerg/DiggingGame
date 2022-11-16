/*****************************************************************************
// File Name :         OnlineAudioPlayer.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     November 16th, 2022
//
// Brief Description : This document controls playing sfx and audio in the
                       online game mode
*****************************************************************************/

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineAudioPlayer : MonoBehaviourPun
{
    /// <summary>
    /// Calls the RPC that plays a sound on all clients OR plays a sound 
    /// locally
    /// 
    /// </summary>
    /// <param name="sound"> name of the sound to be played </param>
    /// <param name="online"> true if the sound plays on both clients </param>
    public void PlaySound(string sound, bool online)
    {
        if (online)
        {
            photonView.RPC("PlaySound", RpcTarget.All, sound);
        }
        else
        {
            PlaySound(sound);
        }
    }

    /// <summary>
    /// Plays a sound
    /// 
    /// </summary>
    /// <param name="sound"> sound to be played </param>
    [PunRPC]
    private void PlaySound(string sound)
    {
        FindObjectOfType<SFXManager>().Play(sound);
    }
}
