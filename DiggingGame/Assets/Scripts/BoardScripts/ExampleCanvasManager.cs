using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExampleCanvasManager : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _cGrassText, _cDirtText, _cStoneText, _cGoldText;
    [SerializeField] private TextMeshProUGUI _rGrassText, _rDirtText, _rStoneText, _rGoldText;
    [SerializeField] private TextMeshProUGUI _sGrassText, _sDirtText, _sStoneText, _sGoldText;
    [SerializeField] private TextMeshProUGUI _bFactories, _bBurrows, _bMines;
    [SerializeField] private TextMeshProUGUI _rFactories, _rBurrows, _rMines;
    [SerializeField] private TextMeshProUGUI _cardText, _goldCardText, _dragCardsText;

    [Header("Object References")]
    [SerializeField] private GameObject _startTurnButton;
    [SerializeField] private GameObject _firstZone, _thenZone, _finallyZone;
    [SerializeField] private GameObject _actionsZone, _fActionsZone, _digZone, _buildZone, _buildMineZone, _activateZone;

    [Header("Other References")]
    [SerializeField] private ActionManager _am;

    [Header("Other")]
    private List<GameObject> _canvasObjects = new List<GameObject>();
    private int _turn = 1;

    private void AssignToList()
    {
        _canvasObjects.Add(_startTurnButton);
        _canvasObjects.Add(_firstZone);
        _canvasObjects.Add(_thenZone);
        _canvasObjects.Add(_finallyZone);
        _canvasObjects.Add(_actionsZone);
        _canvasObjects.Add(_digZone);
        _canvasObjects.Add(_buildZone);
        _canvasObjects.Add(_buildMineZone);
        _canvasObjects.Add(_activateZone);
        _canvasObjects.Add(_fActionsZone);
    }

    private void Start()
    {
        AssignToList();
        _am = FindObjectOfType<ActionManager>();

        DisableObjects();
        _startTurnButton.SetActive(true);
    }

    private void Update()
    {
        UpdateAllText();
    }

    private void UpdateAllText()
    {
        _cGrassText.text = "x" + _am.CollectedGrass.ToString();
        _cDirtText.text = "x" + _am.CollectedDirt.ToString();
        _cStoneText.text = "x" + _am.CollectedStone.ToString();
        _cGoldText.text = "x" + _am.CollectedGold.ToString();
        _rGrassText.text = "x" + _am.RefinedGrass.ToString();
        _rDirtText.text = "x" + _am.RefinedDirt.ToString();
        _rStoneText.text = "x" + _am.RefinedStone.ToString();
        _rGoldText.text = "x" + _am.RefinedGold.ToString();
        _sGrassText.text = "x" + _am.SupplyGrass.ToString();
        _sDirtText.text = "x" + _am.SupplyDirt.ToString();
        _sStoneText.text = "x" + _am.SupplyStone.ToString();
        _sGoldText.text = "x" + _am.SupplyGold.ToString();
        _bFactories.text = "x" + _am.BuiltFactories.ToString();
        _bBurrows.text = "x" + _am.BuiltBurrows.ToString();
        _bMines.text = "x" + (_am.GrassMines + _am.DirtMines + _am.StoneMines).ToString();
        _rFactories.text = _am.RemainingFactories.ToString() + " Left";
        _rBurrows.text = _am.RemainingBurrows.ToString() + " Left";
        _rMines.text = _am.RemainingMines.ToString() + " Left";
        _cardText.text = _am.Cards.ToString() + " Cards";
        _goldCardText.text = _am.GoldCards.ToString() + " Gold Cards";
        _turnText.text = "Start Turn " + _turn.ToString();
        _dragCardsText.text = "Activate up to " + (_am.CardActivations + _am.BuiltBurrows).ToString() + " Cards.";
    }

    private void DisableObjects()
    {
        UpdateAllText();

        foreach(GameObject thing in _canvasObjects)
        {
            thing.SetActive(false);
        }
    }

    public void StartTurn()
    {
        DisableObjects();
        _firstZone.SetActive(true);
        _am.RefineTiles();
        _am.ActivateMines();
        _am.CurrentTurnPhase++;
    }

    public void ToThenPhase()
    {
        DisableObjects();
        _thenZone.SetActive(true);
        _actionsZone.SetActive(true);
        _am.CurrentTurnPhase++;
    }

    public void Move()
    {
        Debug.Log("Wow, you moved!");
    }

    public void Dig()
    {
        DisableObjects();
        _thenZone.SetActive(true);
        _digZone.SetActive(true);
    }

    public void DigTile(string type)
    {
        _am.CollectTile(type);
        Debug.Log("Collected " + type + "!");
    }

    public void Build()
    {
        DisableObjects();
        _thenZone.SetActive(true);
        _buildZone.SetActive(true);
    }

    public void Retrieve()
    {
        if(_am.UseGold())
        {
            Debug.Log("Refined Gold!");
        }
    }

    public void BuildFactory()
    {
        _am.BuildBuilding("Factory", "");
    }

    public void BuildBurrow()
    {
        _am.BuildBuilding("Burrow", "");
    }

    public void OpenMineMenu()
    {
        DisableObjects();
        _thenZone.SetActive(true);
        _buildMineZone.SetActive(true);
    }

    public void BuildMine(string type)
    {
        _am.BuildBuilding("Mine", type);
    }

    public void ToFinallyPhase()
    {
        _am.CurrentTurnPhase++;
        DisableObjects();
        _finallyZone.SetActive(true);
        _fActionsZone.SetActive(true);
    }

    public void ActivateMenu()
    {
        DisableObjects();
        _finallyZone.SetActive(true);
        _activateZone.SetActive(true);
    }

    public void ActivateCard()
    {
        Debug.Log("Wow, you activated a card!");
    }

    public void Back()
    {
        if(_am.CurrentTurnPhase == 2)
        {
            DisableObjects();
            _thenZone.SetActive(true);
            _actionsZone.SetActive(true);
        }
        else if(_am.CurrentTurnPhase == 3)
        {
            DisableObjects();
            _finallyZone.SetActive(true);
            _fActionsZone.SetActive(true);
        }
    }

    public void EndTurn()
    {
        DisableObjects();
        _am.DrawCards(1, _am.CardDraw + _am.BuiltFactories);
        _am.DiscardCards();
        _am.CurrentTurnPhase = 0;
        _startTurnButton.SetActive(true);
        _turn++;
    }
}
