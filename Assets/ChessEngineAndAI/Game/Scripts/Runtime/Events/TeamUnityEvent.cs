using System;
using UnityEngine.Events;

namespace ChessEngine.Game.Events
{
    /// <summary>
    /// Arg0: ChessColor    - The team who is involved in the event.
    /// </summary>
    [Serializable]
    public class TeamUnityEvent : UnityEvent<ChessColor> { }
}
