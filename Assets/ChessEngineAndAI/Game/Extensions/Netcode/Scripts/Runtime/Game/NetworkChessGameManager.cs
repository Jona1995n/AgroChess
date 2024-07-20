using UnityEngine;
using UnityEngine.Events;
using ChessEngine.Game.Events;

namespace ChessEngine.Game.Networking
{
    /// <summary>
    /// An implementation of ChessGameManager for networked games.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class NetworkChessGameManager : ChessGameManager
    {
        [Header("Settings - Network")]
        [Tooltip("The team this game manager is on.")]
        public ChessColor team;

        [Header("Events - Network")]
        [Tooltip("[Shared] An event that is invoked when a network game is started.")]
        public UnityEvent NetworkGameStarted;
        [Tooltip("[Shared] An event that is invoked when a network game is stopped.\n\nArg0: ChessColor - The team whose turn it was when the network game was stopped.\nArg1: GameOvereason - The reason the game ended.")]
        public GameOverUnityEvent NetworkGameStopped;
        [Tooltip("[Shared] An event that is invoked on the server when a network opponent is found.")]
        public UnityEvent NetworkOpponentFound;

        /// <summary>Returns true when the game has started, otherwise false.</summary>
        public bool GameStarted { get; private set; }

        // Public method(s).
        /// <summary>Signals that the game instance running this game manager is ready to play.</summary>
        public void Ready()
        {
            // Broadcast ready message to all players.
            BroadcastToAllPlayers("Ready");
        }

        // Private method(s).
        /// <summary>Broadcasts the message pMessage to all players.</summary>
        /// <param name="pMessage"></param>
        void BroadcastToAllPlayers(string pMessage, object pParameter = null, SendMessageOptions pSendOptions = SendMessageOptions.DontRequireReceiver)
        {
            // Broadcast opponent found message to all NetworkChessGamePlayers.
            NetworkChessGamePlayer[] players = FindObjectsOfType<NetworkChessGamePlayer>();
            if (players != null)
            {
                foreach (NetworkChessGamePlayer player in players)
                {
                    player.BroadcastMessage(pMessage, pParameter, pSendOptions);
                }
            }
        }

        // Public override method(s).
        /// <summary>Override piece selection check method to disallow selections of other team and while game is not started.</summary>
        /// <param name="pVisualPiece"></param>
        /// <param name="pVisualTile"></param>
        /// <returns>true of the piece can be selected, otherwise false.</returns>
        public override bool CanSelectPiece(VisualChessPiece pVisualPiece, VisualChessTableTile pVisualTile)
        {
            // Ensure the game has started, otherwise disallow selection.
            if (!GameStarted)
                return false;

            // Ensure this network chess game manager is on the pieces' team, otherwise disallow selection.
            if (pVisualPiece.Piece.Color != team)
                return false;

            // Check if the piece may be selected.
            return base.CanSelectPiece(pVisualPiece, pVisualTile);
        }

        // Protected override callback(s).
        /// <summary>Invoked when a game over event occurs.</summary>
        /// <param name="pTurn"></param>
        /// <param name="pReason"></param>
        protected override void OnGameOver(ChessColor pTurn, GameOverReason pReason)
        {
            // Mark the game as over on the network.
            OnNetworkGameStopped(pTurn, pReason);

            // Run defualt game over stuff.
            base.OnGameOver(pTurn, pReason);
        }

        // Private callback(s).
        /// <summary>Invoked when a network game is stopped.</summary>
        /// <param name="pTurn"></param>
        /// <param name="pReason"></param>
        void OnNetworkGameStopped(ChessColor pTurn, GameOverReason pReason)
        {
            GameStarted = false;

            // Invoke the network game stopped event.
            NetworkGameStopped?.Invoke(pTurn, pReason);
        }

        // Internal callback(s).
        /// <summary>[SHARED] Invoked on both the server and client when a network game is started by a NetworkChessGamePlayer.</summary>
        internal void Internal_OnNetworkGameStarted()
        {
            // Reset the chess pieces after the network game starts.
            ResetGame();

            // Set the 'GameStarted' boolean to true.
            GameStarted = true;

            // Invoke the network game started event.
            NetworkGameStarted?.Invoke();         
        }
        
        /// <summary>[SHARED] Invoked on both the server and client when an opponent joins a server, or on a client when  they join a server.</summary>
        internal void Internal_OnOpponentFound()
        {
            // Invoke the network opponent found Unity event.
            NetworkOpponentFound?.Invoke();

            // Broadcast opponent found message to all NetworkChessGamePlayers.
            BroadcastToAllPlayers("OpponentFound");
        }
    }
}
