using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkControlOld : MonoBehaviour {

    public int xpos;
    public int ypos;
    NetworkDiscovery discover;
    NetworkManager manager;

    void Start()
    {
        discover = GetComponent<NetworkDiscovery>();
        manager = GetComponent<NetworkManager>();
    }

	public void ServerBroadcast()
    {
        manager.StartHost();
        Initialize();
        discover.StartAsServer();
    }
    public void ClientBroadcast()
    {
        Initialize();
        discover.StartAsClient();
    }
    public void StopBroadcast()
    {
        if (discover.running)
            discover.StopBroadcast();

    }
    void Initialize()
    {
        if(!discover.running)
            discover.Initialize();
    }

    void OnGUI()
    {
        /*const int spacing = 24;
        if (discover.broadcastsReceived != null)
        {
            foreach (var addr in discover.broadcastsReceived.Keys)
            {
                var value = discover.broadcastsReceived[addr];
                if (GUI.Button(new Rect(xpos, ypos + 20, 200, 20), "Game at " + addr) && discover.useNetworkManager)
                {
                    string dataString = BytesToString(value.broadcastData);
                    var items = dataString.Split(':');
                    if (items.Length == 3 && items[0] == "NetworkManager")
                    {
                        if (NetworkManager.singleton != null && NetworkManager.singleton.client == null)
                        {
                            NetworkManager.singleton.networkAddress = items[1];
                            NetworkManager.singleton.networkPort = Convert.ToInt32(items[2]);
                            NetworkManager.singleton.StartClient();
                        }
                    }
                }
                ypos += spacing;
            }
        }*/
    }
}
