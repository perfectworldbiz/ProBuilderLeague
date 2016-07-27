using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.SceneManagement;

public class GameController : PunBehaviour {

    public GameObject PanelESC;
    public GameObject PanelESCServer;
    public GameObject clientUI;
    public GameObject serverUI;
    ClientControl client;

    void Start()
    {
        Invoke("GetClient", 1);
    }
    void GetClient()
    {
        if(GameObject.Find("Client")!= null)
        client = GameObject.Find("Client").GetComponent<ClientControl>();
    }
    public void ShowESCMenuClient()
    {
        PanelESC.SetActive(true);
        client.ShowUI();
    }
    public void ShowESCMenuServer()
    {
        PanelESCServer.SetActive(true);
    }
    public void HideESCMenuClient()
    {
        PanelESC.SetActive(false);
        client.HideUI();
    }
    public void HideESCMenuServer()
    {
        PanelESCServer.SetActive(false);
    }
    public void ShowServerUI()
    {
        serverUI.SetActive(true);
    }
    public void ShowClientUI()
    {
        clientUI.SetActive(true);
        print("Client UI shown");
    }
    public void Exit()
    {
        photonView.RPC("Shutdown", PhotonTargets.AllBufferedViaServer);
    }
    [PunRPC]
    void Shutdown()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        Exit();
    }
}
