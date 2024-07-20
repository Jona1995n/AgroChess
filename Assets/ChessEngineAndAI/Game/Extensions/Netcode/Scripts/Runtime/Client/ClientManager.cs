using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using ChessEngine.Networking.Events;

namespace ChessEngine.Networking
{
	/// <summary>
	/// A component class that controls a network client instance.
	/// </summary>
    /// Author: Intuitive Gaming Solutions
	public class ClientManager : MonoBehaviour
	{
		[Header("Events")]
		[Tooltip("An event that is invoked whenever the client on the NetworkManager associated with this component, NetManager, connects to a server as a client.\n\nArg0: ulong - the connecting client's network ID.")]
		public ClientIDUnityEvent ClientConnected;
		[Tooltip("An event that is invoked whenever the client on the NetworkManager associated with this component, NetManager, disconnects from a server.\n\nArg0: ulong - the disconnected client's network ID.")]
		public ClientIDUnityEvent ClientDisconnected;
		[Tooltip("An event that is invoked whenever the client on the NetworkManager associated with this component, NetManager, fails to connect to a server for any reason.\n\nArg0: ulong - the client who failed to connect's network ID.")]
		public ClientIDUnityEvent ClientConnectFailed;

		/// <summary>The NetworkManager instance associated with this ClientManager, otherwise null.</summary>
		public NetworkManager NetManager { get; private set; }
		/// <summary>Returns true if this ClientManager's relevant NetworkManager is connected as a client, otherwise false.</summary>
		public bool IsConnected { get; private set; }

        // Unity callback(s).
        void OnDestroy()
        {
			// If there is a valid referenced NetworkManager instance...
			if (NetManager != null)
			{
				// Unregister network events.
				NetManager.OnClientConnectedCallback -= OnClientConnected;
				NetManager.OnClientDisconnectCallback -= OnClientDisconnected;

				// Stop tracking NetManager instance.
				NetManager = null;
			}
        }

        // Public method(s).
        /// <summary>
        /// Connects to a server using the existing information in the network transport.
        /// </summary>
        public void Connect()
        {
			// Register network events.
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

			// Track NetworkManager instance.
			NetManager = NetworkManager.Singleton;

			// Start client.
			if (NetworkManager.Singleton.StartClient())
            {
				// Log connection success.
				UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
				Debug.Log("Connecting to host @ " + transport.ConnectionData.Address + ':' + transport.ConnectionData.Port.ToString() + "...");
			}
			else
            {
				// Failed to connect to server, log result.
				Debug.Log("Failed to connect to server!", gameObject);
            }
        }

		// Private callback(s).
		/// <summary>
		/// This callback is public to allow for 'spoofed' connections for hosts.
		/// </summary>
		/// <param name="pClientID"></param>
		void OnClientConnected(ulong pClientID)
		{
			// Track connected status.
			IsConnected = true;

			// Invoke the 'Client Connected' unity event.
			ClientConnected?.Invoke(pClientID);
		}

		/// <summary>
		/// This callback is public to allow for 'spoofed' disconnects for hosts.
		/// </summary>
		/// <param name="pClientID"></param>
		void OnClientDisconnected(ulong pClientID)
		{
			// Track 'was connected'.
			bool wasConnected = IsConnected;

			// Track disconnected status.
			IsConnected = false;

			// Invoke relevant event (disconnect or connect failed) based on 'wasConnected'.
			if (wasConnected)
			{
				// Disconnect event.
				ClientDisconnected?.Invoke(pClientID);
			}
			else
            {
				// Connection failed event.
				ClientConnectFailed?.Invoke(pClientID);
            }
		}
	}
}
