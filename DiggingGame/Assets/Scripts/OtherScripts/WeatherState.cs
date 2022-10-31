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
    [Header("Weather State Information")]
    public string Name;
    public string Description;

    [Header("Point Light")]
    public Color PointLightColor;
    public float PointLightIntensity;

    [Header("Global Light")]
    public Color GlobalLightColor;
    public float GlobalLightIntensity;

    [Header("Particle Systems")]
    public ParticleSystem ActiveParticles;
}
