using UnityEngine;

namespace ChessEngine.Game
{
    /// <summary>
    /// A component that allows a captured visual piece to have it's appearance set.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class CapturedVisualPiece : MonoBehaviour
    {
        #region Editor Serialized Fields & Events
        [Header("Settings - Piece")]
        [Tooltip("The type of the chess piece.")]
        public ChessPieceType pieceType;

        [Header("Settings - Positioning")]
        [Tooltip("An offset to apply to this captured pieces' position.")]
        public Vector3 offset;

        [Header("Settings - Materials")]
        [Tooltip("(Optional) An editor set reference to the Renderer for this captured chess piece.")]
        [SerializeField] Renderer m_RendererOverride;
        [Tooltip("An editor set reference to the material of this piece when it's on the white team.")]
        [SerializeField] Material m_WhiteMaterial = null;
        [Tooltip("An editor set reference to the material of this piece when it's on the black team.")]
        [SerializeField] Material m_BlackMaterial = null;
        #endregion
        #region Public Properties
        /// <summary>The Renderer associated with this chess piece.</summary>
        public Renderer Renderer { get; private set; }
        /// <summary>The default 'localRotation' for this visual chess piece, set in Start(), can be overridden manually.</summary>
        public Quaternion DefaultLocalRotation { get; set; }
        /// <summary>The team the captured piece is a part of.</summary>
        public ChessColor Color { get; private set; }
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            // Find Renderer refrence if no override set.
            if (m_RendererOverride == null)
            {
                Renderer = GetComponentInChildren<Renderer>();
            }
            else { Renderer = m_RendererOverride; }
        }

        void Start()
        {
            // Store default local rotation.
            DefaultLocalRotation = transform.localRotation;
        }
        #endregion

        #region Public Initialization Method(s)
        /// <summary>
        /// Initializes the visualization for the captured chess piece on the given team.
        /// </summary>
        /// <param name="pColor">The team the piece belonged to.</param>
        public void Initialize(ChessColor pColor)
        {
            Color = pColor;

            // Initialize renderer related stuff.
            if (Renderer != null)
            {
                // Set piece color/team.
                if (Color == ChessColor.White)
                {
                    Renderer.material = m_WhiteMaterial;
                }
                else { Renderer.material = m_BlackMaterial; }
            }
        }
        #endregion
    }
}
