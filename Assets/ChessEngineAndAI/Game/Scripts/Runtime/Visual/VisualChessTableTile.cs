using UnityEngine;

namespace ChessEngine.Game
{
    /// <summary>
    /// A ChessTableTile component is to be attached to each individual tile that makes up a chess table, each tile holds information about intself like offset.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class VisualChessTableTile : MonoBehaviour
    {
        #region Editor Serialized Fields
        [Header("Settings - Materials")]
        [Tooltip("A reference to the material used for white team tiles.")]
        [SerializeField] Material m_WhiteTeamMaterial = null;
        [Tooltip("A reference to the material used for black team tiles.")]
        [SerializeField] Material m_BlackTeamMaterial = null;

        [Header("Settings - Positioning")]
        [Tooltip("The local space offset to apply when calculating the 'local position' of the tile.")]
        public Vector3 offset;
        [Tooltip("The alignment move to use when positioning the piece on the X axis.")]
        public AxisAlignment alignX = AxisAlignment.Upper;
        [Tooltip("The alignment move to use when positioning the piece on the Y axis.")]
        public AxisAlignment alignY = AxisAlignment.Center;
        [Tooltip("The alignment move to use when positioning the piece on the Z axis.")]
        public AxisAlignment alignZ = AxisAlignment.Lower;

        [Header("Settings - Dimensions")]
        [Tooltip("The length of a tile.")]
        public float tileLength = 1f;
        [Tooltip("The width of a tile.")]
        public float tileWidth = 1f;
        [Tooltip("The height (or thickness) of a tile.")]
        public float tileHeight = 0.25f;
        #endregion
        #region Public Properties
        /// <summary>Returns a reference to the ChessTableTile that this component visualizes.</summary>
        public ChessTableTile Tile { get; private set; }
        /// <summary>A reference to the VisualChessTable this visual tile is placed on.</summary>
        public VisualChessTable VisualChessTable { get; private set; }
        /// <summary>A reference to the active GameManager in the scene, set by VisualChessTable when it instantiates the tile.</summary>
        public ChessGameManager GameManager { get; internal set; }
        /// <summary>A reference to the Renderer associated with this tile.</summary>
        public Renderer Renderer { get; private set; }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Awake()
        {
            // Find Renderer reference.
            Renderer = GetComponent<Renderer>();
        }
        #endregion

        // Public method(s).
        #region Initialization & Engine Interface
        /// <summary>Initializes the visual tile, positions it at the given index on the relevant VisualChessTable, and stores the relevant ChessTableTile reference.</summary>
        /// <param name="pVisualChessTable">The visual chess table to place the tile on.</param>
        /// <param name="pTile">The chess table to spawn the tile on.</param>
        public void Initialize(VisualChessTable pVisualChessTable, ChessTableTile pTile)
        {
            Tile = pTile;
            VisualChessTable = pVisualChessTable;
            transform.localPosition = GetLocalPosition(pVisualChessTable);
        }

        /// <summary>
        /// Returns the VisualChessPiece on this tile, otherwise null.
        /// Only valid when called after this tile has been placed on the board using 'Initialize'.
        /// </summary>
        /// <returns>VisualChessPiece on this tile, otherwise null.</returns>
        public VisualChessPiece GetVisualPiece()
        {
            // Try to find a chess piece on this tile.
            for (int i = 0; i < VisualChessTable.VisualPieceCount; ++i)
            {
                VisualChessPiece piece = VisualChessTable.GetVisualPieceByIndex(i);
                if (piece != null && piece.Piece.TileIndex == Tile.TileIndex)
                    return piece;
            }

            return null;
        }
        #endregion
        #region Positioning
        /// <summary>Computes and returns the local position of the chess table tile using the provided tile dimensions and offset(s).</summary>
        /// <param name="pChessTable"></param>
        /// <returns>the local position of the chess table tile using the provided tile dimensions and offset(s).</returns>
        public Vector3 GetLocalPosition(VisualChessTable pChessTable)
        {
            Vector3 localPosition;

            float xOffset = 0f;
            float yOffset = 0f;
            float zOffset = 0f;

            // Calculate offsets based on alignment modes.
            switch (alignX)
            {
                case AxisAlignment.Center:
                    xOffset = -(2 * tileWidth) + (tileWidth * Tile.TileIndex.x) + tileWidth / 2;
                    break;
                case AxisAlignment.Upper:
                    xOffset = -(4 * tileWidth) + (tileWidth * Tile.TileIndex.x);
                    break;
                case AxisAlignment.Lower:
                    xOffset = -(4 * tileWidth) + (tileWidth * Tile.TileIndex.x);
                    break;
            }

            switch (alignY)
            {
                case AxisAlignment.Center:
                    yOffset = tileHeight / 2;
                    break;
                case AxisAlignment.Upper:
                    yOffset = tileHeight;
                    break;
                case AxisAlignment.Lower:
                    yOffset = 0f;
                    break;
            }

            switch (alignZ)
            {
                case AxisAlignment.Center:
                    zOffset = -(2 * tileLength) + (tileLength * Tile.TileIndex.y) + tileLength / 2;
                    break;
                case AxisAlignment.Upper:
                    zOffset = -(4 * tileLength) + (tileLength * Tile.TileIndex.y);
                    break;
                case AxisAlignment.Lower:
                    zOffset = -(4 * tileLength) + (tileLength * Tile.TileIndex.y);
                    break;
            }

            localPosition = new Vector3(xOffset, yOffset, zOffset) + offset;

            return localPosition;
        }
        #endregion
        #region Rendering
        /// <summary>Resets the material on a given tile to it's appropriate color.</summary>
        public void ResetMaterial()
        {
            // Reset the material on this tile.
            Renderer.material = Tile.Color == ChessColor.White ? m_WhiteTeamMaterial : m_BlackTeamMaterial;
        }
        #endregion
        #region Selection
        /// <summary>Selects this tile on whatever GameManager is associated with this VisualChessTableTile.</summary>
        public void Select()
        {
            // if there is a valid GameManager referenced select this tile.
            if (GameManager != null)
            {
                GameManager.SelectTile(this);
            }
        }
        #endregion
    }
}
