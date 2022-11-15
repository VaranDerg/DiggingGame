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
using TMPro;

[System.Serializable]
public class OnlineBuilding : MonoBehaviourPun
{
    //Sprites and Dice Face values.
    [Header("References")]
    [SerializeField] private Sprite _damagedSprite;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private int _showDiceFaceTimes = 30;

    //Information regarding the Building.
    [Header("Values")]
    //Its health.
    public int BuildingHealth = 2;
    //Which player built it
    [HideInInspector] public int PlayerOwning = 0;
    //Its name (Factory, Grass Mine, Burrow, Etc)
    [HideInInspector] public string BuildingType = "";
    //States
    [HideInInspector] public bool CanBeDamaged;
    [HideInInspector] public bool CanBeRepaired;
    //How much damage the building will lose
    [HideInInspector] public int DamageTaken;
    //What suit its on
    [HideInInspector] public string SuitOfPiece;
    //Whether or not its participating in active selection
    [HideInInspector] public bool ActiveBuilding;

    //Buncha stuff for all of Caelie's wonderful animations
    [Header("Animations")]
    [SerializeField] private string _animFirstPartName;
    [SerializeField] private string _animSecondPartName;
    [SerializeField] private string _buildingSpawnName;
    [SerializeField] private string _buildingHideName;
    [SerializeField] private string _buildingClickName;
    [SerializeField] private string _buildingWaitingName;
    [SerializeField] private string _buildingDamagedName;
    [SerializeField] private string _buildingDamagedWaitingName;
    [SerializeField] private float _minAnimWaitTime, _maxAnimWaitTime;
    [SerializeField] private float _removalAnimWaitTime;
    [SerializeField] private GameObject _damageSmokePS;
    [SerializeField] private GameObject _damageClickPS;
    private GameObject _damageDice;
    private int _nextAnim = 0;
    private Coroutine _currentAnimCoroutine;

