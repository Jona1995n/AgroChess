using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace ChessEngine.Game.Networking
{
    /// <summary>
    /// When placed on the same GameObject as a NetworkGameManager component this component manages a multiplayer chess match.
    /// The host drives the team selection and game start.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class NetworkChessGamePlayer : NetworkBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked when the network chess game player is spawned on the network.")]
        public UnityEvent NetworkSpawned;

        /// <summary>Returns true if this listener is ready to play, otherwise false.</summary>
        public bool IsReady { get; private set; }
        /// <summary>Returns the NetworkManager that is controlling the ticks of this component.</summary>
        public NetworkManager NetManager { get; private set; }
        /// <summary>Returns a reference to the NetworkChessGameManager component associated with this component.</summary>
        public NetworkChessGameManager GameManager { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find the NetworkChessGameManager reference.
            GameManager = FindObjectOfType<NetworkChessGameManager>();
            if (GameManager == null)
                Debug.LogError("NETWORK ERROR: No 'NetworkChessGameManager' found in scene! Unable to register game manager with network chess game player.", gameObject);
        }

        void OnEnable()
        {
            // Store NetworkManager reference.
            NetManager = NetworkManager.Singleton;

            // Subscribe to GameManager events related to syncing the board.
            GameManager.ChessPieceMoved.AddListener(OnChessPieceMoved);
        }

        void OnDisable()
        {
            // Unsubscribe from GameManager events related to syncing the board.
            if (GameManager != null)
            {
                GameManager.ChessPieceMoved.RemoveListener(OnChessPieceMoved);
            }
        }

        // Custom callback(s).
        void OpponentFound()
        {
            // If this is the not the server's player and code is being executed on the server determine who will play as what team.
            if (!IsLocalPlayer && IsServer)
            {
                // TEAM SELECTION.
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    // Server is white team, send black team to client.
                    ReceiveTeamClientRpc(ChessColor.Black);

                    // Set team of server's game manager.
                    GameManager.team = ChessColor.White;
                }
                else
                {
                    // Server is black team, send white team to client.
                    ReceiveTeamClientRpc(ChessColor.White);

                    // Set team of server's game manager.
                    GameManager.team = ChessColor.Black;
                }
            }
        }

        void Ready()
        {
            // Only execute this custom callback on the local player's object.
            if (IsLocalPlayer)
            {
                // Mark as ready.
                ReceiveReadyServerRpc();
            }
        }

        // Netcode callback(s).
        /// <summary>Invoked automatically once this listener is spawned on the network.</summary>
        public override void OnNetworkSpawn()
        {
            // Not ready to play.
            IsReady = false;

            // Invoke the 'NetworkSpawned' event.
            NetworkSpawned?.Invoke();

            // If this is not the local player then an opponent was found.
            if (!IsLocalPlayer)
            {
                // Otherwise if the player is a client and the NetManager is the server (code is being executed on the server) then an opponent has been found.
                GameManager.Internal_OnOpponentFound();
            }
        }

        // Private RPC callback(s).
        #region Client RPC
        /// <summary>Invoked by the server and executed on the client. Receives the client's team.</summary>
        /// <param name="pInputs"></param>
        [ClientRpc]
        void ReceiveTeamClientRpc(ChessColor pTeam, ClientRpcParams pClientRpcParams = default)
        {
            // When executing code on the server it ignores team rpc since it sets the teams.
            if (!IsServer)
            {
                // Set relevant NetworkChessGameManager's 'team' to the received color.
                GameManager.team = pTeam;
            }
        }

        /// <summary>Invoked by the server and executed on the client. Receives a notice that the network game has started.</summary>
        /// <param name="pClientRpcParams"></param>
        [ClientRpc]
        void ReceiveGameStartedClientRpc(ClientRpcParams pClientRpcParams = default)
        {
            // Invoke network game started callback on the network game manager.
            GameManager.Internal_OnNetworkGameStarted();
        }

        /// <summary>Invoked by the server and executed on the client. Performs a move that was done by a network peer.</summary>
        /// <param name="pFrom"></param>
        /// <param name="pTo"></param>
        /// <param name="pClientRpcParams"></param>
        [ClientRpc]
        void ReceiveMoveClientRpc(TileIndex pFrom, TileIndex pTo, ClientRpcParams pClientRpcParams = default)
        {
            // Ignore moves from local player (incase we are a host).
            if (IsLocalPlayer)
                return;

            // Ensure there is a valid tile at 'pFrom'.
            ChessTableTile fromTile = GameManager.ChessInstance.Table.GetTile(pFrom);
            if (fromTile != null)
            {
                // Ensure there is a valid piece on 'fromTile'.
                ChessPiece tilePiece = fromTile.GetPiece();
                if (tilePiece != null)
                {
                    // Ensure it is tilePiece's turn.
                    if (GameManager.ChessInstance.turn == tilePiece.Color)
                    {
                        // Ensure there is a valid tile at 'pTo'.
                        ChessTableTile toTile = GameManager.ChessInstance.Table.GetTile(pTo);
                        if (toTile != null)
                        {
                            // Confirm the move is legal.
                            var validMoves = tilePiece.GetValidMoves();
                            var validAttacks = tilePiece.GetValidAttacks();
                            if (validMoves.Contains(toTile) || ChessTableTile.IsTileAttackable(validAttacks, toTile))
                            {
                                // Move the 'tilePiece' to the 'pTo' tile attacking any piece on 'toTile'.
                                MoveInfo moveInfo = tilePiece.Move(pTo, toTile.GetPiece());

                                // Reset selection.
                                GameManager.Deselect();

                                // End the turn.
                                GameManager.ChessInstance.EndTurn(moveInfo);
                            }
                            else { Debug.LogWarning("NetworkChessGamePlayer received 'best move' that was non-legal. (From Tile index: " + pFrom + " | To Tile index: " + pTo + ")", gameObject); }
                        }
                        else { Debug.LogWarning("NetworkChessGamePlayer received 'best move' with invalid 'to' tile. (Tile index: " + pTo + ")", gameObject); }
                    }
                    else { Debug.LogWarning("NetworkChessGamePlayer received 'best move' for team '" + tilePiece.Color + "' while it is team '" + GameManager.ChessInstance.turn + "' turn! Either best move was received out-of-turn order or AI is considering pieces that do not belong to it. (From Tile index: " + pTo + ")", gameObject); }
                }
                else { Debug.LogWarning("ChessAIGameManager received 'best move' with empty 'from' tile. (Tile index: " + pFrom + ")", gameObject); }
            }
            else { Debug.LogWarning("ChessAIGameManager received 'best move' with invalid 'from' tile. (Tile index: " + pFrom + ")", gameObject); }
        }
        #endregion

        #region Server RPC
        /// <summary>Invoked by the client and executed on the server. Once executed on the server the server knows the client is ready.</summary>
        /// <param name="pServerRpcParams"></param>
        [ServerRpc]
        void ReceiveReadyServerRpc(ServerRpcParams pServerRpcParams = default)
        {
            // Mark as ready.
            IsReady = true;

            // Invoke OnReadyReceived() callback.
            OnReadyReceived();
        }

        /// <summary>Invoked by the client and executed on the server. Once executed on the server the server then sends the movements to all clients.</summary>
        /// <param name="pFromTile">The TileIndex being moved from.</param>
        /// <param name="pToTile">The TileIndex being moved to.</param>
        /// <param name="pServerRpcParams"></param>
        [ServerRpc]
        void ReceiveMoveServerRpc(TileIndex pFromTile, TileIndex pToTile, ServerRpcParams pServerRpcParams = default)
        {
            // Find the correct client to send the movement to.
            ulong targetClientID = ulong.MaxValue;
            foreach (ulong clientID in NetManager.ConnectedClientsIds)
            {
                if (clientID != pServerRpcParams.Receive.SenderClientId)
                {
                    targetClientID = clientID;
                    break;
                }
            }

            if (targetClientID != ulong.MaxValue)
            {
                // Send move to proper client(s).
                ReceiveMoveClientRpc(
                    pFromTile, pToTile,
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[1] { targetClientID }
                        }
                    }
                );
            }
            else { Debug.LogWarning("NETWORK WARNING: Failed to find target client to send move to!", gameObject); }
        }
        #endregion

        // Private callback(s).
        /// <summary>Invoked when a chess piece performs the move described by pMoveInfo.</summary>
        /// <param name="pVisualPiece">The VisualChessPiece that was moved.</param>
        /// <param name="pMoveInfo"></param>
        void OnChessPieceMoved(VisualChessPiece pVisualPiece, MoveInfo pMoveInfo)
        {
            // Only sync moves performed by the owner.
            if (IsOwner)
            {
                // Only sync moves performed by this player.
                if (pMoveInfo.piece.Color == GameManager.team)
                {
                    // Send move over the network.
                    ReceiveMoveServerRpc(pMoveInfo.fromTileIndex, pMoveInfo.toTileIndex);
                }
            }
        }

        // Private network callback(s).
        /// <summary>[SERVER] Invoked on the server when this component recives a ready message.</summary>
        void OnReadyReceived()
        {
            // Check if all NetworkChessGamePlayer components are ready, if all are ready start a game (if not already started).
            if (!GameManager.GameStarted)
            {
                // Start with state as ready.
                bool ready;
                NetworkChessGamePlayer[] listeners = FindObjectsOfType<NetworkChessGamePlayer>();
                if (listeners != null && listeners.Length > 1)
                {
                    ready = true;
                    foreach (NetworkChessGamePlayer listener in listeners)
                    {
                        if (!listener.IsReady)
                        {
                            // Not ready since a non-ready listener was found.
                            ready = false;
                            break;
                        }
                    }
                }
                else { ready = false; } // Not ready.

                // If ready start the game.
                if (ready)
                {
                    ReceiveGameStartedClientRpc();
                }
            }
        }
    }
}
