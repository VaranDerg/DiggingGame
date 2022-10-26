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
                    card.GetComponentInChildren<CardController>().MadePersistentP1 = true;
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
                    card.GetComponentInChildren<CardController>().MadePersistentP2 = true;
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
    public bool CheckForPersistentCard(int player, string cardName, bool discardAfterUse)
    {
        bool hasCard = false;
        if(player == 1)
        {
            foreach(GameObject card in P1PersistentCards)
            {
                if(card.gameObject.name == cardName)
                {
                    hasCard = true;
                    if (discardAfterUse)
                    {
                        card.GetComponentInChildren<CardController>().ToDiscard();
                    }
                }
                else
                {
                    hasCard = false;
                }
            }
        }
        else if(player == 2)
        {
            foreach (GameObject card in P2PersistentCards)
            {
                if (card.gameObject.name == cardName)
                {
                    hasCard = true;
                    if (discardAfterUse)
                    {
                        card.GetComponentInChildren<CardController>().ToDiscard();
                    }
                }
                else
                {
                    hasCard = false;
                }
            }
        }
        return hasCard;
    }

    /// <summary>
    /// Discard a Persistent Card.
    /// </summary>
    public IEnumerator PersistentCardDiscardProcess()
    {
        int cardCount = 0;
        if (_am.CurrentPlayer == 1)
        {
            _gcm.UpdateCurrentActionText("Player 2, Discard a Persistent Card.");
            foreach (GameObject card in P2PersistentCards)
            {
                card.GetComponentInChildren<CardController>().CanBeDiscarded = true;
                cardCount++;
            }

            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.ToFinallyPhase();
        }
        else
        {
            _gcm.UpdateCurrentActionText("Player 1, Discard a Persistent Card.");
            foreach (GameObject card in P1PersistentCards)
            {
                card.GetComponentInChildren<CardController>().CanBeDiscarded = true;
            }

            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.ToFinallyPhase();
        }
    }

    /// <summary>
    /// UI Button that lets the player skip movement with Morning Jog.
    /// </summary>
    public void SkipMorningJog()
    {
        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            piece.GetComponent<PieceController>().StopCoroutine("PawnMovement");
        }
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
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
    /// Coroutine for Retribtion process.
    /// </summary>
    /// <returns>Default hold time.</returns>
    public IEnumerator StartRetribution()
    {
        int possibleDamages = 0;
        foreach(GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if(building.GetComponent<Building>().PlayerOwning == _am.CurrentPlayer)
            {
                continue;
            }

            building.GetComponent<Building>().CanBeDamaged = true;
            possibleDamages++;
        }

        _bm.SetActiveCollider("Building");

        if (possibleDamages >= _ce.RetributionDamages)
        {
            possibleDamages = _ce.RetributionDamages;
        }

        BuildingsDamaged = 0;

        while(BuildingsDamaged != possibleDamages)
        {
            _gcm.UpdateCurrentActionText("Damage " + (possibleDamages - BuildingsDamaged) + " more Buildings!");
            yield return null;
        }

        _bm.SetActiveCollider("Pawn");
        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }
}
