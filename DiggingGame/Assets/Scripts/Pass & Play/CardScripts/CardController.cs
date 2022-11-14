/*****************************************************************************
// File Name :         CardManager.cs
// Author :            Rudy Wolfer
// Creation Date :     October 10th, 2022
//
// Brief Description : Script managing card/mouse interactivity and Activation.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _mouseOverPos;
    [SerializeField] private Transform _selectedPos;
    [SerializeField] private Transform _defaultPos;
    [SerializeField] private GameObject _cardVisualToMaximize;
    [SerializeField] private GameObject _cardBody;
    [SerializeField] private GameObject _cardBackground;

    [Header("Values")]
    [SerializeField] private float _cardSlideSpeed;
    [SerializeField] private Color _cardDiscardColor;
    [SerializeField] private Color _cardDefaultColor;

    [Header("Other")]
    private CardManager _cm;
    private ActionManager _am;
    private GameCanvasManagerNew _gcm;
    private BoardManager _bm;
    private CardEffects _ce;
    private PersistentCardManager _pcm;
    [HideInInspector] public int HandPosition;
    [HideInInspector] public int PHandPosition;
    [HideInInspector] public int HeldByPlayer;
    private bool _currentlyMaximized = false;
    private GameObject _maximizedCard;
    private Transform _maximizeAnchor;
    [HideInInspector] public Vector3 NextPos;

    [Header("Selection Variables")]
    [HideInInspector] public bool CanBeSelected;
    [HideInInspector] public bool CanBeDiscarded;
    [HideInInspector] public bool CanBeActivated;
    [HideInInspector] public bool Selected;

    [Header("Activation Variables")]
    [HideInInspector] public bool MadePersistentP1;
    [HideInInspector] public bool MadePersistentP2;

    [Header("Animation")]
    [SerializeField] private Animator _cardAnimator;
    [SerializeField] private float _discardAnimWaitTime;
    private bool _gettingDiscarded;

    /// <summary>
    /// Assigns partner scripts and the maximize anchor.
    /// </summary>
    private void Awake()
    {
        _maximizeAnchor = GameObject.FindGameObjectWithTag("MaximizeAnchor").GetComponent<Transform>();
        _cardBody.gameObject.name = GetComponentInChildren<CardVisuals>().ThisCard.CardName;
        _cm = FindObjectOfType<CardManager>();
        _am = FindObjectOfType<ActionManager>();
        _bm = FindObjectOfType<BoardManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _pcm = FindObjectOfType<PersistentCardManager>();
        _ce = FindObjectOfType<CardEffects>();
        HeldByPlayer = 0;
    }

    /// <summary>
    /// Adjusts the card's visual position.
    /// </summary>
    private void FixedUpdate()
    {
        if (_gettingDiscarded)
        {
            return;
        }

        if (MadePersistentP1 || MadePersistentP2)
        {
            transform.position = _defaultPos.position;
            return;
        }

        if(Selected)
        {
            transform.position = _selectedPos.position;
            return;
        }

        if(transform.position != NextPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, NextPos, _cardSlideSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Effects that occur when the mouse hovers over a card.
    /// </summary>
    private void OnMouseEnter()
    {
        if(_gettingDiscarded)
        {
            return;
        }

        if (CanBeDiscarded)
        {
            _cardBackground.GetComponent<Image>().color = _cardDiscardColor;
        }

        if(MadePersistentP1 || MadePersistentP2)
        {
            return;
        }

        FindObjectOfType<SFXManager>().Play("HoverCard");

        NextPos = _mouseOverPos.position;
    }

    /// <summary>
    /// Effects that occur when the mouse leaves a card.
    /// </summary>
    private void OnMouseExit()
    {
        if (_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }

        if (_gettingDiscarded)
        {
            return;
        }

        if (CanBeDiscarded)
        {
            _cardBackground.GetComponent<Image>().color = _cardDefaultColor;
        }

        if (MadePersistentP1 || MadePersistentP2)
        {
            return;
        }

        NextPos = _defaultPos.position;
    }

    /// <summary>
    /// On click events with the card.
    /// </summary>
    private void OnMouseOver()
    {
        MaximizeCard(_cardVisualToMaximize);

        if(_gettingDiscarded)
        {
            return;
        }

        if (CanBeDiscarded)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(ToDiscard());
            }
        }

        if (MadePersistentP1 || MadePersistentP2)
        {
            return;
        }

        if (CanBeSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectCard();
            }
        }

        if(CanBeActivated)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                ActivateCard();
            }
        }
    }

    /// <summary>
    /// Selects the card.
    /// </summary>
    private void SelectCard()
    {
        FindObjectOfType<SFXManager>().Play("SelectCard");

        if (!Selected)
        {
            _cm.SelectedCards.Add(_cardBody);
            Selected = true;
        }
        else
        {
            _cm.SelectedCards.Remove(_cardBody);
            Selected = false;
        }
    }

    /// <summary>
    /// Activates the card through cardeffects.
    /// </summary>
    private void ActivateCard()
    {
        CardVisuals cv = _cardBody.GetComponentInChildren<CardVisuals>();
        int grassCost = cv.ThisCard.GrassCost;
        int dirtCost = cv.ThisCard.DirtCost;
        int stoneCost = cv.ThisCard.StoneCost;

        if(_cm.AllowedActivations == 0)
        {
            _gcm.UpdateCurrentActionText("You've Activated the max amount of Cards.");
            FindObjectOfType<SFXManager>().PlayButtonSound();
            return;
        }

        if(_am.CurrentPlayer == 1)
        {
            if(_am.P1RefinedPile[0] >= grassCost && _am.P1RefinedPile[1] >= dirtCost && _am.P1RefinedPile[2] >= stoneCost)
            {
                StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Activation", 1);

                _am.P1RefinedPile[0] -= grassCost;
                _am.P1RefinedPile[1] -= dirtCost;
                _am.P1RefinedPile[2] -= stoneCost;
                _am.SupplyPile[0] += grassCost;
                _am.SupplyPile[1] += dirtCost;
                _am.SupplyPile[2] += stoneCost;

                if(MultiSceneData.s_WeatherOption == 0)
                {
                    FindObjectOfType<WeatherManager>().SetActiveWeather(cv.ThisCard.ChangeWeatherTo);
                }

                FindObjectOfType<SFXManager>().Play("ActivateCard");

                if (cv.ThisCard.GrassSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Grass", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.DirtSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Dirt", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.StoneSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Stone", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.GoldSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Gold", cv.ThisCard.CardName, _cardBody));
                }
                _cm.AllowedActivations--;
                _gcm.UpdateTextBothPlayers();
                _cm.StopCardActivating(_am.CurrentPlayer);
                if (!GetComponentInChildren<CardVisuals>().ThisCard.persistent)
                {
                    StartCoroutine(ToDiscard());
                }
                else
                {
                    _cardAnimator.Play("CardDiscard");
                }
            }
            else
            {
                _gcm.UpdateCurrentActionText("Not enough Pieces to Activate this Card!");
                FindObjectOfType<SFXManager>().PlayButtonSound();
            }
        }
        else
        {
            if (_am.P2RefinedPile[0] >= grassCost && _am.P2RefinedPile[1] >= dirtCost && _am.P2RefinedPile[2] >= stoneCost)
            {
                StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Activation", 1);

                _am.P2RefinedPile[0] -= grassCost;
                _am.P2RefinedPile[1] -= dirtCost;
                _am.P2RefinedPile[2] -= stoneCost;
                _am.SupplyPile[0] += grassCost;
                _am.SupplyPile[1] += dirtCost;
                _am.SupplyPile[2] += stoneCost;

                if(MultiSceneData.s_WeatherOption == 0)
                {
                    FindObjectOfType<WeatherManager>().SetActiveWeather(cv.ThisCard.ChangeWeatherTo);
                }

                FindObjectOfType<SFXManager>().Play("ActivateCard");

                if (cv.ThisCard.GrassSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Grass", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.DirtSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Dirt", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.StoneSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Stone", cv.ThisCard.CardName, _cardBody));
                }
                else if (cv.ThisCard.GoldSuit)
                {
                    _ce.StartCoroutine(_ce.ActivateCardEffect("Gold", cv.ThisCard.CardName, _cardBody));
                }
                _cm.AllowedActivations--;
                _gcm.UpdateTextBothPlayers();
                _cm.StopCardActivating(_am.CurrentPlayer);
                if (!GetComponentInChildren<CardVisuals>().ThisCard.persistent)
                {
                    StartCoroutine(ToDiscard());
                }
                else
                {
                    _cardAnimator.Play("CardDiscard");
                }
            }
            else
            {
                _gcm.UpdateCurrentActionText("Not enough Pieces to Activate this card!");
                FindObjectOfType<SFXManager>().PlayButtonSound();
            }
        }
    }

    /// <summary>
    /// Discards the card.
    /// </summary>
    public IEnumerator ToDiscard()
    {
        _cardBackground.GetComponent<Image>().color = _cardDefaultColor;

        if (!MadePersistentP1 && !MadePersistentP2)
        {
            if (HeldByPlayer == 1)
            {
                if (_cardBody.CompareTag("Card"))
                {
                    _am.P1Cards--;
                }
                else if (_cardBody.CompareTag("GoldCard"))
                {
                    _am.P1GoldCards--;
                }
                _cm.P1OpenHandPositions[HandPosition] = true;
                _cm.P1Hand.Remove(_cardBody);
            }
            else if (HeldByPlayer == 2)
            {
                if (_cardBody.CompareTag("Card"))
                {
                    _am.P2Cards--;
                }
                else if (_cardBody.CompareTag("GoldCard"))
                {
                    _am.P2GoldCards--;
                }
                _cm.P2OpenHandPositions[HandPosition] = true;
                _cm.P2Hand.Remove(_cardBody);
            }

            StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Card", 1);
            HeldByPlayer = 0;
            Selected = false;
            CanBeSelected = false;
            CanBeDiscarded = false;
            CanBeActivated = false;
            _cm.DPile.Add(_cardBody);

            _cm.UpdatePileText();
        }
        else if(MadePersistentP1 || MadePersistentP2)
        {
            if(MadePersistentP1)
            {
                _pcm.P1OpenPCardSlots[PHandPosition] = true;
                _pcm.P1PersistentCards.Remove(_cardBody);
            }
            else
            {
                _pcm.P2OpenPCardSlots[PHandPosition] = true;
                _pcm.P2PersistentCards.Remove(_cardBody);
            }

            StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Card", 1);
            HeldByPlayer = 0;
            Selected = false;
            CanBeSelected = false;
            CanBeDiscarded = false;
            CanBeActivated = false;
            MadePersistentP1 = false;
            MadePersistentP2 = false;
            _pcm.DiscardedPersistentCard = true;
            _cm.DPile.Add(_cardBody);

            _cm.UpdatePileText();
        }

        if (_currentlyMaximized)
        {
            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }

        _cardAnimator.Play("CardDiscard");
        FindObjectOfType<SFXManager>().Play("DiscardCard");
        _gettingDiscarded = true;
        yield return new WaitForSeconds(_discardAnimWaitTime);
        _gettingDiscarded = false;
        _cardBody.SetActive(false);
    }

    /// <summary>
    /// Maximizes a card for easier view.
    /// </summary>
    /// <param name="thingToMaximize">Card zone to maximize</param>
    private void MaximizeCard(GameObject thingToMaximize)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_currentlyMaximized)
            {
                return;
            }

            FindObjectOfType<SFXManager>().Play("SelectCard");

            _maximizedCard = Instantiate(thingToMaximize, _maximizeAnchor);
            _maximizedCard.transform.position = _maximizeAnchor.transform.position;
            _currentlyMaximized = true;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(!_currentlyMaximized)
            {
                return;
            }

            FindObjectOfType<SFXManager>().Play("SelectCard");

            Destroy(_maximizedCard);
            _currentlyMaximized = false;
        }
    }
}
