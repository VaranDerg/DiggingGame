/*****************************************************************************
// File Name :         BGMManager.cs
// Author :            Rudy W.
// Creation Date :     November 11th, 2022
//
// Brief Description : Similar to SFXManager, but for Music. Is able to iterate
                       through a list of songs for Day and Night and fade them
                       in and out.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    /// <summary>
    /// Prepares the Nondestructable.
    /// </summary>
    public static BGMManager s_Instance;
    private void Awake()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    [Header("Values")]
    public float SongFadeTime;

    [Header("Audio Clips")]
    [SerializeField] private Sound _mainMenuTheme;
    [SerializeField] private Sound[] _daytimeTracks;
    [SerializeField] private Sound[] _nighttimeTracks;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _soundEffectsMixerGroup;

    [Header("Other Variables")]
    [HideInInspector] public bool IsPlayingDayTrack = false;
    private Sound _currentTheme;
    private int _currentSongIndex;

    /// <summary>
    /// Assigns Music & starts Menu Theme.
    /// </summary>
    private void Start()
    {
        _mainMenuTheme.Source = gameObject.AddComponent<AudioSource>();
        _mainMenuTheme.Source.clip = _mainMenuTheme.Clip;
        _mainMenuTheme.Source.volume = _mainMenuTheme.Volume;
        _mainMenuTheme.Source.pitch = _mainMenuTheme.Pitch;
        _mainMenuTheme.Source.loop = _mainMenuTheme.Loop;

        switch (_mainMenuTheme.AudioType)
        {
            case Sound.AudioTypes.SoundEffect:
                _mainMenuTheme.Source.outputAudioMixerGroup = _soundEffectsMixerGroup;
                break;

            case Sound.AudioTypes.Music:
                _mainMenuTheme.Source.outputAudioMixerGroup = _musicMixerGroup;
                break;
        }

        foreach (Sound s in _daytimeTracks)
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

        foreach (Sound s in _nighttimeTracks)
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

        Invoke("PlayMenuTheme", SongFadeTime);
    }

    public void PlayMenuTheme()
    {
        StopAllCoroutines();

        _mainMenuTheme.Source.Play();
        StartCoroutine(FadeTrack(_mainMenuTheme, null));
        _currentTheme = _mainMenuTheme;

        StartCoroutine(FadeTrack(null, _daytimeTracks[_currentSongIndex]));
        StartCoroutine(FadeTrack(null, _nighttimeTracks[_currentSongIndex]));
        _currentSongIndex = 0;
        IsPlayingDayTrack = false;
    }

    /// <summary>
    /// Swaps tracks from the Main Menu theme to Game music, and Game music between Day & Night variants.
    /// </summary>
    /// <param name="toMenu">True if going back to menu.</param>
    /// <param name="fromMenu">True if leaving the menu.</param>
    public void SwapTrack()
    {
        StopAllCoroutines();

        if (IsPlayingDayTrack)
        {
            StartCoroutine(FadeTrack(_currentTheme, _daytimeTracks[_currentSongIndex]));
        }
        else
        {
            StartCoroutine(FadeTrack(_currentTheme, _nighttimeTracks[_currentSongIndex]));

            _currentSongIndex++;
            if (_currentSongIndex >= _daytimeTracks.Length || _currentSongIndex >= _nighttimeTracks.Length)
            {
                _currentSongIndex = 0;
            }
        }

        IsPlayingDayTrack = !IsPlayingDayTrack;
    }

    /// <summary>
    /// Fades a given track in, and then a given track out.
    /// </summary>
    /// <param name="trackIn">Track to fade in. Can be null.</param>
    /// <param name="trackOut">Track to fade out. Can be null.</param>
    /// <returns></returns>
    private IEnumerator FadeTrack(Sound trackIn, Sound trackOut)
    {
        float fadeTime = SongFadeTime;
        float fadeElapsed = 0;

        if(trackOut != null)
        {
            while (fadeElapsed < fadeTime)
            {
                trackOut.Source.volume = Mathf.Lerp(trackOut.Volume, 0, fadeElapsed / fadeTime);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }

            trackOut.Source.Stop();
        }

        fadeTime = SongFadeTime;
        fadeElapsed = 0;

        if (trackIn != null)
        {
            trackIn.Source.Play();
            _currentTheme = trackIn;

            while (fadeElapsed < fadeTime)
            {
                trackIn.Source.volume = Mathf.Lerp(0, trackIn.Volume, fadeElapsed / fadeTime);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
