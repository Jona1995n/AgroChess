using UnityEngine;
using System.Collections.Generic;

namespace ChessEngine.Game
{
    /// <summary>
    /// The VisualChessTable component may be used to visualize a 'ChessEngine.ChessTable'.
    /// Once initialized this component will automatically keep the visual chess components synced with the chess engine table.
    /// 
    /// NOTE: Chess tables are implemented with a bottom left origin meaning VisualTiles[0][0] is the bottom-left most corner when the white team is at the 'bottom' of the board.
    /// </summary>
	/// Author: Intuitive Gaming Solutions
    public class VisualChessTable : MonoBehaviour
    {
        #region Editor Serialized Settings
        [Header("Settings")]
        [Tooltip("An editor-set reference to the tile prefab.")]
        [SerializeField] GameObject m_TilePrefab = null;
        [Tooltip("An editor-set reference to the king piece prefab.")]
        [SerializeField] GameObject m_KingPiecePrefab = null;
        [Tooltip("An editor-set reference to the queen piece prefab.")]
        [SerializeField] GameObject m_QueenPiecePrefab = null;
        [Tooltip("An editor-set reference to the bishop piece prefab.")]
        [SerializeField] GameObject m_BishopPiecePrefab = null;
        [Tooltip("An editor-set reference to the knight piece prefab.")]
        [SerializeField] GameObject m_KnightPiecePrefab = null;
        [Tooltip("An editor-set reference to the rook piece prefab.")]
        [SerializeField] GameObject m_RookPiecePrefab = null;
        [Tooltip("An editor-set reference to the pawn piece prefab.")]
        [SerializeField] GameObject m_PawnPiecePrefab = null;
        #endregion
        #region Public Properties
        /// <summary>Returns the number of visual chess pieces still on the board.</summary>
        public int VisualPieceCount { get { return m_VisualChessPieces.Count; } }
        /// <summary>A reference to the ChessTable this component is visualizing.</summary>
        public ChessTable Table { get; private set; }
        /// <summary>A 2D array of VisualChessTableTiles that make up the table.</summary>
        public VisualChessTableTile[][] VisualTiles { get; private set; }
        #endregion
        #region Private Fields
        /// <summary>A list of visual chess pieces on the board.</summary>
        List<VisualChessPiece> m_VisualChessPieces = new List<VisualChessPiece>();
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void OnDestroy()
        {
            // unsubscribe from chess instance events.
            UnsubscribeFromChessInstanceEvents();
        }
        #endregion

        // Public method(s).
        #region Initialization & Reset
        /// <summary>Initializes this visual chess table to maintain an accurate visualization for pChessTable.</summary>
        /// <param name="pChessTable"></param>
        public void Initialize(ChessTable pTable)
        {
            // Set 'Table' reference.
            Table = pTable;

            // Allocate our VisualTiles array.
            VisualTiles = new VisualChessTableTile[8][];
            for (int i = 0; i < VisualTiles.Length; ++i)
            {
                VisualTiles[i] = new VisualChessTableTile[8];
            }

            // Create the tiles to form an 8x8 board.
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    // Instantiate the tile, place it, and set it's color.
                    GameObject tileObject = Instantiate(m_TilePrefab, transform);
                    if (tileObject != null)
                    {
                        VisualChessTableTile tile = tileObject.GetComponent<VisualChessTableTile>();
                        tile.Initialize(this, Table.Tiles[x][y]);

                        // Add the tile to the VisualTiles array.
                        VisualTiles[x][y] = tile;

                        // Reset the tiles material.
                        tile.ResetMaterial();
                    }
                    else { Debug.LogWarning("Failed to instantiate visual tile on VisualChessTable!"); }
                }
            }

            // Sync the chess pieces with the instance
            SyncChessPiecesWithTable();

