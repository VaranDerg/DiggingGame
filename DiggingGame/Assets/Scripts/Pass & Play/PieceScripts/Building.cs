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

[System.Serializable]
public class Building : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private Color _damagedColor;
    [SerializeField] private Color _defaultColor;

    public int BuildingHealth = 2;
    [HideInInspector] public int PlayerOwning = 0;
    [HideInInspector] public string BuildingType = "";
    [HideInInspector] public bool CanBeDamaged;
    [HideInInspector] public bool CanBeRepaired;
    [HideInInspector] public int DamageTaken;
    [HideInInspector] public string SuitOfPiece;
    [HideInInspector] public bool ActiveBuilding;
    [HideInInspector] public bool DamageProtectionResponse;

    private List<GameObject> _boardPieces = new List<GameObject>();
    private ActionManager _am;
    private PersistentCardManager _pcm;
    private GameCanvasManagerNew _gcm;
    private CardEffects _ce;
    private Animator _anims;

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
    }

    /// <summary>
    /// Calls FindBoardPieces.
    /// </summary>
    private void Start()
    {
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
                StartCoroutine(DamageBuiliding(_ce.CalculateBuildingDamage()));
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
    public IEnumerator DamageBuiliding(int damage)
    {
        bool hasCard = false;
        ActiveBuilding = true;

        //Weed Whacker and Dam Code
        if (SuitOfPiece == "Grass")
        {
            hasCard = _pcm.CheckForPersistentCard("Weed Whacker", false);
        }
        else if (SuitOfPiece == "Dirt")
        {
            hasCard = _pcm.CheckForPersistentCard("Dam", false);
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
                    _pcm.CheckForPersistentCard("Weed Whacker", true);
                }
                else if (SuitOfPiece == "Dirt")
                {
                    _pcm.CheckForPersistentCard("Dam", true);
                }
                _ce.CurrentDamages++;
                yield break;
            }

            ActiveBuilding = false;
        }
        //End Weed Whacker and Dam Code

        BuildingHealth -= damage;

        if(BuildingHealth <= 0)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s Building has been destroyed!");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);

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
                else if (BuildingType == "GMine")
                {
                    _am.P1BuiltBuildings[2]--;
                }
                else if (BuildingType == "DMine")
                {
                    _am.P1BuiltBuildings[3]--;
                }
                else if (BuildingType == "SMine")
                {
                    _am.P1BuiltBuildings[4]--;
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
                else if (BuildingType == "GMine")
                {
                    _am.P2BuiltBuildings[2]--;
                }
                else if (BuildingType == "DMine")
                {
                    _am.P2BuiltBuildings[3]--;
                }
                else if (BuildingType == "SMine")
                {
                    _am.P2BuiltBuildings[4]--;
                }
            }

            GetComponentInParent<PieceController>().HasP1Building = false;
            GetComponentInParent<PieceController>().HasP2Building = false;

            if (_pcm.CheckForPersistentCard("Retribution", true))
            {
                _pcm.StartCoroutine(_pcm.StartRetribution());
            }

            _am.ScorePoints(1);

            _anims.Play("TempPawnDefault");
            gameObject.SetActive(false);
        }
        else if(BuildingHealth == 1)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s Building has taken damage!");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
            GetComponent<SpriteRenderer>().color = _damagedColor;
            _anims.Play("TempPawnDefault");
        }
        else if(BuildingHealth == 2)
        {
            _gcm.UpdateCurrentActionText("Bad Luck! Player " + PlayerOwning + "'s Building has taken no damage!");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }

        _ce.CurrentDamages++;
        _pcm.BuildingsDamaged++;
        ActiveBuilding = false;
        CanBeDamaged = false;
    }

    /// <summary>
    /// Repairs a building.
    /// </summary>
    public void RepairBuilding()
    {
        BuildingHealth++;
        GetComponent<SpriteRenderer>().color = _defaultColor;
        _ce.RepairedBuildings++;
        _anims.Play("TempPawnDefault");
        CanBeRepaired = false;
        ActiveBuilding = true;

        //_am.ScorePoints cannot be used here since this specific interaction inverses point scoring.
        if(_am.CurrentPlayer == 1 && PlayerOwning == 2)
        {
            _am.P1Score++;
            _gcm.UpdateTextBothPlayers();
        }
        else if(_am.CurrentPlayer == 2 && PlayerOwning == 1)
        {
            _am.P2Score++;
            _gcm.UpdateTextBothPlayers();
        }

        CanBeRepaired = false;
        ActiveBuilding = false;
    }
}
