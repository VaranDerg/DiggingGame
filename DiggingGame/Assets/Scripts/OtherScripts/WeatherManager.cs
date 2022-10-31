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

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private List<WeatherState> _weatherStates = new List<WeatherState>();

    [Header("Other")]
    public Weather ActiveWeather;

    /// <summary>
    /// Weather enum from 1-10 determining the weather's state.
    /// </summary>
    public enum Weather
    {
        Day,
        Night,
        Rain,
        Thunderstorm,
        Flowers,
        Pollen,
        Dirty,
        Tornado,
        Smoke,
        Holy
    }

    /// <summary>
    /// Sets the active weather state.
    /// </summary>
    /// <param name="weatherIndex">1-10 based on the Weather enum in WeatherManager</param>
    public void SetActiveWeather(int weatherIndex)
    {
        switch(weatherIndex)
        {
            case 1:
                ActiveWeather = Weather.Day;
                break;
            case 2:
                ActiveWeather = Weather.Night;
                break;
            case 3:
                ActiveWeather = Weather.Rain;
                break;
            case 4:
                ActiveWeather = Weather.Thunderstorm;
                break;
            case 5:
                ActiveWeather = Weather.Flowers;
                break;
            case 6:
                ActiveWeather = Weather.Pollen;
                break;
            case 7:
                ActiveWeather = Weather.Dirty;
                break;
            case 8:
                ActiveWeather = Weather.Tornado;
                break;
            case 9:
                ActiveWeather = Weather.Smoke;
                break;
            case 10:
                ActiveWeather = Weather.Holy;
                break;
        }
    }

    /// <summary>
    /// Updates the game's lighting.
    /// </summary>
    /// <param name="pointLightColor">Color of the sun/moon</param>
    /// <param name="globalLightColor">Color of the world</param>
    public void UpdateLighting(Color pointLightColor, Color globalLightColor, int pointLightIntensity, int globalLightIntensity)
    {

    }
}
