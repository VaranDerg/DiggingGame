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
    private bool InfoShowing;
    [HideInInspector] public bool DiscardedPersistentCard;

    [Header("Partner Scripts")]
    private ActionManager _am;
    private BoardManager _bm;
    private CardManager _cm;
    private GameCanvasManagerNew _gcm;

    private void Awake()
    {
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _cm = FindObjectOfType<CardManager>();
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
                    Debug.Log("Made " + card.name + " persistent for player " + _am.CurrentPlayer + "!");
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
                    Debug.Log("Made " + card.name + " persistent for player " + _am.CurrentPlayer + "!");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Discard a Persistent Card.
    /// </summary>
    public IEnumerator PersistentCardDiscardProcess()
    {
        _gcm.UpdateCurrentActionText("Discard 1 Persistent Card.");

        if (_am.CurrentPlayer == 1)
        {
            foreach(GameObject card in P1PersistentCards)
            {
                card.GetComponent<CardController>().CanBeDiscarded = true;
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
            foreach (GameObject card in P2PersistentCards)
            {
                card.GetComponent<CardController>().CanBeDiscarded = true;
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
}
