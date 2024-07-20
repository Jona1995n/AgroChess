using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using ChessEngine.Game.Events;

namespace ChessEngine.Game
{
    /// <summary>A structure that represents a selected tile and all information about it.</summary>
    public struct Selection
    {
        /// <summary>The VisualChessPiece related to the selection.</summary>
        public VisualChessPiece visualPiece;
        /// <summary>The VisualChessTableTile related to the selection.</summary>
        public VisualChessTableTile visualTile;
        /// <summary>A list of valid ChessTableTiles that the selection may be moved to. Use ChessGameManager.GetVisualTile(ChessTableTile) to get the visual tile reference.</summary>
        public List<ChessTableTile> validMoves;
        /// <summary>A list of valid AttackInfos that the selection may attack to. Use ChessGameManager.GetVisualTile(ChessTableTile) to get the visual tile reference.</summary>
        public List<AttackInfo> validAttacks;
    }

    /// <summary>
    /// The GameManager component manages everything related to gameplay like turns, piece selection, piece movement, win loss detection, etc.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ChessGameManager : MonoBehaviour
    {
        #region Editor Serialized Settings & Events
        [Header("Settings")]
        [Tooltip("Should ChessGameManager.NewGame() be invoked automatically on Start()?")]
        public bool autoStartGame = true;
        [Tooltip("Used to Switch to Premove System")]
        public bool PremoveSystemActive = false;
        [Tooltip("The tile Premove has to go to")]
        public VisualChessTableTile PremoveToTile;
        [Tooltip("The tile Premove Piece was in")]
        public VisualChessTableTile PremoveTile;
        [Tooltip("A reference to a VisualChessTable component that will be used to visualize the 'ChessInstance' associated with this chess game manager.")]
        public VisualChessTable visualTable;
        [Tooltip("An editor-set reference to the selected tile material.")]
        [SerializeField] Material m_SelectedHighlightMaterial = null;
        [Tooltip("An editor-set reference to the selected PreMove tile material.")]
        [SerializeField] Material m_PreMoveHighlightMaterial = null;
        [Tooltip("An editor-set reference to the valid-move tile material.")]
        [SerializeField] Material m_MoveHighlightMaterial = null;
        [Tooltip("An editor-set reference to the valid-attack tile material.")]
        [SerializeField] Material m_AttackHighlightMaterial = null;

        [Header("Events")]
        [Tooltip("An event that is invoked when a game is initialized. This is invoked whenever a new ChessInstance is instantiated by the game manager.")]
        public UnityEvent GameInitialized;
        [Tooltip("An event that is invoked when a turn is ended.\n\nArg0: ChessColor - The color whose turn was ended.\nArg1: MoveInfo - The MoveInfo from the turn that was ended. ")]
        public EndTurnUnityEvent TurnEnded;
        [Tooltip("An event that is invoked when a turn is started.\n\nArg0: ChessColor - The team whose turn was started.")]
        public TeamUnityEvent TurnStarted;
        [Tooltip("An event that is invoked before the ChessGameManager is reset.")]
        public UnityEvent PreGameReset;
        [Tooltip("An event that is invoked after the ChessGamemManager is reset.")]
        public UnityEvent PostGameReset;
        [Tooltip("An event that is invoked when the game is finished.\n\nArg0: ChessColor - The color whose turn it was when the game ended.\nArg1: GameOverReason - The reason the game ended.")]
        public GameOverUnityEvent GameOver;
        [Tooltip("An event that is invoked when the ChessGameManager's 'Selected' Selection chnages.")]
        public UnityEvent SelectionChanged;
        [Tooltip("An event that is invoked when a chess piece is moved by this game manager.\n\nArg0: ChessPiece - The chess piece that was moved.\nArg1: MoveInfo - Information about the move.")]
        public MoveUnityEvent ChessPieceMoved;
        #endregion
        #region Public Properties
        /// <summary>A reference to the ChessEngine.Instance that is simulating the chess game.</summary>
        public Instance ChessInstance { get; private set; }
        /// <summary>
        /// Returns a Selection instance that contains information about the current selection, otherwise null if no piece is selected.
        /// SIDE EFFECT: Can change visual tiles material to/from highlighted.
        /// </summary>
        public Selection Selected
        {
            get { return m_Selected; }
            set
            {
                // Dehighlight previous selection.//Added Another condition to make sure its not a premove
                
                if (m_Selected.visualTile != null)
                    m_Selected.visualTile.ResetMaterial();

                // Update selected value.
                m_Selected = value;

                // Highligh current selection.
                if (value.visualTile != null)
                    value.visualTile.Renderer.material = m_SelectedHighlightMaterial;

                // Call the OnSelectionChanged() callback.
                OnSelectionChanged();
            }
        }
        /// <summary>The number of turns played by the current game in this game manager. Incremented immediately at the end of each turn by ChessGameManager.OnTurnEnded.</summary>
        public int TurnCount { get; set; } = 0;
        #endregion
        #region Protected Fields
        /// <summary>A List of highlighted VisualChessTableTiles.</summary>
        protected List<VisualChessTableTile> m_HighlightedTiles = new List<VisualChessTableTile>();

        // Hidden backing field(s).
        /// <summary>The hidden backing field for the 'Selected' property.</summary>
        protected Selection m_Selected;
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        protected virtual void Start()
        {
            // If set to automatically start a new game then start one.
            if (autoStartGame)
                NewGame();
        }

        protected virtual void OnEnable()
        {
            // Subscribe to chess instance events.
            if (ChessInstance != null)
                SubscribeToChessInstanceEvents();
        }

        protected virtual void OnDisable()
        {
            // Unsubscribe from chess instance events.
            if (ChessInstance != null)
                UnsubscribeFromChessInstanceEvents();
        }
        #endregion

        // Public method(s).
        #region Game State Management (Restarting, etc...)
        /// <summary>Restarts the chess game. Resets the existing chess instance, or starts a new game if there is no existing chess instance.</summary>
        public void ResetGame()
        {
            // If there is an existing chess instance reset it.
            if (ChessInstance != null)
            {
                // Reset the chess engine to a new game.
                ChessInstance.ResetGame();
            }
            // Otherwise start a new game.
            else { NewGame(); }

            // Turn count is 0.
            TurnCount = 0;
        }

        /// <summary>Invokes ChessInstance.StartTurn(). Note that this is only neccesary after a ResetGame() not a NewGame().</summary>
        public void StartTurn()
        {
            ChessInstance.StartTurn();
        }

        /// <summary>
        /// Creates a new game. Unsubscribes from ChessInstance events, initializes visual chess table, and then resubscribes to new ChessInstance events.
        /// Resets the game and starts the first turn.
        /// </summary>
        public void NewGame()
        {
            // Unsubscribe from previous 'ChessInstance' events.
            if (ChessInstance != null)
                UnsubscribeFromChessInstanceEvents();

            // Instantiate a new chess instance.
            ChessInstance = new Instance();

            // Initialize the visual chess table.
            visualTable.Initialize(ChessInstance.Table);

            // Assign game manager reference to VisualChessTableTiles.
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    visualTable.VisualTiles[x][y].GameManager = this;
                }
            }

