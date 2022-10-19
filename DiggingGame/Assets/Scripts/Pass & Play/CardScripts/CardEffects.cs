/*****************************************************************************
// File Name :         Card.cs
// Author :            Rudy Wolfer
// Creation Date :     October 18th, 2022
//
// Brief Description : Script to hold every Card Effect.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffects : MonoBehaviour
{
    [Header("General Values")]
    [SerializeField] private float activateAnimWaitTime;

    [Header("Flowers")]
    [SerializeField] private int grassPiecesToPlace;

    [Header("Placing")]
    [HideInInspector] public int PlacedPieces = 0;

    [Header("Other")]
    private GameCanvasManagerNew _gcm;
    private CardManager _cm;
    private BoardManager _bm;
    private ActionManager _am;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _cm = FindObjectOfType<CardManager>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
    }

    /// <summary>
    /// Pretty cringe method for starting effect coroutines. Keeps them localized here w/ an animation.
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    /// <param name="effectName">Name of the effect, matches coroutine. Capitalize each letter, spaces between words.</param>
    public IEnumerator ActivateCardEffect(string suit, string effectName)
    {
        ShowActivationText(suit, effectName);

        yield return new WaitForSeconds(activateAnimWaitTime);

        //DO NOT OPEN THIS YOU WILL BE JUMPSCARED
        switch(effectName)
        {
            case "Flowers":
                StartCoroutine(Flowers());
                break;
            case "Garden":
                StartCoroutine(Garden());
                break;
            case "Lawnmower":
                StartCoroutine(Lawnmower());
                break;
            case "Morning Jog":
                StartCoroutine(MorningJog());
                break;
            case "Planned Profit":
                StartCoroutine(PlannedProfit());
                break;
            case "Thief":
                StartCoroutine(Thief());
                break;
            case "Walkway":
                StartCoroutine(Walkway());
                break;
            case "Weed Whacker":
                StartCoroutine(WeedWhacker());
                break;
            case "Dam":
                StartCoroutine(Dam());
                break;
            case "Dirty Thief":
                StartCoroutine(DirtyThief());
                break;
            case "Excavator":
                StartCoroutine(Excavator());
                break;
            case "Fertilizer":
                StartCoroutine(Fertilizer());
                break;
            case "Flood":
                StartCoroutine(Flood());
                break;
            case "Mudslide":
                StartCoroutine(Mudslide());
                break;
            case "Secret Tunnels":
                StartCoroutine(SecretTunnels());
                break;
            case "Shovel":
                StartCoroutine(Shovel());
                break;
            case "Thunderstorm":
                StartCoroutine(Thunderstorm());
                break;
            case "Compaction":
                StartCoroutine(Compaction());
                break;
            case "Earthquake":
                StartCoroutine(Earthquake());
                break;
            case "Erosion":
                StartCoroutine(Erosion());
                break;
            case "Geologist":
                StartCoroutine(Geologist());
                break;
            case "Master Builder":
                StartCoroutine(MasterBuilder());
                break;
            case "Metal Detector":
                StartCoroutine(MetalDetector());
                break;
            case "Planned Gamble":
                StartCoroutine(PlannedGamble());
                break;
            case "Discerning Eye":
                StartCoroutine(DiscerningEye());
                break;
            case "Golden Shovel":
                StartCoroutine(GoldenShovel());
                break;
            case "Holy Idol":
                StartCoroutine(HolyIdol());
                break;
            case "Master Thief":
                StartCoroutine(MasterThief());
                break;
            case "Reconstruction":
                StartCoroutine(Reconstruction());
                break;
            case "Regeneration":
                StartCoroutine(Regeneration());
                break;
            case "Retribution":
                StartCoroutine(Retribution());
                break;
            case "Teleportation":
                StartCoroutine(Teleportation());
                break;
            case "Tornado":
                StartCoroutine(Tornado());
                break;
            case "Transmutation":
                StartCoroutine(Transmutation());
                break;
            default:
                Debug.LogWarning("No effect with name " + effectName + ".");
                break;
        }
    }

    /// <summary>
    /// Function that plays an animation when a player activates a card.
    /// </summary>
    /// <param name="effectName"></param>
    private void ShowActivationText(string suit, string effectName)
    {
        Debug.Log("Play " + suit + " animation here.");
        _gcm.UpdateCurrentActionText("Player " + _am.CurrentPlayer + " has activated " + effectName + "!");
    }

    //Grass Cards

    /// <summary>
    /// Card effect Coroutine for Flowers. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Flowers()
    {
        _gcm.DisableListObjects();

        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Two)
            {
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
            }
        }

        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Garden. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Garden()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Lawnmower. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Lawnmower()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Morning Jog. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MorningJog()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Overgrowth. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Overgrowth()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Planned Profit. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator PlannedProfit()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thief()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Walkway. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Walkway()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Weed Whacker. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator WeedWhacker()
    {
        yield return null;
    }

    //Dirt Cards

    /// <summary>
    /// Card effect Coroutine for Dam. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Dam()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Dirty Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DirtyThief()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Excavator. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Excavator()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Fertilizer. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Fertilizer()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Flood. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Flood()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Mudslide. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Mudslide()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Secret Tunnels. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator SecretTunnels()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Shovel. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Shovel()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Thunderstorm. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thunderstorm()
    {
        yield return null;
    }

    //Stone Cards

    /// <summary>
    /// Card effect Coroutine for Compaction. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Compaction()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Earthquake. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Earthquake()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Erosion. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Erosion()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Geologist. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Geologist()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Master Builder. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MasterBuilder()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Metal Detector. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MetalDetector()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Planned Gamble. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator PlannedGamble()
    {
        yield return null;
    }

    //Gold Cards

    /// <summary>
    /// Card effect Coroutine for Discerning Eye. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DiscerningEye()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Golden Shovel. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator GoldenShovel()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Holy Idol. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator HolyIdol()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Master Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MasterThief()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Reconstruction. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Reconstruction()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Regeneration. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Regeneration()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Retribution. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Retribution()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Teleportation. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Teleportation()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Tornado. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Tornado()
    {
        yield return null;
    }

    /// <summary>
    /// Card effect Coroutine for Transmutation. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Transmutation()
    {
        yield return null;
    }
}
