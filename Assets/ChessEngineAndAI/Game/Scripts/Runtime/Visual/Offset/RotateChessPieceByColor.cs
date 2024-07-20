using UnityEngine;

namespace ChessEngine.Game
{
    /// <summary>
    /// A simple component that can be attached to a VisualChessPiece that allows an initial euler angles offset to be applied.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(VisualChessPiece))]
    public class RotateChessPieceByColor : MonoBehaviour
    {
        #region Editor Serialized Settings
        [Header("Settings - White")]
        [Tooltip("The degrees of rotation to rotate the 'VisualPiece' around 'whiteAxis' when on the white team.")]
        public float whiteRotation;
        [Tooltip("The axis to rotate the 'VisualPiece' around when on the white team. (Local space.)")]
        public Vector3 whiteAxis;

        [Header("Settings - Black")]
        [Tooltip("The degrees of rotation to rotate the 'VisualPiece' around 'blackAxis' when on the black team.")]
        public float blackRotation;
        [Tooltip("The axis to rotate the 'VisualPiece' around when on the black team. (Local space.)")]
        public Vector3 blackAxis;        
        #endregion
        #region Public Properties
        /// <summary>A reference to the VisualChessPiece that this component rotates.</summary>
        public VisualChessPiece VisualPiece { get; private set; }
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            // Find 'VisualPiece' reference.
            VisualPiece = GetComponent<VisualChessPiece>();
        }

        void Start()
        {
            // IF the piece is on the white team use 'white' prefixed settings.
            if (VisualPiece.Piece.Color == ChessColor.White)
            {
                // Rotate 'VisualPiece.transform' 'whiteRotation' degrees around the 'whiteAxis' in local space.
                VisualPiece.transform.Rotate(whiteAxis, whiteRotation);
            }
            // Otherwise the piece is on the black team, use 'black' prefixed settings.
            else
            {
                // Rotate 'VisualPiece.transform' 'blackRotation' degrees around the 'blackAxis' in local space.
                VisualPiece.transform.Rotate(blackAxis, blackRotation);
            }
        }
        #endregion
    }
}
