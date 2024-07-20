using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace ChessEngine.Networking.Events
{
    /// <summary>
    /// A simple NetworkBehaviour component that dispatches key editor events.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class NetworkEventDispatcher : NetworkBehaviour
    {
        [Header("Clientside Events")]
        [Tooltip("An event that is invoked when OnNetworkSpawn() is invoked while executing code as a client.")]
        public UnityEvent ClientNetworkSpawned;
        [Tooltip("An event that is invoked when OnNetworkDespawn() is invoked while executing code as a client.")]
        public UnityEvent ClientNetworkDespawned;

        [Header("Serverside Events")]
        [Tooltip("An event that is invoked when OnNetworkSpawn() is invoked while executing code as a server.")]
        public UnityEvent ServerNetworkSpawned;
        [Tooltip("An event that is invoked when OnNetworkDespawn() is invoked while executing code as a server.")]
        public UnityEvent ServerNetworkDespawned;

        [Header("Host Events")]
        [Tooltip("An event that is invoked when OnNetworkSpawn() is invoked while executing code as a host.")]
        public UnityEvent HostNetworkSpawned;
        [Tooltip("An event that is invoked when OnNetworkDespawn() is invoked while executing code as a host.")]
        public UnityEvent HostNetworkDespawned;

        // Public override callback(s).
        public override void OnNetworkSpawn()
        {
            // Invoke base method.
            base.OnNetworkSpawn();

            // Invoke spawn event(s).
            if (IsHost)
            {
                HostNetworkSpawned?.Invoke();
            }
            else if (IsClient)
            {
                ClientNetworkSpawned?.Invoke();
            }
            else if (IsServer) { ServerNetworkSpawned?.Invoke(); }         
        }

        public override void OnNetworkDespawn()
        {
            // Invoke base method.
            base.OnNetworkDespawn();

            // Invoke despawn event(s).
            if (IsHost)
            {
                HostNetworkDespawned?.Invoke();
            }
            else if (IsClient)
            {
                ClientNetworkDespawned?.Invoke();
            }
            else if (IsServer) { ServerNetworkDespawned?.Invoke(); }
        }
    }
}
