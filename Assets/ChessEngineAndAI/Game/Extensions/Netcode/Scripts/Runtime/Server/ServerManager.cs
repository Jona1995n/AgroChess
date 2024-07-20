using UnityEngine;
using UnityEngine.Events;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using ChessEngine.Networking.Events;

namespace ChessEngine.Networking
{
	/// <summary>
	/// A component class that controls a network server instance.
	/// </summary>
    /// Author: Intuitive Gaming Solutions
	public class ServerManager : MonoBehaviour
	{
        [Header("Events")]
        [Tooltip("An event that is invoked whenever a server instance is started on the relevant NetworkManager.\nNOTE: After this event is invoked NetManager is equal to the NetworkManager instance the server started on.")]
        public UnityEvent Started;
        //[Tooltip("An event that is invoked whenever a server instance is stopped on the relevant Networkmanager, NetManager.")]
        //public UnityEvent Stopped;
        [Tooltip("An event that is invoked whenever a call to ServerManager.StartHost or ServerManager.StartServer fails.\nNOTE: This is NOT invoked when a server is not started via this component's provided methods.")]
        public UnityEvent StartFailed;
        [Tooltip("An event that is invoked whenever the server on the NetworkManager associated with this component, NetManager, has a new client connect.\n\nArg0: ulong - the connecting client's network ID.")]
        public ClientIDUnityEvent ClientConnected;
        [Tooltip("An event that is invoked whenever the server on the NetworkManager associated with this component, NetManager, has a client disconnect.\n\nArg0: ulong - the disconnected client's network ID.")]
        public ClientIDUnityEvent ClientDisconnected;

        /// <summary>Returns true if the server is ready to accept clients, otherwise false.</summary>
        public bool IsReady { get; private set; }
        /// <summary>Returns true if we're running a server according to the NetworkManager, otherwise false.</summary>
        public bool IsActive { get { return NetworkManager.Singleton.IsServer; } }
        /// <summary>Returns true if we're hosting a server (acting as a server and a client), otherwise false.</summary>
        public bool IsHost { get { return NetworkManager.Singleton.IsHost; } }
        /// <summary>The password for the server, or null if none.</summary>
        public string Password { get; private set; }
        /// <summary>The NetworkManager instance associated with this ServerManager, otherwise null.</summary>
        public NetworkManager NetManager { get; private set; }

        // Unity callback(s).
        void Start()
        {
#if UNITY_SERVER
        ServerFunction();
#else
            ClientFunction();
#endif
           

        }

        private void ClientFunction()
        {
            Debug.Log("Destroying object on server...");
            Destroy(this);
        }

        private void ServerFunction()
        {
            // Subscribe to Netcode callback(s).
            NetworkManager.Singleton.OnServerStarted += OnStarted;
            //NetworkManager.Singleton.OnServerStopped += OnStopped;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;

            // Track this NetworkManager instance.
            NetManager = NetworkManager.Singleton;
        }
        void OnDestroy()
        {
            if (NetManager != null)
            {
                // Unsubscribe from Netcode callback(s).
                NetManager.OnServerStarted -= OnStarted;
                //NetManager.OnServerStopped -= OnStopped;
                NetManager.OnClientConnectedCallback -= OnClientConnected;
                NetManager.OnClientDisconnectCallback -= OnClientDisconnected;
                if (NetManager.ConnectionApprovalCallback == ConnectionApprovalCallback)
                    NetManager.ConnectionApprovalCallback = null;

                // Stop tracking this NetworkManager instance.
                NetManager = null;
            }
        }

