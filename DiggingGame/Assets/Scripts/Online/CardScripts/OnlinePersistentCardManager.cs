/*****************************************************************************
// File Name :         OnlinePersistentCardManager.cs
// Author :            Rudy Wolfer, Andrea Swihart-DeCoster
// Creation Date :     October 20th, 2022
//
// Brief Description : A script controlling Persistent Card processes.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

// Edited: Andrea SD - Modified for online use

public class OnlinePersistentCardManager : MonoBehaviourPun
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
    private OnlineCanvasManager _gcm;
    private OnlineCardManager _cm;
    private OnlineAudioPlayer _ap;

    [SerializeField] int _pCardCount = 0;

    /// <summary>
    /// Assigns Partner scripts.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<OnlineActionManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _ap = FindObjectOfType<OnlineAudioPlayer>();
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
                    float xPos = _p1PCardPositions[i].position.x;
                    float yPos = _p1PCardPositions[i].position.y;
                    float zPos = _p1PCardPositions[i].position.z;
                    CallMovePersistent(card.GetComponentInChildren<OnlineCardController>().GetCardID(), 1, xPos, yPos, zPos);
                   
                    card.GetComponentInChildren<OnlineCardController>().PHandPosition = i;
                    //Removes it from the Hand.
                    FindObjectOfType<OnlineCardManager>().P1OpenHandPositions[card.GetComponentInChildren<OnlineCardController>().HandPosition] = true;
                   
                    // ASD

                    //Lowers count.
                    if (card.CompareTag("Card"))
                    {
                        _cm.CallNormalCards(1, -1);
                       
                    }
                    else if (card.CompareTag("GoldCard"))
                    {
                        _cm.CallGoldCards(1, -1);
                    }

                    //Adds to newest list.
                    CallAddToPersistent(card.GetComponentInChildren<OnlineCardController>().GetCardID(), 1);
                    P1OpenPCardSlots[i] = false;
                    _ap.PlaySound("DrawCard", false);
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
                    float xPos = _p2PCardPositions[i].position.x;
                    float yPos = _p2PCardPositions[i].position.y;
                    float zPos = _p2PCardPositions[i].position.z;
                    CallMovePersistent(card.GetComponentInChildren<OnlineCardController>().GetCardID(), 2, xPos, yPos, zPos);
                   
                    card.GetComponentInChildren<OnlineCardController>().PHandPosition = i;
                    FindObjectOfType<OnlineCardManager>().P2OpenHandPositions[card.GetComponentInChildren<OnlineCardController>().HandPosition] = true;

                    if (card.CompareTag("Card"))
                    {
                        _cm.CallNormalCards(2, -1);
                    }
                    else if (card.CompareTag("GoldCard"))
                    {
                        _cm.CallGoldCards(2, -1);
                    }
                    CallAddToPersistent(card.GetComponentInChildren<PhotonView>().ViewID, 2);
                    P2OpenPCardSlots[i] = false;
                    _ap.PlaySound("DrawCard", false);
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
                    StartCoroutine(P1PersistentCards[i].GetComponentInChildren<OnlineCardController>().ToDiscard());
                    _ap.PlaySound("UsePersistent", false);
                }
            }
        }
        else if (player == 2)
        {
            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                if (P2PersistentCards[i].name == cardName)
                {
                    StartCoroutine(P2PersistentCards[i].GetComponentInChildren<OnlineCardController>().ToDiscard());
                    _ap.PlaySound("UsePersistent", false);
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
            _pCardCount = 0;
            _gcm.UpdateCurrentActionText("Waiting for " + _am.PlayerTwoName + " to discard a card.");
            CallPersistentCardDiscard(2);

            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                _pCardCount++;
            }

            //Stops if no cards.
            if (_pCardCount == 0)
            {
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
                yield break;
            }

            DiscardedPersistentCard = false;

            //Waits until selected.
            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            //Returns every other card to its original state.
            CallCanBeDiscarded(2);

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
        }
        //Identical for player 2.
        else
        {
            _pCardCount = 0;

            //Puts every card into that state.
            _gcm.UpdateCurrentActionText("Waiting for " + _am.PlayerTwoName + " to discard a card.");

            _gcm.UpdateOnlineActionText(_am.PlayerTwoName + "s, Discard a Persistent Card.");

            CallPersistentCardDiscard(1);

            for (int i = 0; i < P2PersistentCards.Count; i++)
            {
                _pCardCount++;
            }

            if (_pCardCount == 0)
            {
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
                yield break;
            }

            DiscardedPersistentCard = false;

            while (!DiscardedPersistentCard)
            {
                yield return null;
            }

            CallCanBeDiscarded(1);

            DiscardedPersistentCard = false;
            _bm.DisableAllBoardInteractions();
            CallGoBack();
        }
    }

    /// <summary>
    /// Retribution method.
    /// </summary>
    /// <param name="retributionPlayer"> 1 or 2 </param>
    /// <param name="suit">"Grass" "Dirt" or "Stone"</param>
    public void RetributionStart(int retributionPlayer, string suit)
    {
        //Sends every piece a player has to the supply, based on the Retribution card.
        int sentPieces = 0;

        //Note, should affect the opposite player.
        if (retributionPlayer == 2)
        {
            if (suit == "Grass")
            {
                sentPieces += _am.P1CollectedPile[0];
                sentPieces += _am.P1RefinedPile[0];
                _am.SupplyPileRPC(0, sentPieces);
                _am.CallUpdatePieces(1, 1, 0, -_am.P1RefinedPile[0]);
                _am.CallUpdatePieces(0, 1, 0, -_am.P1CollectedPile[0]);
            }
            else if (suit == "Dirt")
            {
                sentPieces += _am.P1CollectedPile[1];
                sentPieces += _am.P1RefinedPile[1];
                _am.SupplyPileRPC(1, sentPieces);
                _am.CallUpdatePieces(1, 1, 1, -_am.P1RefinedPile[1]);
                _am.CallUpdatePieces(0, 1, 1, -_am.P1CollectedPile[1]);
            }
            else if (suit == "Stone")
            {
                sentPieces += _am.P1CollectedPile[2];
                sentPieces += _am.P1RefinedPile[2];
                _am.SupplyPileRPC(2, sentPieces);
                _am.CallUpdatePieces(1, 1, 2, -_am.P1RefinedPile[2]);
                _am.CallUpdatePieces(0, 1, 2, -_am.P1CollectedPile[2]);
            }
        }
        else
        {
            if (suit == "Grass")
            {
                sentPieces += _am.P2CollectedPile[0];
                sentPieces += _am.P2RefinedPile[0];
                _am.SupplyPileRPC(0, sentPieces);
                _am.CallUpdatePieces(1, 2, 0, -_am.P2RefinedPile[0]);
                _am.CallUpdatePieces(0, 2, 0, -_am.P2CollectedPile[0]);
            }
            else if (suit == "Dirt")
            {
                sentPieces += _am.P2CollectedPile[1];
                sentPieces += _am.P2RefinedPile[1];
                _am.SupplyPileRPC(1, sentPieces);
                _am.CallUpdatePieces(1, 2, 1, -_am.P2RefinedPile[0]);
                _am.CallUpdatePieces(0, 2, 1, -_am.P2CollectedPile[0]);
            }
            else if (suit == "Stone")
            {
                sentPieces += _am.P2CollectedPile[2];
                sentPieces += _am.P2RefinedPile[2];
                _am.SupplyPileRPC(2, sentPieces);
                _am.CallUpdatePieces(1, 2, 2, -_am.P2RefinedPile[0]);
                _am.CallUpdatePieces(0, 2, 2, -_am.P2CollectedPile[0]);
            }
        }
    }

    #region RPC Functions

    /// <summary>
    /// Calls the RPC that returns to the previous phase
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void CallGoBack()
    {
        photonView.RPC("GoBack", RpcTarget.Others);
    }

    /// <summary>S
    /// Returns to the previous phase
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void GoBack()
    {
        _gcm.Back();
    }

    /// <summary>
    /// Calls the RPC that adds a card to persistant cards and removes it from
    /// player's hand
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="pos"> pos of card being removed </param>
    /// <param name="player"> player's hand </param>
    public void CallAddToPersistent(int cardID, int player)
    {
        photonView.RPC("AddToPersistent", RpcTarget.All, cardID, player);
    }

    /// <summary>
    /// Adds the card to persistant cards and removes from players hand
    /// 
    /// Author: ASD
    /// </summary>
    /// <param name="pos"> pos of card being removed </param>
    /// <param name="player"> player's hand </param>
    [PunRPC]
    public void AddToPersistent(int cardID, int player)
    {
        GameObject card = PhotonView.Find(cardID).transform.parent.gameObject;
        card.SetActive(true);

        //Plays an Animation.
        card.GetComponentInChildren<Animator>().Play("CardPersistent");

        switch (player)
        {
            case 1:
                P1PersistentCards.Add(card);
                FindObjectOfType<OnlineCardManager>().P1Hand.Remove(card);
                break;
            case 2:
                P2PersistentCards.Add(card);
                FindObjectOfType<OnlineCardManager>().P2Hand.Remove(card);
                break;
        }
    }

    public void CallMovePersistent(int cardID, int player, float xPos, float yPos, float zPos)
    {
        photonView.RPC("MovePersistentCard", RpcTarget.All, cardID, player, xPos, yPos, zPos);
    }

    [PunRPC]
    public void MovePersistentCard(int cardID, int player, float xPos, float yPos, float zPos)
    {
        GameObject card = PhotonView.Find(cardID).transform.parent.gameObject;

        //Moves it there.
        card.transform.position = new Vector3(xPos, yPos, zPos);

        //Plays an Animation.
        //card.GetComponentInChildren<Animator>().Play("CardPersistent");

        //Says the Card is persistent and gives its current HandPosition.
        if (player == 1)
        {
            card.GetComponentInChildren<OnlineCardController>().MadePersistentP1 = true;
        }
        else
        {
            card.GetComponentInChildren<OnlineCardController>().MadePersistentP2 = true;
        }
    }


    /// <summary>
    /// Has the other player discard a card
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    public void CallPersistentCardDiscard(int player)
    {
        photonView.RPC("PersistentDiscardONL", RpcTarget.Others, player);
    }

    /// <summary>
    /// Has the other player discard a card
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    [PunRPC]
    public void PersistentDiscardONL(int player)
    {
        switch (player)
        {
            case 1:
                for (int i = 0; i < P1PersistentCards.Count; i++)
                {
                    P1PersistentCards[i].GetComponentInChildren<OnlineCardController>().CanBeDiscarded = true;
                }
                break;
            case 2:
                for (int i = 0; i < P2PersistentCards.Count; i++)
                {
                    P2PersistentCards[i].GetComponentInChildren<OnlineCardController>().CanBeDiscarded = true;
                }
                break;
        }
    }

    /// <summary>
    /// Calls the RPC thats sets the other players cards to non-discardable
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    public void CallCanBeDiscarded(int player)
    {
        photonView.RPC("SetCanBeDiscarded", RpcTarget.Others, player);
    }

    /// <summary>
    /// Thats sets the other players cards to non-discardable
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    [PunRPC]
    public void SetCanBeDiscarded(int player)
    {  
        switch (player)
        {
            case 1:
                for (int i = 0; i < P1PersistentCards.Count; i++)
                {
                    _gcm.UpdateCurrentActionText(_am.PlayerOneName + "s, Discard a Persistent Card.");
                    P1PersistentCards[i].GetComponentInChildren<OnlineCardController>().CanBeDiscarded = false;
                }
                break;
            case 2:
                for (int i = 0; i < P2PersistentCards.Count; i++)
                {
                    _gcm.UpdateCurrentActionText(_am.PlayerTwoName + "s, Discard a Persistent Card.");
                    P2PersistentCards[i].GetComponentInChildren<OnlineCardController>().CanBeDiscarded = false;
                }
                break;
        }
    }

    /// <summary>
    /// Calls the RPC that modifies the PCardCount
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> how much PCardCount is changed by </param>
    public void CallPCardCount(int amount)
    {
        photonView.RPC("PCardCount", RpcTarget.All, amount);
    }

    /// <summary>
    /// Modifies the PCardCount
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> how much PCardCount is changed by </param>
    [PunRPC]
    public void PCardCount(int amount)
    {
        _pCardCount += amount;
    }

    #endregion
}
