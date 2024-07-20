using System;
using UnityEngine.Events;

namespace ChessEngine.Game.Clickables.Events
{
    /// <summary>
    /// Arg0: Clickable - The last hovered Clickable, or null.
    /// Arg1: Clickable - The new hovered Clickable, or null.
    /// </summary>
    [Serializable]
    public class HoverTargetChangedUnityEvent : UnityEvent<Clickable, Clickable> { }
}
