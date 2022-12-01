/*****************************************************************************
// File Name :         OnlineCardManager.cs
// Author :            Rudy Wolfer, Andrea SD
// Creation Date :     October 10th, 2022
//
// Brief Description : Script managing card draw piles, dealing, and shuffling.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;

public class OnlineCardManager : MonoBehaviourPun
{
    //Edit: Andrea SD - Added online functionality

    [Header("References")]
    [SerializeField] private Transform[] _handPositions;
    [SerializeField] private TextMeshProUGUI _uDeckSizeText, _gDeckSizeText, _dPileSizeText;

    [Header("Other")]
    private List<GameObject> _uDeck = new List<GameObject>();
    private List<GameObject> _gDeck = new List<GameObject>();
    [HideInInspector] public List<GameObject> DPile = new List<GameObject>();
    [HideInInspector] public List<GameObject> P1Hand = new List<GameObject>();
    [HideInInspector] public List<GameObject> P2Hand = new List<GameObject>();
    [HideInInspector] public List<GameObject> SelectedCards = new List<GameObject>();
    [HideInInspector] public bool[] P1OpenHandPositions;
    [HideInInspector] public bool[] P2OpenHandPositions;
    [HideInInspector] public int AllowedActivations;
    private OnlineActionManager _am;
    private OnlineCanvasManager _gcm;
    private OnlineAudioPlayer _ap;

    [Header("Selection Requiements")]
    [HideInInspector] public string RequiredSuit = "";
    [HideInInspector] public int RequiredCardAmount = 0;

    [Header("Animations")]
    [SerializeField] private float _cardShowHideTime;

    /// <summary>
    /// Assigns partner scripts
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<OnlineActionManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _ap = FindObjectOfType<OnlineAudioPlayer>();
    }

    /// <summary>
    /// Runs "AddCards" and "PrepareOpenHandSlots."
    /// </summary>
    private void Start()
    {
        // Called by master client only
        //if (PhotonNetwork.IsMasterClient)    // Andrea SD
        // {
        // CallAddCards();
        // }
        PrepareOpenHandSlots();
        CallPileText();
    }

    /// <summary>
    /// Prepares each players' hand slots for use during play.
    /// </summary>
    private void PrepareOpenHandSlots()
    {
        P1OpenHandPositions = new bool[_handPositions.Length];
        P2OpenHandPositions = new bool[_handPositions.Length];

        for(int i = 0; i < P1OpenHandPositions.Length; i++)
        {
            P1OpenHandPositions[i] = true;
        }

        for (int i = 0; i < P2OpenHandPositions.Length; i++)
        {
            P2OpenHandPositions[i] = true;
        }
    }

    /// <summary>
    /// Draws a card.
    /// 
    /// Edited: Andrea SD - modified for online use
    /// </summary>
    /// <param name="deck">"Universal" or "Gold"</param>
    public IEnumerator DrawCard(string deck)
    {
        if (_uDeck.Count == 0)
        {
            CallShuffleDiscard();
        }

        GameObject randomCard = null;
        if (deck == "Universal")
        {
            randomCard = _uDeck[Random.Range(0, _uDeck.Count)];
        }
        else if (deck == "Gold")
        {
            if (_gDeck.Count == 0)
            {
                _gcm.UpdateCurrentActionText("Gold deck is empty! Scored 1 Point!");
                _am.CallUpdateScore(_am.CurrentPlayer, 1);
                yield break;
            }

            randomCard = _gDeck[Random.Range(0, _gDeck.Count)];
        }

        int randomCardID = randomCard.GetComponentInChildren<OnlineCardController>().GetCardID();

        if (_am.CurrentPlayer == 1 && PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < P1OpenHandPositions.Length; i++)
            {
                if (P1OpenHandPositions[i] == true)
                {

                    _ap.PlaySound("DrawCard", false);

                    randomCard.SetActive(true);
                    randomCard.transform.position = _handPositions[i].position;
                    randomCard.GetComponentInChildren<OnlineCardController>().HandPosition = i;
                    randomCard.GetComponentInChildren<OnlineCardController>().HeldByPlayer = 1;
                    randomCard.GetComponentInChildren<OnlineCardController>().NextPos = randomCard.transform.position;
                    CallAddCardToHand(1, randomCardID);
                    P1OpenHandPositions[i] = false;
                    if (deck == "Universal")
                    {
                        CallRemoveCard("Universal", randomCardID);
                    }
                    else
                    {
                        CallRemoveCard("Gold", randomCardID);
                    }

                    if (randomCard.CompareTag("GoldCard"))
                    {
                        CallGoldCards(1, 1);
                    }
                    else
                    {
                        CallNormalCards(1, 1);
                    }
                    randomCard.SetActive(true);
                    randomCard.GetComponentInChildren<Animator>().Play("CardDraw");
                    yield return new WaitForSeconds(_cardShowHideTime);
                    CallPileText();
                    yield break;
                }
            }
        }
        else
        {
            for (int i = 0; i < P2OpenHandPositions.Length; i++)
            {
                if (P2OpenHandPositions[i] == true)
                {
                    _ap.PlaySound("DrawCard", false);

                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = _handPositions[i].position;
                    randomCard.GetComponentInChildren<OnlineCardController>().HandPosition = i;
                    randomCard.GetComponentInChildren<OnlineCardController>().HeldByPlayer = 2;
                    randomCard.GetComponentInChildren<OnlineCardController>().NextPos = randomCard.transform.position;
                    CallAddCardToHand(2, randomCardID);
                    P2OpenHandPositions[i] = false;
                    if (deck == "Universal")
                    {
                        CallRemoveCard("Universal", randomCardID);  // ASD
                    }
                    else
                    {
                        CallRemoveCard("Gold", randomCardID);   // ASD
                    }

                    if (randomCard.CompareTag("GoldCard"))
                    {
                        CallGoldCards(2, 1);
                    }
                    else
                    {
                        CallNormalCards(2, 1);
                    }
                    randomCard.SetActive(true);
                    randomCard.GetComponentInChildren<Animator>().Play("CardDraw");
                    yield return new WaitForSeconds(_cardShowHideTime);
                    CallPileText();
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// Prepares selection value for checks related to spending cards.
    /// </summary>
    /// <param name="cardAmount">The amount of cards needed for an action.</param>
    /// <param name="suit">"Grass" "Dirt" "Stone" "Any"</param>
    /// <param name="remove">Mark true to set all variables to their defaults.</param>
    public void PrepareCardSelection(int cardAmount, string suit, bool remove)
    {
        if (remove)
        {
            RequiredCardAmount = 0;
            RequiredSuit = "";

            if (_am.CurrentPlayer == 1)
            {
                foreach (GameObject card in P1Hand)
                {
                    card.GetComponentInChildren<OnlineCardController>().CanBeSelected = false;
                }
            }
            else
            {
                foreach (GameObject card in P2Hand)
                {
                    card.GetComponentInChildren<OnlineCardController>().CanBeSelected = false;
                }
            }

            return;
        }

        if (_am.CurrentPlayer == 1)
        {
            foreach (GameObject card in P1Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeSelected = true;
            }
        }
        else
        {
            foreach (GameObject card in P2Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeSelected = true;
            }
        }

        RequiredSuit = suit;
        RequiredCardAmount = cardAmount;
        if (RequiredSuit == "Gold")
        {
            _gcm.UpdateCurrentActionText("Select " + RequiredCardAmount + " Stone suited Cards!");
        }
        else
        {
            _gcm.UpdateCurrentActionText("Select " + RequiredCardAmount + " " + RequiredSuit + " suited Cards!");
        }
    }

    /// <summary>
    /// Checks the "Value" of selected cards. Once the correct amount of cards are selected, it will return true.
    /// </summary>
    public bool CheckCardSelection()
    {
        int requiredCardValue = RequiredCardAmount * 2;
        int selectedCardValue = 0;

        foreach (GameObject card in SelectedCards)
        {
            if (card.GetComponentInChildren<OnlineCardController>().Selected)
            {
                if (card.GetComponentInChildren<CardVisuals>().ThisCard.GrassSuit && RequiredSuit == "Grass")
                {
                    selectedCardValue += 2;
                }
                else if (card.GetComponentInChildren<CardVisuals>().ThisCard.DirtSuit && RequiredSuit == "Dirt")
                {
                    selectedCardValue += 2;
                }
                else if (card.GetComponentInChildren<CardVisuals>().ThisCard.StoneSuit && RequiredSuit == "Stone")
                {
                    selectedCardValue += 2;
                }
                else if (card.GetComponentInChildren<CardVisuals>().ThisCard.GoldSuit)
                {
                    selectedCardValue += 2;
                }
                else if (RequiredSuit == "Any")
                {
                    selectedCardValue += 2;
                }
                else
                {
                    selectedCardValue++;
                }
            }
        }

        if (selectedCardValue == requiredCardValue)
        {
            SpendSelectedCards();
            return true;
        }
        else if (selectedCardValue > requiredCardValue)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Spends every card in SelectedCards.
    /// </summary>
    private void SpendSelectedCards()
    {
        foreach(GameObject card in SelectedCards)
        {
            StartCoroutine(card.GetComponentInChildren<OnlineCardController>().ToDiscard());
        }

        SelectedCards.Clear();
    }

    /// <summary>
    /// Deselects every selected card.
    /// </summary>
    public void DeselectSelectedCards()
    {
        foreach(GameObject card in SelectedCards)
        {
            card.GetComponentInChildren<OnlineCardController>().Selected = false;
            card.GetComponentInChildren<OnlineCardController>().CanBeSelected = false;
        }

        SelectedCards.Clear();
    }  

    /// <summary>
    /// Draws cards for a selected player.
    /// </summary>
    /// <param name="cardsToDraw">How many cards you should draw.</param>
    public void DrawAlottedCards(int cardsToDraw)
    {
        for (int i = cardsToDraw; i > 0; i--)
        {
            StartCoroutine(DrawCard("Universal"));
        }
    }

    /// <summary>
    /// Discard cards down to the hand limit at the end of your turn.
    /// </summary>
    public IEnumerator CardDiscardProcess(int player)
    {
        if (player == 1)
        {
            if (_am.P1Cards + _am.P1GoldCards > _am.HandLimit)
            {
                PrepareCardSelection(_am.P1Cards + _am.P1GoldCards - _am.HandLimit, "Any", false);
                _gcm.UpdateCurrentActionText("Discard " + (_am.P1Cards + _am.P1GoldCards - _am.HandLimit) + " Cards.");
            }
        }
        else if (player == 2)
        {
            if (_am.P2Cards + _am.P2GoldCards > _am.HandLimit)
            {
                PrepareCardSelection(_am.P2Cards + _am.P2GoldCards - _am.HandLimit, "Any", false);
                _gcm.UpdateCurrentActionText("Discard " + (_am.P2Cards + _am.P2GoldCards - _am.HandLimit) + " Cards.");
            }
        }

        while (!CheckCardSelection())
        {
            yield return null;
        }
        PrepareCardSelection(0, "", true);

        _am.EndTurn(_am.CurrentPlayer);
    }

    /// <summary>
    /// Prepares cards for activation.
    /// </summary>
    /// <param name="player">1 or 2</param>
    /// <param name="maxActivateAmount">Default amount + Burrows</param>
    public void PrepareCardActivating(int player, int maxActivateAmount, bool activatePhaseJustStarted)
    {
        if (activatePhaseJustStarted)
        {
            AllowedActivations = maxActivateAmount;
        }

        if (player == 1)
        {
            foreach (GameObject card in P1Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeActivated = true;
            }
        }
        else
        {
            foreach (GameObject card in P2Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeActivated = true;
            }
        }
    }

    /// <summary>
    /// Stops cards for activation.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void StopCardActivating(int player)
    {
        if (player == 1)
        {
            foreach (GameObject card in P1Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeActivated = false;
            }
        }
        else
        {
            foreach (GameObject card in P2Hand)
            {
                card.GetComponentInChildren<OnlineCardController>().CanBeActivated = false;
            }
        }
    }

    /// <summary>
    /// Shows cards based on the player.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public IEnumerator HideCards(int player)
    {
        if (player == 1)
        {
            for (int i = 0; i < P1Hand.Count; i++)
            {
                P1Hand[i].GetComponentInChildren<Animator>().Play("CardHide");
                _ap.PlaySound("DrawCard", false);
                yield return new WaitForSeconds(_cardShowHideTime);
                P1Hand[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < P2Hand.Count; i++)
            {
                P2Hand[i].GetComponentInChildren<Animator>().Play("CardHide");
                _ap.PlaySound("DrawCard", false);
                yield return new WaitForSeconds(_cardShowHideTime);
                P2Hand[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Hides cards based on the player.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public IEnumerator ShowCards(int player)
    {
        if (player == 1)
        {
            for (int i = 0; i < P1Hand.Count; i++)
            {
                P1Hand[i].SetActive(true);
                P1Hand[i].GetComponentInChildren<Animator>().Play("CardDraw");
                _ap.PlaySound("DrawCard", false);
                yield return new WaitForSeconds(_cardShowHideTime);
            }
        }
        else
        {
            for (int i = 0; i < P2Hand.Count; i++)
            {
                P2Hand[i].SetActive(true);
                P2Hand[i].GetComponentInChildren<Animator>().Play("CardDraw");
                _ap.PlaySound("DrawCard", false);
                yield return new WaitForSeconds(_cardShowHideTime);
            }
        }
    }

    #region RPC Functions

    /// <summary>
    /// Calls the ShuffleDiscardPile RPC which shuffles the discard pile back
    /// into the main deck.
    /// 
    /// Author: Andrea SD
    /// </summary>
    private void CallShuffleDiscard()
    {
        photonView.RPC("ShuffleDiscardPile", RpcTarget.All);
    }

    /// <summary>
    /// Shuffles the discard pile back into the main deck.
    /// 
    /// Edited: Andrea SD - Turned into an RPC for online use
    /// </summary>
    [PunRPC]
    public void ShuffleDiscardPile()
    {
        if (DPile.Count >= 1)
        {
            int shuffledUCards = 0;
            int shuffledGCards = 0;

            foreach (GameObject card in DPile)
            {
                _uDeck.Add(card);
                if (card.CompareTag("Card"))
                {
                    shuffledUCards++;
                }
                else if (card.CompareTag("GoldCard"))
                {
                    shuffledGCards++;
                }
            }

            DPile.Clear();
            CallPileText();
        }
    }

    /// <summary>
    /// Calls the RemoveCard RPC which removes the card at deckPos from deck
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="deck"> Either removed from "Universal" or "Gold" </param>
    /// <param name="cardID"> ID of the card being removed </param>
    private void CallRemoveCard(string deck, int cardID)
    {
        photonView.RPC("RemoveCard", RpcTarget.All, deck, cardID);
    }

    /// <summary>
    /// Removes the card at deckPos from deck
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="deck"> Either removed from "Universal" or "Gold" </param>
    /// <param name="deckPos"> Position of the card in the deck </param>
    [PunRPC]
    public void RemoveCard(string deck, int cardID)
    {
        GameObject card = PhotonView.Find(cardID).transform.parent.gameObject;
        switch (deck)
        {
            case "Universal":
                _uDeck.Remove(card);
                break;
            default:
                _gDeck.Remove(card);
                break;
        }
    }

    /// <summary>
    /// Calls the RPC AddCards() which adds cards in the scene to their 
    /// respective decks.
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void CallAddCards()
    {
        photonView.RPC("AddCards", RpcTarget.AllBuffered);
    }

    /// <summary>
    /// Adds cards in the scene to their respective decks.
    /// 
    /// Edited: Andrea SD - Turned into RPC
    /// </summary>
    [PunRPC]
    private void AddCards()
    {
        int uCardAmount = 0;
        int gCardAmount = 0;
        foreach (GameObject uCard in GameObject.FindGameObjectsWithTag("Card"))
        {
            _uDeck.Add(uCard);
            uCard.SetActive(false);
            uCardAmount++;
            uCard.GetComponent<OnlineCardController>().EnableReadyToMove();
        }
        foreach (GameObject gCard in GameObject.FindGameObjectsWithTag("GoldCard"))
        {
            _gDeck.Add(gCard);
            gCard.SetActive(false);
            gCardAmount++;
            gCard.GetComponent<OnlineCardController>().EnableReadyToMove();
        }
    }

    /// <summary>
    /// Adds a card to player's hand
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> player 1 or 2 </param>
    /// <param name="cardID"> ID of the card game object </param>
    public void CallAddCardToHand(int player, int cardID)
    {
        photonView.RPC("AddCardToHand", RpcTarget.All, player, cardID);
    }

    /// <summary>
    /// Adds a card to player's hand
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> player 1 or 2 </param>
    /// <param name="cardID"> ID of the card game object </param>
    [PunRPC]
    public void AddCardToHand(int player, int cardID)
    {
        GameObject card = PhotonView.Find(cardID).gameObject;
        switch (player)
        {
            case 1:
                P1Hand.Add(card);
                break;
            case 2:
                P2Hand.Add(card);
                break;
        }
    }

    /// <summary>
    /// Calls the RPC that changes player's number of normal cards
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the cards are changed by </param>
    public void CallNormalCards(int player, int amount)
    {
        photonView.RPC("ModifyNormalCards", RpcTarget.All, player, amount);
    }

    /// <summary>
    /// Changes player's number of normal cards
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the cards are changed by </param>
    [PunRPC]
    public void ModifyNormalCards(int player, int amount)
    {
        switch (player)
        {
            case 1:
                _am.P1Cards += amount;
                break;
            case 2:
                _am.P2Cards += amount;
                break;
        }
    }

    /// <summary>
    /// Calls the RPC that changes player's number of gold cards
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the cards are changed by </param>
    public void CallGoldCards(int player, int amount)
    {
        photonView.RPC("ModifyGoldCards", RpcTarget.All, player, amount);
    }

    /// <summary>
    /// Changes player's number of gold cards
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    /// <param name="amount"> amount the cards are changed by </param>
    [PunRPC]
    public void ModifyGoldCards(int player, int amount)
    {
        switch (player)
        {
            case 1:
                _am.P1GoldCards += amount;
                break;
            case 2:
                _am.P2GoldCards += amount;
                break;
        }
    }

    /// <summary>
    /// Calls the RPC that updates the text for each pile of cards.
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void CallPileText()
    {
        photonView.RPC("UpdatePileText", RpcTarget.All);
    }

    /// <summary>
    /// Updates the text for each pile of cards.
    /// 
    /// Edited: Andrea SD - Turned into an RPC
    /// </summary>
    [PunRPC]
    public void UpdatePileText()
    {
        _uDeckSizeText.text = _uDeck.Count.ToString();
        _gDeckSizeText.text = _gDeck.Count.ToString();
        _dPileSizeText.text = DPile.Count.ToString();
    }

    #endregion
}