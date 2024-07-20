using System;
using UnityEngine.Events;

namespace ChessEngine.Networking.Events
{
    // ClientIDUnityEvent.
    /// <summary>
    /// Arg0: ulong - a network client ID.
    /// </summary>
    [Serializable]
    public class ClientIDUnityEvent : UnityEvent<ulong> { };
}
