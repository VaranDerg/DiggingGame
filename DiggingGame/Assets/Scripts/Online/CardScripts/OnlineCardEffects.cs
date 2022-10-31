/*****************************************************************************
// File Name :         Card.cs
// Author :            Rudy Wolfer
// Creation Date :     October 18th, 2022
//
// Brief Description : Script to hold every Card Effect.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OnlineCardEffects : MonoBehaviour
{
    [Header("General Values")]
    [SerializeField] private float activateAnimWaitTime;

    [Header("Activation UI References")]
    [SerializeField] private GameObject _activateResponseBox;
    [SerializeField] private Color _grassColor, _dirtColor, _stoneColor, _goldColor;
    [SerializeField] private TextMeshProUGUI _cardActivatedText;

    [Header("Card UI References")]
    [SerializeField] private GameObject _thiefUI;
    public GameObject ProtectBuildingUI;
    [SerializeField] private GameObject _grassThiefButton, _dirtThiefButton, _stoneThiefButton, _goldThiefButton;
    [SerializeField] private TextMeshProUGUI _remainingStealsText;
    [SerializeField] private GameObject _holyIdolUI;
    [SerializeField] private GameObject _regenerationUI;
    [SerializeField] private GameObject _tornadoUI;

    [Header("Placement Effects")]
    [SerializeField] private int _gardenPiecesToPlace;
    [SerializeField] private int _flowersPiecesToPlace;
    [SerializeField] private int _fertilizerPiecesToPlace;
    [SerializeField] private int _compactionPiecesToPlace;

    [Header("Digging Effects")]
    [SerializeField] private int _lawnmowerPiecesToDig;
    [SerializeField] private int _excavatorPiecesToDig;
    [SerializeField] private int _erosionPiecesToDig;
    [SerializeField] private int _goldenShovelPiecesToDig;

    [Header("Placing & Regeneration")]
    [HideInInspector] public int PlacedPieces = 0;
    [HideInInspector] public int DugPieces = 0;
    [SerializeField] private int _maxPiecesToRegen;
    [SerializeField] private int _regenPiecesRequiredToScore;
    private int _piecesToRegen;
    private bool _regenSuitChosen;

    [Header("Damaging Buildings")]
    public float BuildingDamageStatusWaitTime;
    [SerializeField] private int _overgrowthDamages;
    [SerializeField] private int _floodDamages;
    [SerializeField] private int _earthquakeDamages;
    [SerializeField] private int _thunderstormDamages;
    [SerializeField] private int _tornadoDamages;
    private int _damageBuildingDieSides = 4;
    [HideInInspector] public Building SelectedBuilding;
    [HideInInspector] public int AllowedDamages;
    [SerializeField] private int _allowedRepairs;
    [HideInInspector] public int RepairedBuildings;
    private int _tornadoBuildingToDamage;
    private bool _tornadoBuildingChosen;
    [HideInInspector] public int CurrentDamages;
    [HideInInspector] public bool EarthquakePieceSelected;

    [Header("Planned Profit")]
    public int PiecesToCollect;

    [Header("Master Builder")]
    public int NewBuildingCost;

    [Header("Planned Gamble")]
    public int PlannedGambleCardsToDraw;

    [Header("Stone Flipping")]
    public int MetalDetectorStoneToFlip;
    public int DiscerningEyeStoneToFlip;
    [HideInInspector] public int RemainingFlips;

    [Header("Thief Cards")]
    public int ThiefPiecesToTake;
    public int DirtyThiefPiecesToTake;
    public int MasterThiefPiecesToTake;
    private int _remainingPiecesToSteal;

    [Header("Holy Idol")]
    public int PiecesToClaim;
    public int GoldToClaim;
    private bool _claimedPieces;

    [Header("Other")]
    private GameCanvasManagerNew _gcm;
    private CardManager _cm;
    private BoardManager _bm;
    private ActionManager _am;
    private PersistentCardManager _pcm;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _cm = FindObjectOfType<CardManager>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _pcm = FindObjectOfType<PersistentCardManager>();
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
        _gcm.UpdateCurrentActionText("Activated Card!");
        _cardActivatedText.text = "Player " + _am.CurrentPlayer + " has Activated " + effectName + "!";
        if(suit == "Grass")
        {
            _activateResponseBox.GetComponent<Image>().color = _grassColor;
        }
        else if(suit == "Dirt")
        {
            _activateResponseBox.GetComponent<Image>().color = _dirtColor;
        }
        else if(suit == "Stone")
        {
            _activateResponseBox.GetComponent<Image>().color = _stoneColor;
        }
        else if(suit == "Gold")
        {
            _activateResponseBox.GetComponent<Image>().color = _goldColor;
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
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    public void StealPiece(string suit)
    {
        if(_am.CurrentPlayer == 1)
        {
            if(suit == "Grass")
            {
                _am.P1CollectedPile[0]++;
                if(_am.P2CollectedPile[0] != 0)
                {
                    _am.P2CollectedPile[0]--;
                }
                else
                {
                    _am.P2RefinedPile[0]--;
                }
            }
            else if (suit == "Dirt")
            {
                _am.P1CollectedPile[1]++;
                if (_am.P2CollectedPile[1] != 0)
                {
                    _am.P2CollectedPile[1]--;
                }
                else
                {
                    _am.P2RefinedPile[1]--;
                }
            }
            else if (suit == "Stone")
            {
                _am.P1CollectedPile[2]++;
                if (_am.P2CollectedPile[2] != 0)
                {
                    _am.P2CollectedPile[2]--;
                }
                else
                {
                    _am.P2RefinedPile[2]--;
                }
            }
            else if (suit == "Gold")
            {
                _am.P1CollectedPile[3]++;
                if (_am.P2CollectedPile[3] != 0)
                {
                    _am.P2CollectedPile[3]--;
                }
                else
                {
                    _am.P2RefinedPile[3]--;
                }
                _remainingPiecesToSteal--;
            }
        }
        else
        {
            if (suit == "Grass")
            {
                _am.P2CollectedPile[0]++;
                if (_am.P1CollectedPile[0] != 0)
                {
                    _am.P1CollectedPile[0]--;
                }
                else
                {
                    _am.P1RefinedPile[0]--;
                }
            }
            else if (suit == "Dirt")
            {
                _am.P2CollectedPile[1]++;
                if (_am.P1CollectedPile[1] != 0)
                {
                    _am.P1CollectedPile[1]--;
                }
                else
                {
                    _am.P1RefinedPile[1]--;
                }
            }
            else if (suit == "Stone")
            {
                _am.P2CollectedPile[2]++;
                if (_am.P1CollectedPile[2] != 0)
                {
                    _am.P1CollectedPile[2]--;
                }
                else
                {
                    _am.P1RefinedPile[2]--;
                }
            }
            else if (suit == "Gold")
            {
                _am.P2CollectedPile[3]++;
                if (_am.P1CollectedPile[3] != 0)
                {
                    _am.P1CollectedPile[3]--;
                }
                else
                {
                    _am.P1RefinedPile[3]--;
                }
                _remainingPiecesToSteal--;
            }
        }

        _remainingPiecesToSteal--;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";
        _gcm.UpdateTextBothPlayers();
    }

    /// <summary>
    /// Claims Pieces with Holy Idol.
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" "Gold" or "Point"</param>
    public void ClaimPiece(string suit)
    {
        if (_am.CurrentPlayer == 1)
        {
            if (suit == "Grass")
            {
                if(_am.SupplyPile[0] >= PiecesToClaim)
                {
                    _am.P1CollectedPile[0] += PiecesToClaim;
                    _am.SupplyPile[0] -= PiecesToClaim;
                }
                else
                {
                    _am.P1CollectedPile[0] += _am.SupplyPile[0];
                    _am.SupplyPile[0] -= _am.SupplyPile[0];
                }
            }
            else if (suit == "Dirt")
            {
                if (_am.SupplyPile[1] >= PiecesToClaim)
                {
                    _am.P1CollectedPile[1] += PiecesToClaim;
                    _am.SupplyPile[1] -= PiecesToClaim;
                }
                else
                {
                    _am.P1CollectedPile[1] += _am.SupplyPile[1];
                    _am.SupplyPile[1] -= _am.SupplyPile[1];
                }
            }
            else if (suit == "Stone")
            {
                if (_am.SupplyPile[2] >= PiecesToClaim)
                {
                    _am.P1CollectedPile[2] += PiecesToClaim;
                    _am.SupplyPile[2] -= PiecesToClaim;
                }
                else
                {
                    _am.P1CollectedPile[2] += _am.SupplyPile[2];
                    _am.SupplyPile[2] -= _am.SupplyPile[2];
                }
            }
            else if (suit == "Gold")
            {
                if (_am.SupplyPile[3] >= GoldToClaim)
                {
                    _am.P1CollectedPile[3] += GoldToClaim;
                    _am.SupplyPile[3] -= GoldToClaim;
                }
                else
                {
                    _am.P1CollectedPile[3] += _am.SupplyPile[3];
                    _am.SupplyPile[3] -= _am.SupplyPile[3];
                }
            }
            else if(suit == "Point")
            {
                _am.ScorePoints(1);
            }
        }
        else
        {
            if (suit == "Grass")
            {
                if (_am.SupplyPile[0] >= PiecesToClaim)
                {
                    _am.P2CollectedPile[0] += PiecesToClaim;
                    _am.SupplyPile[0] -= PiecesToClaim;
                }
                else
                {
                    _am.P2CollectedPile[0] += _am.SupplyPile[0];
                    _am.SupplyPile[0] -= _am.SupplyPile[0];
                }
            }
            else if (suit == "Dirt")
            {
                if (_am.SupplyPile[1] >= PiecesToClaim)
                {
                    _am.P2CollectedPile[1] += PiecesToClaim;
                    _am.SupplyPile[1] -= PiecesToClaim;
                }
                else
                {
                    _am.P2CollectedPile[1] += _am.SupplyPile[1];
                    _am.SupplyPile[1] -= _am.SupplyPile[1];
                }
            }
            else if (suit == "Stone")
            {
                if (_am.SupplyPile[2] >= PiecesToClaim)
                {
                    _am.P2CollectedPile[2] += PiecesToClaim;
                    _am.SupplyPile[2] -= PiecesToClaim;
                }
                else
                {
                    _am.P2CollectedPile[2] += _am.SupplyPile[2];
                    _am.SupplyPile[2] -= _am.SupplyPile[2];
                }
            }
            else if (suit == "Gold")
            {
                if (_am.SupplyPile[3] >= GoldToClaim)
                {
                    _am.P2CollectedPile[3] += GoldToClaim;
                    _am.SupplyPile[3] -= GoldToClaim;
                }
                else
                {
                    _am.P2CollectedPile[3] += _am.SupplyPile[3];
                    _am.SupplyPile[3] -= _am.SupplyPile[3];
                }
            }
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
        bool enoughPieces = false;
        int newPieceCount = 0;
        int openPieces = 0;

        if (suit == "Grass")
        {
            if (_am.SupplyPile[0] >= _maxPiecesToRegen)
            {
                enoughPieces = true;
            }
            else
            {
                newPieceCount = _am.SupplyPile[0];
                enoughPieces = false;
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if(piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if(piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Two)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
                openPieces++;
            }
        }
        else if(suit == "Dirt")
        {
            if (_am.SupplyPile[1] >= _maxPiecesToRegen)
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
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Three)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
                openPieces++;
            }
        }
        else if(suit == "Stone")
        {
            if (_am.SupplyPile[2] >= _maxPiecesToRegen)
            {
                enoughPieces = true;
            }
            else
            {
                newPieceCount = _am.SupplyPile[2];
                enoughPieces = false;
            }

            foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Four)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
                openPieces++;
            }
        }

        if (enoughPieces && openPieces >= _maxPiecesToRegen)
        {
            _piecesToRegen = _maxPiecesToRegen;
        }
        else if (enoughPieces)
        {
            _piecesToRegen = openPieces;
        }
        else if (openPieces >= newPieceCount)
        {
            _piecesToRegen = newPieceCount;
        }
        else
        {
            _piecesToRegen = openPieces;
        }

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
    public int CalculateBuildingDamage()
    {
        int sideOfDie = Random.Range(0, _damageBuildingDieSides + 1);
        int damageToTake;

        if(sideOfDie != 0 && sideOfDie != _damageBuildingDieSides)
        {
            damageToTake = 1;
        }
        else if(sideOfDie == _damageBuildingDieSides)
        {
            damageToTake = 2;
        }
        else
        {
            damageToTake = 0;
        }

        return damageToTake;
    }

    //Grass Cards

    /// <summary>
    /// Card effect Coroutine for Flowers. 
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
            if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Two)
            {
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<PieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
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
            _am.ScorePoints(1);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Garden. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Garden()
    {
        _gcm.DisableListObjects();
        bool enoughPieces;
        int newPieceCount = 0;
        int openPieces = 0;
        if (_am.SupplyPile[0] >= _gardenPiecesToPlace)
        {
            enoughPieces = true;
        }
        else
        {
            newPieceCount = _am.SupplyPile[0];
            enoughPieces = false;
        }

        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Two)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<PieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<PieceController>().ShowHidePlaceable(true);
                }
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
        if (enoughPieces && openPieces >= _gardenPiecesToPlace)
        {
            while (PlacedPieces != _gardenPiecesToPlace)
            {
                _gcm.UpdateCurrentActionText("Place " + _gardenPiecesToPlace + " Grass Pieces adjacent to your Pawns!");
                yield return null;
            }
        }
        else if(openPieces >= newPieceCount)
        {
            while (PlacedPieces != newPieceCount)
            {
                _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Grass Pieces adjacent to your Pawns!");
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

        if(PlacedPieces > 0 && PlacedPieces < _gardenPiecesToPlace)
        {
            _am.ScorePoints(1);
        }
        else if(PlacedPieces == _gardenPiecesToPlace)
        {
            _am.ScorePoints(2);
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Lawnmower. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Lawnmower()
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
                    if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.One && piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Six)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<PieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
                }
            }
        }

        DugPieces = 0;

        if(openPieces >= _lawnmowerPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _lawnmowerPiecesToDig + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != _lawnmowerPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + openPieces + " Grass Pieces adjacent to your Pawns!");
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
    /// (P) Card effect Coroutine for Morning Jog. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MorningJog(GameObject cardBody)
    {
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
        _gcm.UpdateCurrentActionText("Complete Thief actions!");
        _thiefUI.SetActive(true);
        _grassThiefButton.SetActive(true);
        _dirtThiefButton.SetActive(true);
        _goldThiefButton.SetActive(true);
        _remainingPiecesToSteal = ThiefPiecesToTake;
        _remainingStealsText.text = _remainingPiecesToSteal + " Remaining";

        if (_am.CurrentPlayer == 1)
        {
            while (_remainingPiecesToSteal != 0 && _am.P2CollectedPile[0] + _am.P2RefinedPile[0] + _am.P2RefinedPile[1] + _am.P2RefinedPile[1] + _am.P2CollectedPile[3] + _am.P2RefinedPile[3] != 0)
            {
                if (_remainingPiecesToSteal != ThiefPiecesToTake)
                {
                    _goldThiefButton.SetActive(false);
                }

                if(_am.P2CollectedPile[0] + _am.P2RefinedPile[0] == 0)
                {
                    _grassThiefButton.SetActive(false);
                }

                if (_am.P2CollectedPile[1] + _am.P2RefinedPile[1] == 0)
                {
                    _dirtThiefButton.SetActive(false);
                }

                if(_am.P2CollectedPile[3] + _am.P2RefinedPile[3] == 0)
                {
                    _goldThiefButton.SetActive(false);
                }

                yield return null;
            }
        }
        else
        {
            while (_remainingPiecesToSteal != 0 && _am.P1CollectedPile[0] + _am.P1RefinedPile[0] + _am.P1RefinedPile[1] + _am.P1RefinedPile[1] + _am.P1CollectedPile[3] + _am.P1RefinedPile[3] != 0)
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
        _bm.DisableAllBoardInteractions();

        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                pawn.GetComponent<PlayerPawn>().IsUsingWalkway = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _gcm.UpdateCurrentActionText("Select a Pawn to Dig and Move with Walkway!");
        _bm.SetActiveCollider("Pawn");
    }

    /// <summary>
    /// (P) Card effect Coroutine for Weed Whacker. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator WeedWhacker(GameObject cardBody)
    {
        FindObjectOfType<PersistentCardManager>().MakeCardPersistent(cardBody);

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
                    if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Two)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<PieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
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
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Three)
            {
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<PieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
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
            _am.ScorePoints(1);
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
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
            {
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if (!piece.GetComponent<PieceController>().CheckedByPawn)
                {
                    openPieces++;
                }
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
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
            _am.ScorePoints(1);
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
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Three)
            {
                if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideEarthquake(true);
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
                    if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Three && piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Five)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<PieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
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
            if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Three)
            {
                if(piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                stoneOnBoard++;
                piece.GetComponent<PieceController>().ShowHideFlippable(true);
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
            piece.GetComponent<PieceController>().ShowHideFlippable(false);
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
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Three)
            {
                if (piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                stoneOnBoard++;
                piece.GetComponent<PieceController>().ShowHideFlippable(true);
                piece.GetComponent<PieceController>().DiscerningEye = true;
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
            piece.GetComponent<PieceController>().ShowHideFlippable(false);
            piece.GetComponent<PieceController>().DiscerningEye = false;
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
                    if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    if (!piece.GetComponent<PieceController>().CheckedByPawn)
                    {
                        openPieces++;
                    }
                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
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

        _am.ScorePoints(pointsToScore);

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
        _am.ScorePoints(1);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }
}