using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ChessDedicatedServer : MonoBehaviour
{
    public NetworkManager m_NetworkManager;
    private Dictionary<int, List<ulong>> m_Sessions; // Dictionary to hold sessions for each game duration

    private void Start()
    {
#if UNITY_SERVER
        ServerFunction();
#else
        ClientFunction();
#endif

        
    }
    private void ClientFunction()
    {
        Destroy(this);
    }

    private void ServerFunction()
    {
        m_NetworkManager = this.GetComponent<NetworkManager>();
        m_NetworkManager.StartServer();
        m_NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        m_NetworkManager.OnClientConnectedCallback += ClientConnected;

        m_Sessions = new Dictionary<int, List<ulong>>();
    }
    private void ClientConnected(ulong clientId)
    {
        Debug.Log("Client Connected with ID : " + clientId);
    }
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Get the requested game duration from the client
        //int requestedGameDuration = request
        int requestedGameDuration = 1;

        // Check if the requested game duration is within a valid range
        if (requestedGameDuration >= 0 && requestedGameDuration <= 20)
        {
            // Check if a session already exists for the requested game duration
            if (!m_Sessions.ContainsKey(requestedGameDuration))
            {
                // Create a new session list for the requested game duration
                m_Sessions.Add(requestedGameDuration, new List<ulong>());
            }

            // Add the player to the session for the requested game duration
            m_Sessions[requestedGameDuration].Add(request.ClientNetworkId);

            // If the session is full (2 players), remove it from the dictionary
            if (m_Sessions[requestedGameDuration].Count == 2)
            {
                m_Sessions.Remove(requestedGameDuration);
            }
        }
        else
        {
            // If the requested game duration is invalid, reject the connection
            response.Approved = false; ;
            return;
        }

        // Approve the connection
        response.Approved = true;
    }
}
