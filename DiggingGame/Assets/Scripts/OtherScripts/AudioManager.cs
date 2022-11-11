/*****************************************************************************
// File Name :         AudioManager.cs
// Author :            Sean Forrester, Rudy W
// Creation Date :     October 27th, 2022
//
// Brief Description : Audio Manager Script
*****************************************************************************/

using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _soundEffectsMixerGroup;

    [Header("Audio Clips")]
    public Sound[] Sounds;

    [Header("NDE Information")]
    public static AudioManager s_Instance;

    /// <summary>
    /// Prepares the Audio Manager as a Nondestructable.
    /// </summary>
    void Awake() 
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in Sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            switch(s.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = _soundEffectsMixerGroup;
                    break;

                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = _musicMixerGroup;
                    break;
            }
        }
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="name">Sound Name</param>
    public void Play(string name)
    {
       Sound s = Array.Find(Sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found");
            return;
        }

        s.source.Play();
    }

    /// <summary>
    /// Plays a Sound and returns the sound for future reference.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Sound PlayReferenced(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found");
            return null;
        }

        s.source.Play();
        return s;
    }

    /// <summary>
    /// Stops a sound
    /// </summary>
    /// <param name="name">Sound name</param>
    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found");
            return;
        }

        s.source.Stop();
    }
}

// FindObjectOfType<AudioManager>().Play("NameofSoundEffect");//
//this line of code is able to be placed into any script