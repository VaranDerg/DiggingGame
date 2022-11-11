/*****************************************************************************
// File Name :         WeatherState.cs
// Author :            Rudy Wolfer
// Creation Date :     October 30th, 2022
//
// Brief Description : A Serializable script for Weather states.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeatherState
{
    public Weather WeatherEnum;

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
        Holy,
        Clear
    }

    [Header("Weather State Information")]
    public string Name;
    public string Description;
    public Sound WeatherAmbiance;
    public Color BackgroundColor;

    [Header("Point Light")]
    public Color PointLightColor;
    public float PointLightIntensity;

    [Header("Global Light")]
    public Color GlobalLightColor;
    public float GlobalLightIntensity;

    [Header("Bonus Light")]
    public bool IsBonusLightState;
    public Color BonusLightColor;
    public float BonusLightIntensity;

    [Header("Particle Systems")]
    public GameObject ActiveParticles;
}
