using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ServerLobby : NetworkBehaviour {

    bool countTime = false;
    float time = 0;

    public GameObject WaitPanel;
    public Text countText;
    public GameObject PanelLobby;
    public Text ServerIP;
    public Text ClientIP;
    public GameObject serverStart;

    NetworkIdentity net;

	void Start ()
    {
        net = GetComponent<NetworkIdentity>();
        if (net.isServer)
            ShowWaitPanel();
        Test();
	}
	void Test()
    {
        print("NETWORK Is Server and Is client: " + Network.isServer + " and " + Network.isClient);
        print("IDENTITY Is Server and Is client: " + net.isServer + " and " + net.isClient);
    }
	void ShowWaitPanel()
    {
        TogglePanel(WaitPanel);
        countTime = true;
    }

    void Update()
    {
        if (countTime)
        {
            time += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string timeText = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            countText.text = ":    " + timeText;
        }
    }
    void OnPlayerConnected(NetworkPlayer player)
    {
        print("Ktoś się dołączył");
        TogglePanel(WaitPanel);
        TogglePanel(PanelLobby);
        TogglePanel(serverStart);

        string sip = GameObject.Find("NetworkControl").GetComponent<NetworkManager>().networkAddress;
        ServerIP.text = "Serwer: " + sip;
        ClientIP.text = "Klient: " + player.ipAddress + " Nie gotowy";
    }

    void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }

    public void DisconnectButton()
    {
        GameObject NetMan = GameObject.Find("NetworkControl");
        NetMan.GetComponent<MyDiscovery>().StopBroadcast();
        //NetMan.GetComponent<NetworkManager>().StopServer();
        NetMan.GetComponent<NetworkManager>().StopHost();
        Destroy(NetMan);
        //Network.Disconnect();
        SceneManager.LoadScene(0);
    }
}
