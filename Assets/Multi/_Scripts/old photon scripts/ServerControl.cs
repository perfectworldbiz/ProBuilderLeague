using UnityEngine;
using System.Collections;
using Photon;

public class ServerControl : PunBehaviour {

    GameController gamecon;

    void Start()
    {
        transform.name = "Server";
       // if (!photonView.isMine)
       //     return;
        gamecon = GameObject.Find("GameController").GetComponent<GameController>();
        gamecon.ShowServerUI();
        GetComponent<MouseOrbitImproved>().enabled = true;
    }

	
}
