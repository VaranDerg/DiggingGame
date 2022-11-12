/*****************************************************************************
// File Name :         ResultsDisplay.cs
// Author :            Rudy Wolfer
// Creation Date :     November 6th, 2022
//
// Brief Description : A script that displays values from StatManager at the
                       end of a game.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;

public class ResultsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private TextMeshProUGUI _p1Text;
    [SerializeField] private TextMeshProUGUI _p2Text;

    /// <summary>
    /// Runs AssignValuesToText
    /// </summary>
    private void Start()
    {
        AssignValuesToText();
    }

    /// <summary>
    /// Sets values from StatManager to Text objects.
    /// </summary>
    private void AssignValuesToText()
    {
        _p1Text.text =
            "Buildings Built: " + StatManager.s_P1BuildingTotal + Environment.NewLine +
            "Buildings Destroyed: " + StatManager.s_P1DestroyTotal + Environment.NewLine +
            "Cards Activated: " + StatManager.s_P1ActivationTotal + Environment.NewLine +
            "Cards Spent: " + StatManager.s_P1CardSpendTotal + Environment.NewLine +
            "Pieces Stolen: " + StatManager.s_P1StealTotal + Environment.NewLine +
            "Pieces Placed: " + StatManager.s_P1PlaceTotal + Environment.NewLine +
            "Pieces Dug: " + StatManager.s_P1DigTotal + Environment.NewLine +
            "Gold Retrieved: " + StatManager.s_P1RetrieveTotal + Environment.NewLine +
            Environment.NewLine + "Final Score: " + StatManager.s_P1FinalScore;

        _p2Text.text =
            "Buildings Built: " + StatManager.s_P2BuildingTotal + Environment.NewLine +
            "Buildings Destroyed: " + StatManager.s_P2DestroyTotal + Environment.NewLine +
            "Cards Activated: " + StatManager.s_P2ActivationTotal + Environment.NewLine +
            "Cards Spent: " + StatManager.s_P2CardSpendTotal + Environment.NewLine +
            "Pieces Stolen: " + StatManager.s_P2StealTotal + Environment.NewLine +
            "Pieces Placed: " + StatManager.s_P2PlaceTotal + Environment.NewLine +
            "Pieces Dug: " + StatManager.s_P2DigTotal + Environment.NewLine +
            "Gold Retrieved: " + StatManager.s_P2RetrieveTotal + Environment.NewLine +
            Environment.NewLine + "Final Score: " + StatManager.s_P2FinalScore;

        if(StatManager.s_P1FinalScore >= 18)
        {
            _winText.text = "Moles Win!";
        }
        else
        {
            _winText.text = "Meerkats Win!";
        }
    }

    /// <summary>
    /// Returns to menu.
    /// </summary>
    public void BackToMenu()
    {
        StatManager.s_Instance.ResetStatistics();
        FindObjectOfType<SceneLoader>().LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }
}
