/*****************************************************************************
// File Name :         WeatherManager.cs
// Author :            Rudy Wolfer
// Creation Date :     October 30th, 2022
//
// Brief Description : A script to manage the game's day/night cycle and 
                       its weather mechanic through card effects.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private List<WeatherState> _weatherStates = new List<WeatherState>();

    [Header("References")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Light2D _pointLight;
    [SerializeField] private Light2D _bonusLight;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private SpriteRenderer _backgroundCover;

    [Header("Values")]
    [SerializeField] private float _weatherChangeSpeed;
    private int _activeWeatherIndex;
    private bool _psSwitched = true;
    [HideInInspector] public WeatherState.Weather CurrentWeatherEnum;
    private Sound _currentAmbiance;

    [Header("Audio")]
    [SerializeField] private AudioMixerGroup _soundEffectsMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    private GameObject _currentPS;

    /// <summary>
    /// Sets the weather to Day & prepares Ambiance.
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < _weatherStates.Count; i++)
        {
            if(_weatherStates[i].WeatherAmbiance == null)
            {
                continue;
            }

            _weatherStates[i].WeatherAmbiance.Source = gameObject.AddComponent<AudioSource>();
            _weatherStates[i].WeatherAmbiance.Source.clip = _weatherStates[i].WeatherAmbiance.Clip;
            _weatherStates[i].WeatherAmbiance.Source.volume = _weatherStates[i].WeatherAmbiance.Volume;
            _weatherStates[i].WeatherAmbiance.Source.pitch = _weatherStates[i].WeatherAmbiance.Pitch;
            _weatherStates[i].WeatherAmbiance.Source.loop = _weatherStates[i].WeatherAmbiance.Loop;

            switch (_weatherStates[i].WeatherAmbiance.AudioType)
            {
                case Sound.AudioTypes.SoundEffect:
                    _weatherStates[i].WeatherAmbiance.Source.outputAudioMixerGroup = _soundEffectsMixerGroup;
                    break;

                case Sound.AudioTypes.Music:
                    _weatherStates[i].WeatherAmbiance.Source.outputAudioMixerGroup = _musicMixerGroup;
                    break;
            }
        }    

        SetActiveWeather(WeatherState.Weather.Day);
    }

    /// <summary>
    /// Sets the active weather state.
    /// </summary>
    /// <param name="weatherIndex">0-10 based on the Weather enum in WeatherManager</param>
    public void SetActiveWeather(WeatherState.Weather ws)
    {
        _psSwitched = false;

        switch (ws)
        {
            case WeatherState.Weather.Day:
                _activeWeatherIndex = 0;
                CurrentWeatherEnum = WeatherState.Weather.Day;
                break;
            case WeatherState.Weather.Night:
                _activeWeatherIndex = 1;
                CurrentWeatherEnum = WeatherState.Weather.Night;
                break;
            case WeatherState.Weather.Rain:
                _activeWeatherIndex = 2;
                CurrentWeatherEnum = WeatherState.Weather.Rain;
                break;
            case WeatherState.Weather.Thunderstorm:
                _activeWeatherIndex = 3;
                CurrentWeatherEnum = WeatherState.Weather.Thunderstorm;
                break;
            case WeatherState.Weather.Flowers:
                _activeWeatherIndex = 4;
                CurrentWeatherEnum = WeatherState.Weather.Flowers;
                break;
            case WeatherState.Weather.Pollen:
                _activeWeatherIndex = 5;
                CurrentWeatherEnum = WeatherState.Weather.Pollen;
                break;
            case WeatherState.Weather.Dirty:
                _activeWeatherIndex = 6;
                CurrentWeatherEnum = WeatherState.Weather.Dirty;
                break;
            case WeatherState.Weather.Tornado:
                _activeWeatherIndex = 7;
                CurrentWeatherEnum = WeatherState.Weather.Tornado;
                break;
            case WeatherState.Weather.Smoke:
                _activeWeatherIndex = 8;
                CurrentWeatherEnum = WeatherState.Weather.Smoke;
                break;
            case WeatherState.Weather.Holy:
                _activeWeatherIndex = 9;
                CurrentWeatherEnum = WeatherState.Weather.Holy;
                break;
            case WeatherState.Weather.Clear:
                _activeWeatherIndex = 10;
                CurrentWeatherEnum = WeatherState.Weather.Clear;
                break;
        }

        //Debug.Log("Setting active weather to " + _weatherStates[_activeWeatherIndex].Name + "; " + _weatherStates[_activeWeatherIndex].Description + ".");
    }

    /// <summary>
    /// Lerps colors and values to their current weather state based on _activeWeatherIndex.
    /// </summary>
    private void FixedUpdate()
    {
        if(!_weatherStates[_activeWeatherIndex].IsBonusLightState)
        {
            _globalLight.color = Color.Lerp(_globalLight.color, _weatherStates[_activeWeatherIndex].GlobalLightColor, _weatherChangeSpeed * Time.deltaTime);
            _globalLight.intensity = Mathf.Lerp(_globalLight.intensity, _weatherStates[_activeWeatherIndex].GlobalLightIntensity, _weatherChangeSpeed * Time.deltaTime);
            _pointLight.color = Color.Lerp(_pointLight.color, _weatherStates[_activeWeatherIndex].PointLightColor, _weatherChangeSpeed * Time.deltaTime);
            _pointLight.intensity = Mathf.Lerp(_pointLight.intensity, _weatherStates[_activeWeatherIndex].PointLightIntensity, _weatherChangeSpeed * Time.deltaTime);
            _backgroundCover.color = Color.Lerp(_backgroundCover.color, _weatherStates[_activeWeatherIndex].BGCoverColor, _weatherChangeSpeed * Time.deltaTime);
        }
        else
        {
            _bonusLight.color = Color.Lerp(_bonusLight.color, _weatherStates[_activeWeatherIndex].BonusLightColor, _weatherChangeSpeed * Time.deltaTime);
            _bonusLight.intensity = Mathf.Lerp(_bonusLight.intensity, _weatherStates[_activeWeatherIndex].BonusLightIntensity, _weatherChangeSpeed * Time.deltaTime);
            _background.color = Color.Lerp(_background.color, _weatherStates[_activeWeatherIndex].BackgroundColor, _weatherChangeSpeed * Time.deltaTime);

            if(!_psSwitched)
            {
                if (_currentAmbiance != null)
                {
                    _currentAmbiance.Source.Stop();
                    _currentAmbiance = null;
                }
            }
        }

        if(!_psSwitched)
        {
            if (_weatherStates[_activeWeatherIndex].WeatherAmbiance.Source.clip != null)
            {
                _currentAmbiance = _weatherStates[_activeWeatherIndex].WeatherAmbiance;
                _currentAmbiance.Source.Play();
            }

            if (_currentPS != null && _weatherStates[_activeWeatherIndex].ActiveParticles != null)
            {
                _currentPS.GetComponent<ParticleSystem>().Stop();
                Destroy(_currentPS.gameObject, 5f);
            }

            if (_weatherStates[_activeWeatherIndex].ActiveParticles != null)
            {
                _currentPS = Instantiate(_weatherStates[_activeWeatherIndex].ActiveParticles);
            }

            if (_currentPS != null)
            {
                _currentPS.GetComponent<ParticleSystem>().Play();
            }

            _psSwitched = true;
        }
    }
}
