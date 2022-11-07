/*****************************************************************************
// File Name :         AudioManager.cs
// Author :            Sean Forrester
// Creation Date :     October 27th, 2022
//
// Brief Description : Audio Manager Script
*****************************************************************************/

using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found");
            return;
        }

        s.source.Play();
    }
}

// FindObjectOfType<AudioManger>().Play("NameofSoundEffect");//
//this line of code is able to be placed into any script