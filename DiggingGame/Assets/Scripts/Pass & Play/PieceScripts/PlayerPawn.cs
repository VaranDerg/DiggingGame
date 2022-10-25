/*****************************************************************************
// File Name :         PlayerPawn.cs
// Author :            Rudy Wolfer
// Creation Date :     October 6th, 2022
//
// Brief Description : Script that controls Players' Pawn pieces.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [Header("References/Values")]
    //1 or 2
    [Range(1, 2)] public int PawnPlayer;
    [SerializeField] private Sprite _moleSprite;
    [SerializeField] private Sprite _meerkatSprite;

    [Header("Other")]
    //The (up to) 4 Board Pieces surrounding a player. NSEW.
    private List<GameObject> _shownPieces = new List<GameObject>();
    private List<GameObject> _boardPieces = new List<GameObject>();
    private BoardManager _bm;
    private ActionManager _am;
    private PersistentCardManager _pcm;
    private GameCanvasManagerNew _gcm;
    private Animator _anims;
    private CardEffects _ce;
    [SerializeField] private SpriteRenderer _sr;

    [Header("Pawn Status for Other Scripts")]
    [HideInInspector] public bool IsMoving = false, IsBuilding = false, IsDigging = false, IsPlacing;
    [HideInInspector] public string BuildingToBuild = "";

    [Header("Card Effect Things")]
    [HideInInspector] public bool SecretTunnelsMove;
    [HideInInspector] public bool MudslideMove;
    [HideInInspector] public bool WalkwaySelect;
    [HideInInspector] public bool TeleportationMove;

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
    /// Adjusts the Pawn's values to fit a player.
    /// </summary>
    /// <param name="player">1 or 2</param>
    public void SetPawnToPlayer(int player)
    {
        if (player == 1)
        {
            _sr.sprite = _moleSprite;
            PawnPlayer = 1;
        }
        else
        {
            _sr.sprite = _meerkatSprite;
            PawnPlayer = 2;
        }
    }

    /// <summary>
    /// Assigns partner scripts and components.
    /// </summary>
    private void Awake()
    {
        _bm = FindObjectOfType<BoardManager>();
        _am = FindObjectOfType<ActionManager>();
        _gcm = FindObjectOfType<GameCanvasManagerNew>();
        _pcm = FindObjectOfType<PersistentCardManager>();
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
    /// Update, but for the mouse!
    /// </summary>
    private void OnMouseOver()
    {
        if (IsMoving)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(PreparePawnMovement());
            }
        }

        if(IsBuilding)
        {
            PreparePawnBuilding();
        }

        if(IsDigging)
        {
            PreparePawnDigging();
        }

        if(MudslideMove)
        {
            PrepareMudslide();
        }
    }

    /// <summary>
    /// Preps pawn for moving. Only selects Grass Pieces if moving with morning jog.
    /// </summary>
    private IEnumerator PreparePawnMovement()
    {
        //Start of Secret Tunnels code
        bool hasSecretTunnels = false;
        if(_am.CurrentPlayer == 1)
        {
            foreach(GameObject card in _pcm.P1PersistentCards)
            {
                if(card.gameObject.name == "Secret Tunnels")
                {
                    hasSecretTunnels = true;
                }
            }
        }
        else
        {
            foreach (GameObject card in _pcm.P2PersistentCards)
            {
                if (card.gameObject.name == "Secret Tunnels")
                {
                    hasSecretTunnels = true;
                }
            }
        }

        if(hasSecretTunnels)
        {
            _ce.SecretTunnelsUI.SetActive(true);

            while(!_pcm.DecidedSecretTunnelsMove)
            {
                yield return null;
            }

            _ce.SecretTunnelsUI.SetActive(false);

            _pcm.DecidedSecretTunnelsMove = false;
            SecretTunnelsMove = _pcm.SecretTunnelsMoveDecision;
        }
        //End of Secret Tunnels code

        if(!SecretTunnelsMove && !TeleportationMove)
        {
            foreach (GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                if(piece.GetComponent<PieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideMovable(true);
                _shownPieces.Add(piece);
            }

            if(_shownPieces.Count > 0)
            {
                foreach(GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }
        }
        else if(SecretTunnelsMove)
        {
            foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if(piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Two)
                {
                    continue;
                }

                if(piece.GetComponent<PieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideMovable(true);
                _shownPieces.Add(piece);
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }

            SecretTunnelsMove = false;
        }
        else if(TeleportationMove)
        {
            foreach(GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
            {
                if(piece.GetComponent<PieceController>().HasPawn)
                {
                    continue;
                }

                piece.GetComponent<PieceController>().ShowHideMovable(true);
                _shownPieces.Add(piece);
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }

            TeleportationMove = false;
        }

        _bm.BoardColliderSwitch(true);
    }

    /// <summary>
    /// Preps pawn for digging.
    /// </summary>
    private void PreparePawnDigging()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach(GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                if(piece.GetComponent<PieceController>().HasPawn || piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    continue;
                }

                if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
                {
                    continue;
                }

                if(WalkwaySelect)
                {
                    if(piece.GetComponent<PieceController>().ObjState != PieceController.GameState.One || piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Six)
                    {
                        continue;
                    }
                }

                piece.GetComponent<PieceController>().ShowHideDiggable(true);
                if(WalkwaySelect)
                {
                    piece.GetComponent<PieceController>().WalkwayDig = true;
                    WalkwaySelect = false;
                }
                _shownPieces.Add(piece);
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }
            else
            {
                _gcm.UpdateCurrentActionText("No valid digging locations at this pawn.");
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
            }

            _bm.BoardColliderSwitch(true);
        }
    }

    /// <summary>
    /// Preps pawn for building.
    /// </summary>
    private void PreparePawnBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (GameObject piece in _bm.GenerateAdjacentPieceList(ClosestPieceToPawn()))
            {
                bool dontHighlight = false;
                if (piece.GetComponent<PieceController>().HasP1Building || piece.GetComponent<PieceController>().HasP2Building)
                {
                    dontHighlight = true;
                }

                if (piece.GetComponent<PieceController>().HasPawn)
                {
                    dontHighlight = true;
                }

                if(piece.GetComponent<PieceController>().ObjState == PieceController.GameState.Four)
                {
                    dontHighlight = true;
                }

                foreach (GameObject pieceSquared in _bm.GenerateAdjacentPieceList(piece))
                {
                    if (pieceSquared.GetComponent<PieceController>().HasP1Building || pieceSquared.GetComponent<PieceController>().HasP2Building)
                    {
                        dontHighlight = true;
                    }
                }

                if (!dontHighlight)
                {
                    piece.GetComponent<PieceController>().ShowHideBuildable(true);
                    _shownPieces.Add(piece);
                }
            }

            if (_shownPieces.Count > 0)
            {
                foreach (GameObject piece in _shownPieces)
                {
                    piece.GetComponent<PieceController>().CurrentPawn = gameObject;
                }
            }
            else
            {
                _gcm.UpdateCurrentActionText("No valid building locations at this pawn.");
                _bm.DisableAllBoardInteractions();
                _gcm.Back();
            }

            _bm.BoardColliderSwitch(true);
        }
    }

    /// <summary>
    /// Preps pawn for moving with the card Mudslide.
    /// </summary>
    private void PrepareMudslide()
    {
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("BoardPiece"))
        {
            if (piece.GetComponent<PieceController>().HasPawn)
            {
                continue;
            }

            if(piece.GetComponent<PieceController>().ObjState != PieceController.GameState.Two)
            {
                continue;
            }

            piece.GetComponent<PieceController>().ShowHideMovable(true);
            _shownPieces.Add(piece);
        }

        foreach(GameObject pawn in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if(pawn == this)
            {
                continue;
            }

            pawn.GetComponent<Animator>().Play("TempPawnDefault");
        }

        if (_shownPieces.Count > 0)
        {
            foreach (GameObject piece in _shownPieces)
            {
                piece.GetComponent<PieceController>().CurrentPawn = gameObject;
            }
        }

        MudslideMove = false;
        _bm.BoardColliderSwitch(true);
    }

    /// <summary>
    /// Finds the closest piece to the pawn.
    /// </summary>
    /// <returns>GameObject "Piece" that's closest to the current Pawn.</returns>
    public GameObject ClosestPieceToPawn()
    {
        GameObject closestPiece = null;
        float curShortestDist = Mathf.Infinity;
        Vector3 pawnPosition = transform.position;
        foreach(GameObject piece in _boardPieces)
        {
            float pawnToPieceDist = Vector3.Distance(piece.transform.position, pawnPosition);
            if(pawnToPieceDist < curShortestDist)
            {
                closestPiece = piece;
                curShortestDist = pawnToPieceDist;
            }
        }

        return closestPiece;
    }

    /// <summary>
    /// Sets currently adjacent tiles as no longer adjacent.
    /// </summary>
    public void UnassignAdjacentTiles()
    {
        for(int i = 0; i < _shownPieces.Count; i++)
        {
            if(_shownPieces[i] != null)
            {
                _shownPieces[i].GetComponent<PieceController>().ShowHideMovable(false);
                _shownPieces[i].GetComponent<PieceController>().ShowHideBuildable(false);
                _shownPieces[i].GetComponent<PieceController>().ShowHidePlaceable(false);
                _shownPieces[i].GetComponent<PieceController>().ShowHideDiggable(false);
                _shownPieces[i].GetComponent<PieceController>().PieceIsSelected = false;
            }
        }

        IsMoving = false;
        IsBuilding = false;
        IsDigging = false;
        IsPlacing = false;
        WalkwaySelect = false;
        SecretTunnelsMove = false;
        MudslideMove = false;
        BuildingToBuild = "";
        _anims.Play("TempPawnDefault");
        _shownPieces.Clear();
    }

    /// <summary>
    /// Hides tiles that aren't waiting
    /// </summary>
    public void HideNonSelectedTiles()
    {
        foreach(GameObject piece in _shownPieces)
        {
            if(!piece.GetComponent<PieceController>().PieceIsSelected)
            {
                piece.GetComponent<PieceController>().ShowHideMovable(false);
                piece.GetComponent<PieceController>().ShowHideBuildable(false);
                piece.GetComponent<PieceController>().ShowHidePlaceable(false);
                piece.GetComponent<PieceController>().ShowHideDiggable(false);
            }
        }
    }
}
