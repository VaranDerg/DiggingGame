/*****************************************************************************
// File Name :         ProtoCardManager.cs
// Author :            Rudy Wolfer
// Creation Date :     September 21st, 2022
//
// Brief Description : Old script for basic card functionality. 
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProtoCardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _defaultCard;
    [SerializeField] private Transform _cardParent;
    [SerializeField] private TextMeshProUGUI _drawPileSizeText;
    [SerializeField] private TextMeshProUGUI _discardPileSizeText;
    [SerializeField] private Transform[] _handPositions;
    [SerializeField] public Transform PlayZoneTL, PlayZoneBR;

    [Header("Values")]
    [SerializeField] private int _cardsToSpawn;

    [Header("Other")]
    private List<GameObject> _deck = new List<GameObject>();
    [HideInInspector] public List<GameObject> Discard = new List<GameObject>();
    [HideInInspector] public bool[] OpenHandPositions;
    [SerializeField] private bool _generateEmptyDeck;

    /// <summary>
    /// Spawns cards
    /// </summary>
    private void InstantiateCards()
    {
        for(int i = 0; i < _cardsToSpawn; i++)
        {
            GameObject newCard = Instantiate(_defaultCard, _cardParent);
            newCard.name = "Card " + i.ToString();
            _deck.Add(newCard);
            newCard.SetActive(false);
        }

        Debug.Log("Spawned 30 test cards.");
    }

    /// <summary>
    /// Adds cards in scene
    /// </summary>
    private void AddCards()
    {
        int cardAmount = 0;
        foreach(GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            _deck.Add(card);
            card.SetActive(false);
            cardAmount++;
        }
        Debug.Log("Added " + cardAmount + " cards.");
    }

    /// <summary>
    /// Preps the card slots
    /// </summary>
    private void PrepareOpenHandSlots()
    {
        OpenHandPositions = new bool[_handPositions.Length];

        for(int i = 0; i < OpenHandPositions.Length; i++)
        {
            OpenHandPositions[i] = true;
        }

        Debug.Log("There are " + OpenHandPositions.Length + " open card positions.");
    }

    /// <summary>
    /// Runs the above functions
    /// </summary>
    private void Start()
    {
        if(_generateEmptyDeck)
        {
            InstantiateCards();
        }
        else
        {
            AddCards();
        }

        PrepareOpenHandSlots();
    }

    /// <summary>
    /// Updates the Pile's text
    /// </summary>
    private void Update()
    {
        _drawPileSizeText.text = _deck.Count.ToString();
        _discardPileSizeText.text = Discard.Count.ToString();
    }

    /// <summary>
    /// Draws a Card
    /// </summary>
    public void DrawCard()
    {
        //If the deck has cards in it...
        if(_deck.Count >= 1)
        {
            //...draw a random card
            GameObject randomCard = _deck[Random.Range(0, _deck.Count)];
            //Put it in your hand...
            for(int i = 0; i < OpenHandPositions.Length; i++)
            {
                //...if a slot is available. 
                if(OpenHandPositions[i] == true)
                {
                    //Enable the card
                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = _handPositions[i].position;
                    randomCard.transform.rotation = _handPositions[i].rotation;
                    //The card knows what position its in. 
                    randomCard.GetComponentInChildren<ProtoCardController>()._handPosition = i;
                    //The card is no longer played.
                    randomCard.GetComponentInChildren<ProtoCardController>().Played = false;
                    //The position is closed and the card has left the deck
                    OpenHandPositions[i] = false;
                    _deck.Remove(randomCard);
                    Debug.Log("Drew " + randomCard.name + ".");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Shuffles the discard pile
    /// </summary>
    public void ShuffleDiscard()
    {
        if(Discard.Count >= 1)
        {
            int shuffledCards = 0;

            foreach(GameObject card in Discard)
            {
                _deck.Add(card);
                shuffledCards++;
            }

            Debug.Log("Shuffled " + shuffledCards + " back into the draw pile.");

            Discard.Clear();
        }
    }
}
