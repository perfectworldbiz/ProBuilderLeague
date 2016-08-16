using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NetworkControlNew : NetworkBehaviour {

    public int xpos;
    public int ypos;
    MyDiscovery discover;
    NetworkManager manager;
    public GameObject serverButton;
    public GameObject serverList;
    bool inMenu = true;
    bool created = false;
    public List<string> displayedAddress = new List<string>();
    int nOfServers = 0;
    float timeToClearList = 2f;
    void Awake()
    {
        if (!created)
        {
            // this is the first instance - make it persist
            DontDestroyOnLoad(this.gameObject);
            created = true;
            gameObject.AddComponent<MyDiscovery>();
        }
        else {
            // this must be a duplicate from a scene reload - DESTROY!
            Destroy(this.gameObject);
        }
        GetComponents();
    }

    void GetComponents(){
        discover = GetComponent<MyDiscovery>();
        manager = GetComponent<NetworkLobbyManager>();
    }

	public void ServerBroadcast()
    {
        ResetNetwork();
        manager.StartHost();
        Initialize();
        discover.StartAsServer();
    }
    public void ClientBroadcast()
    {
        Initialize();
        discover.StartAsClient();
    }
    public void ResetNetwork()
    {
        GetComponents();
        if (discover.running)
            discover.StopBroadcast();

    }
    void Initialize()
    {
        if(!discover.running)
            discover.Initialize();
    }

    public void PopulateServers(string fromAddress, string data)
    {
        if (!inMenu)
            return;
        print("OnReceivedBroadcast in control");
        if (discover.broadcastsReceived != null)
        {
            timeToClearList = 2f;
            //if list changed, build a new one
            if(nOfServers != discover.broadcastsReceived.Keys.Count)
            {
                ClearServerList();

                //populate the list with servers
                nOfServers = 0;
                foreach (string address in discover.broadcastsReceived.Keys)
                {
                    nOfServers++;
                    //var value = discover.broadcastsReceived[address];
                    displayedAddress.Add(address);
                    print("Server " + nOfServers + " found");
                    GameObject button = (GameObject)Instantiate(serverButton, Vector3.zero, Quaternion.identity);
                    button.transform.SetParent(serverList.transform);
                    button.GetComponentInChildren<Text>().text = "Serwer - " + address.Remove(0, 7);
                    button.name = address;
                    button.GetComponent<Button>().onClick.AddListener(delegate { Connect(button.name, data); });
                }
            }
            
        }
    }
    void ClearServerList()
    {
        if (!inMenu)
            return;
        print("Updating list");
        //destroy old list
        displayedAddress.Clear();
        for (int i = serverList.transform.childCount; i > 0; i--)
        {
            Destroy(serverList.transform.GetChild(i - 1).gameObject);
        }
    }

    public void Connect(string address, string data)
    {
        print("button pressed, address is " + address);
        if (!discover.useNetworkManager)
            return;
        print("button past use networkmanager");
        /*
        var value = discover.broadcastsReceived[address];
        string dataString = data; //Convert.ToBase64String(value.broadcastData);
        var items = dataString.Split(':');*/
        //if (items.Length == 3 && items[0] == "NetworkManager")
        //{
            if (NetworkManager.singleton != null && NetworkManager.singleton.client == null)
            {
            print("arrived in connect code");
            NetworkManager.singleton.networkAddress = address;
                //NetworkManager.singleton.networkPort = Convert.ToInt32(items[2]);
                NetworkManager.singleton.StartClient();
            }
        //}
        inMenu = false;
    }

    void Update()
    {
        if (!inMenu)
            return;
        timeToClearList -= Time.deltaTime;
        if(timeToClearList < 0 && displayedAddress.Count > 0)
        {
            ClearServerList();
        }
    }
    void OnPlayerConnected()
    {
        print("someone connected");
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
        }
         public void PopulateServers(string fromAddress, string data)
    {
        if (!inMenu)
            return;
        print("OnReceivedBroadcast in control");
        if (discover.broadcastsReceived != null)
        {
            //if list has children, destroy them
            if(serverList.transform.childCount > 0)
            {
                displayedAddress.Clear();
                for (int i = serverList.transform.childCount; i>0; i--)
                {
                    Destroy(serverList.transform.GetChild(i - 1).gameObject);
                }
            }
            //populate the list with servers
            int amount = 0;
            foreach (string address in discover.broadcastsReceived.Keys)
            {

                //var value = discover.broadcastsReceived[address];
                displayedAddress.Add(address);
                print("Server "+ amount + " found");
                GameObject button = (GameObject)Instantiate(serverButton, Vector3.zero, Quaternion.identity);
                button.transform.SetParent(serverList.transform);
                button.GetComponentInChildren<Text>().text = "Serwer - " + address.Remove(0, 7);
                button.GetComponent<Button>().onClick.AddListener(delegate { Connect(displayedAddress[amount-1], data); });
                amount++;
            }
        }
    }
        */
    }
}