            // Subscribe to chess instance events.
            SubscribeToChessInstanceEvents();

            // Invoke the 'OnGameInitialized' callback.
            OnGameInitialized();

            // Reset the chess game.
            ChessInstance.ResetGame();

            // Turn count is 0.
            TurnCount = 0;

            // Start the first turn.
            ChessInstance.StartTurn();
        }

        /// <summary>Loads a game from a FEN string.</summary>
        /// <param name="pFEN">The FEN string to load the game from.</param>
        public void LoadGameFromFEN(string pFEN)
        {
            // Unsubscribe from previous 'ChessInstance' events.
            if (ChessInstance != null)
                UnsubscribeFromChessInstanceEvents();

            try
            {
                // Instantiate a new chess instance.
                ChessInstance = new Instance(pFEN);

                // Initialize the visual chess table.
                visualTable.Initialize(ChessInstance.Table);

                // Assign game manager reference to VisualChessTableTiles.
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        visualTable.VisualTiles[x][y].GameManager = this;
                    }
                }

                // Subscribe to chess instance events.
                SubscribeToChessInstanceEvents();

                // Invoke the 'OnGameInitialized' callback.
                OnGameInitialized();

                // Turn count starts at 0.
                TurnCount = 0;

                // Start the chess game by forcefully starting the first turn.
                ChessInstance.StartTurn();
            }
            catch (Exception pException) { Debug.LogWarning("Failed to 'LoadGameFromFen(string)' using FEN string '" + pFEN + "'! [Exception: '" + pException.ToString() + "']", gameObject); }        
        }
        #endregion
        #region Piece & Tile Selection
        /// <summary>Selects the selected chess piece and tile for this game manager.</summary>
        /// <param name="pPiece"></param>
        /// <param name="pTile"></param>
        public void SelectPiece(VisualChessPiece pPiece, VisualChessTableTile pTile)
        {
            Selected = new Selection { visualTile = pTile, visualPiece = pPiece };
        }

        /// <summary>Nullifies the selection of this game manager.</summary>
        public void Deselect() { Selected = new Selection { visualTile = null, visualPiece = null }; }

        /// <summary>Handles the selection of a tile (active selection leads to move/attack, no active selection selects the tile & piece on the tile)..</summary>
        public void SelectTile(VisualChessTableTile pVisualTile)
        {
            // Handle tile selection when no tile is selected, or when we're trying to select another tile occupied by a friendly piece.
            // Get the visual piece on this tile.
            VisualChessPiece visualPiece = pVisualTile.GetVisualPiece();
            if (PremoveSystemActive)
            {
                goto PremoveSystemActive;
            }
            // If visualPiece is not null and the selected 'visual piece' is equal to 'visualPiece' then deselect the tile.
            if (visualPiece != null && Selected.visualPiece == visualPiece)
            {
                Deselect();
            }
            // Otherwise if there is a piece on the selected slot and it is this game manager's turn...
            else if (visualPiece != null && visualPiece.Piece.Color == ChessInstance.turn)
            {
                // Ensure the piece may be selected.
                if (CanSelectPiece(visualPiece, pVisualTile))
                    SelectPiece(visualPiece, pVisualTile); PremoveSystemActive = false;
            }
            // Otherwise if a piece is already selected move it to the selected  tile if possible.
            else if (Selected.visualPiece != null)
            {
                // If we have a valid, occupied tile selected handle movement of the piece on it.
                if (Selected.validMoves.Contains(pVisualTile.Tile) || ChessTableTile.IsTileAttackable(Selected.validAttacks, pVisualTile.Tile))
                {
                    // Don't allow us to kill a king.
                    if (visualPiece == null || !visualPiece.IsPiece<King>())
                    {
                        // Move the piece.
                        MoveInfo moveInfo = Selected.visualPiece.Piece.Move(pVisualTile.Tile.TileIndex, visualPiece != null ? visualPiece.Piece : null);
                        // Reset selection.
                        Deselect();
                        // End the turn.
                        ChessInstance.EndTurn(moveInfo);
                    }
                    
                }
            }
            else 
            {
                PremoveTile = pVisualTile;              
                SelectPiece(visualPiece, pVisualTile);
                PremoveSystemActive = true;
            }
            return;
        PremoveSystemActive:
            {
                PremoveToTile = pVisualTile;//tile saving logic
                PremoveSystemActive = false;
                Deselect();
                pVisualTile.Renderer.material = m_PreMoveHighlightMaterial;
                return;
            }
        }
        public void EndGame(ChessColor TimerRanOut)
        {
            OnGameOver(TimerRanOut,GameOverReason.TimeExpired);
        }
        // Public virtual method(s).
        /// <summary>A method that returns a boolean, true if the ChessPiece pPiece may be selected by this game manager, otherwise false.</summary>
        /// <param name="pPiece">The ChessPiece on the tile being selected.</param>
        /// <param name="pTile">The ChessTableTile being selected.</param>
        /// <returns>true if the ChessPiece pPiece may be selected by this game manager, otherwise false.</returns>
        public virtual bool CanSelectPiece(VisualChessPiece pPiece, VisualChessTableTile pTile) { return true; }
        #endregion

        // Protected virtual callback(s).
        #region Protected Virtual Callbacks
        /// <summary>Called whenever the chess game manager initializes a new ChessInstance (via NewGame, LoadGame, etc...) </summary>
        protected virtual void OnGameInitialized()
        {
            // Invoke the game initialized Unity event.
            GameInitialized?.Invoke();
        }

        /// <summary>Called when the game ends, the winner is whoever's turn it currently is.</summary>
        /// <param name="pTeam">The ChessColor of the team whose turn it was when the game ended.</param>
        /// <param name="pReason">The GameOverReason for the game ending.</param>
        protected virtual void OnGameOver(ChessColor pTeam, GameOverReason pReason)
        {
            // Invoke the game over Unity event.
            GameOver?.Invoke(pTeam, pReason);
        }

        /// <summary>Invoked when a turn is started.</summary>
        /// <param name="pTurn">The color whose turn was started.</param>
        protected virtual void OnTurnStarted(ChessColor pTurn)
        {
            // Invoke the turn started Unity event.
            TurnStarted?.Invoke(pTurn);

            StartCoroutine(ExecutionOfPreMoves(pTurn));
        }
       
        IEnumerator ExecutionOfPreMoves(ChessColor pTurn)
        {
            yield return  new WaitForSeconds(0.2f);
            if (pTurn == ChessColor.White)
            {
                Debug.Log("Checking for Premoves");
                if (PremoveTile != null)
                {
                    Debug.Log("Premove is Valid");

                    //SelectPiece(PremoveChessPiece,PremoveTile);
                    VisualChessTableTile pVisualTile = PremoveToTile;

                    VisualChessPiece visualPiece = PremoveTile.GetVisualPiece();

                    SelectPiece(visualPiece, PremoveTile);

                    visualPiece = PremoveToTile.GetVisualPiece();

                    if (visualPiece != null && Selected.visualPiece == visualPiece)
                    {
                        Deselect();
                    }
                    // Otherwise if there is a piece on the selected slot and it is this game manager's turn...
                    else if (visualPiece != null && visualPiece.Piece.Color == ChessInstance.turn)
                    {
                        // Ensure the piece may be selected.
                        if (CanSelectPiece(visualPiece, pVisualTile))
                            SelectPiece(visualPiece, pVisualTile); PremoveSystemActive = false;
                    }
                    // Otherwise if a piece is already selected move it to the selected  tile if possible.
                    else if (Selected.visualPiece != null)
                    {
                        // If we have a valid, occupied tile selected handle movement of the piece on it.
                        if (Selected.validMoves.Contains(pVisualTile.Tile) || ChessTableTile.IsTileAttackable(Selected.validAttacks, pVisualTile.Tile))
                        {
                            // Don't allow us to kill a king.
                            if (visualPiece == null || !visualPiece.IsPiece<King>())
                            {
                                // Move the piece.
                                MoveInfo moveInfo = Selected.visualPiece.Piece.Move(pVisualTile.Tile.TileIndex, visualPiece != null ? visualPiece.Piece : null);
                                // Reset selection.
                                Deselect();
                                // End the turn.
                                ChessInstance.EndTurn(moveInfo);

                                //Making Vars Null
                                PremoveToTile = null;
                                PremoveTile = null;
                            }

                        }
                    }
                }
                else
                {
                    Debug.Log("Premove is Invalid");
                }

            }
        }


        /// <summary>Invoked when a turn is ended.</summary>
        /// <param name="pLastTurn">The ChessColor whose turn ended.</param>
        /// <param name="pMove">The move the turn ended on.</param>
        protected virtual void OnTurnEnded(ChessColor pLastTurn, MoveInfo pMove)
        {
            // Increment turn count.
            ++TurnCount;

            // Invoke the 'TurnEnded' Unity event.
            TurnEnded?.Invoke(pLastTurn, pMove);
        }

        /// <summary>Invoked just before the chess game is reset.</summary>
        protected virtual void OnPreGameReset()
        {
            // Invoke the 'PreGameReset' Unity event.
            PreGameReset?.Invoke();
        }

        /// <summary>Invoked after the chess game has been reset.</summary>
        protected virtual void OnPostGameReset()
        {
            // Invoke the 'PostGameReset' Unity event.
            PostGameReset?.Invoke();
        }

        /// <summary>Invoked whenever a chess piece is moved.</summary>
        /// <param name="pMoveInfo">The MoveInfo that describes the move.</param>
        protected virtual void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            // Invoke the 'ChessPieceMoved' Unity event.
            ChessPieceMoved?.Invoke(visualTable.GetVisualPiece(pMoveInfo.piece), pMoveInfo);
        }

        /// <summary>Invoked whenver the chess engines 'IsGameOverEventCallback' has been invoked.</summary>
        /// <param name="pInstance">The chess Instance that is testing if the game is over.</param>
        /// <param name="pIsGameOver"></param>
        protected virtual void IsGameOverEventCallback(Instance pInstance, ref bool pIsGameOver)
        {
            // NOTE: The game can be ended for GameOverReason.Unknown by simply setting 'pIsGameOver = true;' in this function body.
        }

        /// <summary>Called whenever the GameManager's 'Selected' accessor property is modified.</summary>
        protected virtual void OnSelectionChanged()
        {
            // Unhighlight highlighted tiles.
            foreach (VisualChessTableTile tile in m_HighlightedTiles)
            {
                tile.ResetMaterial();
            }
            m_HighlightedTiles.Clear();

            // Only highlight tiles if a valid, occupied tile is selected.
            if (m_Selected.visualPiece != null)
            {
                // Calculate valid moves and attacks for the current selection.
                m_Selected.validMoves = m_Selected.visualPiece.Piece.GetValidMoves();
                m_Selected.validAttacks = m_Selected.visualPiece.Piece.GetValidAttacks();

                // Loop through all valid moves for the selected piece, highlight tiles.
                foreach (ChessTableTile tile in m_Selected.validMoves)
                {
                    VisualChessTableTile visualTile = visualTable.GetVisualTile(tile);
                    visualTile.Renderer.material = m_MoveHighlightMaterial;
                    m_HighlightedTiles.Add(visualTile);
                }

                // Loop through all valid attacks for the selected peice, highlight attacks.
                foreach (AttackInfo attackInfo in m_Selected.validAttacks)
                {
                    VisualChessTableTile visualTile = visualTable.GetVisualTile(attackInfo.moveToTile);
                    visualTile.Renderer.material = m_AttackHighlightMaterial;
                    m_HighlightedTiles.Add(visualTile);
                }
            }

            // Invoke the 'SelectionChanged' unity event.
            SelectionChanged?.Invoke();
        }
        #endregion

        // Protected method(s).
        /// <summary>Subscribes to relevant ChessInstance events.</summary>
        protected void SubscribeToChessInstanceEvents()
        {
            // Subscribe to chess instance events.
            ChessInstance.TurnEnded += OnTurnEnded;
            ChessInstance.TurnStarted += OnTurnStarted;
            ChessInstance.GameOver += OnGameOver;
            ChessInstance.PreGameReset += OnPreGameReset;
            ChessInstance.PostGameReset += OnPostGameReset;
            ChessInstance.ChessPieceMoved += OnChessPieceMoved;

            // Subscribe to chess instance event callbacks. (Events that provide some 'ref' that may be modified to change engine behaviour.)
            ChessInstance.IsGameOverCallback += IsGameOverEventCallback;
        }

        /// <summary>Unsubscribes from relevant ChessInstance events.</summary>
        protected void UnsubscribeFromChessInstanceEvents()
        {
            // Unsubscribe from chess instance events.
            ChessInstance.TurnEnded -= OnTurnEnded;
            ChessInstance.TurnStarted -= OnTurnStarted;
            ChessInstance.GameOver -= OnGameOver;
            ChessInstance.PreGameReset -= OnPreGameReset;
            ChessInstance.PostGameReset -= OnPostGameReset;
            ChessInstance.ChessPieceMoved -= OnChessPieceMoved;

            // Unsubscribe from chess instance event callbacks. (Events that provide some 'ref' that may be modified to change engine behaviour.)
            ChessInstance.IsGameOverCallback -= IsGameOverEventCallback;
        }
    }
}
