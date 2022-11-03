using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PersistentCardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> _p1PCardPositions = new List<Transform>();
    [SerializeField] private List<Transform> _p2PCardPositions = new List<Transform>();

    [Header("Card Stuff")]
    [HideInInspector] public List<GameObject> P1PersistentCards = new List<GameObject>();
    [HideInInspector] public List<GameObject> P2PersistentCards = new List<GameObject>();
    [HideInInspector] public bool[] P1OpenPCardSlots;
    [HideInInspector] public bool[] P2OpenPCardSlots;

    [Header("Other")]
    [HideInInspector] public bool DiscardedPersistentCard;
    [HideInInspector] public bool DecidedBuildingProtection;

    [Header("Retribution")]
    [HideInInspector] public int BuildingsDamaged;

    [Header("Partner Scripts")]
    private ActionManager _am;
    private BoardManager _bm;
    private CardManager _cm;
    private CardEffects _ce;
    private GameCanvasManagerNew _gcm;

    private void Awake()
    {
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _cm = FindObjectOfType<CardManager>();
        _ce = FindObjectOfType<CardEffects>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
    }

    private void PreparePCardSlots()
    {
        P1OpenPCardSlots = new bool[_p1PCardPositions.Count];
        P2OpenPCardSlots = new bool[_p2PCardPositions.Count];

        for(int i = 0; i < P1OpenPCardSlots.Length; i++)
        {
            P1OpenPCardSlots[i] = true;
        }
        for (int i = 0; i < P2OpenPCardSlots.Length; i++)
        {
            P2OpenPCardSlots[i] = true;
        }
    }

    private void Start()
    {
        PreparePCardSlots();
    }

    public void MakeCardPersistent(GameObject card)
    {
        if(_am.CurrentPlayer == 1)
        {
            for(int i = 0; i < P1OpenPCardSlots.Length; i++)
            {
                if(P1OpenPCardSlots[i] == true)
                {
                    card.transform.position = _p1PCardPositions[i].position;
                    card.GetComponentInChildren<Animator>().Play("CardPersistent");
                    card.GetComponentInChildren<CardController>().MadePersistentP1 = true;
                    card.GetComponentInChildren<CardController>().PHandPosition = i;
                    FindObjectOfType<CardManager>().P1OpenHandPositions[card.GetComponentInChildren<CardController>().HandPosition] = true;
                    FindObjectOfType<CardManager>().P1Hand.Remove(card);
                    if (card.CompareTag("Card"))
                    {
                        _am.P1Cards--;
                    }
                    else if(card.CompareTag("GoldCard"))
                    {
                        _am.P1GoldCards--;
                    }
                    P1PersistentCards.Add(card);
                    P1OpenPCardSlots[i] = false;
                    //Debug.Log("Made " + card.name + " persistent for player " + _am.CurrentPlayer + "!");
                    return;
                }
            }
        }
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
        if(player == 1)
        {
            for(int i = 0; i < P1PersistentCards.Count; i++)
            {
                if(P1PersistentCards[i].name == cardName)
                {
                    return true;
                }
            }
        }
        else if(player == 2)
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
        if (player == 1)
        {
            for(int i = 0; i < P1PersistentCards.Count; i++)
            {
                if(P1PersistentCards[i].name == cardName)
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
        if (_am.CurrentPlayer == 1)
        {
            int pCardCount = 0;
            _gcm.UpdateCurrentActionText("Player 2, Discard a Persistent Card.");
            for(int i = 0; i < P2PersistentCards.Count; i++)
            {
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = true;
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

            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = false;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
        }
        else
        {
            int pCardCount = 0;
            _gcm.UpdateCurrentActionText("Player 1, Discard a Persistent Card.");
            for (int i = 0; i < P1PersistentCards.Count; i++)
            {
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = true;
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
                P2PersistentCards[i].GetComponentInChildren<CardController>().CanBeDiscarded = false;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
        }
    }

    /// <summary>
    /// Simple yes/no for using a building protection card.
    /// </summary>
    /// <param name="answer">Yes/No</param>
    /// <returns></returns>
    public void ProtectBuilding(bool answer)
    {
        DecidedBuildingProtection = true;

        foreach(GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if(building.GetComponent<Building>().ActiveBuilding)
            {
                building.GetComponent<Building>().DamageProtectionResponse = answer;
            }
        }
    }

    /// <summary>
    /// Retribution method.
    /// </summary>
    /// <param name="retributionPlayer">1 or 2</param>
    /// <param name="suit">"Grass" "Dirt" or "Stone"</param>
    public void RetributionStart(int retributionPlayer, string suit)
    {
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