        // Public method(s).
        public void StartHost(string pAddress, ushort pPort, string pPassword = null)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                // Setup transport information.
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport != null)
                {
                    // Set transport information.
                    transport.ConnectionData.Address = pAddress;
                    transport.ConnectionData.Port = pPort;

                    // Setup initial server data.
                    Password = pPassword;

                    // Start the host.
                    StartHost();
                }
                else { Debug.LogError("Failed to StartHost(), no UnityTransport component was found on the NetworkManager!"); }
            }
            else { Debug.LogWarning("Cannot 'Server.StartHost()' while another server instance is already running!"); }
        }

        public void StartHost()
        {
            // Start the host.
            if (NetworkManager.Singleton.StartHost())
            {
                // Log host start.
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                Debug.Log("Host started @ " + transport.ConnectionData.Address + ':' + transport.ConnectionData.Port.ToString() + '!');
            }
            else
            {
                // Log failed to start host.
                Debug.Log("Failed to start the host!");

                // Invoke the 'StartFailed' event when the 
                StartFailed?.Invoke();
            }
        }

        public void StartServer(string pAddress, ushort pPort, string pPassword = null)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                // Setup transport information.
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport != null)
                {
                    // Set transport information.
                    transport.ConnectionData.Address = pAddress;
                    transport.ConnectionData.Port = pPort;

                    // Setup initial server data.
                    Password = pPassword;

                    // Start the server.
                    StartServer();
                }
                else { Debug.LogError("Failed to StartServer(), no UnityTransport component was found on the NetworkManager!"); }
            }
            else { Debug.LogWarning("Cannot 'Server.StartServer()' while another server instance is already running!"); }
        }

        public void StartServer()
        {
            // Start the server.
            if (NetworkManager.Singleton.StartServer())
            {
                // Log successful server start.
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                Debug.Log("Server started @ " + transport.ConnectionData.Address + ':' + transport.ConnectionData.Port.ToString() + '!');
            }
            else
            {
                // Log server start failure.
                Debug.Log("Failed to start the server!");

                // Invoke the 'StartFailed' event when the 
                StartFailed?.Invoke();
            }
        }

        public void Stop()
		{
            NetworkManager.Singleton.Shutdown();
		}
			
		// Private callback(s).	
		/// <summary>
        /// This callback is public to allow for 'spoofed' connections for hosts.
        /// </summary>
        /// <param name="pClientID"></param>
        void OnClientConnected(ulong pClientID)
        {
			// Invoke the client connected event.
            ClientConnected?.Invoke(pClientID);
        }

        /// <summary>
        /// This callback is public to allow for 'spoofed' disconnects for hosts.
        /// </summary>
        /// <param name="pClientID"></param>
        void OnClientDisconnected(ulong pClientID)
        {
			// Invoke the client disconnected event.
            ClientDisconnected?.Invoke(pClientID);
        }

        void OnStarted()
		{
            // The server is ready to accept clients immediately after starting.
            IsReady = true;

            // Execute the server started event.
            Started?.Invoke();
        }
		/*
		void OnStopped()
		{
            // Execute the server stopped event.
            Stopped?.Invoke();

            // The server is no longer ready to accept clients.
            IsReady = false;

            // Unsubscribe from Netcode callback(s).
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.ConnectionApprovalCallback = null;
            //NetworkManager.Singleton.OnServerStopped -= OnStopped;
            NetworkManager.Singleton.OnServerStarted -= OnStarted;
		}
        */

        /// <summary>Invoked to determine whether or not a client has permission to connect to the server.</summary>
        /// <param name="pRequest"></param>
        /// <param name="pResponse"></param>
        void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest pRequest, NetworkManager.ConnectionApprovalResponse pResponse)
        {
            bool canConnect = true;
            if (!IsReady && pRequest.ClientNetworkId != NetworkManager.Singleton.LocalClientId)
            {
                // Log client drop.
                Debug.Log("Client #" + pRequest.ClientNetworkId.ToString() + " dropped! Server was not ready to accept clients.", gameObject);
                //pResponse.Reason = "Server not ready.";

                // Refuse connection.
                canConnect = false;
            }

            // If the server is password protected, see if the provided password matches.
            if (canConnect)
            {
                if (Password != null && Password.Length > 0)
                {
                    if (pRequest.Payload.Length > 0)
                    {
                        if (!Password.Equals(Encoding.ASCII.GetString(pRequest.Payload)))
                        {
                            canConnect = false; // Cannot connect, incorrect password given.
                            //pResponse.Reason = "Incorrect password.";

                            // Log dropped client.
                            Debug.Log("Client #" + pRequest.ClientNetworkId.ToString() + " dropped! Incorrect password.", gameObject);
                        }
                    }
                    else
                    {
                        canConnect = false; // Cannot connect, password required but none provided.
                        //pResponse.Reason = "Incorrect password.";

                        // Log dropped client due to no password provided.
                        Debug.Log("Client #" + pRequest.ClientNetworkId.ToString() + " dropped! No password provided.", gameObject);
                    }
                }
            }

            // Generate connection response.
            pResponse.Approved = canConnect;
            pResponse.CreatePlayerObject = true;
            pResponse.PlayerPrefabHash = null; // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used.
            pResponse.Position = Vector3.zero;
            pResponse.Rotation = Quaternion.identity;

            // Submit connection response.
            pResponse.Pending = false;

            // Log successful connect.
            Debug.Log("Client #" + pRequest.ClientNetworkId.ToString() + " connected!", gameObject);
        }
	}
}
