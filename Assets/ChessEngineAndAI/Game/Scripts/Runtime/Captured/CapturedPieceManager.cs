using UnityEngine;
using System.Collections.Generic;

namespace ChessEngine.Game
{
    /// <summary>
    /// A component that manages visualization of captured pieces.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ChessGameManager))]
    public class CapturedPieceManager : MonoBehaviour
    {
        #region Editor Serialized Fields & Events
        [Header("Prefabs")]
        [Tooltip("An editor-set reference to the king captured piece prefab.")]
        [SerializeField] GameObject m_KingPiecePrefab = null;
        [Tooltip("An editor-set reference to the queen captured piece prefab.")]
        [SerializeField] GameObject m_QueenPiecePrefab = null;
        [Tooltip("An editor-set reference to the bishop captured piece prefab.")]
        [SerializeField] GameObject m_BishopPiecePrefab = null;
        [Tooltip("An editor-set reference to the knight captured piece prefab.")]
        [SerializeField] GameObject m_KnightPiecePrefab = null;
        [Tooltip("An editor-set reference to the rook captured piece prefab.")]
        [SerializeField] GameObject m_RookPiecePrefab = null;
        [Tooltip("An editor-set reference to the pawn captured piece prefab.")]
        [SerializeField] GameObject m_PawnPiecePrefab = null;

        [Header("Piece Placement - White")]
        [Tooltip("Captured position Transform references for the white team King piece. (1 required.)")]
        public Transform whiteKingTransform;
        [Tooltip("Captured position Transform references for the white team Queen piece. (1 required.)")]
        public Transform whiteQueenTransform;
        [Tooltip("Captured position Transform references for white team Bishop pieces. (2 required.)")]
        public Transform[] whiteBishopTransforms;
        [Tooltip("Captured position Transform references for white team Knight pieces. (2 required.)")]
        public Transform[] whiteKnightTransforms;
        [Tooltip("Captured position Transform references for white team Rook pieces. (2 required.)")]
        public Transform[] whiteRookTransforms;
        [Tooltip("Captured position Transform references for white team Pawn pieces. (8 required.)")]
        public Transform[] whitePawnTransforms;

        [Header("Piece Placement - Black")]
        [Tooltip("Captured position Transform references for the black team King piece. (1 required.)")]
        public Transform blackKingTransform;
        [Tooltip("Captured position Transform references for the black team Queen piece. (1 required.)")]
        public Transform blackQueenTransform;
        [Tooltip("Captured position Transform references for black team Bishop pieces. (2 required.)")]
        public Transform[] blackBishopTransforms;
        [Tooltip("Captured position Transform references for black team Knight pieces. (2 required.)")]
        public Transform[] blackKnightTransforms;
        [Tooltip("Captured position Transform references for black team Rook pieces. (2 required.)")]
        public Transform[] blackRookTransforms;
        [Tooltip("Captured position Transform references for black team Pawn pieces. (8 required.)")]
        public Transform[] blackPawnTransforms;
        #endregion
        #region Public Properties
        /// <summary>A reference to the ChessGameManager this component is manages captured pieces for.</summary>
        public ChessGameManager GameManager { get; private set; }
        /// <summary>The number of captured visual piece components instantiated in the world.</summary>
        public int CapturedVisualPieceCount { get { return m_CapturedVisualPieces.Count; } }
        #endregion
        #region Private Field(s)
        /// <summary>A list of CapturedVisualPieces that are registered to the game.</summary>
        List<CapturedVisualPiece> m_CapturedVisualPieces = new List<CapturedVisualPiece>();
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            // Find GameManager reference.
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            GameManager.GameInitialized.AddListener(OnGameInitialized);
        }

        void OnDestroy()
        {
            // Unsubscribe from relevant event(s).
            if (GameManager != null && GameManager.ChessInstance != null)
            {
                GameManager.ChessInstance.GameOver -= OnGameOver;
                GameManager.ChessInstance.ChessPieceMoved -= OnChessPieceMoved;
                GameManager.ChessInstance.PreGameReset -= OnPreGameReset;
            }
        }
        #endregion