            // Subscribe to chess instance events.
            SubscribeToChessInstanceEvents();
        }

        /// <summary>
        /// When called removes all existing visual chess pieces and instantiates new ones at the corresponding engine ChessPiece location.
        /// This may or may not create the piece at the initial locations as the 'Instance' may be in any state when this method is invoked.
        /// </summary>
        public void SyncChessPiecesWithTable()
        {
            // Destroy all existing visual chess pieces.
            foreach (VisualChessPiece visualPiece in m_VisualChessPieces)
            {
                if (visualPiece != null)
                    Destroy(visualPiece);
            }
            m_VisualChessPieces.Clear();

            // Loop over all chess pieces in the instance and create a visual representation for the piece.
            for (int i = 0; i < Table.PieceCount; ++i)
            {
                if (!CreateVisualPiece(Table.GetPieceByIndex(i)))
                    Debug.LogWarning("Failed to create visual representation of chess piece in index '" + i + "'!", gameObject);
            }
        }
        #endregion
        #region Visual Tiles
        /// <summary>Returns VisualTiles[pTileIndex.x][pTileIndex.y].</summary>
        /// <param name="pTileIndex"></param>
        /// <returns>A reference to the ChessTableTile in Tiles[pTileIndex.x][pTileIndex.y].</returns>
        public VisualChessTableTile GetVisualTile(TileIndex pTileIndex) { return GetVisualTile(pTileIndex.x, pTileIndex.y); }

        /// <summary>Returns VisualTiles[pX][pY].</summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns>A reference to the ChessTableTile in Tiles[pX][pY].</returns>
        public VisualChessTableTile GetVisualTile(int pX, int pY) { return VisualTiles[pX][pY]; }

        /// <summary>Returns the VisualChessTableTile representation of a ChessTableTile. (Visual representation of engine chess table tile)</summary>
        /// <param name="pTile"></param>
        /// <returns>the VisualChessTableTile representation of a ChessTableTile</returns>
        public VisualChessTableTile GetVisualTile(ChessTableTile pTile) { return VisualTiles[pTile.TileIndex.x][pTile.TileIndex.y]; }

        /// <summary>Returns the VisualChessTableTile where VisualChessTableTile.Tile.TileIndex.GetTileID() == pID, otherwise null.</summary>
        /// <param name="pID"></param>
        /// <returns>the VisualChessTableTile where VisualChessTableTile.Tile.TileIndex.GetTileID() == pID, otherwise null</returns>
        public VisualChessTableTile GetVisualTileByID(string pID)
        {
            // Iterate through all tiles looking for ID match.
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    // Check for tile ID match.
                    VisualChessTableTile tile = GetVisualTile(x, y);
                    if (tile.Tile.TileIndex.GetTileID() == pID)
                        return tile; // Match found! return the VisualChessTableTile reference.
                }
            }

            // No tile found, return null.
            return null;
        }
        #endregion Tiles
        #region Visual Chess Pieces
        /// <summary>Returns the VisualChessPiece that visualizes the ChessPiece, pPiece, otherwise null.</summary>
        /// <param name="pPiece"></param>
        /// <returns>the VisualChessPiece that visualizes the ChessPiece, pPiece, otherwise null.</returns>
        public VisualChessPiece GetVisualPiece(ChessPiece pPiece)
        {
            foreach (VisualChessPiece visualPiece in m_VisualChessPieces)
            {
                // If the visualPiece is visualizing pPiece return a reference to it.
                if (visualPiece != null && visualPiece.Piece == pPiece)
                    return visualPiece;
            }

            // Piece not found, return null.
            return null;
        }

        /// <summary>Returns the VisualChessPiece in the visual chess pieces array at the given index, otherwise null.</summary>
        /// <param name="pIndex"></param>
        /// <returns>the VisualChessPiece in the visual chess pieces array at the given index, otherwise null.</returns>
        public VisualChessPiece GetVisualPieceByIndex(int pIndex) { return m_VisualChessPieces[pIndex]; }

        /// <summary>Returns the VisualChessPiece in the given TileIndex, pIndex, otherwise null.</summary>
        /// <param name="pIndex"></param>
        /// <returns>the VisualChessPiece in the given TileIndex, pIndex, otherwise null.</returns>
        public VisualChessPiece GetVisualPieceByTileIndex(TileIndex pIndex)
        {
            foreach (VisualChessPiece chessPiece in m_VisualChessPieces)
            {
                if (chessPiece.Piece.TileIndex == pIndex)
                    return chessPiece;
            }

            return null;
        }
        #endregion

        // Private method(s).
        #region Chess Instance Event Subscription & Unsubscription
        /// <summary>Subscribes to all 'ChessInstance' events.</summary>
        void SubscribeToChessInstanceEvents()
        {
            // Ensure the chess table is non-null.
            if (Table != null)
            {
                // Subscribe to chess table events.
                Table.ChessPieceCreated += OnChessPieceCreated;
                Table.ChessPieceDestroyed += OnChessPieceDestroyed;
                Table.ChessPieceMoved += OnChessPieceMoved;
            }
        }

        /// <summary>Unsubscribes from all 'ChessInstance' events.</summary>
        void UnsubscribeFromChessInstanceEvents()
        {
            // Ensure the chess instance is non-null.
            if (Table != null)
            {
                // Unsubscribe from chess table events.
                Table.ChessPieceCreated -= OnChessPieceCreated;
                Table.ChessPieceDestroyed -= OnChessPieceDestroyed;
                Table.ChessPieceMoved -= OnChessPieceMoved;
            }
        }
        #endregion
        #region Visual Piece Creation
        /// <summary>Instantiates a king and returns it's ChessPiece component, or null.</summary>
        /// <param name="pPiece"></param>
        /// <returns>ChessPiece component of the instantiated king or null.</returns>
        VisualChessPiece CreateVisualKing(ChessPiece pPiece)
        {
            GameObject kingObject = Instantiate(m_KingPiecePrefab, transform, true);
            if (kingObject != null)
            {
                VisualChessPiece piece = kingObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Instantiates a queen and returns it's ChessPiece component, or null.
        /// This method is public because the Pawn needs to beable to replace itself with a queen.
        /// </summary>
        /// <param name="pPiece"></param>
        /// <returns>ChessPiece component of the instantiated queen or null.</returns>
        public VisualChessPiece CreateVisualQueen(ChessPiece pPiece)
        {
            GameObject queenObject = Instantiate(m_QueenPiecePrefab, transform, true);
            if (queenObject != null)
            {
                VisualChessPiece piece = queenObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Instantiates a bishop and returns it's ChessPiece component, or null.
        /// </summary>
        /// <param name="pPiece"></param>
        /// <returns>ChessPiece component of the instantiated bishop or null.</returns>
        VisualChessPiece CreateVisualBishop(ChessPiece pPiece)
        {
            GameObject bishopObject = Instantiate(m_BishopPiecePrefab, transform, true);
            if (bishopObject != null)
            {
                VisualChessPiece piece = bishopObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Instantiates a knight and returns it's ChessPiece component, or null.
        /// </summary>
        /// <param name="pPiece"></param>
        /// <returns>ChessPiece component of the instantiated knight or null.</returns>
        VisualChessPiece CreateVisualKnight(ChessPiece pPiece)
        {
            GameObject knightObject = Instantiate(m_KnightPiecePrefab, transform, true);
            if (knightObject != null)
            {
                VisualChessPiece piece = knightObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Instantiates a rook and returns it's ChessPiece component, or null.
        /// </summary>
        /// <param name="pPiece"></param>
        /// <param name="pIsKingSide">true if the Rook is the king side Rook, otherwise false if it is the queen side Rook.</param>
        /// <returns>ChessPiece component of the instantiated rook or null.</returns>
        VisualChessPiece CreateVisualRook(ChessPiece pPiece)
        {
            GameObject rookObject = Instantiate(m_RookPiecePrefab, transform, true);
            if (rookObject != null)
            {
                VisualChessPiece piece = rookObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Instantiates a pawn and returns it's ChessPiece component, or null.
        /// </summary>
        /// <param name="pPiece"></param>
        /// <returns>ChessPiece component of the instantiated pawn or null.</returns>
        VisualChessPiece CreateVisualPawn(ChessPiece pPiece)
        {
            GameObject pawnObject = Instantiate(m_PawnPiecePrefab, transform, true);
            if (pawnObject != null)
            {
                VisualChessPiece piece = pawnObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        /// <summary>Creates and returns the visual representation of the given engine chess piece.</summary>
        /// <param name="pPiece"></param>
        /// <returns>a VisualChessPiece representation of the engine chess piece, pPiece.</returns>
        VisualChessPiece CreateVisualPiece(ChessPiece pPiece)
        {
            // Handle piece based on type.
            // Type: Pawn.
            if (pPiece is Pawn)
            {
                return CreateVisualPawn(pPiece);
            }
            else if (pPiece is Rook)
            {
                return CreateVisualRook(pPiece);
            }
            else if (pPiece is Knight)
            {
                return CreateVisualKnight(pPiece);
            }
            else if (pPiece is Bishop)
            {
                return CreateVisualBishop(pPiece);
            }
            else if (pPiece is Queen)
            {
                return CreateVisualQueen(pPiece);
            }
            else if (pPiece is King)
            {
                return CreateVisualKing(pPiece);
            }
            else
            {
                Debug.LogWarning("VisualChessTable component failed to create visual piece for a chess piece with an invalid type! (Not pawn, rook, knight, bishop, queen, nor king.)", gameObject);
                return null; 
            }
        }
        #endregion

        // Private callback(s).
        #region Chess Instance Event Callbacks
        /// <summary>Invoked whenever a chess piece is created on 'Table'.</summary>
        /// <param name="pPiece"></param>
        /// <param name="pTileIndex"></param>
        void OnChessPieceCreated(ChessPiece pPiece, TileIndex pTileIndex)
        {
            // Create visual piece if a visualization does not already exist for it.
            VisualChessPiece visualPiece = GetVisualPiece(pPiece);
            if (visualPiece == null)
            {
                // Create visual piece.
                if (!CreateVisualPiece(pPiece))
                    Debug.LogWarning("Failed to create visual representation of chess piece!", gameObject);
            }
        }

        /// <summary>Invoked whenever a chess piece is destroyed on 'Table'.</summary>
        /// <param name="pPiece"></param>
        void OnChessPieceDestroyed(ChessPiece pPiece)
        {
            // Remove visual piece if a visualization exists for it.
            VisualChessPiece visualPiece = GetVisualPiece(pPiece);
            if (visualPiece != null)
            {
                // Destroy visual piece's gameObject.
                Destroy(visualPiece.gameObject);
            }
        }

        /// <summary>Invoked whenever a chess piece is moved on 'Table'.</summary>
        /// <param name="pMoveInfo">The MoveInfo that describes the move.</param>
        void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            //NOTE: Piece movement visualizations are handled by their own components, they will automatically call UpdatePositions() when their underlying ChessPiece moves.
        }
        #endregion
    }
}
