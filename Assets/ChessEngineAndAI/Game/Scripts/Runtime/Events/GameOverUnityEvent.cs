using System;
using UnityEngine.Events;

namespace ChessEngine.Game.Events
{
    /// <summary>
    /// Arg0: ChessColor        - The team whose turn it was when the game ended.
    /// Arg1: GameOverReason    - The reason the game ended.
    /// </summary>
    [Serializable]
    public class GameOverUnityEvent : UnityEvent<ChessColor, GameOverReason> { }
}
