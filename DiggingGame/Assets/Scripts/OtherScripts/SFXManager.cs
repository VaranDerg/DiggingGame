/*****************************************************************************
// File Name :         AudioManager.cs
// Author :            Sean Forrester, Rudy W
// Creation Date :     October 27th, 2022
//
// Brief Description : Manager for Sound Effects.
*****************************************************************************/

using UnityEngine.Audio;
using System;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    /// <summary>
    /// Prepares the Audio Manager as a Nondestructable.
    /// </summary>
    public static SFXManager s_Instance;
    void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(s_Instance);
            s_Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _soundEffectsMixerGroup;

    [Header("Audio Clips")]
    public Sound[] Sounds;

    /// <summary>
    /// Preps each Sound Effect.
    /// </summary>
    private void Start()
    {
        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;

            switch (s.AudioType)
            {
                case Sound.AudioTypes.SoundEffect:
                    s.Source.outputAudioMixerGroup = _soundEffectsMixerGroup;
                    break;

                case Sound.AudioTypes.Music:
                    s.Source.outputAudioMixerGroup = _musicMixerGroup;
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
       Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            return;
        }

        if(s.Source == null)
        {
            return;
        }

        s.Source.Play();
    }

    /// <summary>
    /// Stops a sound
    /// </summary>
    /// <param name="name">Sound name</param>
    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.Source.Stop();
    }

    /// <summary>
    /// Plays a sound for clicking buttons.
    /// </summary>
    public void PlayButtonSound()
    {
        int sound2play = UnityEngine.Random.Range(1, 4);
        if (sound2play == 1)
        {
            FindObjectOfType<SFXManager>().Play("Button1");
        }
        else if (sound2play == 2)
        {
            FindObjectOfType<SFXManager>().Play("Button2");
        }
        else
        {
            FindObjectOfType<SFXManager>().Play("Button3");
        }
    }
}

// FindObjectOfType<AudioManager>().Play("NameofSoundEffect");//
//this line of code is able to be placed into any script