        #region Public Captured Visual Piece Management Method(s)
        /// <summary>Creates and returns the visual representation of the described captured chess piece, or returns null if unable to create.</summary>
        /// <param name="pType">The type of chess piece.</param>
        /// <param name="pColor">The team the piece was on.</param>
        /// <returns>a CapturedPieceManager representation of the described captured chess piece, returns null if unable to create.</returns>
        CapturedVisualPiece CreateCapturedPiece(ChessPieceType pType, ChessColor pColor)
        {
            // Pick spawn point.
            int capturedPieceCount = GetCapturedVisualPieceCount(pType, pColor);
            Transform spawnTransform = null;
            switch (pType)
            {
                case ChessPieceType.Pawn:
                    if (pColor == ChessColor.White)
                    {
                        if (whitePawnTransforms != null && capturedPieceCount < whitePawnTransforms.Length)
                            spawnTransform = whitePawnTransforms[capturedPieceCount];
                    }
                    else
                    {
                        if (blackPawnTransforms != null && capturedPieceCount < blackPawnTransforms.Length)
                            spawnTransform = blackPawnTransforms[capturedPieceCount];
                    }
                    break;
                case ChessPieceType.Rook:
                    if (pColor == ChessColor.White)
                    {
                        if (whiteRookTransforms != null && capturedPieceCount < whiteRookTransforms.Length)
                            spawnTransform = whiteRookTransforms[capturedPieceCount];
                    }
                    else
                    {
                        if (blackRookTransforms != null && capturedPieceCount < blackRookTransforms.Length)
                            spawnTransform = blackRookTransforms[capturedPieceCount];
                    }
                    break;
                case ChessPieceType.Knight:
                    if (pColor == ChessColor.White)
                    {
                        if (whiteKnightTransforms != null && capturedPieceCount < whiteKnightTransforms.Length)
                            spawnTransform = whiteKnightTransforms[capturedPieceCount];
                    }
                    else
                    {
                        if (blackKnightTransforms != null && capturedPieceCount < blackKnightTransforms.Length)
                            spawnTransform = blackKnightTransforms[capturedPieceCount];
                    }
                    break;
                case ChessPieceType.Bishop:
                    if (pColor == ChessColor.White)
                    {
                        if (whiteBishopTransforms != null && capturedPieceCount < whiteBishopTransforms.Length)
                            spawnTransform = whiteBishopTransforms[capturedPieceCount];
                    }
                    else
                    {
                        if (blackBishopTransforms != null && capturedPieceCount < blackBishopTransforms.Length)
                            spawnTransform = blackBishopTransforms[capturedPieceCount];
                    }
                    break;
                case ChessPieceType.Queen:
                    if (pColor == ChessColor.White)
                    {
                        spawnTransform = whiteQueenTransform;
                    }
                    else { spawnTransform = blackQueenTransform; }
                    break;
                case ChessPieceType.King:
                    if (pColor == ChessColor.White)
                    {
                        spawnTransform = whiteKingTransform;
                    }
                    else { spawnTransform = blackKingTransform; }
                    break;
                default:
                    Debug.LogWarning("CapturedPieceManager component failed to create captured visual piece for a chess piece with an invalid type! (Not pawn, rook, knight, bishop, queen, nor king.)", gameObject);
                    break;
            }

            // Only create visual captured piece if a valid spawn is available.
            if (spawnTransform != null)
            {
                // Create piece.
                GameObject pieceObject = null;
                switch (pType)
                {
                    case ChessPieceType.Pawn:
                        if (m_PawnPiecePrefab != null)
                            pieceObject = Instantiate(m_PawnPiecePrefab);
                        break;
                    case ChessPieceType.Rook:
                        if (m_RookPiecePrefab != null)
                            pieceObject = Instantiate(m_RookPiecePrefab);
                        break;
                    case ChessPieceType.Knight:
                        if (m_KnightPiecePrefab != null)
                            pieceObject = Instantiate(m_KnightPiecePrefab);
                        break;
                    case ChessPieceType.Bishop:
                        if (m_BishopPiecePrefab != null)
                            pieceObject = Instantiate(m_BishopPiecePrefab);
                        break;
                    case ChessPieceType.Queen:
                        if (m_QueenPiecePrefab != null)
                            pieceObject = Instantiate(m_QueenPiecePrefab);
                        break;
                    case ChessPieceType.King:
                        if (m_KingPiecePrefab != null)
                            pieceObject = Instantiate(m_KingPiecePrefab);
                        break;
                    default:
                        Debug.LogWarning("CapturedPieceManager component failed to create captured visual piece for a chess piece with an invalid type! (Not pawn, rook, knight, bishop, queen, nor king.)", gameObject);
                        break;
                }

                // Ensure a valid pieceObject was created.
                if (pieceObject != null)
                {
                    // Ensure the pieceObject has a CapturedVisualPiece component.
                    CapturedVisualPiece capturedPiece = pieceObject.GetComponent<CapturedVisualPiece>();
                    if (capturedPiece != null)
                    {
                        // Initialize the piece.
                        capturedPiece.Initialize(pColor);

                        // Position the piece.
                        capturedPiece.transform.SetPositionAndRotation(
                            spawnTransform.position + capturedPiece.transform.TransformDirection(capturedPiece.offset),
                            spawnTransform.rotation
                        );

                        // Register the piece.
                        m_CapturedVisualPieces.Add(capturedPiece);

                        return capturedPiece;
                    }
                    // Otherwise log a warning and destroy the piece.
                    else
                    {
                        Debug.LogWarning("CapturedPieceManager instantiated captured visual piece for piece piece type '" + pType.ToString() + "' on team '" + pColor.ToString() + "' but no CapturedVisualPiece component was found. Ensure the prefab for this piece type includes a CapturedVisualPiece component. Visual piece destroyed.", gameObject);
                        Destroy(pieceObject);
                        return null;
                    }
                }
                else
                {
                    Debug.LogWarning("CapturedPieceManager failed to instantiate captured visual piece for piece type '" + pType.ToString() + "' on team '" + pColor.ToString() + "'!", gameObject);
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("CapturedPieceManager failed to instantiate captured visual piece for piece type '" + pType.ToString() + "' on team '" + pColor.ToString() + "'! No spawn point available.", gameObject);
                return null;
            }
        }

        /// <summary>Returns the number of captured visual pieces of pType on team pColor currently in the piece manager.</summary>
        /// <param name="pType"></param>
        /// <param name="pColor"></param>
        /// <returns>the number of captured visual pieces of pType on team pColor currently in the piece manager.</returns>
        public int GetCapturedVisualPieceCount(ChessPieceType pType, ChessColor pColor)
        {
            int count = 0;
            foreach (CapturedVisualPiece capturedPiece in m_CapturedVisualPieces)
            {
                if (capturedPiece != null && capturedPiece.pieceType == pType && capturedPiece.Color == pColor)
                    ++count;
            }

            return count;
        }

        /// <summary>Destroys the CapturedVisualPiece at the specified index.</summary>
        /// <param name="pIndex"></param>
        public void DestroyCapturedVisualPieceByIndex(int pIndex)
        {
            CapturedVisualPiece capturedPiece = m_CapturedVisualPieces[pIndex];
            if (capturedPiece != null)
                Destroy(m_CapturedVisualPieces[pIndex].gameObject);
            m_CapturedVisualPieces.RemoveAt(pIndex);
        }

        /// <summary>Destroys all CapturedVisualPieces that were instantiated by this captured piece manager.</summary>
        public void DestroyAllCapturedVisualPieces()
        {
            if (m_CapturedVisualPieces != null)
            {
                for (int i = m_CapturedVisualPieces.Count - 1; i >= 0; --i)
                {
                    if (m_CapturedVisualPieces[i] != null)
                        Destroy(m_CapturedVisualPieces[i].gameObject);
                }
                m_CapturedVisualPieces.Clear();
            }
            else { m_CapturedVisualPieces = new List<CapturedVisualPiece>(); }
        }

        /// <summary>Returns the CapturedVisualPiece at the given index in the 'captured visual pieces' list.</summary>
        /// <param name="pIndex"></param>
        /// <returns>the CapturedVisualPiece at the given index in the 'captured visual pieces' list.</returns>
        public CapturedVisualPiece GetCapturedVisualPiece(int pIndex)
        {
            return m_CapturedVisualPieces[pIndex];
        }
        #endregion

        #region Private Game Callback(s)
        /// <summary>Invoked after the chess game instnace is initialized.</summary>
        void OnGameInitialized()
        {
            // Subscribe to relevant event(s).
            GameManager.ChessInstance.GameOver += OnGameOver;
            GameManager.ChessInstance.ChessPieceMoved += OnChessPieceMoved;
            GameManager.ChessInstance.PreGameReset += OnPreGameReset;
        }

        /// <summary>Invoked when the game ends on pEndOnTurn color's turn for pReason.</summary>
        /// <param name="pEndOnTurn"></param>
        /// <param name="pReason"></param>
        void OnGameOver(ChessColor pEndOnTurn, GameOverReason pReason)
        {
            // Create the king captured piece when valid.
            if (pReason == GameOverReason.Won)
                CreateCapturedPiece(ChessPieceType.King, pEndOnTurn == ChessColor.White ? ChessColor.Black : ChessColor.White);
        }

        /// <summary>Invoked after a chess piece is moved.</summary>
        /// <param name="pMoveInfo"></param>
        void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            // Only create a captured visible piece if the move resulted in a piece being caputed.
            if (pMoveInfo.capturedPiece != null)
            {
                // Create a captured visual piece.
                CreateCapturedPiece(pMoveInfo.capturedPiece.GetChessPieceType(), pMoveInfo.capturedPiece.Color);
            }
        }

        /// <summary>Invoked just before the game is reset.</summary>
        void OnPreGameReset()
        {
            // Destroy all captured visual pieces.
            DestroyAllCapturedVisualPieces();
        }
        #endregion
    }
}
