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

public class CardEffects : MonoBehaviour
{
    [Header("General Values")]
    [SerializeField] private float activateAnimWaitTime;

    [Header("Activation UI References")]
    [SerializeField] private GameObject _activateResponseBox;
    [SerializeField] private Color _grassColor, _dirtColor, _stoneColor, _goldColor;
    [SerializeField] private TextMeshProUGUI _cardActivatedText;

    [Header("Card UI References")]
    [SerializeField] private GameObject _morningJogUI;
    [SerializeField] private GameObject _thiefUI;
    public GameObject SecretTunnelsUI;
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
    [SerializeField] private int _maxPiecesToRegenerate;
    [SerializeField] private int _regenPiecesRequiredToScore;
    private int _regenSpotsOnBoard;
    private bool _regenSuitChosen;

    [Header("Damaging Buildings")]
    [SerializeField] private int _overgrowthDamages;
    [SerializeField] private int _floodDamages;
    [SerializeField] private int _earthquakeDamages;
    [SerializeField] private int _thunderstormDamages;
    [SerializeField] private int _tornadoDamages;
    public int RetributionDamages;
    private int _damageBuildingDieSides = 4;
    [HideInInspector] public Building SelectedBuilding;
    [HideInInspector] public PieceController SelectedPiece;
    [HideInInspector] public int AllowedDamages;
    [SerializeField] private int _allowedRepairs;
    [HideInInspector] public int RepairedBuildings;
    private string _tornadoBuildingToDamage;


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
        _morningJogUI.SetActive(false);
        _thiefUI.SetActive(false);
        ProtectBuildingUI.SetActive(false);
        _grassThiefButton.SetActive(false);
        _dirtThiefButton.SetActive(false);
        _stoneThiefButton.SetActive(false);
        _goldThiefButton.SetActive(false);
        _holyIdolUI.SetActive(false);
        SecretTunnelsUI.SetActive(false);
        _tornadoUI.SetActive(false);
        _activateResponseBox.SetActive(false);
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
            case "Flood":
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
            case "Thunderstorm":
                StartCoroutine(Thunderstorm());
                break;
            case "Compaction":
                StartCoroutine(Compaction());
                break;
            case "Earthquake":
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
            case "Tornado":
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
        _gcm.UpdateCurrentActionText("Player " + _am.CurrentPlayer + " has activated " + effectName + "!");
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
        if(suit == "Grass")
        {
            foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
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
                _regenSpotsOnBoard++;
            }
        }
        else if(suit == "Dirt")
        {
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
                _regenSpotsOnBoard++;
            }
        }
        else if(suit == "Stone")
        {
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
                _regenSpotsOnBoard++;
            }
        }

        _regenSuitChosen = true;
    }

    /// <summary>
    /// For Tornado. Pick a building type to damage.
    /// </summary>
    /// <param name="type">"Factory" "Burrow" or "Mine"</param>
    public void SelectBuildingToDamage(string type)
    {
        _tornadoBuildingToDamage = type;
    }

    /// <summary>
    /// Damages a building based on a dice roll.
    /// </summary>
    /// <returns>A number from 0 to 2</returns>
    private int CalculateBuildingDamage()
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
        if(_am.SupplyPile[0] >= _flowersPiecesToPlace)
        {
            enoughPieces = true;
            newPieceCount = _flowersPiecesToPlace - _am.SupplyPile[0];
        }
        else
        {
            enoughPieces = false;
        }

        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Two)
            {
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
            }
        }

        if(enoughPieces)
        {
            _gcm.UpdateCurrentActionText("Place " + _flowersPiecesToPlace + " Grass Pieces onto the Board!");
            while (PlacedPieces != _flowersPiecesToPlace)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Grass Pieces onto the Board!");
            while (PlacedPieces != newPieceCount)
            {
                yield return null;
            }
        }

        if(enoughPieces)
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
        if (_am.SupplyPile[0] >= _gardenPiecesToPlace)
        {
            enoughPieces = true;
            newPieceCount = _gardenPiecesToPlace - _am.SupplyPile[0];
        }
        else
        {
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

                    piece.GetComponent<PieceController>().ShowHidePlaceable(true);
                }
            }
        }

        if (enoughPieces)
        {
            _gcm.UpdateCurrentActionText("Place " + _gardenPiecesToPlace + " Grass Pieces adjacent to your Pawns!");
            while (PlacedPieces != _gardenPiecesToPlace)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Grass Pieces adjacent to your Pawns!");
            while (PlacedPieces != newPieceCount)
            {
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

        int possiblePieces = 0;
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

                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
                    possiblePieces++;
                }
            }
        }

        if(possiblePieces >= _lawnmowerPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _lawnmowerPiecesToDig + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != _lawnmowerPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + possiblePieces + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != possiblePieces)
            {
                yield return null;
            }
        }

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

        AllowedDamages = _overgrowthDamages;

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning != _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass")
                {
                    building.GetComponent<Animator>().Play("TempPawnBlink");
                    building.GetComponent<Building>().CanBeDamaged = true;
                    buildingCount++;
                }
            }
        }

        _bm.BoardColliderSwitch(false);

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                StartCoroutine(SelectedBuilding.DamageBuiliding(CalculateBuildingDamage()));

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Grass!");
            }
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
        }

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
        _remainingStealsText.text = _remainingPiecesToSteal + "Remaining";

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
        foreach (GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                pawn.GetComponent<PlayerPawn>().IsDigging = true;
                pawn.GetComponent<PlayerPawn>().WalkwaySelect = true;
                pawn.GetComponent<Animator>().Play("TempPawnBlink");
            }
        }

        _bm.BoardColliderSwitch(false);
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
        _remainingStealsText.text = _remainingPiecesToSteal + "Remaining";

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

        int possiblePieces = 0;
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

                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
                    possiblePieces++;
                }
            }
        }

        if (possiblePieces >= _excavatorPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _excavatorPiecesToDig + " Dirt Pieces adjacent to your Pawns!");
            while (DugPieces != _excavatorPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + possiblePieces + " Dirt Pieces adjacent to your Pawns!");
            while (DugPieces != possiblePieces)
            {
                yield return null;
            }
        }

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
        if (_am.SupplyPile[1] >= _fertilizerPiecesToPlace)
        {
            enoughPieces = true;
            newPieceCount = _fertilizerPiecesToPlace - _am.SupplyPile[1];
        }
        else
        {
            enoughPieces = false;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Three)
            {
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
            }
        }

        if (enoughPieces)
        {
            _gcm.UpdateCurrentActionText("Place " + _fertilizerPiecesToPlace + " Dirt Pieces adjacent to your Pawns!");
            while (PlacedPieces != _fertilizerPiecesToPlace)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Dirt Pieces adjacent to your Pawns!");
            while (PlacedPieces != newPieceCount)
            {
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
                    building.GetComponent<Animator>().Play("TempPawnBlink");
                    building.GetComponent<Building>().CanBeDamaged = true;
                    buildingCount++;
                }
            }
        }

        _bm.BoardColliderSwitch(false);

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                StartCoroutine(SelectedBuilding.DamageBuiliding(CalculateBuildingDamage()));

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Dirt!");
            }
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
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

        _bm.BoardColliderSwitch(false);
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

        AllowedDamages = _thunderstormDamages;

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning != _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass" || building.GetComponent<Building>().SuitOfPiece == "Dirt")
                {
                    building.GetComponent<Animator>().Play("TempPawnBlink");
                    building.GetComponent<Building>().CanBeDamaged = true;
                    buildingCount++;
                }
            }
        }

        _bm.BoardColliderSwitch(false);

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                StartCoroutine(SelectedBuilding.DamageBuiliding(CalculateBuildingDamage()));

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Grass or Dirt!");
            }
        }

        foreach(GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
        }

        _gcm.UpdateCurrentActionText("Select a building on a Grass or Dirt Piece to damage!");
        int yourBuildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning == _am.CurrentPlayer)
            {
                if (building.GetComponent<Building>().SuitOfPiece == "Grass" || building.GetComponent<Building>().SuitOfPiece == "Dirt")
                {
                    building.GetComponent<Animator>().Play("TempPawnBlink");
                    building.GetComponent<Building>().CanBeDamaged = true;
                    yourBuildingCount++;
                }
            }
        }

        _bm.BoardColliderSwitch(false);

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (yourBuildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                StartCoroutine(SelectedBuilding.DamageBuiliding(CalculateBuildingDamage()));

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Grass or Dirt!");
            }
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
        }

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
        if (_am.SupplyPile[2] >= _compactionPiecesToPlace)
        {
            enoughPieces = true;
            newPieceCount = _compactionPiecesToPlace - _am.SupplyPile[2];
        }
        else
        {
            enoughPieces = false;
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
            {
                piece.GetComponent<PieceController>().ShowHidePlaceable(true);
            }
        }

        if (enoughPieces)
        {
            _gcm.UpdateCurrentActionText("Place " + _compactionPiecesToPlace + " Stone Pieces adjacent to your Pawns!");
            while (PlacedPieces != _compactionPiecesToPlace)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Place " + newPieceCount + " Stone Pieces adjacent to your Pawns!");
            while (PlacedPieces != newPieceCount)
            {
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

        AllowedDamages = _earthquakeDamages;

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

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (pieceCount != 0)
            {
                while (SelectedPiece == null)
                {
                    yield return null;
                }

                foreach(GameObject piece in _bm.GenerateAdjacentPieceList(SelectedPiece.gameObject))
                {
                    if(piece.GetComponentInChildren<Building>())
                    {
                        StartCoroutine(piece.GetComponentInChildren<Building>().DamageBuiliding(CalculateBuildingDamage()));
                    }
                }

                SelectedPiece = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Dirt!");
            }
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
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

        int possiblePieces = 0;
        List<GameObject> pawns = FindEveryPawnOfCurrentPlayer();

        foreach (GameObject pawn in pawns)
        {
            if (pawn.GetComponent<PlayerPawn>().PawnPlayer == _am.CurrentPlayer)
            {
                foreach (GameObject piece in _bm.GenerateAdjacentPieceList(pawn.gameObject))
                {
                    if (piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Three)
                    {
                        continue;
                    }

                    if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building || piece.GetComponent<PieceController>().HasPawn)
                    {
                        continue;
                    }

                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
                    possiblePieces++;
                }
            }
        }

        if (possiblePieces >= _erosionPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _erosionPiecesToDig + " Stone Pieces adjacent to your Pawns!");
            while (DugPieces != _lawnmowerPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + possiblePieces + " Stone Pieces adjacent to your Pawns!");
            while (DugPieces != possiblePieces)
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
            if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Five)
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

        while(RemainingFlips != 0)
        {
            _gcm.UpdateCurrentActionText("Select " + RemainingFlips + " Stone Pieces to look for Gold in!");
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
                card.GetComponent<CardController>().ToDiscard();
            }

            for(int i = PlannedGambleCardsToDraw; i != 0; i--)
            {
                _cm.DrawCard("Universal");
            }
        }
        else
        {
            foreach (GameObject card in _cm.P2Hand)
            {
                card.GetComponent<CardController>().ToDiscard();
            }

            for (int i = PlannedGambleCardsToDraw; i != 0; i--)
            {
                _cm.DrawCard("Universal");
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
            if (piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Five)
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

        int possiblePieces = 0;
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

                    piece.GetComponent<PieceController>().FromActivatedCard = true;
                    piece.GetComponent<PieceController>().ShowHideDiggable(true);
                    possiblePieces++;
                }
            }
        }

        if (possiblePieces >= _goldenShovelPiecesToDig)
        {
            _gcm.UpdateCurrentActionText("Dig " + _goldenShovelPiecesToDig + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != _goldenShovelPiecesToDig)
            {
                yield return null;
            }
        }
        else
        {
            _gcm.UpdateCurrentActionText("Dig " + possiblePieces + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != possiblePieces)
            {
                yield return null;
            }
        }

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
        _remainingStealsText.text = _remainingPiecesToSteal + "Remaining";

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
            building.GetComponent<Building>().CanBeRepaired = true;
            building.GetComponent<Animator>().Play("TempPawnBlink");
            maxRepairs++;
        }

        _bm.BoardColliderSwitch(false);
        
        if(maxRepairs >= _allowedRepairs)
        {
            maxRepairs = _allowedRepairs;
        }

        while(maxRepairs != RepairedBuildings)
        {
            _gcm.UpdateCurrentActionText("Repair " + (maxRepairs - RepairedBuildings) + " more Buildings!");
            yield return null;
        }

        RepairedBuildings = 0;

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

        if(_regenSpotsOnBoard >= _maxPiecesToRegenerate)
        {
            _regenSpotsOnBoard = _maxPiecesToRegenerate;
        }

        while(PlacedPieces != _regenSpotsOnBoard)
        {
            _gcm.UpdateCurrentActionText("Regenerate " + (_regenSpotsOnBoard - PlacedPieces) + " Pieces!");
            yield return null;
        }

        int pointsToScore = 0;
        int curInterval = 0;
        for(int i = 0; i != _regenSpotsOnBoard; i++)
        {
            if(curInterval != _regenPiecesRequiredToScore)
            {
                curInterval++;
                continue;
            }

            pointsToScore++;
        }

        _am.ScorePoints(pointsToScore);

        _regenSuitChosen = false;
        _regenSpotsOnBoard = 0;

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

        _bm.BoardColliderSwitch(false);

        _bm.DisableAllBoardInteractions();
        _gcm.Back();
    }

    /// <summary>
    /// Card effect Coroutine for Tornado. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Tornado()
    {
        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a Building type to damage!");

        AllowedDamages = _tornadoDamages;

        _tornadoUI.SetActive(true);
        while(_tornadoBuildingToDamage == "")
        {
            _gcm.UpdateCurrentActionText("Select a Building type to damage!");
            yield return null;
        }
        _tornadoUI.SetActive(false);

        int buildingCount = 0;
        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            if (building.GetComponent<Building>().PlayerOwning == _am.CurrentPlayer)
            {
                continue;
            }

            if (_tornadoBuildingToDamage != "Mine")
            {
                if (building.GetComponent<Building>().BuildingType == _tornadoBuildingToDamage)
                {
                    building.GetComponent<Animator>().Play("TempPawnBlink");
                    building.GetComponent<Building>().CanBeDamaged = true;
                    buildingCount++;
                }
            }
            else
            {
                building.GetComponent<Animator>().Play("TempPawnBlink");
                building.GetComponent<Building>().CanBeDamaged = true;
                buildingCount++;
            }
        }

        _tornadoBuildingToDamage = "";

        for (int i = AllowedDamages; i != 0; i--)
        {
            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                StartCoroutine(SelectedBuilding.DamageBuiliding(CalculateBuildingDamage()));

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings of that type!");
            }
        }

        foreach (GameObject building in GameObject.FindGameObjectsWithTag("Building"))
        {
            building.GetComponent<Building>().CanBeDamaged = false;
            building.GetComponent<Animator>().Play("TempPawnDefault");
        }

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