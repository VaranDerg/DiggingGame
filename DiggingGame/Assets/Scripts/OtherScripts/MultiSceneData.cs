/*****************************************************************************
// File Name :         MultiSceneData.cs
// Author :            Rudy Wolfer
// Creation Date :     November 12th, 2022
//
// Brief Description : A script to pull information from scenes and use them
                       in different scenes.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSceneData : MonoBehaviour
{
    /// <summary>
    /// Prepares the Nondestructable.
    /// </summary>
    public static MultiSceneData s_Instance;
    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static int s_WeatherOption;
}
