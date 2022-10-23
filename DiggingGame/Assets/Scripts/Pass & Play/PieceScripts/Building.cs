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

    public int BuildingHealth = 2;
    [HideInInspector] public int PlayerOwning = 0;
    [HideInInspector] public string BuildingType = "";
    [HideInInspector] public bool CanBeDamaged;
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
        if(CanBeDamaged)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                FindObjectOfType<CardEffects>().SelectedBuilding = this;
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
        string cardName = "";
        ActiveBuilding = true;

        if(_am.CurrentPlayer == 1)
        {
            foreach(GameObject card in _pcm.P1PersistentCards)
            {
                if(card.gameObject.name == "Weed Whacker" && SuitOfPiece == "Grass")
                {
                    hasCard = true;
                    cardName = "Weed Whacker";
                }
                else if(card.gameObject.name == "Dam" && SuitOfPiece == "Dirt")
                {
                    hasCard = true;
                    cardName = "Dam";
                }
            }
        }
        else
        {
            foreach (GameObject card in _pcm.P2PersistentCards)
            {
                if (card.gameObject.name == "Weed Whacker" && SuitOfPiece == "Grass")
                {
                    hasCard = true;
                    cardName = "Weed Whacker";
                }
                else if (card.gameObject.name == "Dam" && SuitOfPiece == "Dirt")
                {
                    hasCard = true;
                    cardName = "Dam";
                }
            }
        }

        if(hasCard)
        {
            _ce.ProtectBuildingUI.SetActive(true);

            if(PlayerOwning == 1)
            {
                _gcm.UpdateCurrentActionText("Player 1, protect your building with " + cardName + "?");
            }
            else
            {
                _gcm.UpdateCurrentActionText("Player 2, protect your building with " + cardName + "?");
            }

            while(!_pcm.DecidedBuildingProtection)
            {
                yield return null;
            }

            _ce.ProtectBuildingUI.SetActive(false);

            _pcm.DecidedBuildingProtection = false;

            if(DamageProtectionResponse == true)
            {
                if(_am.CurrentPlayer == 1)
                {
                    foreach (GameObject card in _pcm.P1PersistentCards)
                    {
                        if (card.gameObject.name == cardName)
                        {
                            card.GetComponent<CardController>().ToDiscard();
                        }
                    }
                }
                else
                {
                    foreach (GameObject card in _pcm.P2PersistentCards)
                    {
                        if (card.gameObject.name == cardName)
                        {
                            card.GetComponent<CardController>().ToDiscard();
                        }
                    }
                }
                yield break;
            }

            ActiveBuilding = false;
        }

        BuildingHealth -= damage;

        if(BuildingHealth <= 0)
        {
            Debug.Log("Player " + PlayerOwning + "'s " + BuildingType + " has been destroyed!");

            if(_am.CurrentPlayer == 1)
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

            _anims.Play("TempPawnDefault");
            Destroy(gameObject);
        }
        else if(BuildingHealth == 1)
        {
            GetComponent<SpriteRenderer>().color = _damagedColor;
            _anims.Play("TempPawnDefault");
        }

        ActiveBuilding = false;
    }
}
