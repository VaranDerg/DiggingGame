/*****************************************************************************
// File Name :         CardManager.cs
// Author :            Rudy Wolfer
// Creation Date :     October 10th, 2022
//
// Brief Description : Script managing card draw piles, dealing, and shuffling.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] _handPositions;
    [SerializeField] private TextMeshProUGUI _uDeckSizeText, _gDeckSizeText, _dPileSizeText;

    [Header("Other")]
    private List<GameObject> _uDeck = new List<GameObject>();
    private List<GameObject> _gDeck = new List<GameObject>();
    [HideInInspector] public List<GameObject> DPile = new List<GameObject>();
    private bool[] _p1OpenHandPositions;
    private bool[] _p2OpenHandPositions;
    private ActionManager _am;
    private BoardManager _bm;
    private GameCanvasManagerNew _gcm;

    /// <summary>
    /// Assigns partner scripts
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
    }

    /// <summary>
    /// Adds cards in the scene to their respective decks.
    /// </summary>
    private void AddCards()
    {
        int uCardAmount = 0;
        int gCardAmount = 0;
        foreach(GameObject uCard in GameObject.FindGameObjectsWithTag("Card"))
        {
            _uDeck.Add(uCard);
            uCard.SetActive(false);
            uCardAmount++;
        }
        foreach(GameObject gCard in GameObject.FindGameObjectsWithTag("GoldCard"))
        {
            _gDeck.Add(gCard);
            gCard.SetActive(false);
            gCardAmount++;
        }
        Debug.Log("Added " + uCardAmount + " Cards to the Universal Deck and " + gCardAmount + " Gold Cards to the Gold Deck.");
    }

    /// <summary>
    /// Prepares each players' hand slots for use during play.
    /// </summary>
    private void PrepareOpenHandSlots()
    {
        _p1OpenHandPositions = new bool[_handPositions.Length];
        _p2OpenHandPositions = new bool[_handPositions.Length];

        for(int i = 0; i < _p1OpenHandPositions.Length; i++)
        {
            _p1OpenHandPositions[i] = true;
        }

        for (int i = 0; i < _p2OpenHandPositions.Length; i++)
        {
            _p2OpenHandPositions[i] = true;
        }

        Debug.Log("Prepared " + _p1OpenHandPositions.Length + " hand positions for Player 1 and " + _p2OpenHandPositions.Length + " hand positions for Player 2.");
    }

    /// <summary>
    /// Runs "AddCards" and "PrepareOpenHandSlots."
    /// </summary>
    private void Start()
    {
        AddCards();
        PrepareOpenHandSlots();
        UpdatePileText();
    }

    /// <summary>
    /// Updates the text for each pile of cards.
    /// </summary>
    private void UpdatePileText()
    {
        _uDeckSizeText.text = "Deck (" + _uDeck.Count + ")";
        _gDeckSizeText.text = "Gold Deck (" + _gDeck.Count + ")";
        _dPileSizeText.text = "Discard (" + DPile.Count + ")";
    }

    /// <summary>
    /// Draws a card.
    /// </summary>
    /// <param name="deck">"Universal" or "Gold"</param>
    public void DrawCard(string deck)
    {
        if(_uDeck.Count == 0)
        {
            ShuffleDiscardPile();
        }

        GameObject randomCard = null;
        if(deck == "Universal")
        {
            randomCard = _uDeck[Random.Range(0, _uDeck.Count)];
        }
        else if(deck == "Gold")
        {
            if(_gDeck.Count == 0)
            {
                Debug.Log("No Gold Cards remain!");
                return;
            }

            randomCard = _gDeck[Random.Range(0, _gDeck.Count)];
        }
        else
        {
            Debug.LogWarning("Incorrect deck parameter provided: " + deck);
        }

        if(_am.CurrentPlayer == 1)
        {
            for(int i = 0; i < _p1OpenHandPositions.Length; i++)
            {
                if(_p1OpenHandPositions[i] == true)
                {
                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = _handPositions[i].position;
                    randomCard.GetComponentInChildren<CardController>().HandPosition = i;
                    randomCard.GetComponentInChildren<CardController>().HeldByPlayer = _am.CurrentPlayer;
                    _p1OpenHandPositions[i] = false;
                    if(deck == "Universal")
                    {
                        _uDeck.Remove(randomCard);
                    }
                    else
                    {
                        _gDeck.Remove(randomCard);
                    }
                    Debug.Log("Drew " + randomCard.name + " to Player " + _am.CurrentPlayer + ".");
                    UpdatePileText();
                    return;
                }
            }
        }
        else if(_am.CurrentPlayer == 2)
        {
            for (int i = 0; i < _p2OpenHandPositions.Length; i++)
            {
                if (_p2OpenHandPositions[i] == true)
                {
                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = _handPositions[i].position;
                    randomCard.GetComponentInChildren<CardController>().HandPosition = i;
                    randomCard.GetComponentInChildren<CardController>().HeldByPlayer = _am.CurrentPlayer;
                    _p2OpenHandPositions[i] = false;
                    if (deck == "Universal")
                    {
                        _uDeck.Remove(randomCard);
                    }
                    else
                    {
                        _gDeck.Remove(randomCard);
                    }
                    Debug.Log("Drew " + randomCard.name + " to Player " + _am.CurrentPlayer + ".");
                    UpdatePileText();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Shuffles the discard pile back into the main deck.
    /// </summary>
    public void ShuffleDiscardPile()
    {
        if(DPile.Count >= 1)
        {
            int shuffledUCards = 0;
            int shuffledGCards = 0;

            foreach(GameObject card in DPile)
            {
                _uDeck.Add(card);
                if(card.CompareTag("Card"))
                {
                    shuffledUCards++;
                }
                else if(card.CompareTag("GoldCard"))
                {
                    shuffledGCards++;
                }
            }

            Debug.Log("Shuffled " + shuffledUCards + " Cards and " + shuffledGCards + " Gold Cards back into the Draw Pile.");

            DPile.Clear();
            UpdatePileText();
        }
    }

    /// <summary>
    /// Shows or hides cards based on the player and boolean. WIP.
    /// </summary>
    /// <param name="player">1 or 2</param>
    /// <param name="hide">true to show, false to hide</param>
    public void HideShowCards(int player, bool show)
    {
        List<GameObject> cardsToCheck = new List<GameObject>();

        foreach(GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            cardsToCheck.Add(card);
        }

        foreach(GameObject card in GameObject.FindGameObjectsWithTag("GoldCard"))
        {
            cardsToCheck.Add(card);
        }

        foreach(GameObject card in cardsToCheck)
        {
            if(card.GetComponentInChildren<CardController>().HeldByPlayer != player)
            {
                continue;
            }

            card.SetActive(show);
        }
    }
}