/*****************************************************************************
// File Name :         Sound.cs
// Author :            Sean Forrester, Rudy W.
// Creation Date :     October 27th, 2022
//
// Brief Description : Sound Script
*****************************************************************************/

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public enum AudioTypes
    {
        SoundEffect,
        Music
    }
    public AudioTypes AudioType;
    public bool IsDayMusic;
    public bool IsNightMusic;

    public string Name;

    public AudioClip Clip;
    
    [Range(0f, 1f)]
    public float Volume = 0.5f;
    [Range(.1f, 3f)]
    public float Pitch = 1;

    public bool Loop;

    [HideInInspector]
    public AudioSource Source;
} 