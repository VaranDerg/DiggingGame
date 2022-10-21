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

public class CardEffects : MonoBehaviour
{
    [Header("General Values")]
    [SerializeField] private float activateAnimWaitTime;

    [Header("Placement Effects")]
    [SerializeField] private int _gardenPiecesToPlace;
    [SerializeField] private int _flowersPiecesToPlace;
    [SerializeField] private int _fertilizerPiecesToPlace;
    [SerializeField] private int _compactionPiecesToPlace;

    [Header("Digging Effects")]
    [SerializeField] private int _lawnmowerPiecesToDig;
    [SerializeField] private int _excavatorPiecesToDig;
    [SerializeField] private int _erosionPiecesToDig;

    [Header("Placing")]
    [HideInInspector] public int PlacedPieces = 0;
    [HideInInspector] public int DugPieces = 0;

    [Header("Damaging Buildings")]
    [SerializeField] private int _overgrowthDamages;
    [SerializeField] private int _floodDamages;
    [SerializeField] private int _earthquakeDamages;
    [SerializeField] private int _thunderstormDamages;
    [SerializeField] private int _tornadoDamages;
    private int _damageBuildingDieSides = 4;
    [HideInInspector] public Building SelectedBuilding;
    [HideInInspector] public int AllowedDamages;

    [Header("Other")]
    private GameCanvasManagerNew _gcm;
    private CardManager _cm;
    private BoardManager _bm;
    private ActionManager _am;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _cm = FindObjectOfType<CardManager>();
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
    }

    /// <summary>
    /// Pretty cringe method for starting effect coroutines. Keeps them localized here w/ an animation.
    /// </summary>
    /// <param name="suit">"Grass" "Dirt" "Stone" or "Gold"</param>
    /// <param name="effectName">Name of the effect, matches coroutine. Capitalize each letter, spaces between words.</param>
    public IEnumerator ActivateCardEffect(string suit, string effectName, GameObject pCardObject)
    {
        ShowActivationText(suit, effectName);

        yield return new WaitForSeconds(activateAnimWaitTime);

        //DO NOT OPEN THIS YOU WILL BE JUMPSCARED
        switch(effectName)
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
                StartCoroutine(Walkway());
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
                StartCoroutine(Mudslide());
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
                StartCoroutine(PlannedGamble());
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
                StartCoroutine(Teleportation());
                break;
            case "Tornado":
                StartCoroutine(Tornado());
                break;
            case "Transmutation":
                StartCoroutine(Transmutation());
                break;
            default:
                Debug.LogWarning("No effect with name " + effectName + ".");
                break;
        }
    }

    /// <summary>
    /// Function that plays an animation when a player activates a card.
    /// </summary>
    /// <param name="effectName"></param>
    private void ShowActivationText(string suit, string effectName)
    {
        Debug.Log("Play " + suit + " animation here.");
        _gcm.UpdateCurrentActionText("Player " + _am.CurrentPlayer + " has activated " + effectName + "!");
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
    /// Damages a building based on a dice roll.
    /// </summary>
    /// <returns>A number from 0 to 2</returns>
    private int CalculateBuildingDamage()
    {
        int sideOfDie = Random.Range(0, _damageBuildingDieSides + 1);
        int damageToTake = 0;

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
            if(_am.CurrentPlayer == 1)
            {
                _am.P1Score++;
            }
            else
            {
                _am.P2Score++;
            }
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
            if(_am.CurrentPlayer == 1)
            {
                _am.P1Score++;
            }
            else
            {
                _am.P2Score++;
            }
        }
        else if(PlacedPieces == _gardenPiecesToPlace)
        {
            if (_am.CurrentPlayer == 1)
            {
                _am.P1Score += 2;
            }
            else
            {
                _am.P2Score += 2;
            }
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
            _gcm.UpdateCurrentActionText("Dig " + _lawnmowerPiecesToDig + " Grass Pieces adjacent to your Pawns!");
            while (DugPieces != possiblePieces)
            {
                yield return null;
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
        for(int i = AllowedDamages; i != 0; i--)
        {
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

            if(buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                SelectedBuilding.DamageBuiliding(CalculateBuildingDamage());

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Grass!");
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thief()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Walkway. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Walkway()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Dirty Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DirtyThief()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
            if (_am.CurrentPlayer == 1)
            {
                _am.P1Score++;
            }
            else
            {
                _am.P2Score++;
            }
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Flood. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Flood()
    {
        //Add discard persistent card part

        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a building on a Grass Piece to damage!");

        AllowedDamages = _floodDamages;

        int buildingCount = 0;
        for (int i = AllowedDamages; i != 0; i--)
        {
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

            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                SelectedBuilding.DamageBuiliding(CalculateBuildingDamage());

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Dirt!");
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Mudslide. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Mudslide()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Thunderstorm. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Thunderstorm()
    {
        //Add part where you damage your own building.

        _gcm.DisableListObjects();
        _gcm.UpdateCurrentActionText("Select a building on a Grass Piece to damage!");

        AllowedDamages = _thunderstormDamages;

        int buildingCount = 0;
        for (int i = AllowedDamages; i != 0; i--)
        {
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

            if (buildingCount != 0)
            {
                while (SelectedBuilding == null)
                {
                    yield return null;
                }

                SelectedBuilding.DamageBuiliding(CalculateBuildingDamage());

                SelectedBuilding = null;
            }
            else
            {
                _gcm.UpdateCurrentActionText("Opponent has no buildings on Grass or Dirt!");
            }
        }

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
            if (_am.CurrentPlayer == 1)
            {
                _am.P1Score++;
            }
            else
            {
                _am.P2Score++;
            }
        }

        PlacedPieces = 0;
        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Earthquake. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Earthquake()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Metal Detector. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MetalDetector()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Planned Gamble. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator PlannedGamble()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    //Gold Cards

    /// <summary>
    /// Card effect Coroutine for Discerning Eye. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator DiscerningEye()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Golden Shovel. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator GoldenShovel()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Holy Idol. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator HolyIdol()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Master Thief. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator MasterThief()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Reconstruction. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Reconstruction()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Regeneration. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Regeneration()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
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
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Teleportation. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Teleportation()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Tornado. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Tornado()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }

    /// <summary>
    /// Card effect Coroutine for Transmutation. 
    /// </summary>
    /// <returns>Wait & Hold time</returns>
    public IEnumerator Transmutation()
    {
        yield return null;

        _bm.DisableAllBoardInteractions();
        _gcm.ToFinallyPhase();
    }
}
