using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Photon;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotonNetworkManager : Photon.PunBehaviour
{
    string Version = "cubeblaster03";
    public MenuController menu;
    private string roomName = "Room";
    RoomInfo[] roomsList;
    RoomOptions defaultRoom;
    TypedLobby sqlLobby;
    //
    
    public GameObject serverButton;
    public GameObject serverList;
    bool inSearchMenu = true;
    bool playerReady = false;
    public List<string> displayedAddress = new List<string>();
    int numberOfServers = 0;
    //float timeToClearList = 2f;

    public Vector2 WidthAndHeight = new Vector2(600, 400);
    //private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;
    private string errorDialog;
    private double timeToClearDialog;

    public string ErrorDialog
    {
        get { return this.errorDialog; }
        private set
        {
            this.errorDialog = value;
            if (!string.IsNullOrEmpty(value))
            {
                this.timeToClearDialog = Time.time + 4.0f;
            }
        }
    }
    void Start()
    {
        Connect();
    }
    public void Connect()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings(Version);
        }

        // generate a name for this player, if none is assigned yet

        PhotonNetwork.playerName = "CubeBlaster" + Random.Range(0, 100);
        roomName = "Serwer: " +PhotonNetwork.playerName;

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }
    
    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = 2 }, null);
        playerReady = false;
    }
    public void SearchGame(bool cond)
    {
        inSearchMenu = cond;
    }
    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        if (!inSearchMenu)
            return;
        if (PhotonNetwork.GetRoomList().Length == 0)
            menu.ShowHourglass(true);
        else
            menu.ShowHourglass(false);
        if (PhotonNetwork.GetRoomList().Length != numberOfServers)
        {
            //if list changed, build a new one
           ClearServerList();

           //populate the list with servers
           numberOfServers = 0;
           foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
           {
               numberOfServers++;
               displayedAddress.Add(roomInfo.name);
               print("Server " + numberOfServers + " found");
               GameObject button = (GameObject)Instantiate(serverButton, Vector3.zero, Quaternion.identity);
               button.transform.SetParent(serverList.transform);
               button.GetComponentInChildren<Text>().text = roomInfo.name;
               button.name = roomInfo.name;
               button.GetComponent<Button>().onClick.AddListener(delegate { JoinRoomName(button.name); });
           }
        }
    }
    void ClearServerList()
    {
        if (!inSearchMenu)
            return;
        print("Updating list");
        //destroy old list
        displayedAddress.Clear();
        for (int i = serverList.transform.childCount; i > 0; i--)
        {
            Destroy(serverList.transform.GetChild(i - 1).gameObject);
        }
    }
    void JoinRoomName(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void StartGame()
    {
        if(PhotonNetwork.isMasterClient && playerReady)
        {
            PhotonNetwork.room.visible = false;
            PhotonNetwork.room.open = false;
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            //tell server player is not ready
            menu.ShowPopup(MenuController.Popup.ClientNotReady);
        }
    }
    [PunRPC]
    void UpdateStrings(string name, bool ready)
    {
        menu.UpdateTexts(PhotonNetwork.room.name, name, ready);
        playerReady = ready;
    }
    public void ToggleClientReady()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            photonView.RPC("UpdateStrings", PhotonTargets.All, PhotonNetwork.player.name, !playerReady);
        }
    }
    public void SetName(string name)
    {
        PhotonNetwork.playerName = name;
        roomName = "Serwer: " + PhotonNetwork.playerName;
        print("name changed to " + name);
    }
    public void OpenRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.open = true;
            PhotonNetwork.room.visible = true;
        }
    }
    // ------------Callbacks-------------- \\
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        if (!PhotonNetwork.isMasterClient)
        {
            menu.ShowClientLobby();
            photonView.RPC("UpdateStrings", PhotonTargets.All, PhotonNetwork.player.name, false);
            playerReady = false;
        }
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        if (PhotonNetwork.isMasterClient)
        {
            menu.ShowServerLobby();
            PhotonNetwork.room.open = false;
            PhotonNetwork.room.visible = false;
        }
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        //tell menu that a client left via prompt and bring back waiting menu
        Score mine = (Score)PhotonNetwork.player.GetScore();
        if(mine == Score.Server)
            menu.ShowPopup(MenuController.Popup.ClientLeft);
        else
            menu.ShowPopup(MenuController.Popup.ServerClosed);
    }

    public void OnPhotonCreateRoomFailed()
    {
        ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public override void OnPhotonJoinRoomFailed(object[] cause)
    {
        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }

    public void OnPhotonRandomJoinFailed()
    {
        ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        menu.CreatedGame();
        Score mine = Score.Server;
        PhotonNetwork.player.SetScore((int)mine);
    }
    public override void OnConnectedToPhoton()
    {
        base.OnConnectedToPhoton();
        menu.ConnectedMainMenu();
    }
    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
        menu.NoPhotonConnection();
    }
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);
        menu.NoPhotonConnection();
    }
    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
        menu.NoPhotonConnection();
    }
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        //tell menu to show popup "server closed" and bring back search menu
        print("THIS NEVER GETS CALLED AMIRITE");
        menu.ShowPopup(MenuController.Popup.ServerClosed);
    }
    public bool Connected()
    {
        bool connection = PhotonNetwork.connectedAndReady;
        return connection;
    }
    public enum Score { Client, Server, }
    
}
