using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Client : MonoBehaviour
{
    public NetworkManager m_NetworkManager;
    private bool Connected = false;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SERVER
        ServerFunction();
#else
        ClientFunction();
#endif
    }
    public bool ConnectedtoServer()
    {
        return Connected;
    }

    private void ClientFunction()
    {
        // Implement client-specific logic here
        Debug.Log("Starting client...");
        m_NetworkManager = this.GetComponent<NetworkManager>();
        m_NetworkManager.StartClient();
        m_NetworkManager.OnClientConnectedCallback += M_NetworkManager_OnClientConnectedCallback;
        
    }

    private void M_NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        Connected = true;
        Debug.Log("Sucessfully Connected to server");
        throw new System.NotImplementedException();
    }

    private void ServerFunction()
    {
        // Implement server-specific logic here
        Debug.Log("Destroying object on server...");
        Destroy(this);
    }
}
