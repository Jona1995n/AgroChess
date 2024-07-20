using System;
using UnityEngine;

namespace ChessEngine.Game.Gamemodes.PieceTypeSteal
{
    /// <summary>
    /// A simple component that is intended to be attached to any GameObject in the scene (the game manager is likely most appropriate) that overrides chess game behaviors to implement a
    /// gamemode where your piece becomes the type of piece that it captures.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PieceTypeStealGamemode : MonoBehaviour
    {
        // Unity callback(s).
        void OnEnable()
        {
            // Subscribe to static 'Instance' events so that any instantiated instance has it's rules overridden.
            Instance.Initialized += OnInstanceInitialized;
            Instance.Deinitialized += OnInstanceDeinitialized;
        }

        void OnDisable()
        {
            // Unsubscribe from static 'Instance' events so that any instantiated instance has it's rules overridden.
            Instance.Initialized -= OnInstanceInitialized;
            Instance.Deinitialized -= OnInstanceDeinitialized;
        }

        // Protected callback(s).
        #region Chess Instance Static Callback(s)
        /// <summary>Invoked just afer any chess Instance is constructed.</summary>
        /// <param name="pInstance"></param>
        protected void OnInstanceInitialized(Instance pInstance)
        {
            // Subscribe to relevant instance event(s).
            pInstance.ChessPieceMoved += OnChessPieceMoved;
        }

        /// <summary>Invoked just after any chess Instance is deconstructed.</summary>
        /// <param name="pInstance"></param>
        protected void OnInstanceDeinitialized(Instance pInstance)
        {
            // Unsubscribe from relevant instance event(s).
            pInstance.ChessPieceMoved -= OnChessPieceMoved;
        }
        #endregion
        #region Chess Instance Specific Callback(s)
        /// <summary>Invoked whenever a chess piece is moved on any chess Instance that is being handled by this component.</summary>
        /// <param name="pMoveInfo"></param>
        protected void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            // Only handle moves by non-kings.
            ChessPieceType movedPieceType = pMoveInfo.piece.GetChessPieceType();
            if (movedPieceType != ChessPieceType.King)
            {
                // NOTE: en-passant is not considered as it is a pawn-on-pawn event only.
                // If a piece was captured perform the type-stealing behaviour.
                if (pMoveInfo.capturedPiece != null)
                {
                    // Detect if the move is by a pawn that will be queen'd this turn.
                    bool isPawnQueenedMove = false;
                    if (movedPieceType == ChessPieceType.Pawn)
                    {
                        // Check if the pawn has reached the end of the board in its forward direction, if so it will be replaced with a queen.
                        if (pMoveInfo.piece.Color == ChessColor.White)
                        {
                            // White team.
                            if (pMoveInfo.piece.TileIndex.y == 7)
                                isPawnQueenedMove = true;
                        }
                        else
                        {
                            // Black team.
                            if (pMoveInfo.piece.TileIndex.y == 0)
                                isPawnQueenedMove = true;
                        }
                    }

                    // Only handle the move if it is not a pawn becoming queened move.
                    if (!isPawnQueenedMove)
                    {
                        // If the moved piece's type does not match the captured piece's type then 'steal' the piece type. (The king check is likely unneccesary but just in case.)
                        ChessPieceType victimPieceType = pMoveInfo.capturedPiece.GetChessPieceType();
                        if (movedPieceType != victimPieceType && victimPieceType != ChessPieceType.King)
                        {
                            // Store the chess table reference.
                            ChessTable table = pMoveInfo.piece.Table;
                            ChessColor color = pMoveInfo.piece.Color;

                            // Destroy the chess piece that moved.
                            table.DestroyPiece(pMoveInfo.piece);

                            // Create a 'copy' chess piece on the attacked tile.
                            table.CreatePieceByType(victimPieceType, pMoveInfo.capturedPiece.TileIndex, color);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
