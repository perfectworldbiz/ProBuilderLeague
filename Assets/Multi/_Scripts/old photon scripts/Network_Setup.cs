using UnityEngine;
using System.Collections;
using Photon;

public class Network_Setup : PunBehaviour {

    public bool isPlayer;

    void Start()
    {
        if (isPlayer && photonView.isMine)
        {
            //Debug.LogError("Enabling client. IsMaster?  " + PhotonNetwork.isMasterClient + ". Is mine? " + photonView.isMine);
            GetComponent<ClientControl>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
        }
        else if(!isPlayer && PhotonNetwork.isMasterClient)
        {
            //Debug.LogError("Enabling server. IsMaster?  " + PhotonNetwork.isMasterClient + ". Is mine? " + photonView.isMine);
            GetComponent<ServerControl>().enabled = true;
            GetComponent<MouseOrbitImproved>().enabled = true;
            GetComponent<Camera>().enabled = true;
        }
    }

    public void NewPosition(Vector3 pos)
    {
        photonView.RPC("Pos", PhotonTargets.All, pos);
    }
    [PunRPC]
    void Pos(Vector3 pos)
    {
        transform.position = pos;
    }
}
