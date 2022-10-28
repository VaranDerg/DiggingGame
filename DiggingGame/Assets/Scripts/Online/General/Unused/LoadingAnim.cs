/*****************************************************************************
// File Name :         LoadingAnim.cs
// Author :            Andrea SD
// Creation Date :     October 28th, 2022
//
// Brief Description : Animates the loading screen text
*****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAnim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loadingText;
    private int pos;

    private string[] _loadingDots = { ".", "", "" };

    private void Awake()
    {
        pos = 0;
    }
    void Start()
    {
        StartCoroutine(TextAnim());   
    }

    public IEnumerator TextAnim()
    {
        while (pos >= 0)
        {
            if (pos == 3)
            {
                _loadingDots[0] = "";
                _loadingDots[1] = "";
                _loadingDots[2] = "";
                pos = 0;
            }
            else if(pos == 0)
            {
                _loadingDots[0] = ".";
                pos++;

            }
            else if (pos == 1)
            {
                _loadingDots[1] = ".";
                pos++;
            }
            else if (pos==2)
            {
                _loadingDots[2] = ".";
                pos++;
            }

            for(int i = 0; i < _loadingDots.Length; i++)
            {
                _loadingText.text = "Loading" + _loadingDots[i];
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
