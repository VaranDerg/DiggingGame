/*****************************************************************************
// File Name :         Card.cs
// Author :            Rudy Wolfer, Andrea Swihart-DeCoster
// Creation Date :     October 18th, 2022
//
// Brief Description : Script to hold every Card Effect.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

// Edited - Andrea SD: Modified for online use. Added comments
public class OnlineCardEffects : MonoBehaviourPun
{
    /// <summary>
    /// Variables, these control animation times.
    /// </summary>
    [Header("General Values")]
    [SerializeField] private float activateAnimWaitTime;

    /// <summary>
    /// Variables, these are references for the bars that appear once cards are activated.
    /// </summary>
    [Header("Activation UI References")]
    [SerializeField] private GameObject _activateResponseBox;
    [SerializeField] private Sprite _grassABar, _dirtABar, _stoneABar, _goldABar;
    [SerializeField] private TextMeshProUGUI _cardActivatedText;

    /// <summary>
    /// Variables, these are references to UI Elements used for specific cards. Check the "Cards" dropdown in the P&PWeather Canvas.
    /// </summary>
    [Header("Card UI References")]
    [SerializeField] private GameObject _thiefUI;
    public GameObject ProtectBuildingUI;
    [SerializeField] private GameObject _grassThiefButton, _dirtThiefButton, _stoneThiefButton, _goldThiefButton;
    [SerializeField] private TextMeshProUGUI _remainingStealsText;
    [SerializeField] private GameObject _holyIdolUI;
    [SerializeField] private GameObject _regenerationUI;
    [SerializeField] private GameObject _tornadoUI;

    /// <summary>
    /// These are numerical values that for cards the place pieces.
    /// </summary>
    [Header("Placement Effects")]
    [SerializeField] private int _gardenPiecesToPlace;
    [SerializeField] private int _flowersPiecesToPlace;
    [SerializeField] private int _fertilizerPiecesToPlace;
    [SerializeField] private int _compactionPiecesToPlace;

    /// <summary>
    /// These are numerical values for cards that dig pieces. 
    /// </summary>
    [Header("Digging Effects")]
    [SerializeField] private int _lawnmowerPiecesToDig;
    [SerializeField] private int _excavatorPiecesToDig;
    [SerializeField] private int _erosionPiecesToDig;
    [SerializeField] private int _goldenShovelPiecesToDig;

    /// <summary>
    /// These are numerical values for cards that regenerate or place pieces.
    /// </summary>
    [Header("Placing & Regeneration")]
    [HideInInspector] public int PlacedPieces = 0;
    [HideInInspector] public int DugPieces = 0;
    [SerializeField] private int _maxPiecesToRegen;
    [SerializeField] private int _regenPiecesRequiredToScore;
    private int _piecesToRegen;
    private bool _regenSuitChosen;

    /// <summary>
    /// These are a LOT of variables related to damaging buildings. Related to animations, referencing other scripts, and so on. Variables will make more sense in Methods.
    /// </summary>
    [Header("Damaging Buildings")]
    public float BuildingDamageStatusWaitTime;
    [SerializeField] private int _overgrowthDamages;
    [SerializeField] private int _floodDamages;
    [SerializeField] private int _earthquakeDamages;
    [SerializeField] private int _thunderstormDamages;
    [SerializeField] private int _tornadoDamages;
    public int DamageDieSides = 4;
    [HideInInspector] public Building SelectedBuilding;
    [HideInInspector] public int AllowedDamages;
    [SerializeField] private int _allowedRepairs;
    [HideInInspector] public int RepairedBuildings;
    private int _tornadoBuildingToDamage;
    private bool _tornadoBuildingChosen;
    [SerializeField] private GameObject _tFactoryButton, _tBurrowButton, _tMineButton;
    [SerializeField] private Sprite _moleFactory, _moleBurrow, _moleMine, _meerFactory, _meerBurrow, _meerMine;
    [HideInInspector] public int CurrentDamages;
    [HideInInspector] public bool EarthquakePieceSelected;

    /// <summary>
    /// Specific Card Variables
    /// </summary>
    [Header("Planned Profit")]
    public int PiecesToCollect;

    [Header("Master Builder")]
    public int BuildingReduction;

    [Header("Planned Gamble")]
    public int PlannedGambleCardsToDraw;

    /// <summary>
    /// Variables for cards that flip stone.
    /// </summary>
    [Header("Stone Flipping")]
    public int MetalDetectorStoneToFlip;
    public int DiscerningEyeStoneToFlip;
    [HideInInspector] public int RemainingFlips;

    /// <summary>
    /// Variables for Thief cards.
    /// </summary>
    [Header("Thief Cards")]
    public int ThiefPiecesToTake;
    public int DirtyThiefPiecesToTake;
    public int MasterThiefPiecesToTake;
    private int _remainingPiecesToSteal;

    /// <summary>
    /// Variables for Holy Idol
    /// </summary>
    [Header("Holy Idol")]
    public int PiecesToClaim;
    public int GoldToClaim;
    private bool _claimedPieces;

    /// <summary>
    /// Partner Scripts
    /// </summary>
    [Header("Other")]
    private OnlineCanvasManager _gcm;
    private OnlineCardManager _cm;
    private OnlineBoardManager _bm;
    private OnlineActionManager _am;
    private OnlinePersistentCardManager _pcm;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _activateResponseBox.SetActive(false);
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _cm = FindObjectOfType<OnlineCardManager>();
        _bm = FindObjectOfType<OnlineBoardManager>();
        _am = FindObjectOfType<OnlineActionManager>();
        _pcm = FindObjectOfType<OnlinePersistentCardManager>();
        DisableCardEffectUI();
    }

    /// <summary>
    /// Disables all card effect UI.
    /// </summary>
    public void DisableCardEffectUI()
    {
        _thiefUI.SetActive(false);
        ProtectBuildingUI.SetActive(false);
        _grassThiefButton.SetActive(false);
        _dirtThiefButton.SetActive(false);
        _stoneThiefButton.SetActive(false);
        _goldThiefButton.SetActive(false);
        _holyIdolUI.SetActive(false);
        _tornadoUI.SetActive(false);
    }

    /// <summary>
    /// Pretty cringe method for starting effect coroutines. Keeps them localized here w/ an animation.
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    /// <param name="effectName">Name of the effect, matches coroutine. Capitalize each letter, spaces between words.</param>
    public IEnumerator ActivateCardEffect(string suit, string effectName, GameObject pCardObject)
    {
        //Enables the Activated response box and calls ShowActivationText.
        _activateResponseBox.SetActive(true);
        ShowActivationText(suit, effectName, true);

        yield return new WaitForSeconds(activateAnimWaitTime);

        ShowActivationText(suit, effectName, false);

        //DO NOT OPEN THIS YOU WILL BE JUMPSCARED
        switch (effectName)
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
                StartCoroutine(MorningJog(pCardObject));
                break;
            case "Overgrowth!":
                StartCoroutine(Overgrowth());
                break;
            case "Planned Profit":
                StartCoroutine(PlannedProfit(pCardObject));
                break;
            case "Thief":
                StartCoroutine(Thief());
                break;
            case "Walkway":
                Walkway();
                break;
            case "Weed Whacker":
                StartCoroutine(WeedWhacker(pCardObject));
                break;
            case "Dam":
                StartCoroutine(Dam(pCardObject));
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
            case "Flood!":
                StartCoroutine(Flood());
                break;
            case "Mudslide":
                Mudslide();
                break;
            case "Secret Tunnels":
                StartCoroutine(SecretTunnels(pCardObject));
                break;
            case "Shovel":
                StartCoroutine(Shovel(pCardObject));
                break;
            case "Thunderstorm!":
                StartCoroutine(Thunderstorm());
                break;
            case "Compaction":
                StartCoroutine(Compaction());
                break;
            case "Earthquake!":
                StartCoroutine(Earthquake());
                break;
            case "Erosion":
                StartCoroutine(Erosion());
                break;
            case "Geologist":
                StartCoroutine(Geologist(pCardObject));
                break;
            case "Master Builder":
                StartCoroutine(MasterBuilder(pCardObject));
                break;
            case "Metal Detector":
                StartCoroutine(MetalDetector());
                break;
            case "Planned Gamble":
                PlannedGamble();
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
                StartCoroutine(Retribution(pCardObject));
                break;
            case "Teleportation":
                Teleportation();
                break;
            case "Tornado!":
                StartCoroutine(Tornado());
                break;
            case "Transmutation":
                Transmutation();
                break;
            default:
                Debug.LogWarning("No effect with name " + effectName + ".");
                break;
        }
    }

    /// <summary>
    /// Function that plays an animation when a player activates a card.
    /// </summary>
    /// <param name="effectName">"Name of the Effect"</param>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    private void ShowActivationText(string suit, string effectName, bool start)
    {
        // Shows the correct UI Sprite and updates the Text correctly.
        _gcm.UpdateCurrentActionText("Activated Card!");
        _cardActivatedText.text = "Player " + _am.CurrentPlayer + " has Activated " + effectName + "!";
        if (suit == "Grass")
        {
            _activateResponseBox.GetComponent<Image>().sprite = _grassABar;
        }
        else if (suit == "Dirt")
        {
            _activateResponseBox.GetComponent<Image>().sprite = _dirtABar;
        }
        else if (suit == "Stone")
        {
            _activateResponseBox.GetComponent<Image>().sprite = _stoneABar;
        }
        else if (suit == "Gold")
        {
            _activateResponseBox.GetComponent<Image>().sprite = _goldABar;
        }

        if (start)
        {
            _activateResponseBox.GetComponent<Animator>().Play("ActivateBoxFadeIn");
            _cardActivatedText.GetComponent<Animator>().Play("ActivateBoxFadeIn");
        }
        else
        {
            _activateResponseBox.GetComponent<Animator>().Play("ActivateBoxFadeOut");
            _cardActivatedText.GetComponent<Animator>().Play("ActivateBoxFadeOut");
        }
    }

    /// <summary>
    /// Generates a list of every pawn.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> FindEveryPawnOfCurrentPlayer()
    {
        List<GameObject> pawns = new List<GameObject>();

        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            pawns.Add(pawn);
        }

        return pawns;
    }

    /// <summary>
    /// Steals a Piece from your opponent.
    /// 
    /// Edit - Andrea SD: Online use
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    public void StealPiece(string suit)
    {
        if(_am.CurrentPlayer == 1)
        {
            if(suit == "Grass")
            {
                _am.CallUpdatePieces(0, 1, 0, 1);
                if(_am.P2CollectedPile[0] != 0)
                {
                    _am.CallUpdatePieces(0, 2, 0, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 2, 0, -1);
                }
            }
            else if (suit == "Dirt")
            {
                _am.CallUpdatePieces(0, 1, 1, 1);
                if (_am.P2CollectedPile[1] != 0)
                {
                    _am.CallUpdatePieces(0, 2, 1, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 2, 1, -1);
                }
            }
            else if (suit == "Stone")
            {
                _am.CallUpdatePieces(0, 1, 2, 1);
                if (_am.P2CollectedPile[2] != 0)
                {
                    _am.CallUpdatePieces(0, 2, 2, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 2, 2, -1);
                }
            }
            else if (suit == "Gold")
            {
                _am.CallUpdatePieces(0, 1, 3, 1);
                if (_am.P2CollectedPile[3] != 0)
                {
                    _am.CallUpdatePieces(0, 2, 3, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 2, 3, -1);
                }
                _remainingPiecesToSteal--;
            }
        }
        else    // If current player is 2...
        { 
            if(suit == "Grass")
            {
                _am.CallUpdatePieces(0, 2, 0, 1);
                if(_am.P1CollectedPile[0] != 0)
                {
                    _am.CallUpdatePieces(0, 1, 0, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 1, 0, -1);
                }
            }
            else if (suit == "Dirt")
            {
                _am.CallUpdatePieces(0, 2, 1, 1);
                if (_am.P1CollectedPile[1] != 0)
                {
                    _am.CallUpdatePieces(0, 1, 1, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 1, 1, -1);
                }
            }
            else if (suit == "Stone")
            {
                _am.CallUpdatePieces(0, 2, 2, 1);
                if (_am.P1CollectedPile[2] != 0)
                {
                    _am.CallUpdatePieces(0, 1, 2, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 1, 2, -1);
                }
            }
            else if (suit == "Gold")
            {
                _am.CallUpdatePieces(0, 2, 3, 1);
                if (_am.P1CollectedPile[3] != 0)
                {
                    _am.CallUpdatePieces(0, 1, 3, -1);
                }
                else
                {
                    _am.CallUpdatePieces(1, 1, 3, -1);
                }
                _remainingPiecesToSteal--;
            }
        }

        //StatManager is a statistic storing Class. 
        StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Steal", 1);
        //Updates variables. These are used to track how many more pieces a player can steal.
        _remainingPiecesToSteal--;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";
        _gcm.UpdateTextBothPlayers();
    }

    /// <summary>
    /// Claims Pieces with Holy Idol.
    /// 
    /// Edited: Andrea SD - modified the supply pile and collection update lines
    /// for online use
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" "Gold" or "Point"</param>
    public void ClaimPiece(string suit)
    {
        if (suit == "Grass")
        {
            if (_am.SupplyPile[0] >= PiecesToClaim)
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 0, PiecesToClaim);
                _am.ModifySupplyPile(0, -PiecesToClaim);
            }
            else
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 0, _am.SupplyPile[0]);
                _am.ModifySupplyPile(0, -_am.SupplyPile[0]);
            }
        }
        else if (suit == "Dirt")
        {
            if (_am.SupplyPile[1] >= PiecesToClaim)
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 1, PiecesToClaim);
                _am.ModifySupplyPile(1, -PiecesToClaim);
            }
            else
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 1, _am.SupplyPile[1]);
                _am.ModifySupplyPile(1, -_am.SupplyPile[1]);
            }
        }
        else if (suit == "Stone")
        {
            if (_am.SupplyPile[2] >= PiecesToClaim)
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 2, PiecesToClaim);
                _am.ModifySupplyPile(2, -PiecesToClaim);
            }
            else
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 2, _am.SupplyPile[2]);
                _am.ModifySupplyPile(2, -_am.SupplyPile[2]);
            }
        }
        else if (suit == "Gold")
        {
            if (_am.SupplyPile[3] >= GoldToClaim)
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 3, PiecesToClaim);
                _am.ModifySupplyPile(3, -PiecesToClaim);
            }
            else
            {
                _am.CallUpdatePieces(0, _am.CurrentPlayer, 3, _am.SupplyPile[2]);
                _am.ModifySupplyPile(3, -_am.SupplyPile[3]);
            }
        }
        else if (suit == "Point")
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
        }
        
        _gcm.UpdateTextBothPlayers();
        _claimedPieces = true;
    }

    /// <summary>
    /// Highlights pieces on the board to be regenerated. 
    /// </summary>
    /// <param name="suit"></param>
    /// <returns></returns>
    public void RegeneratePieces(string suit)
    {
        //This is called through Regeneration's Coroutine and works with it. This code is sectioned off here for it to finish first. 

        //Variables needed to calculate the card's effect. 
        bool enoughPieces = false;
        int piecesInSupply = 0;
        int openPieces = 0;

        //Suit will control which piece is used. This is chosen through a UI button.
        if (suit == "Grass")
        {
            //EnoughPieces is true if the SupplyPile is larger than the given variable.
            if (_am.SupplyPile[0] >= _maxPiecesToRegen)
            {
                enoughPieces = true;
            }
            else
            {
                //piecesInSupply stores the SupplyPile count of the given piece.
                piecesInSupply = _am.SupplyPile[0];
                enoughPieces = false;
            }

            //Resets Piece status just in case.
            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(false);
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                //Does not highlight pieces with Pawns or Buildings
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                //Does not highlight if the Piece isn't Dirt
                if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Two)
                {
                    continue;
                }

                //openPieces raised by 1 if the piece isn't already highlighted.
                if (!piece.GetComponent<OnlinePieceController>().IsPlaceable)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }
        else if (suit == "Dirt")
        {
            if (_am.SupplyPile[1] >= _maxPiecesToRegen)
            {
                enoughPieces = true;
            }
            else
            {
                piecesInSupply = _am.SupplyPile[1];
                enoughPieces = false;
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Three)
                {
                    continue;
                }

                if (!piece.GetComponent<OnlinePieceController>().IsPlaceable)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }
        else if (suit == "Stone")
        {
            if (_am.SupplyPile[2] >= _maxPiecesToRegen)
            {
                enoughPieces = true;
            }
            else
            {
                piecesInSupply = _am.SupplyPile[2];
                enoughPieces = false;
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Four)
                {
                    continue;
                }

                if (!piece.GetComponent<OnlinePieceController>().IsPlaceable)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }

        //If the supply has enough pieces and the board has enough spots, place every piece possible.
        if (enoughPieces && openPieces >= _maxPiecesToRegen)
        {
            _piecesToRegen = _maxPiecesToRegen;
        }
        //If the supply has enough pieces, place a piece onto every open piece
        else if (enoughPieces)
        {
            _piecesToRegen = openPieces;
        }
        //If the supply doesn't have enough pieces but there's more open pieces than currently open pieces, place every supply piece possible.
        else if (openPieces >= piecesInSupply)
        {
            _piecesToRegen = piecesInSupply;
        }
        //If all else fails, place a piece onto every open piece.
        else
        {
            _piecesToRegen = openPieces;
        }
        //Now, the logic above is very messy and may cause issues that I've yet to find. But every time I've tried it, it worked as intended. 

        StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Place", _piecesToRegen);

        _regenSuitChosen = true;
    }

    /// <summary>
    /// For Tornado. Pick a building type to damage.
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    public void SelectBuildingToDamage(int buildingIndex)
    {
        _tornadoBuildingToDamage = buildingIndex;
        _tornadoBuildingChosen = true;
    }

    /// <summary>
    /// Damages a building based on a dice roll.
    /// </summary>
    /// <returns>A number from 0 to 2</returns>
    public int CalculateBuildingDamage(int diceRoll)
    {
        //Converts a Dice Roll of 1 through 4 into a 1 or 2. 
        int damageToTake = 0;

        if (diceRoll == 1)
        {
            damageToTake = 0;
        }
        else if (diceRoll == 2 || diceRoll == 3)
        {
            damageToTake = 1;
        }
        else if (diceRoll == 4)
        {
            damageToTake = 2;
        }

        return damageToTake;
    }

    //Grass Cards

    /// <summary>
    /// Card effect Coroutine for Flowers. 
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Flowers()
    {
        _gcm.DisableListObjects();
        bool enoughPieces;
        int newPieceCount = 0;
        int openPieces = 0;
        if(_am.SupplyPile[0] >= _flowersPiecesToPlace)
        {
            enoughPieces = true;
        }
        else
        {
            newPieceCount = _am.SupplyPile[0];
            enoughPieces = false;
        }

        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if(piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Two)
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }

        if(openPieces == 0)
        {
            PlacedPieces = 0;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
            _gcm.UpdateCurrentActionText("No open Pieces to place on!");
            yield break;
        }

        PlacedPieces = 0;
        if (enoughPieces && openPieces >= _flowersPiecesToPlace)
        {
            while (PlacedPieces != _flowersPiecesToPlace)
            {
                _gcm.UpdateCurrentActionText("Place " + _flowersPiecesToPlace + " Grass Pieces onto Dirt Pieces!");
                yield return null;
            }
        }
        else if (openPieces >= newPieceCount)
        {
            while (PlacedPieces != newPieceCount)
            {
                _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Grass Pieces onto Dirt Pieces!");
                yield return null;
            }
        }
        else
        {
            while (PlacedPieces != openPieces)
            {
                _gcm.UpdateCurrentActionText("Place " + openPieces + " Grass Pieces onto Dirt Pieces!");
                yield return null;
            }
        }

        if (enoughPieces)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Garden. 
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Garden()
    {
        //Very similar logic for Garden. View Flowers for further detail. Garden places adjacent to Pawns instead, so things change for that.
        _gcm.DisableListObjects();
        bool enoughPieces;
        int openPieces = 0;
        if (_am.SupplyPile[0] >= _gardenPiecesToPlace)
        {
            enoughPieces = true;
        }
        else
        {
            enoughPieces = false;
        }

        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(false);
        }

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Two)
                    {
                        continue;
                    }

                    if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<OnlinePieceController>().IsPlaceable)
                    {
                        piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
                        openPieces++;
                    }
                }
            }
        }

        if (openPieces == 0)
        {
            PlacedPieces = 0;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
            _gcm.UpdateCurrentActionText("No open Pieces to place on!");
            yield break;
        }

        _bm.SetActiveCollider("Board");

        PlacedPieces = 0;
        int supplyAmount = _am.SupplyPile[0];
        if (enoughPieces)
        {
            if (openPieces >= _gardenPiecesToPlace)
            {
                while (PlacedPieces != _gardenPiecesToPlace)
                {
                    _gcm.UpdateCurrentActionText("Place " + _flowersPiecesToPlace + " Grass Pieces adjacent to your Pawns!");
                    yield return null;
                }
            }
            else
            {
                while (PlacedPieces != openPieces)
                {
                    _gcm.UpdateCurrentActionText("Place " + openPieces + " Grass Pieces adjacent to your Pawns!");
                    yield return null;
                }
            }
        }
        else
        {
            if (openPieces >= supplyAmount)
            {
                while (PlacedPieces != supplyAmount)
                {
                    _gcm.UpdateCurrentActionText("Place " + supplyAmount + " Grass Pieces adjacent to your Pawns!");
                    yield return null;
                }
            }
            else
            {
                while (PlacedPieces != openPieces)
                {
                    _gcm.UpdateCurrentActionText("Place " + openPieces + " Grass Pieces adjacent to your Pawns!");
                    yield return null;
                }
            }
        }

        StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Place", PlacedPieces);

        //Scores twice if the max piece amount is placed, but only once if any are placed.
        if (PlacedPieces > 0 && PlacedPieces < _gardenPiecesToPlace)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
        }
        else if (PlacedPieces == _gardenPiecesToPlace)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 2);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Lawnmower. 
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Lawnmower()
    {
        //Lawnmower digs pieces adjacent to your Pawns. Has very similar logic to EXCAVATOR, EROSION, and GOLDEN SHOVEL.
        _gcm.DisableListObjects();

        //Generates a list of every Pawn.
        int openPieces = 0;
        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            //If the pawn is of the current player's...
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                //Generate a List of adjacent Pieces to that Pawn...
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    //The piece must be Grass...
                    if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.One && piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Six)
                    {
                        continue;
                    }

                    //It must not have a Building or Pawn...
                    if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                    {
                        continue;
                    }

                    //And will be made Diggable if all this succeeds. 
                    if (!piece.GetComponent<OnlinePieceController>().IsDiggable)
                    {
                        piece.GetComponent<OnlinePieceController>().FromActivatedCard = true;
                        piece.GetComponent<OnlinePieceController>().ShowHideDiggable(true);
                        openPieces++;
                    }
                }
            }
        }

        DugPieces = 0;

        //If there's more open pieces than diggable pieces...
        if (openPieces >= _lawnmowerPiecesToDig)
        {
            //Dig only as many of those as the card is able to dig.
            _gcm.UpdateCurrentActionText("Dig " + _lawnmowerPiecesToDig + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != _lawnmowerPiecesToDig)
            {
                yield return null;
            }
        }
        //If not...
        else
        {
            //Dig every piece shown. This could be 0!
            _gcm.UpdateCurrentActionText("Dig " + openPieces + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != openPieces)
            {
                yield return null;
            }
        }

        //Reset every leftover Piece.
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            piece.GetComponent<OnlinePieceController>().ShowHideDiggable(false);
            piece.GetComponent<OnlinePieceController>().FromActivatedCard = false;
        }

        DugPieces = 0;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Morning Jog. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MorningJog(GameObject cardBody)
    {
        //This card is a Persistent Card. All these Coroutines do is call "MakeCardPersistent" in PersistentCardManager, which is its own beast! Once made Persistent, its code is usable in other scripts.
        //Morning Jog's code can be found in PieceController's Movement methods.
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Overgrowth. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Overgrowth()
    {
        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a building on a Grass Piece to damage!");

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning != _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
        }

        _bm.SetActiveCollider("Building");
        CurrentDamages = 0;

        if(buildingCount < AllowedDamages)
        {
            AllowedDamages = buildingCount;
        }
        else
        {
            AllowedDamages = _overgrowthDamages;
        }

        while(CurrentDamages != AllowedDamages)
        {
            yield return null;
        }

        CurrentDamages = 0;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        _bm.SetActiveCollider("Board");
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Planned Profit. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator PlannedProfit(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thief()
    {
        //Thief steals Pieces from other players. Remember that earlier method? It works with that. It has very similar logic to DIRTY THIEF and MASTER THIEF.

        //Enables UI and sets the steal amount.
        _gcm.UpdateCurrentActionText("Complete Thief actions!");
        _thiefUI.SetActive(true);
        _grassThiefButton.SetActive(true);
        _dirtThiefButton.SetActive(true);
        _goldThiefButton.SetActive(true);
        _remainingPiecesToSteal = ThiefPiecesToTake;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";

        if (_am.CurrentPlayer == 1)
        {
            //Disables certain buttons if no more pieces of those remain.
            while (_remainingPiecesToSteal != 0 && _am.P2CollectedPile[0] + _am.P2RefinedPile[0] + _am.P2CollectedPile[1] + _am.P2RefinedPile[1] + _am.P2CollectedPile[3] + _am.P2RefinedPile[3] != 0)
            {
                //Disables the Gold button if players choose other pieces. You must take Gold OR 2 other Pieces.
                if (_remainingPiecesToSteal != ThiefPiecesToTake)
                {
                    _goldThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[0] + _am.P2RefinedPile[0] == 0)
                {
                    _grassThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[1] + _am.P2RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[3] + _am.P2RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }
        //Identical for Player 2.
        else
        {
            while (_remainingPiecesToSteal != 0 && _am.P1CollectedPile[0] + _am.P1RefinedPile[0] + _am.P1CollectedPile[1] + _am.P1RefinedPile[1] + _am.P1CollectedPile[3] + _am.P1RefinedPile[3] != 0)
            {
                if (_remainingPiecesToSteal != ThiefPiecesToTake)
                {
                    _goldThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[0] + _am.P1RefinedPile[0] == 0)
                {
                    _grassThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[1] + _am.P1RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[3] + _am.P1RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }

        //Disables UI.
        _thiefUI.SetActive(false);
        _grassThiefButton.SetActive(false);
        _dirtThiefButton.SetActive(false);
        _stoneThiefButton.SetActive(false);
        _goldThiefButton.SetActive(false);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect method for Walkway. 
    /// </summary>
    public void Walkway()
    {
        //Walkway is simple and unique. It basically prompts "IsUsingWalkway" in every player's current pawns. This code can be found in PlayerPawn and PieceController in Walkway related code.
        _bm.DisableAllBoardInteractions();

        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<OnlinePlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                pawn.GetComponent<OnlinePlayerPawn>().IsUsingWalkway = true;
                pawn.GetComponent<Animator>().Play(pawn.GetComponent<OnlinePlayerPawn>().WaitingAnimName);
            }
        }

        _gcm.UpdateCurrentActionText("Select a Pawn for Walkway!");
        _bm.SetActiveCollider("Pawn");
    }

    /// <summary>
    /// (P) Card effect Coroutine for Weed Whacker. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator WeedWhacker(GameObject cardBody)
    {
        //Weed Whacker's logic can be found in DamageBuilding (Building).
        FindObjectOfType<OnlinePersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    //Dirt Cards

    /// <summary>
    /// (P) Card effect Coroutine for Dam. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Dam(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Dirty Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DirtyThief()
    {
        _gcm.UpdateCurrentActionText("Complete Dirty Thief actions!");
        _thiefUI.SetActive(true);
        _stoneThiefButton.SetActive(true);
        _dirtThiefButton.SetActive(true);
        _goldThiefButton.SetActive(true);
        _remainingPiecesToSteal = DirtyThiefPiecesToTake;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";

        if (_am.CurrentPlayer == 1)
        {
            while (_remainingPiecesToSteal != 0 && _am.P2CollectedPile[1] + _am.P2RefinedPile[1] + _am.P2RefinedPile[2] + _am.P2RefinedPile[2] + _am.P2CollectedPile[3] + _am.P2RefinedPile[3] != 0)
            {
                if (_remainingPiecesToSteal != DirtyThiefPiecesToTake)
                {
                    _goldThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[1] + _am.P2RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[2] + _am.P2RefinedPile[2] == 0)
                {
                    _stoneThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[3] + _am.P2RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }
        else
        {
            while (_remainingPiecesToSteal != 0 && _am.P1CollectedPile[1] + _am.P1RefinedPile[1] + _am.P1RefinedPile[2] + _am.P1RefinedPile[2] + _am.P1CollectedPile[3] + _am.P1RefinedPile[3] != 0)
            {
                if (_remainingPiecesToSteal != DirtyThiefPiecesToTake)
                {
                    _goldThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[1] + _am.P1RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[2] + _am.P1RefinedPile[2] == 0)
                {
                    _stoneThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[3] + _am.P1RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }

        _thiefUI.SetActive(false);
        _grassThiefButton.SetActive(false);
        _dirtThiefButton.SetActive(false);
        _stoneThiefButton.SetActive(false);
        _goldThiefButton.SetActive(false);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Excavator. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Excavator()
    {
        _gcm.DisableListObjects();

        int openPieces = 0;
        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Two)
                    {
                        continue;
                    }

                    if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<OnlinePieceController>().FromActivatedCard = true;
                    piece.GetComponent<OnlinePieceController>().ShowHideDiggable(true);
                }
            }
        }

        DugPieces = 0;

        if (openPieces >= _excavatorPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _excavatorPiecesToDig + " Dirt Pieces adjacent to your Pawns!");
            while (DugPieces != _excavatorPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + openPieces + " Dirt Pieces adjacent to your Pawns!");
            while (DugPieces != openPieces)
            {
                yield return null;
            }
        }

        DugPieces = 0;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Fertilizer. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Fertilizer()
    {
        _gcm.DisableListObjects();
        bool enoughPieces;
        int newPieceCount = 0;
        int openPieces = 0;
        if (_am.SupplyPile[1] >= _fertilizerPiecesToPlace)
        {
            enoughPieces = true;
        }
        else
        {
            newPieceCount = _am.SupplyPile[1];
            enoughPieces = false;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Three)
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }

        if (openPieces == 0)
        {
            PlacedPieces = 0;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
            _gcm.UpdateCurrentActionText("No open Pieces to place on!");
            yield break;
        }

        PlacedPieces = 0;
        if (enoughPieces && openPieces >= _fertilizerPiecesToPlace)
        {
            while (PlacedPieces != _fertilizerPiecesToPlace)
            {
                _gcm.UpdateCurrentActionText("Place " + _fertilizerPiecesToPlace + " Dirt Pieces onto Stone Pieces!");
                yield return null;
            }
        }
        else if (openPieces >= newPieceCount)
        {
            while (PlacedPieces != newPieceCount)
            {
                _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Dirt Pieces onto Stone Pieces!");
                yield return null;
            }
        }
        else
        {
            while (PlacedPieces != openPieces)
            {
                _gcm.UpdateCurrentActionText("Place " + openPieces + " Dirt Pieces onto Stone Pieces!");
                yield return null;
            }
        }

        if (enoughPieces)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Flood. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Flood()
    {
        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a building on a Dirt Piece to damage!");

        AllowedDamages = _floodDamages;

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning != _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Dirt")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
        }

        _bm.SetActiveCollider("Building");
        CurrentDamages = 0;

        if (buildingCount < AllowedDamages)
        {
            AllowedDamages = buildingCount;
        }
        else
        {
            AllowedDamages = _floodDamages;
        }

        while (CurrentDamages != AllowedDamages)
        {
            yield return null;
        }

        CurrentDamages = 0;
        _bm.SetActiveCollider("Pawn");

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        StartCoroutine(_pcm.PersistentCardDiscardProcess());
    }

    /// <summary>
    /// Card effect Coroutine for Mudslide. 
    /// </summary>
    public void Mudslide()
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            pawn.GetComponent<PlayerPawn>().MudslideMove = true;
            pawn.GetComponent<Animator>().Play("TempPawnBlink");
        }

        _bm.SetActiveCollider("Pawn");
        _gcm.UpdateCurrentActionText("Select a pawn to move and Dirt Piece to move onto.");
    }

    /// <summary>
    /// (P) Card effect Coroutine for Secret Tunnels. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator SecretTunnels(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Shovel. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Shovel(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Thunderstorm. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thunderstorm()
    {
        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a building on a Grass or Dirt Piece to damage!");

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning != _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass" || building.GetComponent<Building>().SuitOfPiece == "Dirt")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
        }

        _bm.SetActiveCollider("Building");
        CurrentDamages = 0;

        if (buildingCount < _thunderstormDamages)
        {
            AllowedDamages = buildingCount;
        }
        else
        {
            AllowedDamages = _thunderstormDamages;
        }

        while (CurrentDamages != AllowedDamages)
        {
            yield return null;
        }

        CurrentDamages = 0;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        _gcm.UpdateCurrentActionText("Select a building on a Grass or Dirt Piece to damage!");
        int yourBuildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning == _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass" || building.GetComponent<Building>().SuitOfPiece == "Dirt")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    yourBuildingCount++;
                }
            }
        }

        _bm.SetActiveCollider("Building");
        CurrentDamages = 0;

        if (yourBuildingCount < AllowedDamages)
        {
            AllowedDamages = yourBuildingCount;
        }
        else
        {
            AllowedDamages = _thunderstormDamages;
        }

        while (CurrentDamages != AllowedDamages)
        {
            yield return null;
        }

        CurrentDamages = 0;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        _bm.SetActiveCollider("Pawn");
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    //Stone Cards

    /// <summary>
    /// Card effect Coroutine for Compaction. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Compaction()
    {
        _gcm.DisableListObjects();
        bool enoughPieces;
        int newPieceCount = 0;
        int openPieces = 0;
        if (_am.SupplyPile[2] >= _compactionPiecesToPlace)
        {
            enoughPieces = true;
        }
        else
        {
            newPieceCount = _compactionPiecesToPlace - _am.SupplyPile[2];
            enoughPieces = false;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Four)
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<OnlinePieceController>().ShowHidePlaceable(true);
            }
        }

        if (openPieces == 0)
        {
            PlacedPieces = 0;
            _bm.DisableAllBoardInteractions();
            _gcm.Back();
            _gcm.UpdateCurrentActionText("No open Pieces to place on!");
            yield break;
        }

        PlacedPieces = 0;
        if (enoughPieces && openPieces >= _compactionPiecesToPlace)
        {
            while (PlacedPieces != _compactionPiecesToPlace)
            {
                _gcm.UpdateCurrentActionText("Place " + _compactionPiecesToPlace + " Stone Pieces onto Bedrock Pieces!");
                yield return null;
            }
        }
        else if (openPieces >= newPieceCount)
        {
            while (PlacedPieces != newPieceCount)
            {
                _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Stone Pieces onto Bedrock Pieces!");
                yield return null;
            }
        }
        else
        {
            while (PlacedPieces != openPieces)
            {
                _gcm.UpdateCurrentActionText("Place " + openPieces + " Stone Pieces onto Bedrock Pieces!");
                yield return null;
            }
        }

        if (enoughPieces)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Earthquake. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Earthquake()
    {
        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a Stone Piece for Earthquake!");
        AllowedDamages = 0;

        int pieceCount = 0;
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Three)
            {
                if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<OnlinePieceController>().ShowHideEarthquake(true);
                pieceCount++;
            }
        }

        if (pieceCount == 0)
        {
            _gcm.Back();
            _bm.DisableAllBoardInteractions();
            _gcm.UpdateCurrentActionText("No Stone Pieces to select for Earthquake!");
            yield break;
        }

        while (!EarthquakePieceSelected)
        {
            yield return null;
        }
        EarthquakePieceSelected = false;

        _bm.SetActiveCollider("Building");

        CurrentDamages = 0;
        _gcm.UpdateCurrentActionText("Damage every adjacent Building!");
        while (CurrentDamages != AllowedDamages)
        {
            yield return null;
        }
        CurrentDamages = 0;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Erosion. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Erosion()
    {
        _gcm.DisableListObjects();

        int openPieces = 0;
        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Three && piece.GetComponent<OnlinePieceController>().ObjState != OnlinePieceController.GameState.Five)
                    {
                        continue;
                    }

                    if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<OnlinePieceController>().FromActivatedCard = true;
                    piece.GetComponent<OnlinePieceController>().ShowHideDiggable(true);
                }
            }
        }

        if (openPieces >= _erosionPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _erosionPiecesToDig + " Stone Pieces adjacent to your Pawns!");
            while (DugPieces != _lawnmowerPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + openPieces + " Stone Pieces adjacent to your Pawns!");
            while (DugPieces != openPieces)
            {
                yield return null;
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Geologist. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Geologist(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Master Builder. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MasterBuilder(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Metal Detector. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MetalDetector()
    {
        int stoneOnBoard = 0;
        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if(piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Three)
            {
                if(piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                stoneOnBoard++;
                piece.GetComponent<OnlinePieceController>().ShowHideFlippable(true);
            }
        }

        if(stoneOnBoard >= MetalDetectorStoneToFlip)
        {
            RemainingFlips = MetalDetectorStoneToFlip;
        }
        else if(stoneOnBoard != 0)
        {
            RemainingFlips = stoneOnBoard;
        }
        else
        {
            _gcm.UpdateCurrentActionText("No Stone to Flip!");
            RemainingFlips = 0;
        }

        _bm.SetActiveCollider("Board");

        while(RemainingFlips != 0)
        {
            _gcm.UpdateCurrentActionText("Select " + RemainingFlips + " more Stone Pieces to look for Gold in!");
            yield return null;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            piece.GetComponent<OnlinePieceController>().ShowHideFlippable(false);
        }

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Planned Gamble. 
    /// </summary>
    public void PlannedGamble()
    {
        if(_am.CurrentPlayer == 1)
        {
            foreach(GameObject card in _cm.P1Hand)
            {
                StartCoroutine(card.GetComponentInChildren<CardController>().ToDiscard());
            }

            for(int i = PlannedGambleCardsToDraw; i != 0; i--)
            {
                StartCoroutine(_cm.DrawCard("Universal"));
            }
        }
        else
        {
            foreach (GameObject card in _cm.P2Hand)
            {
                StartCoroutine(card.GetComponentInChildren<CardController>().ToDiscard());
            }

            for (int i = PlannedGambleCardsToDraw; i != 0; i--)
            {
                StartCoroutine(_cm.DrawCard("Universal"));
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    //Gold Cards

    /// <summary>
    /// Card effect Coroutine for Discerning Eye. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DiscerningEye()
    {
        int stoneOnBoard = 0;
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Three)
            {
                if (piece.GetComponent<OnlinePieceController>().HasPawn || piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building)
                {
                    continue;
                }

                stoneOnBoard++;
                piece.GetComponent<OnlinePieceController>().ShowHideFlippable(true);
                piece.GetComponent<OnlinePieceController>().DiscerningEye = true;
            }
        }

        if (stoneOnBoard >= DiscerningEyeStoneToFlip)
        {
            RemainingFlips = DiscerningEyeStoneToFlip;
        }
        else if (stoneOnBoard != 0)
        {
            RemainingFlips = stoneOnBoard;
        }
        else
        {
            _gcm.UpdateCurrentActionText("No Stone to Flip!");
            RemainingFlips = 0;
        }

        _bm.SetActiveCollider("Board");

        while (RemainingFlips != 0)
        {
            _gcm.UpdateCurrentActionText("Select " + RemainingFlips + " Stone Pieces to look for Gold in!");
            yield return null;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            piece.GetComponent<OnlinePieceController>().ShowHideFlippable(false);
            piece.GetComponent<OnlinePieceController>().DiscerningEye = false;
        }

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Golden Shovel. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator GoldenShovel()
    {
        _gcm.DisableListObjects();

        int openPieces = 0;
        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<OnlinePieceController>().ObjState == OnlinePieceController.GameState.Four)
                    {
                        continue;
                    }

                    if (piece.GetComponent<OnlinePieceController>().HasP1Building || piece.GetComponent<OnlinePieceController>().HasP2Building || piece.GetComponent<OnlinePieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<OnlinePieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<OnlinePieceController>().FromActivatedCard = true;
                    piece.GetComponent<OnlinePieceController>().ShowHideDiggable(true);
                }
            }
        }

        DugPieces = 0;

        if (openPieces >= _goldenShovelPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _goldenShovelPiecesToDig + " Pieces adjacent to your Pawns!");
            while (DugPieces != _goldenShovelPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + openPieces + " Pieces adjacent to your Pawns!");
            while (DugPieces != openPieces)
            {
                yield return null;
            }
        }

        DugPieces = 0;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Holy Idol. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator HolyIdol()
    {
        _gcm.UpdateCurrentActionText("Complete Holy Idol actions!");
        _holyIdolUI.SetActive(true);

        while(!_claimedPieces)
        {
            yield return null;
        }

        _claimedPieces = false;
        _holyIdolUI.SetActive(false);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Master Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MasterThief()
    {
        _gcm.UpdateCurrentActionText("Complete Master Thief actions!");
        _thiefUI.SetActive(true);
        _stoneThiefButton.SetActive(true);
        _dirtThiefButton.SetActive(true);
        _grassThiefButton.SetActive(true);
        _remainingPiecesToSteal = MasterThiefPiecesToTake;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";

        if (_am.CurrentPlayer == 1)
        {
            while (_remainingPiecesToSteal != 0 && _am.P2CollectedPile[1] + _am.P2RefinedPile[1] + _am.P2RefinedPile[2] + _am.P2RefinedPile[2] + _am.P2CollectedPile[0] + _am.P2RefinedPile[0] != 0)
            {
                if (_am.P2CollectedPile[1] + _am.P2RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[2] + _am.P2RefinedPile[2] == 0)
                {
                    _stoneThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[0] + _am.P2RefinedPile[0] == 0)
                {
                    _grassThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[3] + _am.P2RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }
        else
        {
            while (_remainingPiecesToSteal != 0 && _am.P1CollectedPile[1] + _am.P1RefinedPile[1] + _am.P1RefinedPile[2] + _am.P1RefinedPile[2] + _am.P1CollectedPile[0] + _am.P1RefinedPile[0] != 0)
            {
                if (_am.P1CollectedPile[1] + _am.P1RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[2] + _am.P1RefinedPile[2] == 0)
                {
                    _stoneThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[0] + _am.P1RefinedPile[0] == 0)
                {
                    _grassThiefButton.SetActive(false);
                }

                if (_am.P1CollectedPile[3] + _am.P1RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }

        _thiefUI.SetActive(false);
        _grassThiefButton.SetActive(false);
        _dirtThiefButton.SetActive(false);
        _stoneThiefButton.SetActive(false);
        _goldThiefButton.SetActive(false);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Reconstruction. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Reconstruction()
    {
        int maxRepairs = 0;
        foreach(GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if(building.GetComponent<Building>().BuildingHealth != 1)
            {
                continue;
            }

            building.GetComponent<Building>().PrepBuilidingReapiring(true);
            maxRepairs++;
        }

        _bm.SetActiveCollider("Building");
        
        if(maxRepairs >= _allowedRepairs)
        {
            maxRepairs = _allowedRepairs;
        }

        while(maxRepairs != RepairedBuildings)
        {
            _gcm.UpdateCurrentActionText("Repair " + (maxRepairs - RepairedBuildings) + " more Buildings!");
            yield return null;
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().BuildingHealth != 1)
            {
                continue;
            }

            building.GetComponent<Building>().PrepBuilidingReapiring(false);
        }

        RepairedBuildings = 0;

        _bm.SetActiveCollider("Pawn");
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Regeneration. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Regeneration()
    {
        _regenerationUI.SetActive(true);
        _gcm.UpdateCurrentActionText("Select a Piece to Regenerate!");
        while(!_regenSuitChosen)
        {
            yield return null;
        }
        _regenerationUI.SetActive(false);

        PlacedPieces = 0;
        while(PlacedPieces != _piecesToRegen)
        {
            _gcm.UpdateCurrentActionText("Regenerate " + (_piecesToRegen - PlacedPieces) + " more Pieces!");
            yield return null;
        }
        PlacedPieces = 0;

        int pointsToScore = 0;
        int curInterval = 0;
        for(int i = 0; i != _piecesToRegen; i++)
        {
            if(curInterval != _regenPiecesRequiredToScore)
            {
                curInterval++;
                continue;
            }

            pointsToScore++;
        }

        _am.CallUpdateScore(_am.CurrentPlayer, pointsToScore);

        PlacedPieces = 0;
        _piecesToRegen = 0;
        _regenSuitChosen = false;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// (P) Card effect Coroutine for Retribution. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Retribution(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Method for Teleportation. 
    /// </summary>
    public void Teleportation()
    {
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                pawn.GetComponent<PlayerPawn>().IsMoving = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
                pawn.GetComponent<PlayerPawn>().TeleportationMove = true;
            }
        }

        _bm.SetActiveCollider("Pawn");
        _gcm.UpdateCurrentActionText("Select a pawn to teleport and Piece to teleport onto.");
    }

    /// <summary>
    /// Card effect Coroutine for Tornado. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Tornado()
    {
        _gcm.DisableListObjects();

        _tornadoBuildingChosen = false;
        _tornadoUI.SetActive(true);
        while(!_tornadoBuildingChosen)
        {
            _gcm.UpdateCurrentActionText("Select a Building type to damage!");
            yield return null;
        }
        _tornadoUI.SetActive(false);
        _tornadoBuildingChosen = false;

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning == _am.CurrentPlayer)
            {
                continue;
            }

            if (_tornadoBuildingToDamage == 0)
            {
                if (building.GetComponent<Building>().BuildingType == "Factory")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
            else if(_tornadoBuildingToDamage == 1)
            {
                if (building.GetComponent<Building>().BuildingType == "Burrow")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
            else if(_tornadoBuildingToDamage == 2)
            {
                if (building.GetComponent<Building>().BuildingType == "Grass Mine" || building.GetComponent<Building>().BuildingType == "Dirt Mine" || building.GetComponent<Building>().BuildingType == "Stone Mine")
                {
                    building.GetComponent<Building>().PrepBuilidingDamaging(true);
                    buildingCount++;
                }
            }
        }

        _bm.SetActiveCollider("Building");

        _gcm.UpdateCurrentActionText("Damage every Building of that type!");

        if (buildingCount >= AllowedDamages)
        {
            AllowedDamages = buildingCount;
        }
        else
        {
            AllowedDamages = _tornadoDamages;
        }

        CurrentDamages = 0;
        while (CurrentDamages != AllowedDamages)
        {
            yield return null;
        }
        CurrentDamages = 0;

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().PrepBuilidingDamaging(false);
        }

        _bm.SetActiveCollider("Pawn");
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Method for Transmutation. 
    /// </summary>
    public void Transmutation()
    {
        _am.CallUpdateScore(_am.CurrentPlayer, 1);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }
}