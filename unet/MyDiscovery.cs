using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyDiscovery : NetworkDiscovery {

    void Awake()
    {
        showGUI = false;
        broadcastKey = 1984;
    }
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        print("OnReceivedBroadcast in mydiscovery");
        GetComponent<NetworkControlNew>().PopulateServers(fromAddress, data);
    }
}
