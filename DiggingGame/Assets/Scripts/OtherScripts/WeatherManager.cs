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

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private List<WeatherState> _weatherStates = new List<WeatherState>();

    [Header("References")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Light2D _pointLight;
    [SerializeField] private Light2D _bonusLight;
    [SerializeField] private ParticleSystem _ps;
    [SerializeField] private SpriteRenderer _background;

    [Header("Values")]
    [SerializeField] private float _weatherChangeSpeed;
    private int _activeWeatherIndex;
    private bool _psSwitched = true;
    [HideInInspector] public WeatherState.Weather CurrentWeatherEnum;

    /// <summary>
    /// Sets the weather to Day.
    /// </summary>
    private void Start()
    {
        SetActiveWeather(WeatherState.Weather.Day);
    }

    /// <summary>
    /// Sets the active weather state.
    /// </summary>
    /// <param name="weatherIndex">0-10 based on the Weather enum in WeatherManager</param>
    public void SetActiveWeather(WeatherState.Weather ws)
    {
        switch(ws)
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
        _psSwitched = false;
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
        }
        else
        {
            _bonusLight.color = Color.Lerp(_bonusLight.color, _weatherStates[_activeWeatherIndex].BonusLightColor, _weatherChangeSpeed * Time.deltaTime);
            _bonusLight.intensity = Mathf.Lerp(_bonusLight.intensity, _weatherStates[_activeWeatherIndex].BonusLightIntensity, _weatherChangeSpeed * Time.deltaTime);
        }

        _background.color = Color.Lerp(_background.color, _weatherStates[_activeWeatherIndex].BackgroundColor, _weatherChangeSpeed * Time.deltaTime);

        if(!_psSwitched)
        {
            if (_ps != null)
            {
                _ps.Stop();
            }

            if (_weatherStates[_activeWeatherIndex].ActiveParticles != null)
            {
                _ps = _weatherStates[_activeWeatherIndex].ActiveParticles;
            }

            if (_ps != null)
            {
                _ps.Play();
            }

            _psSwitched = true;
        }
    }
}
