using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MenuController : MonoBehaviour {

    public GameObject panelMenu;
    public GameObject panelSearch;
    public GameObject panelWaiting;
    public GameObject panelLobby;
    public GameObject panelPopup;
    public GameObject panelConnecting;
    public GameObject panelNoConnection;
    public GameObject readyButton;
    public GameObject startButton;
    public PhotonNetworkManager net;
    public GameObject hourglass;

    public Text waitingText;
    public Text searchingText;
    public Text serverText;
    public Text clientText;
    public InputField input;
    public Text popupText;
    public Button popupButton;

    bool countTimeHosting = false;
    bool countTimeSearching = false;
    float time = 0;
    string timeText;

    void Start()
    {
        FindNet();
        Cursor.visible = true;
        panelConnecting.SetActive(true); ;
        string name = PlayerPrefs.GetString("PlayerName");
        if (name != "")
        {
            input.text = name;
        }
        if (net.Connected())
        {
            ConnectedMainMenu();
            net.LeaveRoom();
        }
    }
    void Update()
    {
        if (countTimeHosting)
        {
            time += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timeText = ":    " + string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            waitingText.text = timeText;
        }
        else if (countTimeSearching)
        {
            time += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timeText = ":    " + string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            searchingText.text = timeText;
        }
    }
    void FindNet()
    {
        if(net == null)
        {
            net = GameObject.Find("PhotonNetworkManager").GetComponent<PhotonNetworkManager>();
        }
    }
    public void NewGame()
    {
        ChangeUsername();
        net.CreateGame();
    }
    public void CreatedGame() //menu -> waiting for player
    {
        panelMenu.SetActive(false);
        panelWaiting.SetActive(true);
        countTimeHosting = true;
        time = 0;
    }
    public void ShowClientLobby() //searching -> lobby
    {
        panelSearch.SetActive(false);
        panelLobby.SetActive(true);
        readyButton.SetActive(true);
        countTimeSearching = false;
        time = 0;
    }
    public void ShowServerLobby() //waiting -> lobby
    {
        panelWaiting.SetActive(false);
        panelLobby.SetActive(true);
        startButton.SetActive(true);
        countTimeHosting = false;
        time = 0;
    }
    public void UpdateTexts(string sText, string cText, bool ready)
    {
        serverText.text = sText;
        clientText.text = "Klient: " + cText + (ready? " - gotowy":" - nie gotowy");
    }
    public void FindGame() //menu -> searching
    {
        ChangeUsername();
        panelMenu.SetActive(false);
        panelSearch.SetActive(true);
        net.SearchGame(true);
        countTimeSearching = true;
    }
    public void LeaveLobbyClient() //lobby -> searching
    {
        readyButton.SetActive(false);
        panelLobby.SetActive(false);
        panelSearch.SetActive(true);
        net.LeaveRoom();
        countTimeSearching = true;
        time = 0;
    }
    public void StopSearching() //searching -> menu
    {
        panelSearch.SetActive(false);
        panelMenu.SetActive(true);
        net.SearchGame(false);
        countTimeSearching = false;
        time = 0;
    }
    public void LeaveLobbyServer() //lobby -> waiting  | "player disconnected' prompt should lead here
    {
        startButton.SetActive(false);
        panelLobby.SetActive(false);
        panelWaiting.SetActive(true);
        countTimeHosting = true;
        time = 0;
    }
    public void LeaveLobbyServerStopHosting() //lobby -> menu | gdy serwer wcisnie anuluj
    {
        startButton.SetActive(false);
        panelLobby.SetActive(false);
        panelMenu.SetActive(true);
        net.LeaveRoom();
        countTimeHosting = false;
        time = 0;
    }
    public void StopHosting() //waiting -> menu
    {
        panelWaiting.SetActive(false);
        panelMenu.SetActive(true);
        net.LeaveRoom();
        countTimeHosting = false;
        time = 0;
    }
    public void ToggleClientReady()
    {
        net.ToggleClientReady();
    }
    public void ServerStartGame()
    {
        net.StartGame();
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void ConnectedMainMenu() //connecting -> menu
    {
        panelConnecting.SetActive(false);
        panelMenu.SetActive(true);
    }

    /*void Toggle(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }*/
    public void ChangeUsername()
    {
        if (input.text != "")
        {
            net.SetName(input.text);
            PlayerPrefs.SetString("PlayerName", input.text);
            PlayerPrefs.Save();
        }
    }
    public void ShowHourglass(bool state)
    {
        hourglass.SetActive(state);
        if (state == false)
            time = 0;
    }


    public void ShowPopup(Popup i)
    {
        panelPopup.SetActive(true);
        time = 0;
        switch (i)
        {
            case Popup.ClientLeft:
                popupText.text = "Klient opuścił grę. Wracam do poczekalni.";
                startButton.SetActive(false);
                panelLobby.SetActive(false);

                popupButton.onClick.AddListener(delegate { PopupClientLeft(); });
                break;
            case Popup.ClientNotReady:
                popupText.text = "Nie można rozpocząć: klient nie jest gotowy.";
                popupButton.onClick.AddListener(delegate { PopupClientNotReady(); });
                break;
            case Popup.ServerClosed:
                popupText.text = "Serwer zamknięty. Wracam do wyszukiwania gier.";
                readyButton.SetActive(false);
                panelLobby.SetActive(false);
                net.LeaveRoom();
                popupButton.onClick.AddListener(delegate { PopupServerClosed(); });
                break;
        }
    }

    public void PopupClientLeft()
    {
        panelPopup.SetActive(false);
        panelWaiting.SetActive(true);
        countTimeHosting = true;
        net.OpenRoom();
    }
    public void PopupClientNotReady()
    {
        panelPopup.SetActive(false);
    }
    public void PopupServerClosed()
    {
        panelPopup.SetActive(false);
        panelSearch.SetActive(true);
        time = 0;
        countTimeSearching = true;
    }
    public void NoPhotonConnection()
    {
        //hide everything and show info
        //Nie moge polaczyc sie z serwerem. Upewnij sie, ze masz sprawne lacze intenretowe. Polacz/ Wyjdz. Po kliknieciu to znika a pojawia sie ten panelConnecting
        panelMenu.SetActive(false);
        panelSearch.SetActive(false);
        panelWaiting.SetActive(false);
        panelLobby.SetActive(false);
        panelPopup.SetActive(false);
        panelConnecting.SetActive(false);
        panelNoConnection.SetActive(true);
        readyButton.SetActive(false);
        startButton.SetActive(false);
    }
    public void RetryConnection()
    {
        panelNoConnection.SetActive(false);
        panelConnecting.SetActive(true);
        net.Connect();
    }
    public enum Popup {ClientLeft, ClientNotReady, ServerClosed }
}
