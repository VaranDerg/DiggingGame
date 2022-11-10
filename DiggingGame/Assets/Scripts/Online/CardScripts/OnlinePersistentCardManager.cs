using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Edited: Andrea SD - Modified for online use

public class OnlinePersistentCardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> _p1PCardPositions = new List<Transform>();
    [SerializeField] private List<Transform> _p2PCardPositions = new List<Transform>();

    //Lists holding players' persistent cards and bools to see if slots are visually open.
    [Header("Card Stuff")]
    [HideInInspector] public List<GameObject> P1PersistentCards = new List<GameObject>();
    [HideInInspector] public List<GameObject> P2PersistentCards = new List<GameObject>();
    [HideInInspector] public bool[] P1OpenPCardSlots;
    [HideInInspector] public bool[] P2OpenPCardSlots;

    //Functional variables for specific code points.
    [Header("Other")]
    [HideInInspector] public bool DiscardedPersistentCard;
    [HideInInspector] public bool DecidedBuildingProtection;
    [HideInInspector] public Coroutine CurrentBuildingDamageProcess;

    [Header("Retribution")]
    [HideInInspector] public int BuildingsDamaged;

    [Header("Partner Scripts")]
    private OnlineActionManager _am;
    private OnlineBoardManager _bm;
    private OnlineCardManager _cm;
    private OnlineCardEffects _ce;
    private OnlineCanvasManager _gcm;

    /// <summary>
    /// Assigns Partner scripts.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<OnlineActionManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _ce = FindObjectOfType<OnlineCardEffects>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
    }

    /// <summary>
    /// Marks every persistent card slot as open.
    /// </summary>
    private void PreparePCardSlots()
    {
        P1OpenPCardSlots = new bool[_p1PCardPositions.Count];
        P2OpenPCardSlots = new bool[_p2PCardPositions.Count];

        for (int i = 0; i < P1OpenPCardSlots.Length; i++)
        {
            P1OpenPCardSlots[i] = true;
        }
        for (int i = 0; i < P2OpenPCardSlots.Length; i++)
        {
            P2OpenPCardSlots[i] = true;
        }
    }

    /// <summary>
    /// Calls the Method.
    /// </summary>
    private void Start()
    {
        PreparePCardSlots();
    }

    /// <summary>
    /// Makes a card persistent. This is very similar to how a Card is drawn and added to one's hand.
    /// </summary>
    /// <param name="card"></param>
    public void MakeCardPersistent(GameObject card)
    {
        if (_am.CurrentPlayer == 1)
        {
            //Iterates through every open card slot.
            for (int i = 0; i < P1OpenPCardSlots.Length; i++)
            {
                if (P1OpenPCardSlots[i] == true)
                {
                    //Moves it there.
                    card.transform.position = _p1PCardPositions[i].position;
                    //Plays an Animation.
                    card.GetComponentInChildren<Animator>().Play("CardPersistent");
                    //Says the Card is persistent and gives its current HandPosition.
                    card.GetComponentInChildren<CardController>().MadePersistentP1 = true;
                    card.GetComponentInChildren<CardController>().PHandPosition = i;
                    //Removes it from the Hand.
                    FindObjectOfType<CardManager>().P1OpenHandPositions[card.GetComponentInChildren<CardController>().HandPosition] = true;
                    FindObjectOfType<CardManager>().P1Hand.Remove(card);
                    //Lowers count.
                    if (card.CompareTag("Card"))
                    {
                        _am.P1Cards--;
                    }
                    else if (card.CompareTag("GoldCard"))
                    {
                        _am.P1GoldCards--;
                    }
                    //Adds to newest list.
                    P1PersistentCards.Add(card);
                    P1OpenPCardSlots[i] = false;
                    //Debug.Log("Made " + card.name + " persistent for player " + _am.CurrentPlayer + "!");
                    return;
                }
            }
        }
        //Same for player 2.
        else
        {
            for (int i = 0; i < P2OpenPCardSlots.Length; i++)
            {
                if (P2OpenPCardSlots[i] == true)
                {
                    card.transform.position = _p2PCardPositions[i].position;
                    card.GetComponentInChildren<Animator>().Play("CardPersistent");
                    card.GetComponentInChildren<CardController>().MadePersistentP2 = true;
                    card.GetComponentInChildren<CardController>().PHandPosition = i;
                    FindObjectOfType<CardManager>().P2OpenHandPositions[card.GetComponentInChildren<CardController>().HandPosition] = true;
                    FindObjectOfType<CardManager>().P2Hand.Remove(card);
                    if (card.CompareTag("Card"))
                    {
                        _am.P2Cards--;
                    }
                    else if (card.CompareTag("GoldCard"))
                    {
                        _am.P2GoldCards--;
                    }
                    P2PersistentCards.Add(card);
                    P2OpenPCardSlots[i] = false;
                    //Debug.Log("Made " + card.name + " persistent for player " + _am.CurrentPlayer + "!");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Checks for a persistent card based on the card's name. 
    /// </summary>
    /// <param name="cardName">Name of the card.</param>
    /// <param name="discardAfterUse">True if the card should be discarded after this check.</param>
    /// <returns></returns>
    public bool CheckForPersistentCard(int player, string cardName)
    {
        //Checks to see if a Player has a persistent card of the provided name. The name is very case sensitive. True if yes, false if no.

        if (player == 1)
        {
            for (int i = 0; i < P1PersistentCards.Count; i++)
            {
                if (P1PersistentCards[i].name == cardName)
                {
                    return true;
                }
            }
        }
        else if (player == 2)
        {
            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                if (P2PersistentCards[i].name == cardName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Discards a persistent card.
    /// </summary>
    /// <param name="player">1 or 2</param>
    /// <param name="cardName">Name of the card, like "Card Name"</param>
    public void DiscardPersistentCard(int player, string cardName)
    {
        //A wrapper to discard a persistent card of that given name.
        if (player == 1)
        {
            for (int i = 0; i < P1PersistentCards.Count; i++)
            {
                if (P1PersistentCards[i].name == cardName)
                {
                    StartCoroutine(P1PersistentCards[i].GetComponentInChildren<CardController>().ToDiscard());
                }
            }
        }
        else if (player == 2)
        {
            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                if (P2PersistentCards[i].name == cardName)
                {
                    StartCoroutine(P2PersistentCards[i].GetComponentInChildren<CardController>().ToDiscard());
                }
            }
        }
    }

    /// <summary>
    /// Discard a Persistent Card.
    /// </summary>
    public IEnumerator PersistentCardDiscardProcess()
    {
        //This is for use with Flood. It sets Cards into a discardable state. 
        if (_am.CurrentPlayer == 1)
        {
            //Puts every card into that state.
            int pCardCount = 0;
            _gcm.UpdateCurrentActionText("Player 2, Discard a Persistent Card.");
            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = true;
                pCardCount++;
            }

            //Stops if no cards.
            if (pCardCount == 0)
            {
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
                yield break;
            }

            //Waits until selected.
            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            //Returns every other card to its original state.
            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = false;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
        }
        //Identical for player 2.
        else
        {
            int pCardCount = 0;
            _gcm.UpdateCurrentActionText("Player 1, Discard a Persistent Card.");
            for (int i = 0; i < P1PersistentCards.Count; i++)
            {
                P1PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = true;
                pCardCount++;
            }

            if (pCardCount == 0)
            {
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
                yield break;
            }

            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            for (int i = 0; i < P1PersistentCards.Count; i++)
            {
                P1PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = false;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
        }
    }

    /// <summary>
    /// Retribution method.
    /// </summary>
    /// <param name="retributionPlayer">1 or 2</param>
    /// <param name="suit">"Grass" "Dirt" or "Stone"</param>
    public void RetributionStart(int retributionPlayer, string suit)
    {
        //Sends every piece a player has to the supply, based on the Retribution card.
        int sentPieces = 0;

        //Note, should affect the opposite player.
        if(retributionPlayer == 2)
        {
            if(suit == "Grass")
            {
                sentPieces += _am.P1CollectedPile[0];
                sentPieces += _am.P1RefinedPile[0];
                _am.SupplyPile[0] += sentPieces;
                _am.P1CollectedPile[0] = 0;
                _am.P1RefinedPile[0] = 0;
            }
            else if(suit == "Dirt")
            {
                sentPieces += _am.P1CollectedPile[1];
                sentPieces += _am.P1RefinedPile[1];
                _am.SupplyPile[1] += sentPieces;
                _am.P1CollectedPile[1] = 0;
                _am.P1RefinedPile[1] = 0;
            }
            else if(suit == "Stone")
            {
                sentPieces += _am.P1CollectedPile[2];
                sentPieces += _am.P1RefinedPile[2];
                _am.SupplyPile[2] += sentPieces;
                _am.P1CollectedPile[2] = 0;
                _am.P1RefinedPile[2] = 0;
            }
        }
        else
        {
            if (suit == "Grass")
            {
                sentPieces += _am.P2CollectedPile[0];
                sentPieces += _am.P2RefinedPile[0];
                _am.SupplyPile[0] += sentPieces;
                _am.P2CollectedPile[0] = 0;
                _am.P2RefinedPile[0] = 0;
            }
            else if (suit == "Dirt")
            {
                sentPieces += _am.P2CollectedPile[1];
                sentPieces += _am.P2RefinedPile[1];
                _am.SupplyPile[1] += sentPieces;
                _am.P2CollectedPile[1] = 0;
                _am.P2RefinedPile[1] = 0;
            }
            else if (suit == "Stone")
            {
                sentPieces += _am.P2CollectedPile[2];
                sentPieces += _am.P2RefinedPile[2];
                _am.SupplyPile[2] += sentPieces;
                _am.P2CollectedPile[2] = 0;
                _am.P2RefinedPile[2] = 0;
            }
        }

        _gcm.UpdateCurrentActionText("Player " + retributionPlayer + " suffered Retribution! " + sentPieces + " of their " + suit + " Pieces were sent to the Supply!");
    }
}
