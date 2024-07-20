using System;
using UnityEngine.Events;

namespace ChessEngine.Game.Events
{
    /// <summary>
    /// Arg0: ChessColor    - The team whose turn was ended.
    /// Arg1: MoveInfo      - The MoveInfo from the turn that was ended. 
    /// </summary>
    [Serializable]
    public class EndTurnUnityEvent : UnityEvent<ChessColor, MoveInfo> { }
}
