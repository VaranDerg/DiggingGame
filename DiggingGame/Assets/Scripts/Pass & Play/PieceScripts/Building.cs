/*****************************************************************************
// File Name :         Building.cs
// Author :            Rudy Wolfer
// Creation Date :     October 3rd, 2022
//
// Brief Description : General script for universal building parameters.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Building : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private Sprite _damagedSprite;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private int _showDiceFaceTimes = 30;

    public int BuildingHealth = 2;
    [HideInInspector] public int PlayerOwning = 0;
    [HideInInspector] public string BuildingType = "";
    [HideInInspector] public bool CanBeDamaged;
    [HideInInspector] public bool CanBeRepaired;
    [HideInInspector] public int DamageTaken;
    [HideInInspector] public string SuitOfPiece;
    [HideInInspector] public bool ActiveBuilding;
    [HideInInspector] public bool DamageProtectionResponse;

    [Header("Animations")]
    [SerializeField] private float _removalAnimWaitTime;
    [SerializeField] private GameObject _damagedPS;
    private GameObject _damageDice;

    private List<GameObject> _boardPieces = new List<GameObject>();
    private ActionManager _am;
    private PersistentCardManager _pcm;
    private GameCanvasManagerNew _gcm;
    private CardEffects _ce;
    private Animator _anims;
    private BoardManager _bm;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<ActionManager>();
        _pcm = FindObjectOfType<PersistentCardManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _ce = FindObjectOfType<CardEffects>();
        _anims = GetComponent<Animator>();
        _bm = FindObjectOfType<BoardManager>();
    }

    /// <summary>
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
        _damageDice = GameObject.FindGameObjectWithTag("Damage Dice");
        FindBoardPieces();
    }

    /// <summary>
    /// Adds every board piece to a list.
    /// </summary>
    private void FindBoardPieces()
    {
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            _boardPieces.Add(piece);
        }
    }

    private void OnMouseOver()
    {
        if(CanBeDamaged && !ActiveBuilding)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(DamageBuiliding());
            }
        }

        if(CanBeRepaired && !ActiveBuilding)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                RepairBuilding();
            }
        }
    }

    /// <summary>
    /// Damages a building. Allows persistent cards to be used to protect a building.
    /// </summary>
    /// <param name="damage">The amount of damage (1 or 2, for now)</param>
    public IEnumerator DamageBuiliding()
    {
        bool hasCard = false;
        ActiveBuilding = true;
        _bm.SetActiveCollider("Board");

        //Weed Whacker and Dam Code
        if (SuitOfPiece == "Grass")
        {
            hasCard = _pcm.CheckForPersistentCard(PlayerOwning, "Weed Whacker");
        }
        else if (SuitOfPiece == "Dirt")
        {
            hasCard = _pcm.CheckForPersistentCard(PlayerOwning, "Dam");
        }

        if (hasCard)
        {
            _ce.ProtectBuildingUI.SetActive(true);

            if(PlayerOwning == 1)
            {
                _gcm.UpdateCurrentActionText("Player 1, protect your building?");
            }
            else
            {
                _gcm.UpdateCurrentActionText("Player 2, protect your building?");
            }

            while(!_pcm.DecidedBuildingProtection)
            {
                yield return null;
            }

            _ce.ProtectBuildingUI.SetActive(false);

            _pcm.DecidedBuildingProtection = false;

            if(DamageProtectionResponse == true)
            {
                if (SuitOfPiece == "Grass")
                {
                    _pcm.DiscardPersistentCard(PlayerOwning, "Weed Whacker");
                }
                else if (SuitOfPiece == "Dirt")
                {
                    _pcm.DiscardPersistentCard(PlayerOwning, "Dam");
                }
                _ce.CurrentDamages++;
                _pcm.BuildingsDamaged++;
                _bm.SetActiveCollider("Building");
                yield break;
            }

            ActiveBuilding = false;
        }
        //End Weed Whacker and Dam Code

        _damageDice.GetComponent<Animator>().Play("DiceEnter");
        int num = 0;
        for (int i = 0; i <= _showDiceFaceTimes; i++)
        {
            if(num == _ce.DamageDieSides)
            {
                num = 1;
            }
            else
            {
                num++;
            }
            _damageDice.GetComponentInChildren<TextMeshProUGUI>().text = num.ToString();
            yield return new WaitForSeconds(0.05f);
        }

        int damage = _ce.CalculateBuildingDamage(Random.Range(1, _ce.DamageDieSides + 1));
        _damageDice.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();

        BuildingHealth -= damage;

        if(damage == 2 || BuildingHealth == 0)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has been destroyed!");
        }
        else if(damage == 1 && BuildingHealth == 1)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has taken damage!");
        }
        else if(damage == 0)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " avoided taking damage!");
        }

        if(BuildingHealth <= 0)
        {
            if(PlayerOwning == 1)
            {
                if (BuildingType == "Factory")
                {
                    _am.P1BuiltBuildings[0]--;
                }
                else if (BuildingType == "Burrow")
                {
                    _am.P1BuiltBuildings[1]--;
                }
                else if (BuildingType == "Grass Mine")
                {
                    _am.P1BuiltBuildings[2]--;
                }
                else if (BuildingType == "Dirt Mine")
                {
                    _am.P1BuiltBuildings[3]--;
                }
                else if (BuildingType == "Stone Mine")
                {
                    _am.P1BuiltBuildings[4]--;
                }
                else if(BuildingType == "Gold Mine")
                {
                    _am.P1BuiltBuildings[5]--;
                }
            }
            else
            {
                if (BuildingType == "Factory")
                {
                    _am.P2BuiltBuildings[0]--;
                }
                else if (BuildingType == "Burrow")
                {
                    _am.P2BuiltBuildings[1]--;
                }
                else if (BuildingType == "Grass Mine")
                {
                    _am.P2BuiltBuildings[2]--;
                }
                else if (BuildingType == "Dirt Mine")
                {
                    _am.P2BuiltBuildings[3]--;
                }
                else if (BuildingType == "Stone Mine")
                {
                    _am.P2BuiltBuildings[4]--;
                }
                else if (BuildingType == "Gold Mine")
                {
                    _am.P1BuiltBuildings[5]--;
                }
            }

            _anims.Play("TempPawnDamage");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);

            GetComponentInParent<PieceController>().HasP1Building = false;
            GetComponentInParent<PieceController>().HasP2Building = false;

            if (_pcm.CheckForPersistentCard(PlayerOwning, "Retribution"))
            {
                _pcm.DiscardPersistentCard(PlayerOwning, "Retribution");
                _pcm.RetributionStart(PlayerOwning, SuitOfPiece);
            }

            _am.ScorePoints(1);

            _anims.Play("TempPawnRemove");
            yield return new WaitForSeconds(_removalAnimWaitTime);
            PrepBuilidingDamaging(false);
            _anims.enabled = false;
            gameObject.SetActive(false);
        }
        else if(BuildingHealth == 1)
        {
            GetComponent<SpriteRenderer>().sprite = _damagedSprite;
            _anims.Play("TempPawnDamage");
            _damagedPS.SetActive(true);
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }
        else if(BuildingHealth == 2)
        {
            _anims.Play("TempPawnDamage");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }

        _damageDice.GetComponent<Animator>().Play("DiceExit");
        _ce.CurrentDamages++;
        _pcm.BuildingsDamaged++;
        if (_ce.CurrentDamages == _ce.AllowedDamages)
        {
            _bm.SetActiveCollider("Board");
            _gcm.UpdateCurrentActionText("Damaging complete!");
        }
        else
        {
            _bm.SetActiveCollider("Building");
            _gcm.UpdateCurrentActionText("Damage " + (_ce.AllowedDamages - _ce.CurrentDamages) + " more Buildings!");
        }
        PrepBuilidingDamaging(false);
        ActiveBuilding = false;
    }

    /// <summary>
    /// Repairs a building.
    /// </summary>
    public void RepairBuilding()
    {
        BuildingHealth++;
        GetComponent<SpriteRenderer>().sprite = _defaultSprite;
        _ce.RepairedBuildings++;
        ActiveBuilding = true;

        //_am.ScorePoints cannot be used here since this specific interaction inverses point scoring.
        if(_am.CurrentPlayer == 1 && PlayerOwning == 2)
        {
            _am.ScorePoints(1);
            _gcm.UpdateTextBothPlayers();
        }
        else if(_am.CurrentPlayer == 2 && PlayerOwning == 1)
        {
            _am.ScorePoints(1);
            _gcm.UpdateTextBothPlayers();
        }

        ActiveBuilding = false;
        PrepBuilidingReapiring(false);
    }

    /// <summary>
    /// Prepares a building for taking damage.
    /// </summary>
    /// <param name="show">True = Blink</param>
    public void PrepBuilidingDamaging(bool show)
    {
        if(show)
        {
            CanBeDamaged = true;
            GetComponent<Animator>().Play("TempPawnBlink");
        }
        else
        {
            CanBeDamaged = false;
            GetComponent<Animator>().Play("TempPawnDefault");
        }
    }

    /// <summary>
    /// Prepares a building for repairing.
    /// </summary>
    /// <param name="show">True = Blink</param>
    public void PrepBuilidingReapiring(bool show)
    {
        if (show)
        {
            CanBeRepaired = true;
            GetComponent<Animator>().Play("TempPawnBlink");
        }
        else
        {
            CanBeRepaired = false;
            GetComponent<Animator>().Play("TempPawnDefault");
        }
    }
}
