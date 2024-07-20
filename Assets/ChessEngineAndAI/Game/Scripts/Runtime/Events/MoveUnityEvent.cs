using System;
using UnityEngine.Events;

namespace ChessEngine.Game.Events
{
    /// <summary>
    /// Arg0: ChessPiece    - The ChessPiece that was moved.
    /// Arg1: MoveInfo      - The MoveInfo about the move.
    /// </summary>
    [Serializable]
    public class MoveUnityEvent : UnityEvent<VisualChessPiece, MoveInfo> { }
}