    [Header("Partner Scripts & Values")]
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
        _damageDice = GameObject.FindGameObjectWithTag("Damage Dice");
        FindBoardPieces();
        CallCurrentAnim();     
    }

    /// <summary>
    /// Plays idle animations in random intervals.
    /// 
    /// Edited: Andrea SD - modified for online use
    /// </summary>
    /// <returns></returns>
    private IEnumerator BuildingAnimations()
    {
        if (_nextAnim == 0)
        {
            _anims.Play(_buildingSpawnName);
            _nextAnim++;
        }
        else if (_nextAnim == 1)
        {
            _anims.Play(_animFirstPartName);
            _nextAnim++;
        }
        else if (_nextAnim == 2)
        {
            _anims.Play(_animSecondPartName);
            _nextAnim = 1;
        }

        yield return new WaitForSeconds(Random.Range(_minAnimWaitTime, _maxAnimWaitTime));

        CallCurrentAnim();      // ASD
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

    /// <summary>
    /// For building interaction.
    /// </summary>
    private void OnMouseOver()
    {
        if (CanBeDamaged && !ActiveBuilding)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _pcm.CurrentBuildingDamageProcess = StartCoroutine(DamageBuiliding());
            }
        }
        else if (CanBeDamaged && ActiveBuilding)
        {
            //This part is for my little investigator brain. You can ingnore it.
            Debug.LogWarning("This is the Invincible Mine bug! Building is still somwhow active.");
        }

        if (CanBeRepaired && !ActiveBuilding)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
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
    public IEnumerator DamageBuiliding()
    {
        //Sets colliders
        bool hasCard = false;
        ActiveBuilding = true;
        _bm.SetActiveCollider("Board");

        //Starts rolling the dice.
        _damageDice.GetComponent<Animator>().Play("DiceEnter");
        _gcm.UpdateCurrentActionText("Rolling Damage Dice...");

        //The real result is separate from the visual
        int damageDiceVisual = Random.Range(1, _ce.DamageDieSides + 1);

        //Weed Whacker & Dam
        if (SuitOfPiece == "Grass")
        {
            hasCard = _pcm.CheckForPersistentCard(PlayerOwning, "Weed Whacker");
        }
        else if (SuitOfPiece == "Dirt")
        {
            hasCard = _pcm.CheckForPersistentCard(PlayerOwning, "Dam");
        }

        //Forces a roll of 1 if the defensive card is in play.
        if (hasCard)
        {
            damageDiceVisual = 1;
            if (SuitOfPiece == "Grass")
            {
                _pcm.DiscardPersistentCard(PlayerOwning, "Weed Whacker");
            }
            else if (SuitOfPiece == "Dirt")
            {
                _pcm.DiscardPersistentCard(PlayerOwning, "Dam");
            }
        }
        //End Weed Whacker & Dam

        int num = 0;
        //Shows a face (1, 2, 3, 4) up to the variable's count.
        for (int i = 0; i <= _showDiceFaceTimes; i++)
        {
            if (num == _ce.DamageDieSides)
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

        //Calculates the damage, sets it to the dice, and subtracts that damage from the Building's health.
        int damage = _ce.CalculateBuildingDamage(damageDiceVisual);
        _damageDice.GetComponentInChildren<TextMeshProUGUI>().text = damageDiceVisual.ToString();

        BuildingHealth -= damage;

        //The text updates based on this given damage.
        if (damageDiceVisual == 4)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has taken massive damage!");
        }
        else if (damageDiceVisual == 3 || damageDiceVisual == 2)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " has taken damage!");
        }
        else if (damageDiceVisual == 1)
        {
            _gcm.UpdateCurrentActionText("Player " + PlayerOwning + "'s " + BuildingType + " avoided taking damage!");
        }

        //Fun!
        _damageClickPS.GetComponent<ParticleSystem>().Play();

        //Destroyed
        if (BuildingHealth <= 0)
        {
            //Removes the building from ActionManager's count
            if (PlayerOwning == 1)
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
                else if (BuildingType == "Gold Mine")
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

            StatManager.s_Instance.IncreaseStatistic(_am.CurrentPlayer, "Destroy", 1);

            //Lets the players sob for a bit
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);

            //Resets the Piece
            GetComponentInParent<OnlinePieceController>().HasP1Building = false;
            GetComponentInParent<OnlinePieceController>().HasP2Building = false;

            //Calls retribution here if need so
            if (_pcm.CheckForPersistentCard(PlayerOwning, "Retribution"))
            {
                _pcm.DiscardPersistentCard(PlayerOwning, "Retribution");
                _pcm.RetributionStart(PlayerOwning, SuitOfPiece);
            }

            _am.CallUpdateScore(_am.CurrentPlayer, 1);

            //Hides the building and disables it
            CallPlayAnimation(_buildingHideName);   // ASD
            yield return new WaitForSeconds(_removalAnimWaitTime);
            PrepBuilidingDamaging(false);
            CallAnimStatus(false);      // ASD
            CallGameObjStatus(false);       // ASD
        }
        else if (BuildingHealth == 1)
        {
            //Updates visuals.
            CallCoroutineStop();
            CallPlayAnimation(_buildingDamagedName);    // ASD
            CallChangeSprite(_damagedSprite.name);  // ASD
            CallDamagePS(true);     // ASD
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }
        else if (BuildingHealth == 2)
        {
            //Yippee!
            yield return new WaitForSeconds(_ce.BuildingDamageStatusWaitTime);
        }

        //Additional damages are generally only for Tornado or Earthquake.
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
    /// Prepares a building for taking damage.
    /// </summary>
    /// <param name="show">True = Blink</param>
    public void PrepBuilidingDamaging(bool show)
    {
        CallCoroutineStop();   // ASD

        if (show)
        {
            CanBeDamaged = true;
            if (BuildingHealth == 1)
            {
                _anims.Play(_buildingDamagedWaitingName);
            }
            else
            {
                _anims.Play(_buildingWaitingName);
            }
        }
        else
        {
            CanBeDamaged = false;
            if (BuildingHealth > 1)
            {
                CallCurrentAnim();  // ASD
            }
            else
            {
                CallPlayAnimation(_buildingDamagedName);    // ASD
            }
        }
    }

    /// <summary>
    /// Repairs a building.
    /// </summary>
    public void RepairBuilding()
    {
        //Clicking a building Repairs it.
        CallBuildingHP(1);     // ASD
        CallDamagePS();     // ASD
        CallCurrentAnim();  //ASD
        _ce.RepairedBuildings++;
        CallDamagePS(false);    // ASD
        ActiveBuilding = true;

        //_am.ScorePoints cannot be used here since this specific interaction inverses point scoring.
        if (_am.CurrentPlayer == 1 && PlayerOwning == 2)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
            _gcm.UpdateTextBothPlayers();
        }
        else if (_am.CurrentPlayer == 2 && PlayerOwning == 1)
        {
            _am.CallUpdateScore(_am.CurrentPlayer, 1);
            _gcm.UpdateTextBothPlayers();
        }

        ActiveBuilding = false;
        PrepBuildingRepairing(false);
    }

    /// <summary>
    /// Prepares a building for repairing.
    /// </summary>
    /// <param name="show">True = Blink</param>
    public void PrepBuildingRepairing(bool show)
    {
        StopCoroutine(_currentAnimCoroutine);

        if (show)
        {
            CanBeRepaired = true;
            if (BuildingHealth == 1)
            {
                _anims.Play(_buildingDamagedWaitingName);
            }
            else
            {
                _anims.Play(_buildingWaitingName);
            }
        }
        else
        {
            CanBeRepaired = false;
            if (BuildingHealth > 1)
            {
                _currentAnimCoroutine = StartCoroutine(BuildingAnimations());
            }
            else
            {
                _anims.Play(_buildingDamagedName);
            }
        }
    }

    #region RPCS

    /// <summary>
    /// Calls the RPC that plays the damage particle system
    /// 
    /// Author: Andrea SD
    /// </summary>
    public void CallDamagePS()
    {
        photonView.RPC("PlayDMGParticle", RpcTarget.All);
    }

    /// <summary>
    /// Plays the Damage particle system
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void PlayDMGParticle()
    {
        _damageClickPS.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Calls the RPC that controls whether the damage particle system is
    /// active or inactive
    /// </summary>
    /// <param name="active"> on or off</param>
    public void CallDamagePS(bool active)
    {
        photonView.RPC("DamagePSActivity", RpcTarget.All, active);
    }

    /// <summary>
    /// Sets the damage smoke particle system to active or inactive
    /// </summary>
    /// <param name="active"> on or off</param>
    [PunRPC]
    public void DamagePSActivity(bool active)
    {
        _damageSmokePS.SetActive(active);
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
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="animName"> animation to be player </param>
    [PunRPC]
    public void PlayAnimation(string animName)
    {
        _anims.Play(animName);
    }

    /// <summary>
    /// Calls the RPC that modifies the _nextAnim variable
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"></param>
    /*public void CallNextAnimModify(int amount)
    {
        photonView.RPC("ModifyNextAnim", RpcTarget.All, amount);
    }*/

    /// <summary>
    /// Modifies the _nextAnim variable
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="amount"> amount _nextAnim is modified by </param>
    [PunRPC]
    /*public void ModifyNextAnim(int amount)
    {
        _nextAnim = amount;
    }*/

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
    public void ChangeSprite(string newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(newSprite);
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
    /// Calls the RPC that stops a coroutine online
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="coroutineStop"> coroutine to be stopped </param>
    public void CallCoroutineStop()
    {
        photonView.RPC("StopCoroutineONL", RpcTarget.All);
    }

    /// <summary>
    /// Stops the coroutine across the network
    /// 
    /// Author: Andrea SD
    /// </summary>
    [PunRPC]
    public void StopCoroutineONL()
    {
        StopCoroutine(_currentAnimCoroutine);
    }

    /// <summary>
    /// Calls the RPC to set the current animation coroutine
    /// </summary>
    /// <param name="animCoroutine"> starting animation coroutine </param>
    public void CallCurrentAnim()
    {
        photonView.RPC("SetCurrentAnimCoroutine", RpcTarget.All);
    }

    /// <summary>
    /// Starts the current animation coroutine
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="animCoroutine"> starting animation coroutine </param>
    [PunRPC]
    public void SetCurrentAnimCoroutine()
    {
        if (_currentAnimCoroutine != null)
        {
            StopCoroutine(_currentAnimCoroutine);
        }
        _currentAnimCoroutine = StartCoroutine(BuildingAnimations());
    }

    /// <summary>
    /// Calls the RPC that sets SuitOfPiece to suit
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="suit"> suit of the piece </param>
    public void CallSetSuit(string suit)
    {
        photonView.RPC("SetPieceSuit", RpcTarget.All, suit);
    }  
    
    /// <summary>
    /// Sets SuitOfPiece to suit
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="suit"> suit of the piece </param>

    [PunRPC]
    public void SetPieceSuit(string suit)
    {
        SuitOfPiece = suit;
    }

    /// <summary>
    /// Calls the RPC that sets PlayerOwning to player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"></param>
    public void CallPlayerOwning(int player)
    {
        photonView.RPC("SetPlayerOwning", RpcTarget.All, player);
    }

    /// <summary>
    /// Sets PlayerOwning to player
    /// 
    /// Author: Andrea SD
    /// </summary>
    /// <param name="player"> 1 or 2 </param>
    [PunRPC]
    public void SetPlayerOwning(int player)
    {
        PlayerOwning = player;
    }

    #endregion
}
