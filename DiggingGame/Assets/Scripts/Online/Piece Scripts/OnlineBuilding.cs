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
using Photon.Pun;

[System.Serializable]
public class OnlineBuilding : MonoBehaviourPun
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

    [Header("Animations")]
    [SerializeField] private float _removalAnimWaitTime;

    private List<GameObject> _boardPieces = new List<GameObject>();
    private OnlineActionManager _am;
    private OnlinePersistentCardManager _pcm;
    private OnlineCanvasManager _gcm;
    private OnlineCardEffects _ce;
    private Animator _anims;
    private OnlineBoardManager _bm;

    /// <summary>
    /// Assigns partner scripts.
    /// </summary>
    private void Awake()
    {
        _am = FindObjectOfType<OnlineActionManager>();
        _pcm = FindObjectOfType<OnlinePersistentCardManager>();
        _gcm = FindObjectOfType<OnlineCanvasManager>();
        _ce = FindObjectOfType<OnlineCardEffects>();
        _anims = GetComponent<Animator>();
        _bm = FindObjectOfType<OnlineBoardManager>();
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
    /// 
    /// Edited: Andrea SD - Modified for online use
    /// </summary>
    /// <param name="damage">The amount of damage (1 or 2, for now)</param>
    public IEnumerator DamageBuiliding(int damage)
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

        CallBuildingHP(-damage);

        if(BuildingHealth <= 0)
        {
            // Andrea SD
            if (BuildingType == "Factory")
            {
                CallRemoveBuilding(PlayerOwning, 0); 
            }
            else if (BuildingType == "Burrow")
            {
                CallRemoveBuilding(PlayerOwning, 1);
            }
            else if (BuildingType == "GMine")
            {
                CallRemoveBuilding(PlayerOwning, 2);
            }
            else if (BuildingType == "DMine")
            {
                CallRemoveBuilding(PlayerOwning, 3);
            }
            else if (BuildingType == "SMine")
            {
                CallRemoveBuilding(PlayerOwning, 4);
            }

            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has been destroyed!");
            CallPlayAnimation("TempPawnDamage");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);

            GetComponentInParent<PieceController>().HasP1Building = false;
            GetComponentInParent<PieceController>().HasP2Building = false;

            if (_pcm.CheckForPersistentCard(PlayerOwning, "Retribution"))
            {
                _pcm.DiscardPersistentCard(PlayerOwning, "Retribution");
                _pcm.RetributionStart(PlayerOwning, SuitOfPiece);
            }

            _am.CallUpdateScore(_am.CurrentPlayer, 1);

            CallPlayAnimation("TempPawnRemove");
            yield return new WaitForSeconds(_removalAnimWaitTime);
            PrepBuilidingDamaging(false);
            CallAnimStatus(false);
            CallGameObjStatus(false);
        }
        else if(BuildingHealth == 1)
        {
            CallChangeSprite("damaged");
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has taken damage!");
            CallPlayAnimation("TempPawnDamage");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }
        else if(BuildingHealth == 2)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has avoided damage!");
            CallPlayAnimation("TempPawnDamage");
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }

        _bm.SetActiveCollider("Building");
        _gcm.UpdateCurrentActionText("Damage " + _ce.AllowedDamages + " more Buildings!");
        _ce.CurrentDamages++;
        _pcm.BuildingsDamaged++;
        PrepBuilidingDamaging(false);
        ActiveBuilding = false;
    }

    /// <summary>
    /// Calls the RPC to destroy a building belonging to player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> which player the building belongs to (1, 2)
    /// </param>
    /// <param name="building"> the building bieng destroyed (0 = Factory, 
    /// 1 = burrow, 2 = GMine, 3 = DMine, 4 = SMine </param>
    public void CallRemoveBuilding(int player, int building)
    {
        photonView.RPC("RemoveBuilding", RpcTarget.All, player, building);
    }

    /// <summary>
    /// Destroys a building belonging to player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> which player the building belongs to (1, 2)
    /// </param>
    /// <param name="building"> the building bieng destroyed (0 = Factory, 
    /// 1 = burrow, 2 = GMine, 3 = DMine, 4 = SMine </param>
    [PunRPC]
    public void RemoveBuilding(int player, int building)
    {
        switch(player)
        {
            case 1:
                _am.P1BuiltBuildings[building]--;
                break;
            case 2:
                _am.P1BuiltBuildings[building]--;
                break;
        }
    }

    /// <summary>
    /// Calls the RPC to play an animation on each client
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="animName"> jname of the animation to play </param>
    public void CallPlayAnimation(string animName)
    {
        photonView.RPC("PlayAnimation", RpcTarget.All, animName);
    }
    /// <summary>
    /// Plays an animation on each client
    /// </summary>
    /// <param name="animName"> animation to be player </param>
    [PunRPC]
    public void PlayAnimation(string animName)
    {
        _anims.Play(animName);
    }

    /// <summary>
    /// Calls the RPC that changes the game object status
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="status"> true = active, false = inactive </param>
    public void CallGameObjStatus(bool status)
    {
        photonView.RPC("SetGameObjStatus", RpcTarget.All, status);
    }    
    /// <summary>
    /// Sets the game object to true or false
    /// </summary>
    /// <param name="status"> true = active, false = inactive </param>
    [PunRPC]
    public void SetGameObjStatus(bool status)
    {
        gameObject.SetActive(status);
    }

    /// <summary>
    /// Calls the RPC that modifies animation status to true or false
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="status"> true or false (enables or disables the 
    /// animator </param>
    public void CallAnimStatus(bool status)
    {
        photonView.RPC("SetAnimStatus", RpcTarget.All, status);
    }
    /// <summary>
    /// Sets the animator to true or false across clients
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="status"></param>
    [PunRPC]
    public void SetAnimStatus(bool status)
    {
        _anims.enabled = status;
    }

    /// <summary>
    /// Calls the RPC to modify the sprite
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="effect"> effect the sprite is being modified to </param>
    public void CallChangeSprite(string effect)
    {
        photonView.RPC("ChangeSprite", RpcTarget.All, effect);
    }
    /// <summary>
    /// Modifies the building sprite
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="effect"> effect the sprite is being modified to </param>
    [PunRPC]
    public void ChangeSprite(string effect)
    {
        switch (effect)
        {
            case "damaged":
                GetComponent<SpriteRenderer>().color = _damagedColor;
                break;
            case "default":
                GetComponent<SpriteRenderer>().color = _defaultColor;
                break;
        }
        
    }

    /// <summary>
    /// Repairs a building.
    /// </summary>
    public void RepairBuilding()
    {
        CallBuildingHP(1);
        CallChangeSprite("default");
        _ce.RepairedBuildings++;
        ActiveBuilding = true;

        //_am.ScorePoints cannot be used here since this specific interaction inverses point scoring.
        if(_am.CurrentPlayer == 1 && PlayerOwning == 2)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
            _gcm.UpdateTextBothPlayers();
        }
        else if(_am.CurrentPlayer == 2 && PlayerOwning == 1)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
            _gcm.UpdateTextBothPlayers();
        }

        ActiveBuilding = false;
        PrepBuilidingReapiring(false);
    }

    /// <summary>
    /// Calls the RPC that modifies building health across the network
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> how much the building HP is changed by </param>
    public void CallBuildingHP(int amount)
    {
        photonView.RPC("ModifyBuildingHP", RpcTarget.All, amount);
    }

    /// <summary>
    /// Modifies the building health
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> how much the building HP is changed by </param>
    [PunRPC]
    public void ModifyBuildingHP(int amount)
    {
        BuildingHealth += amount;
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
