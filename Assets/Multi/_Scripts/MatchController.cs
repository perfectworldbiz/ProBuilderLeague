using UnityEngine;
using System.Collections;
using Photon;

public class MatchController : PunBehaviour {

    public GameObject player;

    void Start()
    {
        PhotonNetwork.Instantiate(player.name, Vector3.zero + new Vector3(0,1,0), Quaternion.identity, 0);
    }
}
