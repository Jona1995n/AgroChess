using UnityEngine;
using System;
using ChessEngine.AI;
using TypeReferences;

namespace ChessEngine.Game.AI
{
    /// <summary>
    /// An implementation of ChessGameManager for games that supports using any ChessEngine.AI module to automatically play as any color.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ChessAIGameManager : ChessGameManager
    {
        #region Editor Serialized Settings
        [Header("Settings - AI")]
        [Tooltip("Should AI be forced to use all of their allotted think time? This will delay the AIs best move submission for a value equal to their 'AI think time'. Delays can be interrupted by using ChessAI.DemandMove().")]
        public bool aiUseAllThinkTime;

        [Header("Settings - AI (White)")]
        [Tooltip("Should AI be used to play for the white team?")]
        public bool enableWhiteAI;
        [Tooltip("A reference to the ChessAI type that will play for the white team, or 'None' if the team will be human played. Don't forget to invoke 'InitializeWhiteAI()' if modifying during a chess match.")]
        [ClassExtends(typeof(ChessAI))] public ClassTypeReference whiteChessAI;
        [Min(0)]
        [Tooltip("The maximum number of seconds the white AI may think for. (A value of 0 means think duration is determined by the chess engine.)")]
        public float whiteAIThinkTime = 3;
        [Min(0)]
        [Tooltip("The think depth for the white AI. (Default: 3 | Minimum: 0)")]
        public int whiteAIThinkDepth = 3;

        [Header("Settings - AI (Black)")]
        [Tooltip("Should AI be used to play for the black team?")]
        public bool enableBlackAI;
        [Tooltip("A reference to the ChessAI type that will play for the black team, or 'None' if the team will be human played. Don't forget to invoke 'InitializeBlackAI()' if modifying during a chess match.")]
        [ClassExtends(typeof(ChessAI))] public ClassTypeReference blackChessAI;
        [Min(0f)]
        [Tooltip("The maximum number of seconds the black AI may think for. (A value of 0 means think duration is determined by the chess engine.)")]
        public float blackAIThinkTime = 3;
        [Min(0)]
        [Tooltip("The think depth for the black AI. (Default: 3 | Minimum: 0)")]
        public int blackAIThinkDepth = 3;
        #endregion
        #region Public Properties
        /// <summary>A reference to the ChessAI playing for the white team, otherwise null.</summary>
        public ChessAI WhiteAIInstance { get; private set; }
        /// <summary>A reference to the ChessAI playing for the black team, otherwise null.</summary>
        public ChessAI BlackAIInstance { get; private set; }
        /// <summary>Returns true if 'Enable White AI' is true and WhiteAIInstance is non-null, otherwise false.</summary>
        public bool IsWhiteAIEnabled { get { return enableWhiteAI && WhiteAIInstance != null; } }
        /// <summary>Returns true if 'Enable Black AI' is true and BlackAIInstance is non-null, otherwise false.</summary>
        public bool IsBlackAIEnabled { get { return enableBlackAI && BlackAIInstance != null; } }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        protected virtual void Update()
        {
            // Update any valid chess AIs if there is a valid chess instance.
            if (ChessInstance != null)
            {
                if (WhiteAIInstance != null)
                    WhiteAIInstance.Update();
                if (BlackAIInstance != null)
                    BlackAIInstance.Update();
            }
        }

        protected virtual void OnDestroy()
        {
            // Cleanup AI.
            if (WhiteAIInstance != null)
                DestroyWhiteAI();
            if (BlackAIInstance != null)
                DestroyBlackAI();
        }
        #endregion

        // Public override method(s).
        #region Piece Selection Override(s)
        /// <summary>Override piece selection check method to disallow selections of other team and while game is not started.</summary>
        /// <param name="pVisualPiece"></param>
        /// <param name="pVisualTile"></param>
        /// <returns>true of the piece can be selected, otherwise false.</returns>
        public override bool CanSelectPiece(VisualChessPiece pVisualPiece, VisualChessTableTile pVisualTile)
        {
            // Cannot select piece on AI turn.
            if (ChessInstance.turn == ChessColor.White)
            {
                if (IsWhiteAIEnabled)
                    return false;
            }
            else if (IsBlackAIEnabled) { return false; }

            // Check if the piece may be selected.
            return base.CanSelectPiece(pVisualPiece, pVisualTile);
        }
        #endregion

        // Public method(s)
        #region AI Initialization Method(s)
        /// <summary>Initiailzies an instance of the currently set black chess AI type</summary>
        public void InitializeBlackAI()
        {
            // Destroy existing AI.
            if (BlackAIInstance != null)
                DestroyBlackAI();

            // Initialize the instance for black AI.
            BlackAIInstance = (ChessAI)Activator.CreateInstance(blackChessAI, ChessColor.Black);
            if (BlackAIInstance != null)
                InitializeAI(BlackAIInstance);
        }

        /// <summary>Initiailzies an instance of the currently set white chess AI type</summary>
        public void InitializeWhiteAI()
        {
            // Destroy existing AI.
            if (WhiteAIInstance != null)
                DestroyWhiteAI();

            // Initialize the instance for white AI.
            WhiteAIInstance = (ChessAI)Activator.CreateInstance(whiteChessAI, ChessColor.White);
            if (WhiteAIInstance != null)
                InitializeAI(WhiteAIInstance);
        }
        #endregion

        // Protected override method(s).
        #region ChessGameManager Overridden Callbacks
        protected override void OnGameInitialized()
        {
            // Invoke the base class 'OnGameInitialized' callback.
            base.OnGameInitialized();

            // Instantiate relevant chess AI(s).
            if (whiteChessAI != null && whiteChessAI.Type != null)
                InitializeWhiteAI();
            if (blackChessAI != null && blackChessAI.Type != null)
                InitializeBlackAI();
        }

        protected override void OnTurnStarted(ChessColor pTurn)
        {
            // Invoke the base type 'turn started' method.
            base.OnTurnStarted(pTurn);

            // Request a move from the AI if it is an AI turn.
            // If it is the white teams turn check if AI is playing as white...
            if (pTurn == ChessColor.White)
            {
                // If white AI is enabled request a move from the white chess AI.
                if (IsWhiteAIEnabled)
                {
                    WhiteAIInstance.RequestBestMove(whiteAIThinkDepth, whiteAIThinkTime);

                    // If set use all of allotted think time.
                    if (aiUseAllThinkTime && whiteAIThinkTime > 0)
                        WhiteAIInstance.DelayBestMove(whiteAIThinkTime);
                }
            }
            // Otherwise it is the black teams turn, check if AI is playing as black...
            else
            {
                // If black AI is enabled request a move from the black chess AI.
                if (IsBlackAIEnabled)
                {
                    BlackAIInstance.RequestBestMove(blackAIThinkDepth, blackAIThinkTime);

                    // If set use all of allotted think time.
                    if (aiUseAllThinkTime && blackAIThinkTime > 0)
                        BlackAIInstance.DelayBestMove(blackAIThinkTime);
                }
            }
        }

        protected override void OnPreGameReset()
        {
            // Invoke the base type 'pre game reset' method.
            base.OnPreGameReset();

            // Instantiate relevant chess AI(s).
            if (whiteChessAI != null && whiteChessAI.Type != null)
                InitializeWhiteAI();
            if (blackChessAI != null && blackChessAI.Type != null)
                InitializeBlackAI();
        }
        #endregion

        // Private method(s).
        #region AI Initialization & Destruction
        /// <summary>Initializes the chess AI instance, pChessAI.</summary>
        /// <param name="pChessAI"></param>
        void InitializeAI(ChessAI pChessAI)
        {
            // Share instance with AI.
            pChessAI.SetInstance(ChessInstance);

            // Subscribe to relevant event(s).
            pChessAI.BestMoveSubmitted += OnBestMoveSubmitted;

            // Initialize the AI.
            pChessAI.OnInitialized();
        }

        /// <summary>Destroys & cleans up any black AI instance.</summary>
        void DestroyBlackAI()
        {
            if (BlackAIInstance != null)
                BlackAIInstance.OnDeinitialized();
            BlackAIInstance = null;
        }

        /// <summary>Destroys & cleans up any white AI instance.</summary>
        void DestroyWhiteAI()
        {
            if (WhiteAIInstance != null)
                WhiteAIInstance.OnDeinitialized();
            WhiteAIInstance = null;
        }
        #endregion

        // Private callback(s).
        /// <summary>Invoked whenever a best move is submitted by either AI.</summary>
        /// <param name="pFrom"></param>
        /// <param name="pTo"></param>
        void OnBestMoveSubmitted(TileIndex pFrom, TileIndex pTo)
        {
            // Ensure there is a valid tile at 'pFrom'.
            ChessTableTile fromTile = ChessInstance.Table.GetTile(pFrom);
            if (fromTile != null)
            {
                // Ensure there is a valid piece on 'fromTile'.
                ChessPiece tilePiece = fromTile.GetPiece();
                if (tilePiece != null)
                {
                    // Ensure it is tilePiece's turn.
                    if (ChessInstance.turn == tilePiece.Color)
                    {
                        // Ensure there is a valid tile at 'pTo'.
                        ChessTableTile toTile = ChessInstance.Table.GetTile(pTo);
                        if (toTile != null)
                        {
                            // Confirm the move is legal.
                            var validMoves = tilePiece.GetValidMoves();
                            var validAttacks = tilePiece.GetValidAttacks();
                            if (validMoves.Contains(toTile) || ChessTableTile.IsTileAttackable(validAttacks, toTile))
                            {
                                // Move the 'tilePiece' to the 'pTo' tile attacking any piece on 'toTile'.
                                MoveInfo moveInfo = tilePiece.Move(pTo, toTile.GetPiece());

                                // Reset selection.
                                Deselect();

                                // End the turn.
                                ChessInstance.EndTurn(moveInfo);
                            }
                            else { Debug.LogWarning("ChessAIGameManager received 'best move' that was non-legal. (From Tile index: " + pFrom + " | To Tile index: " + pTo + ")", gameObject); }
                        }
                        else { Debug.LogWarning("ChessAIGameManager received 'best move' with invalid 'to' tile. (Tile index: " + pTo + ")", gameObject); }
                    }
                    else { Debug.LogWarning("ChessAIGameManager received 'best move' for team '" + tilePiece.Color + "' while it is team '" + ChessInstance.turn + "' turn! Either best move was received out-of-turn order or AI is considering pieces that do not belong to it. (From Tile index: " + pTo + ")", gameObject); }
                }
                else { Debug.LogWarning("ChessAIGameManager received 'best move' with empty 'from' tile. (Tile index: " + pFrom + ")", gameObject); }
            }
            else { Debug.LogWarning("ChessAIGameManager received 'best move' with invalid 'from' tile. (Tile index: " + pFrom + ")", gameObject); }
        }
    }
}
