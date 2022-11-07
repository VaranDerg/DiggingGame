/*****************************************************************************
// File Name :         Sound.cs
// Author :            Sean Forrester
// Creation Date :     October 27th, 2022
//
// Brief Description : Sound Script
*****************************************************************************/

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string name;

    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
} 