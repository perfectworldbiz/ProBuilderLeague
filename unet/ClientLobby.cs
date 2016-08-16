using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientLobby : NetworkBehaviour {

    NetworkIdentity net;

    public GameObject PanelLobby;
    public Text ServerIP;
    public Text ClientIP;
    public GameObject clientReady;

    void OnConnectedToServer()
    {
        net = GetComponent<NetworkIdentity>();
        if (net.isClient && !net.isServer)
        { 
            Debug.LogWarning("as a client we connected");
            TogglePanel(PanelLobby);
            TogglePanel(clientReady);
            //string sip = GameObject.Find("NetworkControl").GetComponent<NetworkManager>().networkAddress;
            ServerIP.text = "Serwer: " + Network.player.externalIP;
            ClientIP.text = "Klient: " + Network.player.ipAddress + " Nie gotowy";
        }
    }
    [Command]
    void CmdInformServer()
    {

    }

    void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }
}
